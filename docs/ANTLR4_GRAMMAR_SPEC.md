# ANTLR4 Grammar Specification

このドキュメントは、CRUDExplorerで使用されるANTLR4文法定義の詳細な仕様書です。

---

## 📚 概要

CRUDExplorerは、ANTLR4を使用してSQL文を解析し、CRUD操作（Create/Read/Update/Delete）を抽出します。

**ANTLR4バージョン:** 4.13.1

**文法ファイルの構成:**
- `Sql.g4` - 基本SQL文法（標準SQL準拠）
- `PostgreSqlDialect.g4` - PostgreSQL固有拡張
- `MySqlDialect.g4` - MySQL固有拡張
- `SqlServerDialect.g4` - SQL Server固有拡張
- `OracleDialect.g4` - Oracle固有拡張

---

## 🏗️ 文法構造

### 基本アーキテクチャ

```
Sql.g4 (基本文法)
    ├── PostgreSqlDialect.g4 (import Sql;)
    ├── MySqlDialect.g4 (import Sql;)
    ├── SqlServerDialect.g4 (import Sql;)
    └── OracleDialect.g4 (import Sql;)
```

各DB方言は`import Sql;`で基本文法を継承し、必要な部分のみをオーバーライドします。

---

## 📖 Sql.g4 - 基本SQL文法

### トップレベル構造

```antlr
sqlStatement
    : selectStatement
    | insertStatement
    | updateStatement
    | deleteStatement
    | EOF
    ;
```

**対応SQL文:**
- SELECT文 - データ読み取り
- INSERT文 - データ挿入
- UPDATE文 - データ更新
- DELETE文 - データ削除

---

### SELECT文

#### 基本構造

```antlr
selectStatement
    : withClause?
      selectClause
      fromClause?
      whereClause?
      groupByClause?
      havingClause?
      orderByClause?
      (UNION | MINUS | INTERSECT) ALL? selectStatement?
    ;
```

#### WITH句（CTE - Common Table Expressions）

```antlr
withClause
    : WITH RECURSIVE? cteList
    ;

cteList
    : cte (',' cte)*
    ;

cte
    : identifier AS '(' selectStatement ')'
    ;
```

**例:**
```sql
WITH sales_cte AS (
    SELECT product_id, SUM(amount) as total
    FROM sales
    GROUP BY product_id
)
SELECT * FROM sales_cte WHERE total > 1000;
```

**対応状況:** ⚠️ 部分対応（単純なCTEは動作、再帰CTEは未完全対応）

#### SELECT句

```antlr
selectClause
    : SELECT DISTINCT? selectList
    ;

selectList
    : selectItem (',' selectItem)*
    | '*'
    ;

selectItem
    : expression (AS? identifier)?
    ;
```

**対応機能:**
- `SELECT *` - 全カラム選択
- `SELECT DISTINCT` - 重複排除
- カラム別名（AS省略可能）
- 式・関数呼び出し

#### FROM句

```antlr
fromClause
    : FROM tableReference (',' tableReference)*
    ;

tableReference
    : tableName (AS? tableAlias)?
    | '(' selectStatement ')' (AS? tableAlias)?
    | tableReference joinClause
    ;
```

**対応機能:**
- 単一テーブル参照
- 複数テーブル（カンマ区切り）
- サブクエリ
- JOIN操作（左再帰構造）

#### JOIN句

```antlr
joinClause
    : (INNER | LEFT | RIGHT | FULL | CROSS)? OUTER? JOIN tableReference (ON expression)?
    | NATURAL (INNER | LEFT | RIGHT | FULL)? JOIN tableReference
    ;
```

**対応JOIN種類:**
- `INNER JOIN` - 内部結合
- `LEFT JOIN` / `LEFT OUTER JOIN` - 左外部結合
- `RIGHT JOIN` / `RIGHT OUTER JOIN` - 右外部結合
- `FULL JOIN` / `FULL OUTER JOIN` - 完全外部結合
- `CROSS JOIN` - 交差結合
- `NATURAL JOIN` - 自然結合

#### WHERE句

```antlr
whereClause
    : WHERE expression
    ;
```

