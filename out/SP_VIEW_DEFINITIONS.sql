-- ============================================================
-- SP_VIEW_DEFINITIONS.sql  VIEW定義（CRUDExplorerのVIEW展開テスト用）
-- ============================================================

-- 単純VIEW: 有効顧客
CREATE OR REPLACE VIEW V_ACTIVE_CUSTOMERS AS
SELECT
    c.CUSTOMER_ID,
    c.NAME,
    c.EMAIL,
    c.PHONE,
    c.CREATED_AT
FROM CUSTOMERS c
WHERE c.CUSTOMER_ID NOT IN (
    SELECT bl.CUSTOMER_ID FROM CUSTOMER_BLACKLIST bl
);

-- JOIN付きVIEW: 注文詳細
CREATE OR REPLACE VIEW V_ORDER_DETAILS AS
SELECT
    o.ORDER_ID,
    o.ORDER_DATE,
    o.STATUS,
    c.CUSTOMER_ID,
    c.NAME AS CUSTOMER_NAME,
    oi.ITEM_ID,
    oi.PRODUCT_ID,
    p.PRODUCT_NAME,
    p.CATEGORY,
    oi.QUANTITY,
    oi.UNIT_PRICE,
    oi.QUANTITY * oi.UNIT_PRICE AS LINE_TOTAL
FROM ORDERS o
JOIN CUSTOMERS c ON c.CUSTOMER_ID = o.CUSTOMER_ID
JOIN ORDER_ITEMS oi ON oi.ORDER_ID = o.ORDER_ID
JOIN PRODUCTS p ON p.PRODUCT_ID = oi.PRODUCT_ID;

-- 集約VIEW: カテゴリ別売上サマリ
CREATE OR REPLACE VIEW V_CATEGORY_SALES_SUMMARY AS
SELECT
    p.CATEGORY,
    COUNT(DISTINCT o.ORDER_ID) AS ORDER_COUNT,
    COUNT(DISTINCT o.CUSTOMER_ID) AS CUSTOMER_COUNT,
    SUM(oi.QUANTITY) AS TOTAL_QTY,
    SUM(oi.QUANTITY * oi.UNIT_PRICE) AS TOTAL_SALES,
    AVG(oi.UNIT_PRICE) AS AVG_UNIT_PRICE
FROM PRODUCTS p
JOIN ORDER_ITEMS oi ON oi.PRODUCT_ID = p.PRODUCT_ID
JOIN ORDERS o ON o.ORDER_ID = oi.ORDER_ID
WHERE o.STATUS = 'COMPLETED'
GROUP BY p.CATEGORY;

-- VIEW参照するプロシージャ: VIEW展開の検証に使用
CREATE OR REPLACE PACKAGE BODY SP_VIEW_CONSUMER IS

    -- VIEWからの単純SELECT
    FUNCTION GetActiveCustomerList RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT CUSTOMER_ID, NAME, EMAIL, PHONE
        FROM V_ACTIVE_CUSTOMERS
        ORDER BY NAME;

        RETURN v_cursor;
    END GetActiveCustomerList;

    -- VIEW + サブクエリ
    FUNCTION GetTopCategorySales (
        p_limit IN NUMBER
    ) RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT *
        FROM V_CATEGORY_SALES_SUMMARY css
        WHERE css.TOTAL_SALES >= (
            SELECT AVG(css2.TOTAL_SALES)
            FROM V_CATEGORY_SALES_SUMMARY css2
        )
        ORDER BY css.TOTAL_SALES DESC;

        RETURN v_cursor;
    END GetTopCategorySales;

    -- VIEW + JOIN: 注文詳細VIEWと在庫を結合
    FUNCTION GetOrdersWithStock RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT
            vod.ORDER_ID,
            vod.CUSTOMER_NAME,
            vod.PRODUCT_NAME,
            vod.QUANTITY AS ORDERED_QTY,
            vod.LINE_TOTAL,
            i.QUANTITY AS STOCK_QTY,
            CASE
                WHEN i.QUANTITY >= vod.QUANTITY THEN 'IN_STOCK'
                WHEN i.QUANTITY > 0 THEN 'PARTIAL'
                ELSE 'OUT_OF_STOCK'
            END AS STOCK_STATUS
        FROM V_ORDER_DETAILS vod
        LEFT JOIN INVENTORY i ON i.PRODUCT_ID = vod.PRODUCT_ID
        WHERE vod.STATUS = 'PENDING'
        ORDER BY vod.ORDER_DATE;

        RETURN v_cursor;
    END GetOrdersWithStock;

    -- VIEWへのINSERT（INSTEAD OF トリガー想定）
    PROCEDURE InsertViaView (
        p_customer_name IN VARCHAR2,
        p_email         IN VARCHAR2
    ) IS
    BEGIN
        INSERT INTO V_ACTIVE_CUSTOMERS (NAME, EMAIL)
        VALUES (p_customer_name, p_email);

        COMMIT;
    END InsertViaView;

    -- VIEWに対するUPDATE
    PROCEDURE UpdateViaView (
        p_customer_id IN NUMBER,
        p_phone       IN VARCHAR2
    ) IS
    BEGIN
        UPDATE V_ACTIVE_CUSTOMERS
        SET PHONE = p_phone
        WHERE CUSTOMER_ID = p_customer_id;

        COMMIT;
    END UpdateViaView;

END SP_VIEW_CONSUMER;
/
