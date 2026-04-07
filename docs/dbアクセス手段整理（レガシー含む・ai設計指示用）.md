# DBアクセス手段整理（レガシー含む・日本で利用の多い言語/フレームワーク拡張版・AI設計指示用）

## 1. 目的

本ドキュメントは、CRUD Explorer リプレースにおいて、プログラムコードからDBアクセスを解析するための対象パターンを網羅的に整理し、AIによる解析エンジン設計に利用することを目的とする。

特に以下を重視する。

- レガシーからモダンまで幅広い言語・フレームワークに対応すること
- 日本の業務システムで現実的に遭遇しやすい技術スタックを優先すること
- 単なる言語列挙ではなく、DBアクセス手段として抽象化して設計できること
- テーブル・カラム・CRUD種別・参照元プログラムを抽出可能な構造にすること

---

## 2. DBアクセス手段の統一分類（言語非依存）

以下の分類を、解析エンジンのコア抽象レイヤとして扱う。

### 2.1 Raw SQL

- 文字列として直接SQLを記述する方式
- SQLファイル読み込みも含む
- 例
  - `SELECT * FROM users`
  - `INSERT INTO orders (...) VALUES (...)`

### 2.2 Parameterized SQL

- プレースホルダ付きSQL
- named parameter / positional parameter
- prepared statement を含む

### 2.3 ORM

- エンティティ/モデル中心でCRUDする方式
- テーブルよりオブジェクトが主語になる

### 2.4 Object Query Language

- JPQL / HQL / LINQ / DQL など
- エンティティを対象に問い合わせる方式

### 2.5 Fluent / Query Builder / DSL

- メソッドチェーンやDSLでクエリを構築する方式
- いわゆる「流れるようなSQL」を含む

### 2.6 Mapper / 定義ファイル

- XML / Annotation / Attribute / 外部SQLファイル
- SQLとプログラムコードが分離される方式

### 2.7 Dynamic SQL

- 条件分岐、ループ、テンプレート展開でSQLを構築する方式
- 実行時に最終SQLが変わるものを含む

### 2.8 Stored Procedure / Function / Trigger

- DB側の処理呼び出し
- アプリ側はCALLやEXECのみでも、内部SQL解析が必要になる

### 2.9 Migration / Schema / DDL

- CREATE / ALTER / DROP
- テーブル定義、インデックス定義、制約定義、ビュー定義

### 2.10 Metadata / Convention Based Access

- メソッド名や設定規約からクエリが自動生成される方式
- 例
  - Spring Data JPA
  - Rails Active Record の convention
  - Laravel Eloquent の relation/magic method

---

## 3. 優先度の考え方（日本の業務システム前提）

日本の業務システムや保守案件で遭遇しやすいものを、概ね以下のように整理する。

### 3.1 最優先で厚く見るべき領域

- Java
  - JDBC
  - JPA / Hibernate
  - MyBatis
  - Spring Data JPA
  - jOOQ
- C# / .NET
  - ADO.NET
  - EF / EF Core
  - Dapper
  - LINQ
- PHP
  - PDO
  - Laravel
  - CakePHP
  - Doctrine
- Ruby
  - Ruby on Rails / Active Record
  - Sequel
- Go
  - `database/sql`
  - `sqlx`
  - GORM
  - `pgx`
- Python
  - DB-API
  - SQLAlchemy
  - Django ORM
- レガシー
  - COBOL + Embedded SQL
  - VB6 / VBA + ADO / DAO
  - C / C++ + ODBC / ESQL
  - Classic ASP
  - PowerBuilder
  - Access

### 3.2 日本で遭遇率が高い・意識したいフレームワーク/系統

- Java: Spring / Spring Boot + JPA / MyBatis
- PHP: Laravel, CakePHP, FuelPHP, Symfony, Doctrine
- Ruby: Ruby on Rails
- C#: ASP.NET + EF Core / Dapper
- Python: Django
- Node.js: TypeORM, Prisma, Sequelize, Knex
- Go: GORM, sqlx, pgx

---

## 4. モダン言語・主要フレームワーク

