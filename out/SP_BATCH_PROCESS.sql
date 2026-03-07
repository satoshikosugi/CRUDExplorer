-- ============================================================
-- SP_BATCH_PROCESS.sql  バッチ処理（カーソルループ・動的SQL・例外処理）
-- ============================================================

CREATE OR REPLACE PACKAGE BODY SP_BATCH_PROCESS IS

    -- カーソルFORループ + 相関サブクエリ: 全倉庫の在庫再計算
    PROCEDURE RecalculateAllInventory IS
        v_expected_qty NUMBER;
    BEGIN
        FOR rec IN (
            SELECT
                i.INV_ID,
                i.PRODUCT_ID,
                i.WAREHOUSE_ID,
                i.QUANTITY AS CURRENT_QTY
            FROM INVENTORY i
            WHERE i.PRODUCT_ID IN (
                SELECT DISTINCT oi.PRODUCT_ID
                FROM ORDER_ITEMS oi
                JOIN ORDERS o ON o.ORDER_ID = oi.ORDER_ID
                WHERE o.ORDER_DATE >= ADD_MONTHS(SYSDATE, -1)
            )
        ) LOOP
            -- 入荷 - 出荷 で期待在庫数を算出
            SELECT
                NVL(received.qty, 0) - NVL(shipped.qty, 0)
            INTO v_expected_qty
            FROM (
                SELECT SUM(sr.QUANTITY) AS qty
                FROM STOCK_RECEIPTS sr
                WHERE sr.PRODUCT_ID = rec.PRODUCT_ID
                AND sr.WAREHOUSE_ID = rec.WAREHOUSE_ID
            ) received,
            (
                SELECT SUM(ss.QUANTITY) AS qty
                FROM STOCK_SHIPMENTS ss
                WHERE ss.PRODUCT_ID = rec.PRODUCT_ID
                AND ss.WAREHOUSE_ID = rec.WAREHOUSE_ID
            ) shipped;

            -- 差異がある場合のみ更新
            IF rec.CURRENT_QTY <> v_expected_qty THEN
                UPDATE INVENTORY
                SET QUANTITY   = v_expected_qty,
                    UPDATED_AT = SYSDATE
                WHERE INV_ID = rec.INV_ID;

                INSERT INTO INVENTORY_AUDIT (
                    AUDIT_ID, INV_ID, PRODUCT_ID, WAREHOUSE_ID,
                    OLD_QTY, NEW_QTY, REASON, CREATED_AT
                ) VALUES (
                    SEQ_AUDIT.NEXTVAL, rec.INV_ID, rec.PRODUCT_ID, rec.WAREHOUSE_ID,
                    rec.CURRENT_QTY, v_expected_qty, 'RECALCULATION', SYSDATE
                );
            END IF;
        END LOOP;

        COMMIT;
    END RecalculateAllInventory;

    -- バッチINSERT: 日次集計の生成（UNION + GROUP BY + サブクエリ）
    PROCEDURE GenerateDailyAggregates (
        p_target_date IN DATE
    ) IS
    BEGIN
        DELETE FROM DAILY_AGGREGATES
        WHERE AGGREGATE_DATE = TRUNC(p_target_date);

        INSERT INTO DAILY_AGGREGATES (
            AGGREGATE_ID, AGGREGATE_DATE, METRIC_TYPE,
            METRIC_KEY, METRIC_VALUE, CREATED_AT
        )
        -- 商品別売上
        SELECT SEQ_AGGREGATE.NEXTVAL, TRUNC(p_target_date), 'PRODUCT_SALES',
               TO_CHAR(oi.PRODUCT_ID), SUM(oi.QUANTITY * oi.UNIT_PRICE), SYSDATE
        FROM ORDER_ITEMS oi
        JOIN ORDERS o ON o.ORDER_ID = oi.ORDER_ID
        WHERE TRUNC(o.ORDER_DATE) = TRUNC(p_target_date)
        AND o.STATUS <> 'CANCELLED'
        GROUP BY oi.PRODUCT_ID

        UNION ALL

        -- カテゴリ別売上
        SELECT SEQ_AGGREGATE.NEXTVAL, TRUNC(p_target_date), 'CATEGORY_SALES',
               p.CATEGORY, SUM(oi.QUANTITY * oi.UNIT_PRICE), SYSDATE
        FROM ORDER_ITEMS oi
        JOIN ORDERS o ON o.ORDER_ID = oi.ORDER_ID
        JOIN PRODUCTS p ON p.PRODUCT_ID = oi.PRODUCT_ID
        WHERE TRUNC(o.ORDER_DATE) = TRUNC(p_target_date)
        AND o.STATUS <> 'CANCELLED'
        GROUP BY p.CATEGORY

        UNION ALL

        -- 新規顧客数
        SELECT SEQ_AGGREGATE.NEXTVAL, TRUNC(p_target_date), 'NEW_CUSTOMERS',
               'COUNT', COUNT(*), SYSDATE
        FROM CUSTOMERS c
        WHERE TRUNC(c.CREATED_AT) = TRUNC(p_target_date)

        UNION ALL

        -- 倉庫別出荷数
        SELECT SEQ_AGGREGATE.NEXTVAL, TRUNC(p_target_date), 'WAREHOUSE_SHIPMENTS',
               TO_CHAR(ss.WAREHOUSE_ID), SUM(ss.QUANTITY), SYSDATE
        FROM STOCK_SHIPMENTS ss
        WHERE TRUNC(ss.SHIPPED_DATE) = TRUNC(p_target_date)
        GROUP BY ss.WAREHOUSE_ID;

        COMMIT;
    END GenerateDailyAggregates;

    -- 複合条件による一括更新: 注文ステータス自動遷移
    PROCEDURE AutoTransitionOrderStatus IS
    BEGIN
        -- PENDING → CONFIRMED: 支払い確認済み
        UPDATE ORDERS
        SET STATUS     = 'CONFIRMED',
            UPDATED_AT = SYSDATE
        WHERE STATUS = 'PENDING'
        AND ORDER_ID IN (
            SELECT py.ORDER_ID
            FROM PAYMENTS py
            WHERE py.PAYMENT_STATUS = 'COMPLETED'
            AND py.PAYMENT_DATE IS NOT NULL
        );

        -- CONFIRMED → SHIPPED: 全品目出荷済み
        UPDATE ORDERS
        SET STATUS     = 'SHIPPED',
            UPDATED_AT = SYSDATE
        WHERE STATUS = 'CONFIRMED'
        AND NOT EXISTS (
            SELECT 1
            FROM ORDER_ITEMS oi
            WHERE oi.ORDER_ID = ORDERS.ORDER_ID
            AND oi.PRODUCT_ID NOT IN (
                SELECT ss.PRODUCT_ID
                FROM STOCK_SHIPMENTS ss
                WHERE ss.ORDER_ID = oi.ORDER_ID
            )
        );

        -- SHIPPED → COMPLETED: 配送完了
        UPDATE ORDERS
        SET STATUS     = 'COMPLETED',
            UPDATED_AT = SYSDATE
        WHERE STATUS = 'SHIPPED'
        AND ORDER_ID IN (
            SELECT d.ORDER_ID
            FROM DELIVERIES d
            WHERE d.DELIVERY_STATUS = 'DELIVERED'
            AND d.DELIVERED_AT IS NOT NULL
        );

        -- 長期PENDING → CANCELLED: 7日間放置
        UPDATE ORDERS
        SET STATUS     = 'CANCELLED',
            UPDATED_AT = SYSDATE
        WHERE STATUS = 'PENDING'
        AND ORDER_DATE < SYSDATE - 7
        AND ORDER_ID NOT IN (
            SELECT py.ORDER_ID
            FROM PAYMENTS py
            WHERE py.PAYMENT_STATUS IN ('COMPLETED', 'PROCESSING')
        );

        COMMIT;
    END AutoTransitionOrderStatus;

    -- Window関数を活用した在庫移動提案
    FUNCTION SuggestStockTransfers RETURN SYS_REFCURSOR IS
        v_cursor SYS_REFCURSOR;
    BEGIN
        OPEN v_cursor FOR
        WITH warehouse_demand AS (
            SELECT
                w.WAREHOUSE_ID,
                w.WAREHOUSE_NAME,
                oi.PRODUCT_ID,
                SUM(oi.QUANTITY) AS DEMAND_QTY,
                ROW_NUMBER() OVER (
                    PARTITION BY oi.PRODUCT_ID
                    ORDER BY SUM(oi.QUANTITY) DESC
                ) AS DEMAND_RANK
            FROM WAREHOUSES w
            JOIN STOCK_SHIPMENTS ss ON ss.WAREHOUSE_ID = w.WAREHOUSE_ID
            JOIN ORDER_ITEMS oi ON oi.ORDER_ID = ss.ORDER_ID
                               AND oi.PRODUCT_ID = ss.PRODUCT_ID
            WHERE ss.SHIPPED_DATE >= ADD_MONTHS(SYSDATE, -3)
            GROUP BY w.WAREHOUSE_ID, w.WAREHOUSE_NAME, oi.PRODUCT_ID
        ),
        warehouse_stock AS (
            SELECT
                i.WAREHOUSE_ID,
                i.PRODUCT_ID,
                i.QUANTITY AS STOCK_QTY,
                AVG(i.QUANTITY) OVER (PARTITION BY i.PRODUCT_ID) AS AVG_STOCK
            FROM INVENTORY i
            WHERE i.QUANTITY > 0
        )
        SELECT
            ws.WAREHOUSE_ID AS FROM_WAREHOUSE,
            wd.WAREHOUSE_ID AS TO_WAREHOUSE,
            wd.WAREHOUSE_NAME AS TO_WAREHOUSE_NAME,
            p.PRODUCT_ID,
            p.PRODUCT_NAME,
            ws.STOCK_QTY AS EXCESS_STOCK,
            wd.DEMAND_QTY,
            LEAST(ws.STOCK_QTY - ws.AVG_STOCK, wd.DEMAND_QTY) AS SUGGESTED_TRANSFER
        FROM warehouse_stock ws
        JOIN warehouse_demand wd ON wd.PRODUCT_ID = ws.PRODUCT_ID
            AND wd.WAREHOUSE_ID <> ws.WAREHOUSE_ID
        JOIN PRODUCTS p ON p.PRODUCT_ID = ws.PRODUCT_ID
        WHERE ws.STOCK_QTY > ws.AVG_STOCK * 1.5
        AND wd.DEMAND_RANK <= 3
        AND NOT EXISTS (
            SELECT 1 FROM INVENTORY i2
            WHERE i2.WAREHOUSE_ID = wd.WAREHOUSE_ID
            AND i2.PRODUCT_ID = ws.PRODUCT_ID
            AND i2.QUANTITY >= wd.DEMAND_QTY
        )
        ORDER BY LEAST(ws.STOCK_QTY - ws.AVG_STOCK, wd.DEMAND_QTY) DESC;

        RETURN v_cursor;
    END SuggestStockTransfers;

END SP_BATCH_PROCESS;
/
