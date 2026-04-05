-- ============================================================
-- SP_CUSTOMER.sql  顧客管理ストアドプロシージャ
-- ============================================================

CREATE OR REPLACE PACKAGE BODY SP_CUSTOMER IS

    -- 顧客登録
    PROCEDURE RegisterCustomer (
        p_name    IN VARCHAR2,
        p_email   IN VARCHAR2,
        p_phone   IN VARCHAR2
    ) IS
    BEGIN
        INSERT INTO CUSTOMERS (CUSTOMER_ID, NAME, EMAIL, PHONE, CREATED_AT)
        VALUES (SEQ_CUSTOMER.NEXTVAL, p_name, p_email, p_phone, SYSDATE);

        COMMIT;
    END RegisterCustomer;

    -- 顧客照会
    FUNCTION GetCustomer (p_customer_id IN NUMBER) RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT CUSTOMER_ID, NAME, EMAIL, PHONE, CREATED_AT
        FROM CUSTOMERS
        WHERE CUSTOMER_ID = p_customer_id;

        RETURN v_cursor;
    END GetCustomer;

    -- 顧客情報更新
    PROCEDURE UpdateCustomer (
        p_customer_id IN NUMBER,
        p_email       IN VARCHAR2,
        p_phone       IN VARCHAR2
    ) IS
    BEGIN
        UPDATE CUSTOMERS
        SET EMAIL = p_email,
            PHONE = p_phone,
            UPDATED_AT = SYSDATE
        WHERE CUSTOMER_ID = p_customer_id;

        COMMIT;
    END UpdateCustomer;

    -- 顧客の注文一覧取得
    FUNCTION GetCustomerOrders (p_customer_id IN NUMBER) RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        SELECT o.ORDER_ID, o.ORDER_DATE, o.STATUS, o.TOTAL_AMOUNT
        FROM ORDERS o
        WHERE o.CUSTOMER_ID = p_customer_id
        ORDER BY o.ORDER_DATE DESC;

        RETURN v_cursor;
    END GetCustomerOrders;

END SP_CUSTOMER;
/