## 4.1 Java

### 主なDBアクセス手段

- JDBC
  - `Statement`
  - `PreparedStatement`
  - `CallableStatement`
- JPA
  - `EntityManager`
  - JPQL
  - Criteria API
- Hibernate
  - HQL
  - native SQL
- Spring Data JPA
  - Repository interface
  - method name derived query
  - `@Query`
- MyBatis
  - XML mapper
  - annotation mapper
  - dynamic SQL (`if`, `choose`, `foreach`)
- jOOQ
  - DSLベース
- Querydsl
  - 型安全クエリDSL
- R2DBC
  - リアクティブDBアクセス

### 特徴

- レガシーとモダンが混在しやすい
- SQL直書き、ORM、DSL、XML mapper が同一案件に共存しやすい
- 日本の業務システムでは Spring + MyBatis / JPA の組み合わせが多い

### 解析ポイント

- SQL文字列リテラル
- `createQuery`, `createNativeQuery`
- `CriteriaBuilder`
- `@Query`
- mapper XML
- `DSLContext`
- Repository interface のメソッド名解析

---

## 4.2 C# / .NET

### 主なDBアクセス手段

- ADO.NET
  - `DbConnection`
  - `DbCommand`
  - `DbDataReader`
  - `DataSet`, `DataTable`
- Entity Framework / EF Core
  - `DbSet`
  - LINQ
  - `FromSql`
  - raw SQL
- Dapper
  - SQL文字列 + POCOマッピング
- LINQ to SQL
- Stored Procedure 呼び出し

### 特徴

- LINQ式木の解析が重要
- EF Core と Dapper の併用が多い
- ストアド主体の業務システムも多い

### 解析ポイント

- `ExecuteReader`, `ExecuteNonQuery`
- `FromSql`, `ExecuteSql`
- `IQueryable`
- Dapper の `Query`, `Execute`
- `CommandType.StoredProcedure`

---

## 4.3 Python

### 主なDBアクセス手段

- DB-API
  - `cursor.execute`
  - `executemany`
- SQLAlchemy
  - Core
  - ORM
  - expression language
- Django ORM
  - QuerySet
  - relation traversal
  - raw SQL
- Peewee
- async 系
  - SQLAlchemy async
  - asyncpg

### 特徴

- ORMと生SQLの併用が多い
- バッチ、分析系、管理系ツールで使われやすい

### 解析ポイント

- `cursor.execute(...)`
- f-string や format によるSQL組み立て
- `select()`, `session.query()`
- `Model.objects.filter(...)`
- migration ファイル

---

## 4.4 Node.js / TypeScript

### 主なDBアクセス手段

- raw driver
  - `pg`
  - `mysql2`
  - `mssql`
  - `sqlite3`
- Prisma
- TypeORM
- Sequelize
- Knex
- Objection.js
- Drizzle ORM

### 特徴

- query builder / ORM / raw SQL の混在が多い
- template literal にSQLが埋まりやすい
- TypeScript では型安全ORMやDSLが増えている

### 解析ポイント

- template literal SQL
- tagged template SQL
- Prisma Client 呼び出し
- TypeORM repository / query builder
- Sequelize model method
- migration / schema 定義

---

## 4.5 PHP

### 主なDBアクセス手段

- PDO
  - `prepare`
  - `execute`
  - `query`
- mysqli
- Laravel
  - Eloquent ORM
  - Query Builder
  - `DB::select`, `DB::statement`
  - migration
- CakePHP
  - ORM
  - Query Builder
  - Table / Entity パターン
- Doctrine ORM / DBAL
  - EntityManager
  - DQL
  - QueryBuilder
- Symfony + Doctrine
- FuelPHP
  - ORM / DB クラス
- CodeIgniter
  - Query Builder
- raw SQL ファイル / 直書きSQL

### 特徴

- 日本では Laravel と CakePHP の遭遇率が高い
- 古い案件では mysqli / PDO 直叩きが多い
- 画面ロジックとSQLが近接しているコードも多い
- query builder と生SQLが混在しやすい

### 解析ポイント