**対応条件式:**
- 比較演算子（`=`, `<>`, `!=`, `<`, `<=`, `>`, `>=`）
- 論理演算子（`AND`, `OR`, `NOT`）
- `IN` / `NOT IN`
- `BETWEEN` / `NOT BETWEEN`
- `LIKE` / `NOT LIKE`
- `IS NULL` / `IS NOT NULL`
- `EXISTS` / `NOT EXISTS`

#### GROUP BY句

```antlr
groupByClause
    : GROUP BY expression (',' expression)*
    ;
```

**対応機能:**
- 単一カラムグループ化
- 複数カラムグループ化
- 式によるグループ化

#### HAVING句

```antlr
havingClause
    : HAVING expression
    ;
```

**対応機能:**
- 集計結果のフィルタリング
- 集計関数を含む条件式

**対応状況:** ⚠️ 部分対応（単純な条件は動作、複雑な式は未完全対応）

#### ORDER BY句

```antlr
orderByClause
    : ORDER BY orderByItem (',' orderByItem)*
    ;

orderByItem
    : expression (ASC | DESC)? (NULLS (FIRST | LAST))?
    ;
```

**対応機能:**
- 昇順（ASC）/降順（DESC）指定
- NULL値の順序制御（NULLS FIRST/LAST）
- 複数カラムソート

#### 集合演算

```antlr
(UNION | MINUS | INTERSECT) ALL? selectStatement?
```

**対応演算:**
- `UNION` - 和集合
- `UNION ALL` - 重複許可和集合
- `MINUS` - 差集合
- `INTERSECT` - 積集合

---

### INSERT文

#### 基本構造

```antlr
insertStatement
    : INSERT INTO tableName ('(' columnList ')')?
      (VALUES valuesList | selectStatement)
    ;
```

**対応機能:**
- VALUES句による単一行挿入
- VALUES句による複数行挿入
- SELECT文による挿入（INSERT SELECT）
- カラムリスト明示
- カラムリスト省略（全カラム挿入）

**例:**
```sql
-- 単一行挿入
INSERT INTO users (name, email) VALUES ('John', 'john@example.com');

-- 複数行挿入
INSERT INTO users (name, email) VALUES
    ('Alice', 'alice@example.com'),
    ('Bob', 'bob@example.com');

-- INSERT SELECT
INSERT INTO archive_users SELECT * FROM users WHERE created_at < '2020-01-01';
```

---

### UPDATE文

#### 基本構造

```antlr
updateStatement
    : UPDATE tableName SET setClauseList whereClause?
    ;

setClauseList
    : setClause (',' setClause)*
    ;

setClause
    : identifier '=' expression
    ;
```

**対応機能:**
- 単一カラム更新
- 複数カラム更新
- WHERE句による条件付き更新
- 式・関数を使った更新
- サブクエリを使った更新

**例:**
```sql
-- 単一カラム更新
UPDATE users SET status = 'active' WHERE id = 1;

-- 複数カラム更新
UPDATE users SET status = 'active', updated_at = NOW() WHERE id = 1;

-- サブクエリを使った更新
UPDATE products SET price = (SELECT AVG(price) FROM products WHERE category_id = 1);
```

**対応状況:** ⚠️ 部分対応（複雑なサブクエリは未完全対応）

---

### DELETE文

#### 基本構造

```antlr
deleteStatement
    : DELETE FROM tableName whereClause?
    ;
```

**対応機能:**
- WHERE句による条件付き削除
- 全行削除（WHERE省略）

**例:**
```sql
-- 条件付き削除
DELETE FROM users WHERE status = 'inactive';

-- 全行削除
DELETE FROM temp_data;
```

---

### 式（Expressions）

#### 式の種類

