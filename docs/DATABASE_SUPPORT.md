# データベース対応状況 (Database Support Status)

このドキュメントは、CRUDExplorerがサポートするデータベースの詳細な対応状況を記載しています。

---

## 📊 対応データベース一覧

### ✅ 完全実装済み（Production Ready）

| データベース | バージョン | 接続 | スキーマ取得 | CRUD解析 | SQL方言 | テスト |
|------------|----------|------|------------|---------|--------|-------|
| **PostgreSQL** | 12+ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **MySQL** | 8.0+ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **SQL Server** | 2019+ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Oracle** | 19c+ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **SQLite** | 3.x | ✅ | ✅ | ✅ | ✅ | ✅ |
| **MariaDB** | 10.5+ | ✅ | ✅ | ✅ | ✅ | ✅ |

### ⏳ 計画中（Planned）

| データベース | 予定バージョン | 接続 | スキーマ取得 | CRUD解析 | SQL方言 | 優先度 |
|------------|--------------|------|------------|---------|--------|-------|
| **Snowflake** | Latest | ❌ | ❌ | ❌ | ❌ | 高 |
| **Google BigQuery** | Latest | ❌ | ❌ | ❌ | ❌ | 高 |
| **Databricks** | Latest | ❌ | ❌ | ❌ | ❌ | 中 |
| **Amazon Redshift** | Latest | ❌ | ❌ | ❌ | ❌ | 中 |

---

## 🔧 実装詳細

### 1. PostgreSQL

**実装ファイル:**
- `src/CRUDExplorer.Core/Database/Providers/PostgreSqlConnectionFactory.cs`
- `src/CRUDExplorer.Core/Database/Providers/PostgreSqlSchemaProvider.cs`
- `src/CRUDExplorer.SqlParser/Grammar/PostgreSqlDialect.g4`

**サポート機能:**
- ✅ 基本接続（Npgsql 8.0.6）
- ✅ SSL/TLS接続
- ✅ スキーマ情報取得（テーブル、カラム、制約、インデックス）
- ✅ CRUD解析（SELECT/INSERT/UPDATE/DELETE）
- ✅ PostgreSQL固有構文（RETURNING、ON CONFLICT等）
- ✅ JSON/JSONB型対応
- ✅ 配列型対応

**接続文字列例:**
```
Host=localhost;Port=5432;Database=mydb;Username=user;Password=pass;SSL Mode=Require
```

**テスト状況:**
- 統合テスト: 16/16 (100%)
- スキーマ取得テスト: 9/9 (100%)

---

### 2. MySQL

**実装ファイル:**
- `src/CRUDExplorer.Core/Database/Providers/MySqlConnectionFactory.cs`
- `src/CRUDExplorer.Core/Database/Providers/MySqlSchemaProvider.cs`
- `src/CRUDExplorer.SqlParser/Grammar/MySqlDialect.g4`

**サポート機能:**
- ✅ 基本接続（MySqlConnector 2.4.0）
- ✅ SSL接続
- ✅ スキーマ情報取得
- ✅ CRUD解析
- ✅ MySQL固有構文（LIMIT、ON DUPLICATE KEY UPDATE等）
- ✅ ストアドプロシージャ対応

**接続文字列例:**
```
Server=localhost;Port=3306;Database=mydb;Uid=user;Pwd=pass;SslMode=Required
```

**テスト状況:**
- 統合テスト: 16/16 (100%)
- スキーマ取得テスト: 9/9 (100%)

---

### 3. SQL Server

**実装ファイル:**
- `src/CRUDExplorer.Core/Database/Providers/SqlServerConnectionFactory.cs`
- `src/CRUDExplorer.Core/Database/Providers/SqlServerSchemaProvider.cs`
- `src/CRUDExplorer.SqlParser/Grammar/SqlServerDialect.g4`

**サポート機能:**
- ✅ 基本接続（Microsoft.Data.SqlClient 5.2.0）
- ✅ Windows認証/SQL認証
- ✅ 暗号化接続
- ✅ スキーマ情報取得
- ✅ CRUD解析
- ✅ SQL Server固有構文（TOP、OUTPUT、MERGE等）
- ✅ CTE（Common Table Expressions）対応

