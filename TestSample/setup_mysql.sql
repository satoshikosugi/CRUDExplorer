-- CRUDExplorer テスト用データベース作成スクリプト
-- TestSample SQLファイルに対応するテーブル定義（日本語コメント付き）

CREATE DATABASE IF NOT EXISTS crud_explorer_sample
  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE crud_explorer_sample;

-- =============================================
-- 1. 顧客テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS CUSTOMERS (
    CUSTOMER_ID   INT           NOT NULL AUTO_INCREMENT COMMENT '顧客ID',
    NAME          VARCHAR(100)  NOT NULL COMMENT '顧客名',
    EMAIL         VARCHAR(200)  NULL     COMMENT 'メールアドレス',
    PHONE         VARCHAR(20)   NULL     COMMENT '電話番号',
    CREATED_AT    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    UPDATED_AT    DATETIME      NULL     COMMENT '更新日時',
    PRIMARY KEY (CUSTOMER_ID)
) COMMENT='顧客マスタ';

-- =============================================
-- 2. 注文テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS ORDERS (
    ORDER_ID      INT           NOT NULL AUTO_INCREMENT COMMENT '注文ID',
    CUSTOMER_ID   INT           NOT NULL COMMENT '顧客ID',
    ORDER_DATE    DATE          NOT NULL COMMENT '注文日',
    STATUS        VARCHAR(20)   NOT NULL DEFAULT 'PENDING' COMMENT '注文ステータス',
    TOTAL_AMOUNT  DECIMAL(12,2) NOT NULL DEFAULT 0 COMMENT '合計金額',
    UPDATED_AT    DATETIME      NULL     COMMENT '更新日時',
    PRIMARY KEY (ORDER_ID),
    FOREIGN KEY (CUSTOMER_ID) REFERENCES CUSTOMERS(CUSTOMER_ID)
) COMMENT='注文';

-- =============================================
-- 3. 注文明細テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS ORDER_ITEMS (
    ITEM_ID       INT           NOT NULL AUTO_INCREMENT COMMENT '明細ID',
    ORDER_ID      INT           NOT NULL COMMENT '注文ID',
    PRODUCT_ID    INT           NOT NULL COMMENT '商品ID',
    QUANTITY      INT           NOT NULL DEFAULT 1 COMMENT '数量',
    UNIT_PRICE    DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT '単価',
    STATUS        VARCHAR(20)   NULL     COMMENT '明細ステータス',
    PRIMARY KEY (ITEM_ID),
    FOREIGN KEY (ORDER_ID) REFERENCES ORDERS(ORDER_ID)
) COMMENT='注文明細';

-- =============================================
-- 4. 商品テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS PRODUCTS (
    PRODUCT_ID    INT           NOT NULL AUTO_INCREMENT COMMENT '商品ID',
    PRODUCT_NAME  VARCHAR(200)  NOT NULL COMMENT '商品名',
    CATEGORY      VARCHAR(50)   NULL     COMMENT 'カテゴリ',
    PRICE         DECIMAL(10,2) NOT NULL DEFAULT 0 COMMENT '価格',
    UPDATED_AT    DATETIME      NULL     COMMENT '更新日時',
    PRIMARY KEY (PRODUCT_ID)
) COMMENT='商品マスタ';

-- =============================================
-- 5. 仕入先テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS SUPPLIERS (
    SUPPLIER_ID   INT           NOT NULL AUTO_INCREMENT COMMENT '仕入先ID',
    SUPPLIER_NAME VARCHAR(200)  NOT NULL COMMENT '仕入先名',
    EMAIL         VARCHAR(200)  NULL     COMMENT 'メールアドレス',
    REGISTERED_AT DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '登録日時',
    PRIMARY KEY (SUPPLIER_ID)
) COMMENT='仕入先マスタ';

-- =============================================
-- 6. 倉庫テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS WAREHOUSES (
    WAREHOUSE_ID   INT          NOT NULL AUTO_INCREMENT COMMENT '倉庫ID',
    WAREHOUSE_NAME VARCHAR(100) NOT NULL COMMENT '倉庫名',
    LOCATION       VARCHAR(200) NULL     COMMENT '所在地',
    CREATED_AT     DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    PRIMARY KEY (WAREHOUSE_ID)
) COMMENT='倉庫マスタ';