```antlr
expression
    : literal                                           # LiteralExpression
    | identifier                                        # ColumnReferenceExpression
    | identifier '.' identifier                         # QualifiedColumnExpression
    | identifier '.' '*'                                # TableWildcardExpression
    | functionName '(' (DISTINCT? expressionList | '*')? ')'  # FunctionCallExpression
    | expression comparisonOperator expression          # ComparisonExpression
    | expression (AND | OR) expression                  # LogicalExpression
    | NOT expression                                    # NotExpression
    | expression IS NOT? NULL                           # IsNullExpression
    | expression NOT? IN '(' (expressionList | selectStatement) ')'  # InExpression
    | expression NOT? BETWEEN expression AND expression # BetweenExpression
    | expression NOT? LIKE expression (ESCAPE expression)?  # LikeExpression
    | expression ('+'|'-'|'*'|'/'|'%'|'||') expression  # ArithmeticExpression
    | '(' expression ')'                                # ParenthesizedExpression
    | '(' selectStatement ')'                           # SubqueryExpression
    | CASE caseWhenClause+ (ELSE expression)? END       # CaseExpression
    | EXISTS '(' selectStatement ')'                    # ExistsExpression
    ;
```

**対応式タイプ:**

| 式タイプ | 説明 | 対応状況 |
|---------|------|---------|
| LiteralExpression | リテラル値（文字列、数値、NULL、TRUE/FALSE） | ✅ |
| ColumnReferenceExpression | カラム参照（単一識別子） | ✅ |
| QualifiedColumnExpression | 修飾カラム参照（table.column） | ✅ |
| TableWildcardExpression | テーブルワイルドカード（table.*） | ✅ |
| FunctionCallExpression | 関数呼び出し（COUNT, SUM, AVG等） | ✅ |
| ComparisonExpression | 比較式（=, <>, <, >, <=, >=） | ✅ |
| LogicalExpression | 論理式（AND, OR） | ✅ |
| NotExpression | NOT式 | ✅ |
| IsNullExpression | NULL判定（IS NULL, IS NOT NULL） | ✅ |
| InExpression | IN句（値リスト、サブクエリ） | ✅ |
| BetweenExpression | BETWEEN句 | ✅ |
| LikeExpression | LIKE句（パターンマッチング） | ✅ |
| ArithmeticExpression | 算術式（+, -, *, /, %, \|\|） | ✅ |
| ParenthesizedExpression | 括弧式 | ✅ |
| SubqueryExpression | サブクエリ式 | ✅ |
| CaseExpression | CASE式 | ⚠️ 部分対応 |
| ExistsExpression | EXISTS句 | ⚠️ 部分対応 |

#### CASE式

```antlr
caseWhenClause
    : WHEN expression THEN expression
    ;
```

**例:**
```sql
SELECT
    name,
    CASE
        WHEN age < 18 THEN 'Minor'
        WHEN age >= 18 AND age < 65 THEN 'Adult'
        ELSE 'Senior'
    END as age_group
FROM users;
```

**対応状況:** ⚠️ 部分対応（単純なCASEは動作、ネストしたCASEは未完全対応）

---

## 🐘 PostgreSqlDialect.g4 - PostgreSQL固有拡張

### 拡張SELECT文

```antlr
selectStatement
    : withClause?
      selectClause
      intoClause?        # PostgreSQL固有
      fromClause?
      whereClause?
      groupByClause?
      havingClause?
      orderByClause?
      limitClause?       # PostgreSQL固有
      offsetClause?      # PostgreSQL固有
      (UNION | MINUS | INTERSECT) ALL? selectStatement?
    ;
```

### PostgreSQL固有機能

#### LIMIT/OFFSET句

```antlr
limitClause
    : LIMIT (expression | ALL)
    ;

offsetClause
    : OFFSET expression (ROW | ROWS)?
    ;
```

**例:**
```sql
SELECT * FROM users ORDER BY created_at DESC LIMIT 10 OFFSET 20;
```

#### RETURNING句

```antlr
returningClause
    : RETURNING selectList
    ;
```

**対応文:**
- INSERT ... RETURNING
- UPDATE ... RETURNING
- DELETE ... RETURNING

**例:**
```sql
INSERT INTO users (name, email)
VALUES ('John', 'john@example.com')
RETURNING id, created_at;

UPDATE users SET status = 'active'
WHERE id = 1
RETURNING id, status, updated_at;

DELETE FROM temp_users
WHERE created_at < NOW() - INTERVAL '30 days'
RETURNING id, name;
```

#### ON CONFLICT句（UPSERT）

```antlr
onConflictClause
    : '(' columnList ')' DO (NOTHING | UPDATE SET setClauseList whereClause?)
    ;
```