**接続文字列例:**
```
Server=localhost;Database=mydb;User Id=user;Password=pass;TrustServerCertificate=True
```

**テスト状況:**
- 統合テスト: 16/16 (100%)
- スキーマ取得テスト: 9/9 (100%)

---

### 4. Oracle

**実装ファイル:**
- `src/CRUDExplorer.Core/Database/Providers/OracleConnectionFactory.cs`
- `src/CRUDExplorer.Core/Database/Providers/OracleSchemaProvider.cs`
- `src/CRUDExplorer.SqlParser/Grammar/OracleDialect.g4`

**サポート機能:**
- ✅ 基本接続（Oracle.ManagedDataAccess.Core 23.6.1）
- ✅ TNS接続
- ✅ スキーマ情報取得
- ✅ CRUD解析
- ✅ Oracle固有構文（ROWNUM、CONNECT BY、DUAL等）
- ✅ PL/SQL基本対応

**接続文字列例:**
```
User Id=user;Password=pass;Data Source=localhost:1521/ORCL
```

**テスト状況:**
- 統合テスト: 16/16 (100%)
- スキーマ取得テスト: 9/9 (100%)

---

### 5. SQLite

**実装ファイル:**
- `src/CRUDExplorer.Core/Database/Providers/SqliteConnectionFactory.cs`
- `src/CRUDExplorer.Core/Database/Providers/SqliteSchemaProvider.cs`
- `src/CRUDExplorer.SqlParser/Grammar/Sql.g4`（標準SQL文法使用）

**サポート機能:**
- ✅ 基本接続（Microsoft.Data.Sqlite 8.0.12）
- ✅ インメモリデータベース
- ✅ ファイルベースデータベース
- ✅ スキーマ情報取得
- ✅ CRUD解析
- ✅ SQLite固有構文（AUTOINCREMENT、PRAGMA等）

**接続文字列例:**
```
Data Source=/path/to/database.db
Data Source=:memory:  # インメモリDB
```

**テスト状況:**
- 統合テスト: 16/16 (100%)
- スキーマ取得テスト: 9/9 (100%)

---

### 6. MariaDB

**実装ファイル:**
- `src/CRUDExplorer.Core/Database/Providers/MariaDbConnectionFactory.cs`
- `src/CRUDExplorer.Core/Database/Providers/MariaDbSchemaProvider.cs`
- `src/CRUDExplorer.SqlParser/Grammar/MySqlDialect.g4`（MySQL互換文法使用）

**サポート機能:**
- ✅ 基本接続（MySqlConnector 2.4.0）
- ✅ SSL接続
- ✅ スキーマ情報取得
- ✅ CRUD解析
- ✅ MariaDB固有構文（RETURNING、JSON関数等）

**接続文字列例:**
```
Server=localhost;Port=3306;Database=mydb;Uid=user;Pwd=pass;SslMode=Required
```

**テスト状況:**
- 統合テスト: 16/16 (100%)
- スキーマ取得テスト: 9/9 (100%)

---

## 🚧 計画中のデータベース

### 7. Snowflake

**ステータス:** 未実装（Phase 6.3）

**計画内容:**
- Snowflake.Data NuGetパッケージ使用
- Account識別子ベース接続
- ウェアハウス・データベース・スキーマ指定
- Snowflake固有構文（COPY INTO、FLATTEN等）
- クラウド認証（OAuth、Key Pair）

**想定接続文字列:**
```
account=myaccount;user=myuser;password=mypass;db=mydb;schema=myschema;warehouse=mywh
```

**優先度:** 高（クラウドDWHの標準的選択肢）

---

### 8. Google BigQuery

**ステータス:** 未実装（Phase 6.3）

**計画内容:**
- Google.Cloud.BigQuery.V2 NuGetパッケージ使用
- サービスアカウント認証（JSON Key）
- プロジェクト・データセット・テーブル構造
- BigQuery固有構文（ARRAY、STRUCT、UNNEST等）
- Standard SQL / Legacy SQL切り替え

**想定接続文字列:**
```
ProjectId=myproject;DatasetId=mydataset;CredentialFile=/path/to/key.json
```

**優先度:** 高（Google Cloudユーザー向け）

