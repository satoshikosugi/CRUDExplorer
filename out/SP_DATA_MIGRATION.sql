-- ============================================================
-- SP_DATA_MIGRATION.sql  データ移行・CTE・UNION・多段サブクエリ
-- ============================================================

CREATE OR REPLACE PACKAGE BODY SP_DATA_MIGRATION IS

    -- CTE (WITH句): 階層的な売上集計
    FUNCTION GetHierarchicalSales (
        p_year IN NUMBER
    ) RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        WITH monthly_sales AS (
            SELECT
                EXTRACT(MONTH FROM o.ORDER_DATE) AS SALE_MONTH,
                oi.PRODUCT_ID,
                SUM(oi.QUANTITY * oi.UNIT_PRICE) AS MONTHLY_TOTAL
            FROM ORDERS o
            JOIN ORDER_ITEMS oi ON oi.ORDER_ID = o.ORDER_ID
            WHERE EXTRACT(YEAR FROM o.ORDER_DATE) = p_year
            AND o.STATUS = 'COMPLETED'
            GROUP BY EXTRACT(MONTH FROM o.ORDER_DATE), oi.PRODUCT_ID
        ),
        category_sales AS (
            SELECT
                ms.SALE_MONTH,
                p.CATEGORY,
                SUM(ms.MONTHLY_TOTAL) AS CATEGORY_TOTAL,
                COUNT(DISTINCT ms.PRODUCT_ID) AS PRODUCT_COUNT
            FROM monthly_sales ms
            JOIN PRODUCTS p ON p.PRODUCT_ID = ms.PRODUCT_ID
            GROUP BY ms.SALE_MONTH, p.CATEGORY
        ),
        grand_total AS (
            SELECT
                cs.SALE_MONTH,
                SUM(cs.CATEGORY_TOTAL) AS MONTH_TOTAL
            FROM category_sales cs
            GROUP BY cs.SALE_MONTH
        )
        SELECT
            cs.SALE_MONTH,
            cs.CATEGORY,
            cs.CATEGORY_TOTAL,
            cs.PRODUCT_COUNT,
            gt.MONTH_TOTAL,
            ROUND(cs.CATEGORY_TOTAL / gt.MONTH_TOTAL * 100, 2) AS CATEGORY_PCT
        FROM category_sales cs
        JOIN grand_total gt ON gt.SALE_MONTH = cs.SALE_MONTH
        ORDER BY cs.SALE_MONTH, cs.CATEGORY_TOTAL DESC;

        RETURN v_cursor;
    END GetHierarchicalSales;

    -- UNION ALL: 複数テーブルから統合マスタ作成
    FUNCTION GetUnifiedMaster RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT 'CUSTOMER' AS ENTITY_TYPE,
               c.CUSTOMER_ID AS ENTITY_ID,
               c.NAME AS ENTITY_NAME,
               c.EMAIL AS CONTACT_INFO,
               c.CREATED_AT
        FROM CUSTOMERS c

        UNION ALL

        SELECT 'SUPPLIER' AS ENTITY_TYPE,
               s.SUPPLIER_ID AS ENTITY_ID,
               s.SUPPLIER_NAME AS ENTITY_NAME,
               s.EMAIL AS CONTACT_INFO,
               s.REGISTERED_AT AS CREATED_AT
        FROM SUPPLIERS s

        UNION ALL

        SELECT 'WAREHOUSE' AS ENTITY_TYPE,
               w.WAREHOUSE_ID AS ENTITY_ID,
               w.WAREHOUSE_NAME AS ENTITY_NAME,
               w.LOCATION AS CONTACT_INFO,
               w.CREATED_AT
        FROM WAREHOUSES w

        ORDER BY ENTITY_TYPE, ENTITY_NAME;

        RETURN v_cursor;
    END GetUnifiedMaster;

    -- 多段ネストサブクエリ: 売れ筋商品が在庫切れの倉庫を特定
    FUNCTION FindStockoutWarehouses RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT
            w.WAREHOUSE_ID,
            w.WAREHOUSE_NAME,
            w.LOCATION,
            missing.PRODUCT_ID,
            missing.PRODUCT_NAME
        FROM WAREHOUSES w
        CROSS JOIN (
            -- 売れ筋TOP10商品
            SELECT p.PRODUCT_ID, p.PRODUCT_NAME
            FROM PRODUCTS p
            WHERE p.PRODUCT_ID IN (
                SELECT oi.PRODUCT_ID
                FROM ORDER_ITEMS oi
                JOIN ORDERS o ON o.ORDER_ID = oi.ORDER_ID
                WHERE o.ORDER_DATE >= ADD_MONTHS(SYSDATE, -3)
                GROUP BY oi.PRODUCT_ID
                HAVING SUM(oi.QUANTITY) >= (
                    SELECT AVG(total_qty)
                    FROM (
                        SELECT SUM(oi2.QUANTITY) AS total_qty
                        FROM ORDER_ITEMS oi2
                        JOIN ORDERS o2 ON o2.ORDER_ID = oi2.ORDER_ID
                        WHERE o2.ORDER_DATE >= ADD_MONTHS(SYSDATE, -3)
                        GROUP BY oi2.PRODUCT_ID
                    ) avg_calc
                )
            )
        ) missing
        WHERE NOT EXISTS (
            SELECT 1
            FROM INVENTORY i
            WHERE i.WAREHOUSE_ID = w.WAREHOUSE_ID
            AND i.PRODUCT_ID = missing.PRODUCT_ID
            AND i.QUANTITY > 0
        )
        ORDER BY w.WAREHOUSE_NAME, missing.PRODUCT_NAME;

        RETURN v_cursor;
    END FindStockoutWarehouses;

    -- CTE + INSERT-SELECT: データ移行バッチ
    PROCEDURE MigrateOrderHistory (
        p_cutoff_date IN DATE
    ) IS
    BEGIN
        -- 移行対象を特定してアーカイブにコピー
        INSERT INTO ORDER_HISTORY (
            HISTORY_ID, ORDER_ID, CUSTOMER_ID, CUSTOMER_NAME,
            ORDER_DATE, STATUS, TOTAL_AMOUNT,
            ITEM_COUNT, ARCHIVED_AT
        )
        WITH order_details AS (
            SELECT
                o.ORDER_ID,
                o.CUSTOMER_ID,
                o.ORDER_DATE,
                o.STATUS,
                o.TOTAL_AMOUNT,
                (SELECT COUNT(*) FROM ORDER_ITEMS oi
                 WHERE oi.ORDER_ID = o.ORDER_ID) AS ITEM_COUNT
            FROM ORDERS o
            WHERE o.ORDER_DATE < p_cutoff_date
            AND o.STATUS IN ('COMPLETED', 'CANCELLED')
            AND o.ORDER_ID NOT IN (
                SELECT oh.ORDER_ID FROM ORDER_HISTORY oh
            )
        )
        SELECT
            SEQ_HISTORY.NEXTVAL,
            od.ORDER_ID,
            od.CUSTOMER_ID,
            c.NAME,
            od.ORDER_DATE,
            od.STATUS,
            od.TOTAL_AMOUNT,
            od.ITEM_COUNT,
            SYSDATE
        FROM order_details od
        JOIN CUSTOMERS c ON c.CUSTOMER_ID = od.CUSTOMER_ID;

        -- 移行済み注文明細の削除
        DELETE FROM ORDER_ITEMS
        WHERE ORDER_ID IN (
            SELECT oh.ORDER_ID
            FROM ORDER_HISTORY oh
            WHERE oh.ARCHIVED_AT >= TRUNC(SYSDATE)
        );

        -- 移行済み注文の削除
        DELETE FROM ORDERS
        WHERE ORDER_ID IN (
            SELECT oh.ORDER_ID
            FROM ORDER_HISTORY oh
            WHERE oh.ARCHIVED_AT >= TRUNC(SYSDATE)
        );

        COMMIT;
    END MigrateOrderHistory;

    -- CASE式 + サブクエリ: 顧客セグメント分類
    FUNCTION ClassifyCustomers RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT
            c.CUSTOMER_ID,
            c.NAME,
            c.EMAIL,
            stats.ORDER_COUNT,
            stats.TOTAL_SPENT,
            stats.LAST_ORDER_DATE,
            CASE
                WHEN stats.TOTAL_SPENT >= (
                    SELECT PERCENTILE_CONT(0.9) WITHIN GROUP (ORDER BY sub.TOTAL_SPENT)
                    FROM (
                        SELECT SUM(o3.TOTAL_AMOUNT) AS TOTAL_SPENT
                        FROM ORDERS o3
                        WHERE o3.STATUS = 'COMPLETED'
                        GROUP BY o3.CUSTOMER_ID
                    ) sub
                ) THEN 'PLATINUM'
                WHEN stats.TOTAL_SPENT >= (
                    SELECT PERCENTILE_CONT(0.7) WITHIN GROUP (ORDER BY sub2.TOTAL_SPENT)
                    FROM (
                        SELECT SUM(o4.TOTAL_AMOUNT) AS TOTAL_SPENT
                        FROM ORDERS o4
                        WHERE o4.STATUS = 'COMPLETED'
                        GROUP BY o4.CUSTOMER_ID
                    ) sub2
                ) THEN 'GOLD'
                WHEN stats.ORDER_COUNT >= 3 THEN 'SILVER'
                ELSE 'BRONZE'
            END AS SEGMENT
        FROM CUSTOMERS c
        JOIN (
            SELECT
                o.CUSTOMER_ID,
                COUNT(*) AS ORDER_COUNT,
                SUM(o.TOTAL_AMOUNT) AS TOTAL_SPENT,
                MAX(o.ORDER_DATE) AS LAST_ORDER_DATE
            FROM ORDERS o
            WHERE o.STATUS = 'COMPLETED'
            GROUP BY o.CUSTOMER_ID
        ) stats ON stats.CUSTOMER_ID = c.CUSTOMER_ID
        ORDER BY stats.TOTAL_SPENT DESC;

        RETURN v_cursor;
    END ClassifyCustomers;

END SP_DATA_MIGRATION;
/