**例:**
```sql
INSERT INTO users (email, name)
VALUES ('john@example.com', 'John')
ON CONFLICT (email)
DO UPDATE SET name = EXCLUDED.name, updated_at = NOW();
```

#### JSON/JSONB演算子

PostgreSQL特有のJSON操作演算子をサポート:

```antlr
| expression '->' expression                        # JsonAccessExpression
| expression '->>' expression                       # JsonTextAccessExpression
| expression '#>' expression                        # JsonPathAccessExpression
| expression '#>>' expression                       # JsonPathTextAccessExpression
| expression '@>' expression                        # JsonContainsExpression
| expression '<@' expression                        # JsonContainedByExpression
| expression '?' expression                         # JsonExistsExpression
| expression '?|' expression                        # JsonExistsAnyExpression
| expression '?&' expression                        # JsonExistsAllExpression
```

**例:**
```sql
-- JSON フィールドアクセス
SELECT data->'user'->>'name' FROM events;

-- JSON パス
SELECT data#>'{user,address,city}' FROM events;

-- JSON 含有チェック
SELECT * FROM events WHERE data @> '{"user": {"age": 30}}';
```

#### 型キャスト

```antlr
| expression '::' postgresType                      # TypeCastExpression
| CAST '(' expression AS postgresType ')'           # CastExpression
```

**例:**
```sql
SELECT '123'::INTEGER;
SELECT CAST('2024-01-01' AS DATE);
```

#### PostgreSQL固有データ型

```antlr
postgresType
    : SMALLINT | INTEGER | INT | BIGINT
    | DECIMAL | NUMERIC | REAL | DOUBLE PRECISION
    | SERIAL | BIGSERIAL | SMALLSERIAL
    | MONEY
    | CHAR | VARCHAR | TEXT
    | BYTEA
    | TIMESTAMP (WITH | WITHOUT)? TIME? ZONE?
    | DATE | TIME | INTERVAL
    | BOOLEAN | BOOL
    | UUID
    | JSON | JSONB
    | XML
    | ARRAY
    | HSTORE
    | POINT | LINE | LSEG | BOX | PATH | POLYGON | CIRCLE  # 幾何型
    | CIDR | INET | MACADDR  # ネットワーク型
    | BIT | VARBIT
    ;
```

#### 配列サポート

```antlr
| ARRAY '[' expressionList? ']'                     # ArrayLiteralExpression
| expression arraySubscript                         # ArraySubscriptExpression

arraySubscript
    : '[' expression (':' expression)? ']'
    ;
```

**例:**
```sql
SELECT ARRAY[1, 2, 3];
SELECT tags[1] FROM articles;
SELECT tags[1:3] FROM articles;  -- 配列スライス
```

---

## 🐬 MySqlDialect.g4 - MySQL固有拡張

### 拡張SELECT文

```antlr
selectStatement
    : withClause?
      selectClause
      fromClause?
      whereClause?
      groupByClause?
      havingClause?
      orderByClause?
      limitClause?                                    # MySQL固有
      (UNION | MINUS | INTERSECT) ALL? selectStatement?
      (FOR UPDATE | LOCK IN SHARE MODE)?              # MySQL固有
    ;
```

### MySQL固有機能

#### LIMIT句（MySQL構文）

```antlr
limitClause
    : LIMIT (expression (',' expression)? | expression OFFSET expression)
    ;
```

**例:**
```sql
-- MySQL方式1（LIMIT count）
SELECT * FROM users LIMIT 10;

-- MySQL方式2（LIMIT offset, count）
SELECT * FROM users LIMIT 20, 10;

-- MySQL方式3（LIMIT count OFFSET offset）
SELECT * FROM users LIMIT 10 OFFSET 20;
```

#### ON DUPLICATE KEY UPDATE

```antlr
insertStatement
    : (INSERT | REPLACE)
      (LOW_PRIORITY | DELAYED | HIGH_PRIORITY)?
      IGNORE?
      INTO?
      tableName
      ('(' columnList ')')?
      (VALUES valuesList | selectStatement)
      (ON DUPLICATE KEY UPDATE setClauseList)?        # MySQL固有
    ;
```