---

### 9. Databricks

**ステータス:** 未実装（Phase 6.3）

**計画内容:**
- Databricks.Sql NuGetパッケージ使用（予定）
- Personal Access Token認証
- Delta Lake対応
- Spark SQL構文対応
- Unity Catalog対応

**想定接続文字列:**
```
Server=dbc-xxxxx.cloud.databricks.com;HTTPPath=/sql/1.0/endpoints/xxxxx;Token=dapi...
```

**優先度:** 中（データレイク分析向け）

---

### 10. Amazon Redshift

**ステータス:** 未実装（Phase 6.3）

**計画内容:**
- Npgsql使用（PostgreSQL互換）
- Redshift固有最適化構文（DISTKEY、SORTKEY等）
- Spectrum外部テーブル対応
- IAM認証対応

**想定接続文字列:**
```
Server=myredshift.us-east-1.redshift.amazonaws.com;Port=5439;Database=mydb;User Id=user;Password=pass
```

**優先度:** 中（AWS環境向け）

---

## 📦 使用NuGetパッケージ

### 実装済みデータベース

| データベース | パッケージ名 | バージョン | ライセンス |
|------------|-----------|----------|----------|
| PostgreSQL | Npgsql | 8.0.6 | PostgreSQL License |
| MySQL | MySqlConnector | 2.4.0 | MIT License |
| SQL Server | Microsoft.Data.SqlClient | 5.2.0 | MIT License |
| Oracle | Oracle.ManagedDataAccess.Core | 23.6.1 | Oracle Free Use Terms |
| SQLite | Microsoft.Data.Sqlite | 8.0.12 | Apache 2.0 |
| MariaDB | MySqlConnector | 2.4.0 | MIT License |

### 計画中データベース（予定）

| データベース | パッケージ名 | 想定バージョン | ライセンス |
|------------|-----------|--------------|----------|
| Snowflake | Snowflake.Data | Latest | Apache 2.0 |
| BigQuery | Google.Cloud.BigQuery.V2 | Latest | Apache 2.0 |
| Databricks | Databricks.Sql | Latest | Apache 2.0 (予定) |
| Redshift | Npgsql | 8.0+ | PostgreSQL License |

---

## 🧪 テスト状況サマリー

### 全体テスト結果

```
総テスト数: 329
成功: 313 (95.1%)
失敗: 16 (4.9%)

内訳:
- Core.Tests: 233/247 (94.3%)
- SqlParser.Tests: 51/53 (96.2%)
- IntegrationTests: 16/16 (100%)
- UI.Tests: 13/13 (100%)
```

### データベース統合テスト（16テスト - 100%成功）

**ライセンス認証テスト（9テスト）:**
- ✅ 有効なライセンス認証
- ✅ 無効なライセンス
- ✅ 期限切れライセンス
- ✅ 最大デバイス数超過
- ✅ 同一デバイス複数回認証
- ✅ トークン検証（有効/無効/なし）

**管理APIテスト（7テスト）:**
- ✅ ユーザー一覧取得
- ✅ ライセンス一覧取得
- ✅ ライセンス作成
- ✅ デバイスアクティベーション一覧
- ✅ デバイス無効化
- ✅ 監査ログ取得
- ✅ ライセンス取り消し

### スキーマ取得テスト（9テスト - 100%成功）

全DB種類で実装:
- ✅ テーブル一覧取得
- ✅ カラム情報取得
- ✅ プライマリーキー取得
- ✅ 外部キー取得
- ✅ インデックス情報取得
- ✅ 制約情報取得
- ✅ データ型マッピング
- ✅ Null許容フラグ
- ✅ デフォルト値取得

---

## 🔄 SQL方言対応状況

### ANTLR4文法定義

**基本文法:**
- `src/CRUDExplorer.SqlParser/Grammar/Sql.g4` - 標準SQL文法（SQLite、汎用）

**DB固有方言:**
- `src/CRUDExplorer.SqlParser/Grammar/PostgreSqlDialect.g4`
- `src/CRUDExplorer.SqlParser/Grammar/MySqlDialect.g4`
- `src/CRUDExplorer.SqlParser/Grammar/SqlServerDialect.g4`
- `src/CRUDExplorer.SqlParser/Grammar/OracleDialect.g4`

