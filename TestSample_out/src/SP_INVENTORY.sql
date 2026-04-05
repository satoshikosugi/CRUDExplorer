-- ============================================================
-- SP_INVENTORY.sql  在庫管理ストアドプロシージャ
-- ============================================================

CREATE OR REPLACE PACKAGE BODY SP_INVENTORY IS

    -- 商品在庫確認
    FUNCTION CheckStock (p_product_id IN NUMBER) RETURN NUMBER IS
        v_qty NUMBER := 0;
    BEGIN
        SELECT QUANTITY
        INTO v_qty
        FROM INVENTORY
        WHERE PRODUCT_ID = p_product_id
        AND WAREHOUSE_ID = 1;

        RETURN v_qty;
    END CheckStock;

    -- 在庫引当（在庫更新）
    PROCEDURE UpdateStock (
        p_product_id  IN NUMBER,
        p_delta       IN NUMBER  -- 正: 入荷, 負: 出荷
    ) IS
        v_product_name VARCHAR2(100);
    BEGIN
        -- 商品マスタ確認
        SELECT PRODUCT_NAME
        INTO v_product_name
        FROM PRODUCTS
        WHERE PRODUCT_ID = p_product_id;

        UPDATE INVENTORY
        SET QUANTITY    = QUANTITY + p_delta,
            UPDATED_AT  = SYSDATE
        WHERE PRODUCT_ID = p_product_id;

        COMMIT;
    END UpdateStock;

    -- 在庫初期登録
    PROCEDURE AddInventory (
        p_product_id   IN NUMBER,
        p_warehouse_id IN NUMBER,
        p_quantity     IN NUMBER
    ) IS
    BEGIN
        INSERT INTO INVENTORY (INV_ID, PRODUCT_ID, WAREHOUSE_ID, QUANTITY, UPDATED_AT)
        VALUES (SEQ_INV.NEXTVAL, p_product_id, p_warehouse_id, p_quantity, SYSDATE);

        COMMIT;
    END AddInventory;

END SP_INVENTORY;
/