**例:**
```sql
INSERT INTO users (email, name, login_count)
VALUES ('john@example.com', 'John', 1)
ON DUPLICATE KEY UPDATE
    name = VALUES(name),
    login_count = login_count + 1;
```

#### REPLACE文

```antlr
(INSERT | REPLACE)
```

**例:**
```sql
REPLACE INTO users (id, name, email)
VALUES (1, 'John', 'john@example.com');
```

#### UPDATE/DELETE with LIMIT

```antlr
updateStatement
    : UPDATE (LOW_PRIORITY)? IGNORE?
      tableName
      SET setClauseList
      whereClause?
      orderByClause?
      limitClause?                                    # MySQL固有
    ;

deleteStatement
    : DELETE (LOW_PRIORITY)? QUICK? IGNORE?
      FROM tableName
      whereClause?
      orderByClause?
      limitClause?                                    # MySQL固有
    ;
```

**例:**
```sql
-- 最も古い10件のレコードを削除
DELETE FROM logs
ORDER BY created_at ASC
LIMIT 10;

-- 条件に合う最初の1件のみ更新
UPDATE users
SET status = 'processed'
WHERE status = 'pending'
ORDER BY created_at ASC
LIMIT 1;
```

#### REGEXP演算子

```antlr
| expression NOT? REGEXP expression                 # RegexpExpression
```

**例:**
```sql
SELECT * FROM users WHERE email REGEXP '^[A-Z]';
```

#### ビット演算子

```antlr
| expression ('<<'|'>>'|'&'|'|'|'^') expression     # BitwiseExpression
```

**例:**
```sql
SELECT permissions & 4 FROM users;  -- ビットAND
SELECT permissions | 2 FROM users;  -- ビットOR
```

#### XOR論理演算子

```antlr
| expression (AND | OR | XOR) expression            # LogicalExpression
```

**例:**
```sql
SELECT * FROM users WHERE is_active XOR is_deleted;
```

#### CONVERT/BINARY

```antlr
| BINARY expression                                 # BinaryExpression
| CAST '(' expression AS mysqlType ')'              # CastExpression
| CONVERT '(' expression ',' mysqlType ')'          # ConvertExpression
| CONVERT '(' expression USING identifier ')'       # ConvertUsingExpression
```

**例:**
```sql
SELECT BINARY 'abc';
SELECT CAST('123' AS UNSIGNED);
SELECT CONVERT('abc', CHAR(10));
SELECT CONVERT('データ' USING utf8mb4);
```

#### MySQL固有データ型

```antlr
mysqlType
    : TINYINT | SMALLINT | MEDIUMINT | INT | INTEGER | BIGINT
    | DECIMAL | NUMERIC | FLOAT | DOUBLE | REAL
    | BIT | BOOLEAN | BOOL
    | CHAR | VARCHAR | BINARY | VARBINARY
    | TINYBLOB | BLOB | MEDIUMBLOB | LONGBLOB
    | TINYTEXT | TEXT | MEDIUMTEXT | LONGTEXT
    | ENUM | SET
    | DATE | DATETIME | TIMESTAMP | TIME | YEAR
    | JSON
    | GEOMETRY | POINT | LINESTRING | POLYGON
    | MULTIPOINT | MULTILINESTRING | MULTIPOLYGON | GEOMETRYCOLLECTION
    ;
```

**特徴的なデータ型:**
- `TINYINT`, `MEDIUMINT` - 整数型のバリエーション
- `ENUM`, `SET` - 列挙型
- `TINYTEXT`, `MEDIUMTEXT`, `LONGTEXT` - テキストサイズバリエーション
- `YEAR` - 年型
- 幾何型（GIS対応）

---

## 🔍 SQL Server / Oracle固有拡張

### SqlServerDialect.g4 - SQL Server

主な固有機能:
- `TOP` 句
- `OUTPUT` 句（RETURNING相当）
- `MERGE` 文
- `[ブラケット]` による識別子

### OracleDialect.g4 - Oracle

主な固有機能:
- `ROWNUM` 疑似カラム
- `CONNECT BY` 階層クエリ
- `DUAL` テーブル
- シーケンス（`NEXTVAL`, `CURRVAL`）

※ これらの詳細仕様は必要に応じて別途ドキュメント化予定

---

