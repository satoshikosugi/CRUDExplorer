-- ============================================================
-- SP_COMPLEX_DML.sql  複雑なDML（MERGE・INSERT-SELECT・複合UPDATE）
-- ============================================================

CREATE OR REPLACE PACKAGE BODY SP_COMPLEX_DML IS

    -- MERGE文: 在庫の存在有無で INSERT/UPDATE を切り替え
    PROCEDURE UpsertInventory (
        p_product_id   IN NUMBER,
        p_warehouse_id IN NUMBER,
        p_quantity     IN NUMBER
    ) IS
    BEGIN
        MERGE INTO INVENTORY tgt
        USING (
            SELECT p_product_id AS PRODUCT_ID,
                   p_warehouse_id AS WAREHOUSE_ID,
                   p_quantity AS QUANTITY
            FROM DUAL
        ) src
        ON (tgt.PRODUCT_ID = src.PRODUCT_ID AND tgt.WAREHOUSE_ID = src.WAREHOUSE_ID)
        WHEN MATCHED THEN
            UPDATE SET
                tgt.QUANTITY   = tgt.QUANTITY + src.QUANTITY,
                tgt.UPDATED_AT = SYSDATE
        WHEN NOT MATCHED THEN
            INSERT (INV_ID, PRODUCT_ID, WAREHOUSE_ID, QUANTITY, UPDATED_AT)
            VALUES (SEQ_INV.NEXTVAL, src.PRODUCT_ID, src.WAREHOUSE_ID, src.QUANTITY, SYSDATE);

        COMMIT;
    END UpsertInventory;

    -- INSERT-SELECT: 月次サマリテーブルへの集計データ挿入
    PROCEDURE GenerateMonthlySummary (
        p_year  IN NUMBER,
        p_month IN NUMBER
    ) IS
    BEGIN
        -- 既存データを削除
        DELETE FROM MONTHLY_SALES_SUMMARY
        WHERE SUMMARY_YEAR = p_year
        AND SUMMARY_MONTH = p_month;

        -- 集計データを挿入
        INSERT INTO MONTHLY_SALES_SUMMARY (
            SUMMARY_ID, SUMMARY_YEAR, SUMMARY_MONTH,
            PRODUCT_ID, TOTAL_QTY, TOTAL_AMOUNT, CUSTOMER_COUNT, CREATED_AT
        )
        SELECT
            SEQ_SUMMARY.NEXTVAL,
            p_year,
            p_month,
            oi.PRODUCT_ID,
            SUM(oi.QUANTITY),
            SUM(oi.QUANTITY * oi.UNIT_PRICE),
            COUNT(DISTINCT o.CUSTOMER_ID),
            SYSDATE
        FROM ORDER_ITEMS oi
        JOIN ORDERS o ON o.ORDER_ID = oi.ORDER_ID
        WHERE EXTRACT(YEAR  FROM o.ORDER_DATE) = p_year
        AND   EXTRACT(MONTH FROM o.ORDER_DATE) = p_month
        AND   o.STATUS = 'COMPLETED'
        GROUP BY oi.PRODUCT_ID;

        COMMIT;
    END GenerateMonthlySummary;

    -- サブクエリ付きUPDATE: 注文合計金額を再計算
    PROCEDURE RecalculateOrderTotals IS
    BEGIN
        UPDATE ORDERS o_main
        SET TOTAL_AMOUNT = (
            SELECT NVL(SUM(oi.QUANTITY * oi.UNIT_PRICE), 0)
            FROM ORDER_ITEMS oi
            WHERE oi.ORDER_ID = o_main.ORDER_ID
        ),
        UPDATED_AT = SYSDATE
        WHERE o_main.STATUS IN ('PENDING', 'COMPLETED')
        AND EXISTS (
            SELECT 1
            FROM ORDER_ITEMS oi2
            WHERE oi2.ORDER_ID = o_main.ORDER_ID
        );

        COMMIT;
    END RecalculateOrderTotals;

    -- サブクエリ付きDELETE: 一定期間注文のない顧客のアーカイブ
    PROCEDURE ArchiveInactiveCustomers (
        p_inactive_days IN NUMBER
    ) IS
    BEGIN
        -- アーカイブテーブルへコピー
        INSERT INTO CUSTOMERS_ARCHIVE (
            CUSTOMER_ID, NAME, EMAIL, PHONE, CREATED_AT, ARCHIVED_AT
        )
        SELECT
            c.CUSTOMER_ID, c.NAME, c.EMAIL, c.PHONE, c.CREATED_AT, SYSDATE
        FROM CUSTOMERS c
        WHERE c.CUSTOMER_ID NOT IN (
            SELECT DISTINCT o.CUSTOMER_ID
            FROM ORDERS o
            WHERE o.ORDER_DATE >= SYSDATE - p_inactive_days
        )
        AND c.CUSTOMER_ID NOT IN (
            SELECT ca.CUSTOMER_ID
            FROM CUSTOMERS_ARCHIVE ca
        );

        -- 関連する注文明細を削除
        DELETE FROM ORDER_ITEMS
        WHERE ORDER_ID IN (
            SELECT o.ORDER_ID
            FROM ORDERS o
            WHERE o.CUSTOMER_ID IN (
                SELECT ca.CUSTOMER_ID
                FROM CUSTOMERS_ARCHIVE ca
                WHERE ca.ARCHIVED_AT >= TRUNC(SYSDATE)
            )
        );

        -- 関連する注文を削除
        DELETE FROM ORDERS
        WHERE CUSTOMER_ID IN (
            SELECT ca.CUSTOMER_ID
            FROM CUSTOMERS_ARCHIVE ca
            WHERE ca.ARCHIVED_AT >= TRUNC(SYSDATE)
        );

        COMMIT;
    END ArchiveInactiveCustomers;

    -- MERGE + サブクエリ: 商品マスタの価格一括更新
    PROCEDURE BulkUpdatePrices (
        p_category       IN VARCHAR2,
        p_increase_pct   IN NUMBER
    ) IS
    BEGIN
        MERGE INTO PRODUCTS tgt
        USING (
            SELECT
                p.PRODUCT_ID,
                p.PRICE * (1 + p_increase_pct / 100) AS NEW_PRICE
            FROM PRODUCTS p
            WHERE p.CATEGORY = p_category
            AND p.PRODUCT_ID IN (
                SELECT oi.PRODUCT_ID
                FROM ORDER_ITEMS oi
                JOIN ORDERS o ON o.ORDER_ID = oi.ORDER_ID
                WHERE o.ORDER_DATE >= ADD_MONTHS(SYSDATE, -12)
                GROUP BY oi.PRODUCT_ID
                HAVING SUM(oi.QUANTITY) > 10
            )
        ) src
        ON (tgt.PRODUCT_ID = src.PRODUCT_ID)
        WHEN MATCHED THEN
            UPDATE SET
                tgt.PRICE      = src.NEW_PRICE,
                tgt.UPDATED_AT = SYSDATE;

        -- 価格変更履歴を記録
        INSERT INTO PRICE_HISTORY (HISTORY_ID, PRODUCT_ID, OLD_PRICE, NEW_PRICE, CHANGED_AT)
        SELECT
            SEQ_PRICE_HIST.NEXTVAL,
            p.PRODUCT_ID,
            p.PRICE / (1 + p_increase_pct / 100),
            p.PRICE,
            SYSDATE
        FROM PRODUCTS p
        WHERE p.CATEGORY = p_category
        AND p.UPDATED_AT >= TRUNC(SYSDATE);

        COMMIT;
    END BulkUpdatePrices;

END SP_COMPLEX_DML;
/