-- =============================================
-- 7. 在庫テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS INVENTORY (
    INV_ID        INT           NOT NULL AUTO_INCREMENT COMMENT '在庫ID',
    PRODUCT_ID    INT           NOT NULL COMMENT '商品ID',
    WAREHOUSE_ID  INT           NOT NULL COMMENT '倉庫ID',
    QUANTITY      INT           NOT NULL DEFAULT 0 COMMENT '在庫数量',
    UPDATED_AT    DATETIME      NULL     COMMENT '更新日時',
    PRIMARY KEY (INV_ID),
    FOREIGN KEY (PRODUCT_ID) REFERENCES PRODUCTS(PRODUCT_ID),
    FOREIGN KEY (WAREHOUSE_ID) REFERENCES WAREHOUSES(WAREHOUSE_ID)
) COMMENT='在庫';

-- =============================================
-- 8. 入庫テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS STOCK_RECEIPTS (
    RECEIPT_ID    INT           NOT NULL AUTO_INCREMENT COMMENT '入庫ID',
    PRODUCT_ID    INT           NOT NULL COMMENT '商品ID',
    WAREHOUSE_ID  INT           NOT NULL COMMENT '倉庫ID',
    QUANTITY      INT           NOT NULL DEFAULT 0 COMMENT '入庫数量',
    RECEIVED_AT   DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '入庫日時',
    PRIMARY KEY (RECEIPT_ID),
    FOREIGN KEY (PRODUCT_ID) REFERENCES PRODUCTS(PRODUCT_ID),
    FOREIGN KEY (WAREHOUSE_ID) REFERENCES WAREHOUSES(WAREHOUSE_ID)
) COMMENT='入庫';

-- =============================================
-- 9. 出庫テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS STOCK_SHIPMENTS (
    SHIPMENT_ID   INT           NOT NULL AUTO_INCREMENT COMMENT '出庫ID',
    PRODUCT_ID    INT           NOT NULL COMMENT '商品ID',
    WAREHOUSE_ID  INT           NOT NULL COMMENT '倉庫ID',
    ORDER_ID      INT           NULL     COMMENT '注文ID',
    QUANTITY      INT           NOT NULL DEFAULT 0 COMMENT '出庫数量',
    SHIPPED_DATE  DATE          NULL     COMMENT '出庫日',
    PRIMARY KEY (SHIPMENT_ID),
    FOREIGN KEY (PRODUCT_ID) REFERENCES PRODUCTS(PRODUCT_ID),
    FOREIGN KEY (WAREHOUSE_ID) REFERENCES WAREHOUSES(WAREHOUSE_ID)
) COMMENT='出庫';

-- =============================================
-- 10. 在庫監査テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS INVENTORY_AUDIT (
    AUDIT_ID      INT           NOT NULL AUTO_INCREMENT COMMENT '監査ID',
    INV_ID        INT           NOT NULL COMMENT '在庫ID',
    PRODUCT_ID    INT           NOT NULL COMMENT '商品ID',
    WAREHOUSE_ID  INT           NOT NULL COMMENT '倉庫ID',
    OLD_QTY       INT           NOT NULL COMMENT '変更前数量',
    NEW_QTY       INT           NOT NULL COMMENT '変更後数量',
    REASON        VARCHAR(200)  NULL     COMMENT '変更理由',
    CREATED_AT    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    PRIMARY KEY (AUDIT_ID),
    FOREIGN KEY (INV_ID) REFERENCES INVENTORY(INV_ID)
) COMMENT='在庫監査ログ';