- `PDO::prepare`, `PDO::query`
- `mysqli_query`
- `DB::table()`, `DB::raw()`, `DB::select()`
- Eloquent relation / scope
- CakePHP Table / Query object
- Doctrine DQL / QueryBuilder
- migration, seeder, schema ファイル

---

## 4.6 Go

### 主なDBアクセス手段

- 標準 `database/sql`
  - `Query`
  - `Exec`
  - `Prepare`
- `sqlx`
- `pgx`
- GORM
- Ent
- SQLBoiler
- Bun
- XORM

### 特徴

- フレームワークよりライブラリ中心で構成されることが多い
- 標準SQL API と ORM/コード生成ツールが並立する
- 日本では GORM と `database/sql` / `sqlx` の遭遇率が高め

### 解析ポイント

- `db.Query`, `db.Exec`, `db.Prepare`
- `sqlx.Select`, `sqlx.Get`
- `pgxpool.Query`
- `gorm.DB.Where(...).Find(...)`
- Ent / SQLBoiler の generated code と query API
- migration ツール定義

---

## 4.7 Ruby

### 主なDBアクセス手段

- Ruby on Rails
  - Active Record
  - association
  - scope
  - migration
  - `find_by_sql`
- Arel
- Sequel
- raw SQL

### 特徴

- 日本では Ruby = Rails の比率がかなり高い
- Active Record の暗黙規約が強い
- relation と scope の連鎖が重要

### 解析ポイント

- `where`, `joins`, `includes`, `pluck`, `select`
- scope 定義
- association 経由アクセス
- `find_by_sql`
- Arel 構築式
- migration / schema.rb

---

## 5. レガシー言語・環境

## 5.1 C / C++

### 主な手段

- ODBC API
  - `SQLExecDirect`
  - `SQLPrepare` / `SQLExecute`
- Embedded SQL (ESQL/C)
  - `EXEC SQL SELECT ...`
- DB固有API
  - Oracle OCI
  - DB2 CLI
  - Pro*C

### 特徴

- SQLがマクロや文字列に分散しやすい
- バインド変数や結果受け取りが手動管理
- レガシー資産では still active なケースがある

### 解析ポイント

- `EXEC SQL` ブロック検出
- ODBC関数呼び出しトレース
- 文字列断片結合の復元
- マクロ展開前後の考慮

---

## 5.2 Visual Basic (VB6 / VBA / VB.NET)

### 主な手段

- ADO
  - `Connection`
  - `Command`
  - `Recordset`
- DAO
- ADO.NET（VB.NET）
- typed dataset

### 特徴

- VB6/VBA は依然として社内ツールやAccess連携で残存しやすい
- SQL文字列連結が多い
- UIコード、帳票ロジック、DBアクセスが密結合しやすい

### 解析ポイント

- `.Execute`, `.Open`
- `CommandText`
- SQL文字列連結
- `RecordSource`
- フォーム/レポート定義内SQL

---

## 5.3 COBOL

### 主な手段

- Embedded SQL
  - `EXEC SQL ... END-EXEC`
- DB2 / Oracle / HiRDB などとの連携
- ファイルI/O主体だが一部DBアクセス混在の案件もある

### 特徴

- ホスト変数ベース
- バッチ処理主体
- 帳票系、基幹系、長寿命システムで残りやすい

### 解析ポイント

- `EXEC SQL` 構文抽出
- ホスト変数とカラムの対応付け
- COPY句や共通定義の解決
- カーソル宣言/OPEN/FETCH/CLOSE の追跡

---

## 5.4 PL/SQL / T-SQL / PLpgSQL などDB内プログラム

### 主な手段

- ストアドプロシージャ
- ファンクション
- トリガー
- パッケージ
- ビュー定義

### 特徴

- アプリ側からは `CALL` / `EXEC` のみでも実際の影響範囲は広い
- DB変更影響調査では必須

### 解析ポイント

- PROCEDURE/FUNCTION 定義解析
- 内部SQL展開
- 呼び出しチェーン解析
- 動的SQL (`EXECUTE IMMEDIATE` など) の扱い

---

