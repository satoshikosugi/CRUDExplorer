-- ============================================================
-- SP_ORDER.sql  注文処理ストアドプロシージャ
-- ============================================================

CREATE OR REPLACE PACKAGE BODY SP_ORDER IS

    -- 注文登録
    PROCEDURE ProcessOrder (
        p_customer_id IN NUMBER,
        p_product_id  IN NUMBER,
        p_quantity    IN NUMBER
    ) IS
        v_order_id NUMBER;
    BEGIN
        -- 注文ヘッダ登録
        INSERT INTO ORDERS (ORDER_ID, CUSTOMER_ID, ORDER_DATE, STATUS, TOTAL_AMOUNT)
        VALUES (SEQ_ORDER.NEXTVAL, p_customer_id, SYSDATE, 'PENDING', 0);

        SELECT MAX(ORDER_ID)
        INTO v_order_id
        FROM ORDERS
        WHERE CUSTOMER_ID = p_customer_id
        AND ORDER_DATE = TRUNC(SYSDATE);

        -- 注文明細登録
        INSERT INTO ORDER_ITEMS (ITEM_ID, ORDER_ID, PRODUCT_ID, QUANTITY, UNIT_PRICE)
        SELECT SEQ_ITEM.NEXTVAL, v_order_id, PRODUCT_ID, p_quantity, PRICE
        FROM PRODUCTS
        WHERE PRODUCT_ID = p_product_id;

        COMMIT;
    END ProcessOrder;

    -- 注文状況照会
    FUNCTION GetOrderStatus (p_order_id IN NUMBER) RETURN VARCHAR2 IS
        v_status VARCHAR2(20);
        v_items  NUMBER;
    BEGIN
        SELECT STATUS
        INTO v_status
        FROM ORDERS
        WHERE ORDER_ID = p_order_id;

        SELECT COUNT(*)
        INTO v_items
        FROM ORDER_ITEMS
        WHERE ORDER_ID = p_order_id;

        RETURN v_status || '(' || v_items || '件)';
    END GetOrderStatus;

    -- 注文キャンセル
    PROCEDURE CancelOrder (p_order_id IN NUMBER) IS
    BEGIN
        UPDATE ORDERS
        SET STATUS = 'CANCELLED', UPDATED_AT = SYSDATE
        WHERE ORDER_ID = p_order_id;

        DELETE FROM ORDER_ITEMS
        WHERE ORDER_ID = p_order_id
        AND STATUS = 'ACTIVE';

        COMMIT;
    END CancelOrder;

END SP_ORDER;
/