-- =============================================
-- 11. 配送テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS DELIVERIES (
    DELIVERY_ID     INT         NOT NULL AUTO_INCREMENT COMMENT '配送ID',
    ORDER_ID        INT         NOT NULL COMMENT '注文ID',
    DELIVERY_STATUS VARCHAR(20) NOT NULL DEFAULT 'PENDING' COMMENT '配送ステータス',
    DELIVERED_AT    DATETIME    NULL     COMMENT '配送完了日時',
    PRIMARY KEY (DELIVERY_ID),
    FOREIGN KEY (ORDER_ID) REFERENCES ORDERS(ORDER_ID)
) COMMENT='配送';

-- =============================================
-- 12. 支払テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS PAYMENTS (
    PAYMENT_ID      INT         NOT NULL AUTO_INCREMENT COMMENT '支払ID',
    ORDER_ID        INT         NOT NULL COMMENT '注文ID',
    PAYMENT_STATUS  VARCHAR(20) NOT NULL DEFAULT 'UNPAID' COMMENT '支払ステータス',
    PAYMENT_DATE    DATE        NULL     COMMENT '支払日',
    PRIMARY KEY (PAYMENT_ID),
    FOREIGN KEY (ORDER_ID) REFERENCES ORDERS(ORDER_ID)
) COMMENT='支払';

-- =============================================
-- 13. 価格履歴テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS PRICE_HISTORY (
    HISTORY_ID    INT           NOT NULL AUTO_INCREMENT COMMENT '履歴ID',
    PRODUCT_ID    INT           NOT NULL COMMENT '商品ID',
    OLD_PRICE     DECIMAL(10,2) NOT NULL COMMENT '旧価格',
    NEW_PRICE     DECIMAL(10,2) NOT NULL COMMENT '新価格',
    CHANGED_AT    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '変更日時',
    PRIMARY KEY (HISTORY_ID),
    FOREIGN KEY (PRODUCT_ID) REFERENCES PRODUCTS(PRODUCT_ID)
) COMMENT='価格変更履歴';

-- =============================================
-- 14. 顧客ブラックリスト
-- =============================================
CREATE TABLE IF NOT EXISTS CUSTOMER_BLACKLIST (
    CUSTOMER_ID   INT           NOT NULL COMMENT '顧客ID',
    REASON        VARCHAR(200)  NULL     COMMENT '理由',
    CREATED_AT    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '登録日時',
    PRIMARY KEY (CUSTOMER_ID),
    FOREIGN KEY (CUSTOMER_ID) REFERENCES CUSTOMERS(CUSTOMER_ID)
) COMMENT='顧客ブラックリスト';

-- =============================================
-- 15. 顧客アーカイブテーブル
-- =============================================
CREATE TABLE IF NOT EXISTS CUSTOMERS_ARCHIVE (
    CUSTOMER_ID   INT           NOT NULL COMMENT '顧客ID',
    NAME          VARCHAR(100)  NOT NULL COMMENT '顧客名',
    EMAIL         VARCHAR(200)  NULL     COMMENT 'メールアドレス',
    PHONE         VARCHAR(20)   NULL     COMMENT '電話番号',
    CREATED_AT    DATETIME      NOT NULL COMMENT '作成日時',
    ARCHIVED_AT   DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'アーカイブ日時',
    PRIMARY KEY (CUSTOMER_ID)
) COMMENT='顧客アーカイブ';

-- =============================================
-- 16. 注文履歴テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS ORDER_HISTORY (
    HISTORY_ID    INT           NOT NULL AUTO_INCREMENT COMMENT '履歴ID',
    ORDER_ID      INT           NOT NULL COMMENT '注文ID',
    CUSTOMER_ID   INT           NOT NULL COMMENT '顧客ID',
    CUSTOMER_NAME VARCHAR(100)  NULL     COMMENT '顧客名',
    ORDER_DATE    DATE          NOT NULL COMMENT '注文日',
    STATUS        VARCHAR(20)   NOT NULL COMMENT 'ステータス',
    TOTAL_AMOUNT  DECIMAL(12,2) NOT NULL COMMENT '合計金額',
    ITEM_COUNT    INT           NOT NULL DEFAULT 0 COMMENT '明細数',
    ARCHIVED_AT   DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'アーカイブ日時',
    PRIMARY KEY (HISTORY_ID)
) COMMENT='注文履歴';