## 5.5 Classic ASP

### 主な手段

- ADO
- VBScript + SQL文字列
- include ファイル経由の共通DB処理

### 特徴

- HTMLとSQLが混在
- SQL直書きが多い
- 画面単位で複数クエリが埋め込まれやすい

### 解析ポイント

- `conn.Execute`
- `rs.Open`
- include 展開
- HTML/VBScript 混在構文からの抽出

---

## 5.6 Delphi / Pascal

### 主な手段

- BDE
- FireDAC
- dbExpress
- `TQuery`, `TTable`, `TADOQuery`

### 特徴

- コンポーネントプロパティにSQLが入ることが多い
- フォーム定義ファイルにSQLが存在することがある

### 解析ポイント

- `.SQL.Text`
- コンポーネント定義
- `.dfm` / `.fmx` の解析

---

## 5.7 PowerBuilder

### 主な手段

- DataWindow
- Embedded SQL
- transaction object

### 特徴

- GUIとSQLが密結合
- 業務系で長く残っていることがある

### 解析ポイント

- DataWindow 定義内SQL
- embedded SQL
- object script からのDB呼び出し

---

## 5.8 Access (VBA + MDB/ACCDB)

### 主な手段

- DAO / ADO
- saved query object
- フォーム/レポートのRecordSource

### 特徴

- SQLがアプリコードだけでなくDBファイル内オブジェクトに存在
- 小規模業務システムで今も多い

### 解析ポイント

- VBA内の `CurrentDb.Execute`
- 保存済みクエリ抽出
- フォーム/レポート定義内SQL

---

## 5.9 Perl

### 主な手段

- DBI
- DBD::* ドライバ
- CGI / バッチ内のSQL直書き
- ORM系
  - DBIx::Class

### 特徴

- 日本の古いWebシステムやバッチで残っていることがある
- SQL文字列直書きが多い

### 解析ポイント

- `prepare`, `execute`
- heredoc SQL
- CGIスクリプト内の動的SQL
- DBIx::Class の schema / result class

---

## 5.10 Shell / バッチスクリプト

### 主な手段

- `sqlplus`
- `psql`
- `mysql` コマンド
- `.sql` ファイル実行
- ジョブ定義内SQL

### 特徴

- 運用バッチや定期処理に多い
- アプリ外のDB更新源として重要

### 解析ポイント

- SQLファイル参照関係
- shell 変数埋め込み
- 実行対象SQLの特定

---

## 6. 日本で意識したい主要フレームワーク・ライブラリ一覧

| 言語 | 特に意識したいもの |
|---|---|
| Java | Spring Boot, Spring Data JPA, MyBatis, Hibernate, jOOQ, Querydsl |
| C# | ASP.NET, EF Core, Dapper, LINQ, ADO.NET |
| PHP | Laravel, CakePHP, Symfony, Doctrine, FuelPHP, CodeIgniter |
| Ruby | Ruby on Rails, Active Record, Arel, Sequel |
| Go | database/sql, sqlx, GORM, pgx, Ent, SQLBoiler |
| Python | Django, SQLAlchemy, Peewee |
| Node.js | Prisma, TypeORM, Sequelize, Knex, Drizzle |
| Legacy | ADO, DAO, ODBC, ESQL/C, COBOL Embedded SQL, DataWindow |

---

## 7. 横断的に難しいパターン

## 7.1 SQL断片の分割

- 文字列連結
- 配列/リスト結合
- 条件付きappend
- 定数クラスや共通定義への分散

## 7.2 条件分岐SQL

- `if`
- `switch`
- ternary
- polymorphism / strategy による分岐

## 7.3 テンプレート生成

- MyBatis
- Freemarker
- Velocity
- JSP / ASP / ERB / Blade

## 7.4 DSL / Fluent API

- LINQ
- jOOQ
- Knex
- SQLAlchemy expression
- Querydsl
- CakePHP Query
- Doctrine QueryBuilder

## 7.5 自動生成クエリ

- メソッド名規約
- association / relation
- convention based ORM

## 7.6 アノテーション/属性/設定ファイル