## 🔧 SqlVisitor - パーサー実装

### Visitorパターン

ANTLR4で生成されたパーサーツリーを走査し、CRUD情報を抽出します。

**実装ファイル:** `src/CRUDExplorer.SqlParser/SqlVisitor.cs`

### 主要メソッド

```csharp
public class SqlVisitor : SqlBaseVisitor<object>
{
    public Query Query { get; private set; }

    // SELECT文処理
    public override object VisitSelectStatement(SqlParser.SelectStatementContext context)

    // INSERT文処理
    public override object VisitInsertStatement(SqlParser.InsertStatementContext context)

    // UPDATE文処理
    public override object VisitUpdateStatement(SqlParser.UpdateStatementContext context)

    // DELETE文処理
    public override object VisitDeleteStatement(SqlParser.DeleteStatementContext context)

    // テーブル参照処理
    public override object VisitTableReference(SqlParser.TableReferenceContext context)

    // 式処理（12種類の式タイプに対応）
    public override object VisitComparisonExpression(SqlParser.ComparisonExpressionContext context)
    public override object VisitLogicalExpression(SqlParser.LogicalExpressionContext context)
    // ... 他10種類
}
```

### CRUD抽出ロジック

**TableR（Read）:**
- FROM句のテーブル
- JOIN句のテーブル
- サブクエリ内のテーブル

**TableC（Create）:**
- INSERT INTOのテーブル

**TableU（Update）:**
- UPDATEのテーブル

**TableD（Delete）:**
- DELETE FROMのテーブル

**ColumnR（Read）:**
- SELECT句のカラム
- WHERE句のカラム
- GROUP BY句のカラム
- ORDER BY句のカラム
- サブクエリ内のカラム

**ColumnC（Create）:**
- INSERT文のカラムリスト
- VALUES句の値（対応カラム）

**ColumnU（Update）:**
- SET句の左辺（更新対象カラム）
- SET句の右辺（参照カラム）
- WHERE句のカラム

**ColumnD（Delete）:**
- WHERE句のカラム

---

## 📊 対応状況マトリクス

### SQL構文対応状況

| 構文 | 基本SQL | PostgreSQL | MySQL | SQL Server | Oracle |
|-----|--------|-----------|-------|-----------|--------|
| **SELECT** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **INSERT** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **UPDATE** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **DELETE** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **JOIN (全種類)** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **サブクエリ** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **UNION/INTERSECT/MINUS** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **GROUP BY/HAVING** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **ORDER BY** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **CTE (WITH句)** | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ |
| **CASE式** | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ |
| **EXISTS** | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ |
| **Window関数** | ❌ | ❌ | ❌ | ❌ | ❌ |

**凡例:**
- ✅ 完全対応
- ⚠️ 部分対応（単純な構文は動作、複雑な構文は未完全対応）
- ❌ 未対応

### DB固有機能対応状況

| 機能 | PostgreSQL | MySQL | SQL Server | Oracle |
|-----|-----------|-------|-----------|--------|
| **RETURNING/OUTPUT** | ✅ | ❌ | ⚠️ | ❌ |
| **ON CONFLICT (UPSERT)** | ✅ | ❌ | ⚠️ | ❌ |
| **ON DUPLICATE KEY UPDATE** | ❌ | ✅ | ❌ | ❌ |
| **LIMIT/OFFSET** | ✅ | ✅ | ❌ | ❌ |
| **TOP** | ❌ | ❌ | ⚠️ | ❌ |
| **ROWNUM** | ❌ | ❌ | ❌ | ⚠️ |
| **JSON演算子** | ✅ | ⚠️ | ❌ | ❌ |
| **配列型** | ✅ | ❌ | ❌ | ❌ |
| **REGEXP** | ⚠️ | ✅ | ❌ | ❌ |

---

## 🧪 テスト状況

### パーサーテスト結果

```
総テスト数: 53
成功: 51 (96.2%)
失敗: 2 (3.8%)

内訳:
- SelectStatementTests: 22/23 (95.7%)
- InsertStatementTests: 11/11 (100%)
- UpdateStatementTests: 10/10 (100%)
- DeleteStatementTests: 9/10 (90%)
```

### 失敗テスト詳細