-- =============================================
-- 17. 日次集計テーブル
-- =============================================
CREATE TABLE IF NOT EXISTS DAILY_AGGREGATES (
    AGGREGATE_ID   INT           NOT NULL AUTO_INCREMENT COMMENT '集計ID',
    AGGREGATE_DATE DATE          NOT NULL COMMENT '集計日',
    METRIC_TYPE    VARCHAR(50)   NOT NULL COMMENT '指標種別',
    METRIC_KEY     VARCHAR(100)  NOT NULL COMMENT '指標キー',
    METRIC_VALUE   DECIMAL(15,2) NOT NULL DEFAULT 0 COMMENT '指標値',
    CREATED_AT     DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    PRIMARY KEY (AGGREGATE_ID)
) COMMENT='日次集計';

-- =============================================
-- 18. 月次売上サマリテーブル
-- =============================================
CREATE TABLE IF NOT EXISTS MONTHLY_SALES_SUMMARY (
    SUMMARY_ID     INT           NOT NULL AUTO_INCREMENT COMMENT 'サマリID',
    SUMMARY_YEAR   INT           NOT NULL COMMENT '対象年',
    SUMMARY_MONTH  INT           NOT NULL COMMENT '対象月',
    PRODUCT_ID     INT           NOT NULL COMMENT '商品ID',
    TOTAL_QTY      INT           NOT NULL DEFAULT 0 COMMENT '合計数量',
    TOTAL_AMOUNT   DECIMAL(12,2) NOT NULL DEFAULT 0 COMMENT '合計金額',
    CUSTOMER_COUNT INT           NOT NULL DEFAULT 0 COMMENT '顧客数',
    CREATED_AT     DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '作成日時',
    PRIMARY KEY (SUMMARY_ID),
    FOREIGN KEY (PRODUCT_ID) REFERENCES PRODUCTS(PRODUCT_ID)
) COMMENT='月次売上サマリ';

-- =============================================
-- VIEWs (SP_VIEW_DEFINITIONS.sql に対応)
-- =============================================
CREATE OR REPLACE VIEW V_ACTIVE_CUSTOMERS AS
SELECT c.CUSTOMER_ID, c.NAME, c.EMAIL, c.PHONE, c.CREATED_AT
FROM CUSTOMERS c
WHERE c.CUSTOMER_ID NOT IN (SELECT bl.CUSTOMER_ID FROM CUSTOMER_BLACKLIST bl);

CREATE OR REPLACE VIEW V_ORDER_DETAILS AS
SELECT
    o.ORDER_ID, o.ORDER_DATE, o.STATUS,
    c.CUSTOMER_ID, c.NAME AS CUSTOMER_NAME,
    oi.ITEM_ID, oi.PRODUCT_ID,
    p.PRODUCT_NAME, p.CATEGORY,
    oi.QUANTITY, oi.UNIT_PRICE,
    oi.QUANTITY * oi.UNIT_PRICE AS LINE_TOTAL
FROM ORDERS o
JOIN CUSTOMERS c ON o.CUSTOMER_ID = c.CUSTOMER_ID
JOIN ORDER_ITEMS oi ON o.ORDER_ID = oi.ORDER_ID
JOIN PRODUCTS p ON oi.PRODUCT_ID = p.PRODUCT_ID;

CREATE OR REPLACE VIEW V_CATEGORY_SALES_SUMMARY AS
SELECT
    p.CATEGORY,
    COUNT(DISTINCT o.ORDER_ID) AS ORDER_COUNT,
    COUNT(DISTINCT o.CUSTOMER_ID) AS CUSTOMER_COUNT,
    SUM(oi.QUANTITY) AS TOTAL_QTY,
    SUM(oi.QUANTITY * oi.UNIT_PRICE) AS TOTAL_SALES,
    AVG(oi.UNIT_PRICE) AS AVG_UNIT_PRICE
FROM PRODUCTS p
JOIN ORDER_ITEMS oi ON p.PRODUCT_ID = oi.PRODUCT_ID
JOIN ORDERS o ON oi.ORDER_ID = o.ORDER_ID
GROUP BY p.CATEGORY;