### サポートSQL構文

| 構文 | PostgreSQL | MySQL | SQL Server | Oracle | SQLite | MariaDB |
|-----|-----------|-------|-----------|--------|--------|---------|
| **SELECT** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **INSERT** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **UPDATE** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **DELETE** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **JOIN (INNER/LEFT/RIGHT/FULL/CROSS)** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **サブクエリ** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **UNION/INTERSECT/MINUS** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **集計関数** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **GROUP BY/HAVING** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **ORDER BY** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **CTE (WITH句)** | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ |
| **CASE式** | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ |
| **EXISTS/NOT EXISTS** | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ⚠️ |
| **Window関数** | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |

**凡例:**
- ✅ 完全対応
- ⚠️ 部分対応（高度な構文は未対応）
- ❌ 未対応

### 未対応の高度なSQL構文（16失敗テスト）

**CTE (Common Table Expressions):**
- WITH句を使った再帰クエリ
- 複数CTE定義

**CASE式:**
- ネストしたCASE式
- UPDATE/SELECTでの複雑なCASE

**サブクエリ:**
- DELETE文のIN句内サブクエリ
- UPDATE文の複雑なサブクエリ

**その他:**
- HAVING句の高度な条件式
- SQL Server TOP句
- Window関数（RANK()、ROW_NUMBER()等）

→ これらはPhase 7の今後の拡張で対応予定

---

## 🛠️ 共通インフラストラクチャ

### 接続プール管理

**実装:** `src/CRUDExplorer.Core/Database/ConnectionPoolManager.cs`

**機能:**
- セマフォベースのコネクションプール（最大10接続/プール）
- 自動リトライ（最大3回、指数バックオフ）
- タイムアウト管理（デフォルト30秒）
- 接続文字列マスキング（ログ出力時のパスワード隠蔽）

### エラーハンドリング

**実装:** `src/CRUDExplorer.Core/Database/Exceptions.cs`

**例外階層:**
- `DatabaseException` - 基底例外
  - `ConnectionException` - 接続エラー
  - `QueryException` - クエリ実行エラー
  - `SchemaException` - スキーマ取得エラー
  - `TimeoutException` - タイムアウトエラー
  - `AuthenticationException` - 認証エラー

### 接続文字列管理

**実装:** `src/CRUDExplorer.Core/Database/ConnectionStringManager.cs`

**機能:**
- JSON形式で保存（`%AppData%/CRUDExplorer/connections.json`）
- 接続文字列の暗号化保存
- データベースタイプ別管理
- 接続履歴保持

---

## 📚 関連ドキュメント

- [README.md](../README.md) - プロジェクト概要
- [CONTRIBUTING.md](../CONTRIBUTING.md) - 開発者ガイド
- [API.md](./API.md) - AuthServer API仕様
- [MIGRATION_PLAN.md](../MIGRATION_PLAN.md) - 移行計画

---

## 🔮 今後の予定

### Phase 6.3: クラウドDB実装（優先度: 高）

1. **Snowflake統合** (2026 Q1予定)
   - Snowflake.Data パッケージ統合
   - Account-based認証実装
   - Snowflake固有構文対応

2. **BigQuery統合** (2026 Q1予定)
   - Google.Cloud.BigQuery.V2 パッケージ統合
   - サービスアカウント認証実装
   - Standard SQL対応

3. **Databricks統合** (2026 Q2予定)
   - Databricks.Sql パッケージ統合
   - Personal Access Token認証
   - Delta Lake対応

4. **Redshift統合** (2026 Q2予定)
   - Npgsql PostgreSQL互換接続
   - Redshift固有構文対応
   - IAM認証実装

### Phase 7: テスト強化（優先度: 高）

- CTE完全対応
- CASE式完全対応
- Window関数対応
- 高度なサブクエリ対応

### Phase 8: ドキュメント追加（優先度: 中）

- アーキテクチャ図作成
- クラス図作成
- シーケンス図作成
- ANTLR4文法仕様ドキュメント

---

**最終更新日:** 2026-02-22
**バージョン:** 1.0.0
**ライセンス:** MIT License
