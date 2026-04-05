-- ============================================================
-- SP_REPORT.sql  レポート生成ストアドプロシージャ
-- ============================================================

CREATE OR REPLACE PACKAGE BODY SP_REPORT IS

    -- 月次売上レポート生成
    FUNCTION GenerateReport (
        p_year  IN NUMBER,
        p_month IN NUMBER
    ) RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        -- 注文・注文明細・顧客・商品を結合してレポート生成
        OPEN v_cursor FOR
        SELECT
            o.ORDER_ID,
            o.ORDER_DATE,
            c.NAME         AS CUSTOMER_NAME,
            c.EMAIL        AS CUSTOMER_EMAIL,
            p.PRODUCT_NAME,
            oi.QUANTITY,
            oi.UNIT_PRICE,
            oi.QUANTITY * oi.UNIT_PRICE AS LINE_TOTAL,
            o.STATUS
        FROM ORDERS o
        JOIN CUSTOMERS c
            ON c.CUSTOMER_ID = o.CUSTOMER_ID
        JOIN ORDER_ITEMS oi
            ON oi.ORDER_ID = o.ORDER_ID
        JOIN PRODUCTS p
            ON p.PRODUCT_ID = oi.PRODUCT_ID
        WHERE EXTRACT(YEAR  FROM o.ORDER_DATE) = p_year
        AND   EXTRACT(MONTH FROM o.ORDER_DATE) = p_month
        ORDER BY o.ORDER_DATE, o.ORDER_ID;

        RETURN v_cursor;
    END GenerateReport;

    -- 在庫サマリレポート
    FUNCTION InventorySummary RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT
            p.PRODUCT_ID,
            p.PRODUCT_NAME,
            p.CATEGORY,
            i.QUANTITY     AS STOCK_QTY,
            i.UPDATED_AT   AS LAST_UPDATED
        FROM PRODUCTS p
        LEFT JOIN INVENTORY i
            ON i.PRODUCT_ID = p.PRODUCT_ID
        ORDER BY p.CATEGORY, p.PRODUCT_NAME;

        RETURN v_cursor;
    END InventorySummary;

END SP_REPORT;
/