- `@Query`
- XML mapper
- Doctrine mapping
- Active Record convention
- Laravel relation 定義

## 7.7 外部資産にSQLがあるケース

- `.sql`
- XML
- DFM/FM X
- Access query object
- DataWindow definition
- migration ファイル

---

## 8. AI設計指針

## 8.1 基本アーキテクチャ

```text
Parser Layer（言語別）
  ↓
Access Pattern Resolver（ORM / SQL / DSL / Mapper）
  ↓
Intermediate Representation（IR）
  ↓
SQL Normalizer / Query Reconstructor
  ↓
Dependency Analyzer
  ↓
Impact Analysis / Visualization
```

---

## 8.2 IR（中間表現）で保持すべき主項目

- language
- framework_or_library
- source_file
- source_symbol
  - class
  - method
  - function
  - procedure
  - script
- line_range
- access_style
  - raw_sql
  - orm
  - query_builder
  - stored_proc
  - migration
- operation
  - SELECT
  - INSERT
  - UPDATE
  - DELETE
  - MERGE
  - DDL
- target_tables
- target_columns
- conditions
- joins
- order_by
- group_by
- call_chain
- unresolved_parts
- confidence

---

## 8.3 プラグイン構成

| レイヤ | 内容 |
|---|---|
| Language Adapter | Java / C# / PHP / Ruby / Go / Python / C / COBOL など |
| Access Pattern Plugin | Raw SQL / ORM / OQL / DSL / Mapper / Stored Proc |
| Query Reconstruction Plugin | 動的SQLやDSLから最終クエリ候補を再構築 |
| Schema Resolver | DBスキーマと照合してテーブル/カラム名を補完 |
| Impact Engine | テーブル・カラム変更の影響範囲を算出 |

---

## 8.4 解析モード

### 静的解析モード

- ソースコードだけで解析
- まずはこれがベース

### 準静的解析モード

- 定数展開
- include / import / mapper 解決
- migration / schema 参照

### 補助的実行時解析モード

- ログやトレースから実SQLを回収
- 静的解析で取りきれない動的SQLを補完

---

## 8.5 優先対応順

### Tier 1（最優先）

- Java
  - JDBC
  - Spring Data JPA
  - Hibernate / JPA
  - MyBatis
- C#
  - ADO.NET
  - EF Core
  - Dapper
- PHP
  - PDO
  - Laravel
  - CakePHP
- Ruby
  - Rails / Active Record
- Go
  - `database/sql`
  - `sqlx`
  - GORM

### Tier 2（高優先）

- Python
  - DB-API
  - SQLAlchemy
  - Django ORM
- Node.js
  - Prisma
  - TypeORM
  - Sequelize
  - Knex
- COBOL
- VB6 / VBA / VB.NET
- C / C++

### Tier 3（必要に応じて）

- Delphi
- PowerBuilder
- Classic ASP
- Access
- Perl
- Shell / バッチ
- DB内プログラムの高度解析

---

## 9. 重要な設計思想

1. 言語ではなく、まずDBアクセス手段で分類する
2. ただし実装上は、日本で遭遇率の高い言語・フレームワークを優先実装する
3. SQLを最終的に正規化し、テーブル・カラム単位で比較可能にする
4. 動的SQLは「完全解決できないこと」を前提に、候補集合と不確実性を保持する
5. ORMやDSLも、最終的には同じIRに落とし込む
6. コード外にあるSQL資産も解析対象に含める
7. DB変更影響調査のため、ストアド・ビュー・トリガーも対象にする
8. 解析不能箇所は隠さず `unresolved_parts` と `confidence` で明示する

---

## 10. 次ステップ

- 各言語/フレームワークごとのAST・抽出ポイント定義
- IRのJSONスキーマ策定
- 動的SQL再構築アルゴリズム設計
- ORM/DSLごとのテーブル・カラム解決戦略定義
- migration / schema / stored procedure との統合設計
- 影響調査画面に必要な検索軸の定義
  - テーブル名
  - カラム名
  - CRUD種別
  - プログラム名
  - フレームワーク
  - 信頼度

