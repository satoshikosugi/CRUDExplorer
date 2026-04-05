-- ============================================================
-- SP_ANALYTICS.sql  分析系クエリ（サブクエリ・集計・Window関数）
-- ============================================================

CREATE OR REPLACE PACKAGE BODY SP_ANALYTICS IS

    -- スカラーサブクエリ: 顧客ごとの最終注文日付を取得
    FUNCTION GetCustomerSummary RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT
            c.CUSTOMER_ID,
            c.NAME,
            c.EMAIL,
            (SELECT MAX(o.ORDER_DATE)
             FROM ORDERS o
             WHERE o.CUSTOMER_ID = c.CUSTOMER_ID) AS LAST_ORDER_DATE,
            (SELECT COUNT(*)
             FROM ORDERS o2
             WHERE o2.CUSTOMER_ID = c.CUSTOMER_ID
             AND o2.STATUS <> 'CANCELLED') AS ORDER_COUNT,
            (SELECT SUM(o3.TOTAL_AMOUNT)
             FROM ORDERS o3
             WHERE o3.CUSTOMER_ID = c.CUSTOMER_ID
             AND o3.STATUS = 'COMPLETED') AS TOTAL_SPENT
        FROM CUSTOMERS c
        ORDER BY c.CUSTOMER_ID;

        RETURN v_cursor;
    END GetCustomerSummary;

    -- EXISTS サブクエリ: 注文実績のある顧客のみ取得
    FUNCTION GetActiveCustomers RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT c.CUSTOMER_ID, c.NAME, c.EMAIL, c.PHONE
        FROM CUSTOMERS c
        WHERE EXISTS (
            SELECT 1
            FROM ORDERS o
            WHERE o.CUSTOMER_ID = c.CUSTOMER_ID
            AND o.STATUS IN ('COMPLETED', 'SHIPPED')
        )
        AND NOT EXISTS (
            SELECT 1
            FROM CUSTOMER_BLACKLIST bl
            WHERE bl.CUSTOMER_ID = c.CUSTOMER_ID
        )
        ORDER BY c.NAME;

        RETURN v_cursor;
    END GetActiveCustomers;

    -- IN サブクエリ: 特定カテゴリ商品を注文した顧客
    FUNCTION GetCustomersByCategory (
        p_category IN VARCHAR2
    ) RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT DISTINCT c.CUSTOMER_ID, c.NAME, c.EMAIL
        FROM CUSTOMERS c
        WHERE c.CUSTOMER_ID IN (
            SELECT o.CUSTOMER_ID
            FROM ORDERS o
            JOIN ORDER_ITEMS oi ON oi.ORDER_ID = o.ORDER_ID
            WHERE oi.PRODUCT_ID IN (
                SELECT p.PRODUCT_ID
                FROM PRODUCTS p
                WHERE p.CATEGORY = p_category
            )
        )
        ORDER BY c.NAME;

        RETURN v_cursor;
    END GetCustomersByCategory;

    -- 相関サブクエリ: 平均注文額を超える注文
    FUNCTION GetAboveAverageOrders RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT
            o.ORDER_ID,
            o.CUSTOMER_ID,
            c.NAME AS CUSTOMER_NAME,
            o.ORDER_DATE,
            o.TOTAL_AMOUNT
        FROM ORDERS o
        JOIN CUSTOMERS c ON c.CUSTOMER_ID = o.CUSTOMER_ID
        WHERE o.TOTAL_AMOUNT > (
            SELECT AVG(o2.TOTAL_AMOUNT)
            FROM ORDERS o2
            WHERE o2.CUSTOMER_ID = o.CUSTOMER_ID
        )
        ORDER BY o.TOTAL_AMOUNT DESC;

        RETURN v_cursor;
    END GetAboveAverageOrders;

    -- FROM句サブクエリ（インラインビュー）: 売上ランキング
    FUNCTION GetSalesRanking (
        p_year  IN NUMBER,
        p_month IN NUMBER
    ) RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT
            ranking.PRODUCT_ID,
            ranking.PRODUCT_NAME,
            ranking.CATEGORY,
            ranking.TOTAL_QTY,
            ranking.TOTAL_SALES,
            ranking.RANK_NO
        FROM (
            SELECT
                p.PRODUCT_ID,
                p.PRODUCT_NAME,
                p.CATEGORY,
                SUM(oi.QUANTITY) AS TOTAL_QTY,
                SUM(oi.QUANTITY * oi.UNIT_PRICE) AS TOTAL_SALES,
                RANK() OVER (ORDER BY SUM(oi.QUANTITY * oi.UNIT_PRICE) DESC) AS RANK_NO
            FROM PRODUCTS p
            JOIN ORDER_ITEMS oi ON oi.PRODUCT_ID = p.PRODUCT_ID
            JOIN ORDERS o ON o.ORDER_ID = oi.ORDER_ID
            WHERE EXTRACT(YEAR  FROM o.ORDER_DATE) = p_year
            AND   EXTRACT(MONTH FROM o.ORDER_DATE) = p_month
            AND   o.STATUS <> 'CANCELLED'
            GROUP BY p.PRODUCT_ID, p.PRODUCT_NAME, p.CATEGORY
        ) ranking
        WHERE ranking.RANK_NO <= 20
        ORDER BY ranking.RANK_NO;

        RETURN v_cursor;
    END GetSalesRanking;

    -- HAVING + サブクエリ: 平均単価以上のカテゴリ
    FUNCTION GetPremiumCategories RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT
            p.CATEGORY,
            COUNT(*)       AS PRODUCT_COUNT,
            AVG(p.PRICE)   AS AVG_PRICE,
            MIN(p.PRICE)   AS MIN_PRICE,
            MAX(p.PRICE)   AS MAX_PRICE
        FROM PRODUCTS p
        WHERE p.PRODUCT_ID IN (
            SELECT oi.PRODUCT_ID
            FROM ORDER_ITEMS oi
        )
        GROUP BY p.CATEGORY
        HAVING AVG(p.PRICE) > (
            SELECT AVG(p2.PRICE) FROM PRODUCTS p2
        )
        ORDER BY AVG(p.PRICE) DESC;

        RETURN v_cursor;
    END GetPremiumCategories;

END SP_ANALYTICS;
/