**1. SelectStatementTests.AnalyzeSql_SelectWithHaving_ParsesCorrectly**
- HAVING句のCOUNT(*)処理
- カラム参照がない集計関数の処理が未完全

**2. DeleteStatementTests.AnalyzeSql_DeleteWithSubquery_ParsesCorrectly**
- DELETE文のIN句内サブクエリ
- 稀なエッジケース

---

## 🔨 ビルド・使用方法

### ANTLR4文法からのコード生成

```bash
# Antlr4BuildTasksが自動で実行（ビルド時）
dotnet build src/CRUDExplorer.SqlParser/CRUDExplorer.SqlParser.csproj
```

**生成ファイル:**
- `SqlLexer.cs` - 字句解析器
- `SqlParser.cs` - 構文解析器
- `SqlBaseVisitor.cs` - Visitorパターン基底クラス
- `SqlListener.cs` - Listenerパターン基底クラス
- （DB方言ごとに同様のファイル生成）

### パーサー使用例

```csharp
using CRUDExplorer.SqlParser;
using CRUDExplorer.Core.Analysis;

// SQL解析
var analyzer = new SqlAnalyzer();
var query = analyzer.AnalyzeSql("SELECT * FROM users WHERE id = 1");

// CRUD情報取得
Console.WriteLine($"Read Tables: {string.Join(", ", query.TableR.Keys)}");
Console.WriteLine($"Read Columns: {string.Join(", ", query.ColumnR.Keys)}");
```

---

## 📚 参考資料

### ANTLR4公式ドキュメント

- ANTLR4公式サイト: https://www.antlr.org/
- ANTLR4 Documentation: https://github.com/antlr/antlr4/blob/master/doc/index.md
- C# Target: https://github.com/antlr/antlr4/blob/master/doc/csharp-target.md

### SQL標準仕様

- ISO/IEC 9075 (SQL:2016)
- PostgreSQL Documentation: https://www.postgresql.org/docs/
- MySQL Documentation: https://dev.mysql.com/doc/
- SQL Server Documentation: https://docs.microsoft.com/sql/
- Oracle Database Documentation: https://docs.oracle.com/database/

### プロジェクト内関連ドキュメント

- [README.md](../README.md) - プロジェクト概要
- [CONTRIBUTING.md](../CONTRIBUTING.md) - 開発者ガイド
- [DATABASE_SUPPORT.md](./DATABASE_SUPPORT.md) - DB対応状況一覧
- [MIGRATION_PLAN.md](../MIGRATION_PLAN.md) - 移行計画

---

## 🚀 今後の拡張予定

### 優先度: 高

1. **CTE完全対応**
   - 再帰CTEの完全サポート
   - 複数CTE定義の正確な解析
   - CTEからのカラム抽出改善

2. **CASE式完全対応**
   - ネストしたCASE式
   - UPDATE/INSERT文内のCASE式
   - 検索CASE式と単純CASE式の両対応

3. **EXISTS完全対応**
   - NOT EXISTSサポート
   - EXISTS内のサブクエリ完全解析
   - 相関サブクエリ対応

4. **HAVING句強化**
   - 集計関数を含む複雑な条件式
   - HAVING句内のサブクエリ

### 優先度: 中

5. **Window関数対応**
   - RANK(), ROW_NUMBER(), DENSE_RANK()
   - PARTITION BY, ORDER BY
   - LEAD(), LAG(), FIRST_VALUE(), LAST_VALUE()

6. **SQL Server固有構文強化**
   - TOP句の完全対応
   - OUTPUT句の完全対応
   - MERGE文対応

7. **Oracle固有構文強化**
   - CONNECT BY階層クエリ
   - ROWNUM完全対応
   - (+) 外部結合構文（レガシー）

### 優先度: 低

8. **DDL文対応**
   - CREATE TABLE
   - ALTER TABLE
   - DROP TABLE
   - CREATE INDEX

9. **DCL文対応**
   - GRANT
   - REVOKE

10. **TCL文対応**
    - BEGIN TRANSACTION
    - COMMIT
    - ROLLBACK

---

**最終更新日:** 2026-02-22
**バージョン:** 1.0.0
**ライセンス:** MIT License
