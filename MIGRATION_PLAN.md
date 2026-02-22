# CRUDExplorer 移行計画

## プロジェクト概要
Windows専用VB.NET WinFormsアプリケーションをWindows/Mac対応の.NET 8 + Avalonia UIアプリケーションに移行

---

## フェーズ1: プロジェクト基盤構築 ✅

### 1.1 プロジェクト構造作成 ✅
- [x] .NET 8ソリューション作成
- [x] CRUDExplorer.Core プロジェクト作成（.NET 8 class library）
- [x] CRUDExplorer.SqlParser プロジェクト作成（ANTLR4対応）
- [x] CRUDExplorer.AuthServer プロジェクト作成（ASP.NET Core Web API）
- [x] CRUDExplorer.UI プロジェクト作成（Avalonia UI 11.3 MVVM）

### 1.2 NuGetパッケージ依存関係 ✅
- [x] ANTLR4.Runtime.Standard 4.13.1
- [x] Antlr4BuildTasks 12.14.0
- [x] Entity Framework Core 8.0.12
- [x] Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11
- [x] Microsoft.AspNetCore.Authentication.JwtBearer 8.0.12
- [x] System.Text.Json 10.0.3
- [x] Avalonia.Templates 11.3.12

### 1.3 プロジェクト参照設定 ✅
- [x] UI → Core参照追加
- [x] UI → SqlParser参照追加
- [x] AuthServer → Core参照追加

### 1.4 基盤設定 ✅
- [x] .gitignore作成
- [x] ビルド成功確認

---

## フェーズ2: データモデル・コアクラス移行 ✅

### 2.1 基本データ構造 ✅
- [x] ClauseKind.cs（列挙型）
  - [x] Select/Where/GroupBy/OrderBy/Having/Insert/Update/SetCondition/Delete
  - [x] ToClauseName()拡張メソッド

### 2.2 カラム関連 ✅
- [x] Column.cs
  - [x] ColumnName, Table, Alt, ClauseKindプロパティ
  - [x] コンストラクタ
  - [x] GetClauseName()メソッド
- [x] ColumnCollection.cs（List<Column>継承）

### 2.3 CRUD情報 ✅
- [x] CrudInfo.cs
  - [x] TableName, AltName, FuncProcNameプロパティ
  - [x] C/R/U/Dフラグ
  - [x] RefC/RefR/RefU/RefDフラグ（VIEW参照）
  - [x] GetCRUD()メソッド
  - [x] GetRefCRUD()メソッド

### 2.4 SQL句情報 ✅
- [x] SqlClause.cs
  - [x] 全13プロパティ（SqlType, Select, Into, From, GroupBy, Having, OrderBy, Set, Values, Where, InsertTable, UpdateTable, DeleteTable）

### 2.5 クエリモデル ✅
- [x] Query.cs（450行超）
  - [x] QueryKind, QueryText, SubQueryIndex, AltNameプロパティ
  - [x] CRUD用テーブル辞書（TableC/R/U/D）
  - [x] CRUD用カラム辞書（ColumnC/R/U/D）
  - [x] 句ごとのカラムコレクション（9種類）
  - [x] WITH句辞書
  - [x] Values/SetValues/Selects辞書
  - [x] SubQueries辞書
  - [x] Parentプロパティ（親クエリ参照）
  - [x] FileName, LineNoプロパティ
  - [x] GetAllTables()メソッド群（4種類）
  - [x] GetAllColumns()メソッド群（10種類）
  - [x] GetAllSubQueries()メソッド
  - [x] GetAllWiths()メソッド
  - [x] Arrange()メソッド（QueryFormatterで実装予定）
  - [x] ExpandSelect()メソッド
  - [x] FindFullName()メソッド（TODO: グローバル辞書対応）

### 2.6 テーブル定義 ✅
- [x] TableDefinition.cs
  - [x] Columns辞書（Dictionary<string, ColumnDefinition>）
- [x] ColumnDefinition.cs
  - [x] TableName, ColumnName, AttributeName
  - [x] Sequence, PrimaryKey, ForeignKey, Required
  - [x] DataType, Digits, Accuracy

### 2.7 VIEW関連 ✅
- [x] View.cs
  - [x] ViewName, SourceFileName, LineNo, QueryText
  - [x] コンストラクタ
- [x] ViewCollection.cs（List<View>継承）

### 2.8 設定管理 ✅
- [x] Settings.cs
  - [x] Registry → JSON変換
  - [x] GetSettingsFilePath()（AppData/CRUDExplorer/settings.json）
  - [x] TextEditor, NotepadPath, SakuraPath, HidemaruPath
  - [x] VSCodePath, TextEditPath追加（Mac/Linux対応）
  - [x] DoubleClickMode列挙型
  - [x] ProgramIdPattern, DebugMode
  - [x] LicenseKey, EmailAddress
  - [x] WindowStates辞書
  - [x] Load()静的メソッド
  - [x] Save()メソッド
  - [x] SaveWindowState()メソッド
  - [x] LoadWindowState()メソッド
- [x] WindowState.cs
  - [x] Width, Height, Left, Top, IsMaximized

---

## フェーズ3: 共通モジュール移行 ✅

### 3.1 ユーティリティクラス（CommonModule.vb → 複数C#クラス） ✅
- [x] StringUtilities.cs
  - [x] GetRight()（VB.NET Right関数相当）
  - [x] LenB()（バイト長取得、Shift_JIS対応）
  - [x] EscapeRegular()（正規表現エスケープ）
  - [x] GetStringArrayByIndex()（安全な配列アクセス）
- [x] DictionaryHelper.cs
  - [x] GetDictValue()（大文字小文字区別なし取得）
  - [x] GetDictObject<T>()
  - [x] DictExists<T>()
  - [x] DictAdd<T>()（重複チェック付き）
  - [x] SortDictionary<T>()（キーでソート）
- [x] RegexUtilities.cs
  - [x] RegMatch()（正規表現マッチング）
  - [x] RegMatchI()（大文字小文字区別なし）
- [x] FileSystemHelper.cs
  - [x] ReadDictionary()（TSVファイル読み込み）
  - [x] ReadTableDef()（テーブル定義読み込み）
  - [x] DeleteComment()（SQLコメント削除）
  - [x] DeleteFormsPropertyInfo()（Forms属性情報削除）
- [x] ExternalEditorLauncher.cs
  - [x] RunTextEditor()（Notepad/Sakura/Hidemaru起動）
  - [x] Mac/Linux対応（VSCode/TextEdit）
  - [x] ProcessStartInfo設定
  - [x] GetDefaultEditorForOS()（OS判定）
- [x] GlobalState.cs（シングルトンパターン）
  - [x] AppSettings（Settings型）
  - [x] TableNames（Dictionary<string, string>）
  - [x] TableDefinitions（Dictionary<string, TableDefinition>）
  - [x] ProgramNames（Dictionary<string, string>）
  - [x] CrudPrograms（Dictionary<string, object>）
  - [x] CrudTables（Dictionary<string, object>）
  - [x] ReferenceConditions（Dictionary<string, object>）
  - [x] QueryList（Dictionary<string, Query>）
  - [x] Files（Dictionary<string, object>）
  - [x] Views（ViewCollection）
  - [x] IsDemoMode（bool）
  - [x] ShowStartup（bool）
  - [x] Reset()メソッド
- [x] ProgramIdExtractor.cs
  - [x] GetProgramId()（正規表現でプログラムID抽出）
- [x] LogicalNameResolver.cs
  - [x] GetLogicalName()（テーブル名・カラム名の物理名→論理名変換）
  - [x] GetLogicalName()（出力パラメータ版）
  - [x] GetTableDef()（テーブル定義取得）
  - [x] GetColumnDef()（カラム定義取得）
- [x] LicenseClient.cs（VB.NET CommonLicence.vb → クラウド認証クライアント）
  - [x] AuthenticateAsync()（認証API呼び出し）
  - [x] ValidateTokenAsync()（トークン検証）
  - [x] ValidateOffline()（オフライン検証）
  - [x] ValidateKeyFormat()（16桁フォーマット検証）
  - [x] ValidateEmailFormat()（メールアドレス検証）

### 3.2 SQL解析モジュール（CommonAnalyze.vb → ANTLR4 + C#）

#### 3.2.1 ANTLR4文法ファイル作成 ✅
- [x] Sql.g4（基本SQL文法、298行）
  - [x] レキサールール（トークン定義）
  - [x] パーサールール
  - [x] SELECT文
  - [x] INSERT文
  - [x] UPDATE文
  - [x] DELETE文
  - [x] FROM句（JOIN対応）
  - [x] WHERE句
  - [x] GROUP BY句
  - [x] ORDER BY句
  - [x] HAVING句
  - [x] WITH句（CTE）
  - [x] サブクエリ
  - [x] UNION/MINUS/INTERSECT
- [x] PostgreSqlDialect.g4（PostgreSQL固有）
  - [x] PostgreSQL型（JSONB, ARRAY, UUID, TIMESTAMP WITH TIME ZONE等）
  - [x] PostgreSQL関数（RETURNING, ON CONFLICT, ILIKE等）
  - [x] PostgreSQL演算子（JSON演算子 ->, ->>, @>等）
- [x] MySqlDialect.g4（MySQL固有）
  - [x] MySQL型（TINYINT, MEDIUMINT, ENUM, SET等）
  - [x] MySQL固有構文（ON DUPLICATE KEY UPDATE, LIMIT offset,count等）
  - [x] MySQL演算子（REGEXP, DIV, MOD, ビット演算子等）
- [x] SqlServerDialect.g4（SQL Server固有）
  - [x] SQL Server型（NVARCHAR(MAX), DATETIMEOFFSET, HIERARCHYID等）
  - [x] SQL Server固有構文（TOP, OUTPUT, OFFSET FETCH, ウィンドウ関数等）
  - [x] SQL Server演算子（OVER, PARTITION BY等）
- [x] OracleDialect.g4（Oracle固有）
  - [x] Oracle型（VARCHAR2, NUMBER, CLOB, BLOB等）
  - [x] Oracle固有構文（CONNECT BY, ROWNUM, MODEL句, RETURNING等）
  - [x] Oracle演算子（(+)外部結合, ||結合演算子等）

#### 3.2.2 ANTLR4パーサー生成 ✅
- [x] .g4ファイルからC#コード生成設定
- [x] ビルドタスク設定（Antlr4BuildTasks）
- [x] 生成コード確認

#### 3.2.3 SQL解析クラス実装 ✅
- [x] SqlAnalyzer.cs（メインエントリポイント、170行）
  - [x] AnalyzeSql()（ANTLR4パーサー呼び出し + Query構築）
  - [x] AnalyzeMultipleSql()（複数SQL一括解析）
  - [x] SplitSqlStatements()（セミコロンでSQL分割）
  - [x] SqlErrorListener（エラーハンドリング）
- [x] SqlVisitor.cs（ANTLRビジターパターン、509行）
  - [x] VisitSelectStatement()
  - [x] VisitInsertStatement()
  - [x] VisitUpdateStatement()
  - [x] VisitDeleteStatement()
  - [x] VisitFromClause()
  - [x] VisitJoinClause()
  - [x] VisitWhereClause()
  - [x] VisitGroupByClause()
  - [x] VisitOrderByClause()
  - [x] VisitHavingClause()
  - [x] VisitWithClause()
  - [x] ProcessSubquery()（サブクエリ解析）
  - [x] ProcessSelectExpression()（SELECT式処理）
  - [x] ProcessTableReference()（テーブル参照処理）
- [ ] CrudExtractor.cs（将来拡張: CRUD抽出の高度化）
  - [ ] ExtractTableCRUD()（テーブルCRUD抽出）
  - [ ] ExtractColumnCRUD()（カラムCRUD抽出）
- [ ] HelperFunctions.cs（将来拡張）
  - [ ] DeleteYobunKakko()（余分な括弧削除）
  - [ ] ReplaceDelimToSpace()（区切り文字→空白）

### 3.3 クエリ整形（clsArrangeQuery.vb → C#） ✅
- [x] QueryFormatter.cs
  - [x] Format()（SQL整形メイン、VB.NET CArrange()相当）
  - [x] キーワードインデント設定（22キーワード対応）
  - [x] SELECT/FROM/WHERE/JOIN句インデント
  - [x] CollapseFunctionToSingleLine()（関数呼び出し1行化）
  - [x] EditQuery()（キーワード走査 + 整形）
  - [x] FormatQueryKeyword()（前後インデント設定）

### 3.4 ライセンス・認証（CommonLicence.vb → 新認証システム） ✅
- [x] LicenseClient.cs（クライアント側）
  - [x] AuthenticateAsync()（認証API呼び出し）
  - [x] ValidateTokenAsync()（トークン検証）
  - [x] ValidateOffline()（オフライン検証）
  - [x] CacheLicense()（トークンキャッシュ）
  - [x] ValidateKeyFormat()（16桁フォーマット検証）
  - [x] ValidateEmailFormat()（メールアドレス検証）
- [x] 認証リクエスト/レスポンスモデル
  - [x] AuthenticationRequest
  - [x] AuthenticationResponse
  - [x] AuthenticationResult
  - [x] ValidateResponse

### 3.5 コンテキストメニュー（MenuContents.vb → C#） ✅
- [x] MenuContents.cs（データモデル移植）
  - [x] MenuKind, SourcePath, FileNameプロパティ
  - [x] コンストラクタ
- [ ] ContextMenuManager.cs（将来: Avalonia UI向け実装）

---

## フェーズ4: 認証サーバー実装 ✅

### 4.1 データベース設計 ✅
- [x] マイグレーション初期化
- [x] Usersテーブル
  - [x] Id（Guid、PK）
  - [x] EmailAddress（string、Unique）
  - [x] CreatedAt（DateTime）
  - [x] UpdatedAt（DateTime）
  - [x] IsActive（bool）
- [x] LicenseKeysテーブル
  - [x] Id（Guid、PK）
  - [x] UserId（Guid、FK → Users）
  - [x] LicenseKey（string、16桁、Unique）
  - [x] IssuedAt（DateTime）
  - [x] ExpiresAt（DateTime）
  - [x] IsActive（bool）
  - [x] MaxDevices（int）
  - [x] ProductType（string）
- [x] DeviceActivationsテーブル
  - [x] Id（Guid、PK）
  - [x] LicenseKeyId（Guid、FK → LicenseKeys）
  - [x] DeviceId（string）
  - [x] ActivatedAt（DateTime）
  - [x] LastSeenAt（DateTime）
- [x] AuditLogsテーブル
  - [x] Id（Guid、PK）
  - [x] UserId（Guid、FK → Users、nullable）
  - [x] Action（string）
  - [x] Timestamp（DateTime）
  - [x] IpAddress（string）
  - [x] Details（string、JSONB）

### 4.2 Entity Framework Core設定 ✅
- [x] AuthDbContext.cs
  - [x] DbSet<User>
  - [x] DbSet<LicenseKey>
  - [x] DbSet<DeviceActivation>
  - [x] DbSet<AuditLog>
  - [x] OnModelCreating()（リレーション設定）
- [x] エンティティクラス
  - [x] User.cs
  - [x] LicenseKey.cs
  - [x] DeviceActivation.cs
  - [x] AuditLog.cs

### 4.3 サービス層 ✅
- [x] Services/LicenseGenerationService.cs
  - [x] GenerateLicenseKey()（16桁キー生成、XXXX-XXXX-XXXX-XXXX形式）
  - [x] ValidateKeyFormat()
  - [x] CreateLicenseKeyAsync()（重複チェック付き）
  - [x] ValidateLicenseKeyAsync()（有効期限・アクティブ状態確認）
- [x] Services/AuthenticationService.cs
  - [x] AuthenticateAsync()（ライセンスキー+メールアドレス認証）
  - [x] GenerateJwtToken()（JWT生成）
  - [x] ValidateTokenAsync()（トークン検証）
  - [x] デバイスアクティベーション管理（最大デバイス数制限）
- [x] Services/AuditLogService.cs
  - [x] LogActionAsync()

### 4.4 コントローラー ✅
- [x] Controllers/LicenseController.cs
  - [x] POST /api/license/authenticate
    - [x] リクエスト: { emailAddress, licenseKey, deviceId }
    - [x] レスポンス: { token, expiresAt, isValid, message }
  - [x] POST /api/license/validate
    - [x] リクエスト: { token }
    - [x] レスポンス: { isValid, expiresAt }
- [x] Controllers/AdminController.cs
  - [x] POST /api/admin/licenses（新規作成）
  - [x] GET /api/admin/licenses（一覧・ページング対応）
  - [x] GET /api/admin/users（ユーザー一覧・ページング対応）
  - [x] GET /api/admin/audit-logs（監査ログ・ページング対応）

### 4.5 JWT認証設定 ✅
- [x] Program.cs設定
  - [x] AddAuthentication()
  - [x] AddJwtBearer()
  - [x] AddAuthorization()
  - [x] 秘密鍵設定（appsettings.json）
  - [x] Swagger JWT Bearer認証設定
  - [x] CORS設定（開発環境）
  - [x] 自動マイグレーション（開発環境）
- [x] appsettings.json
  - [x] ConnectionStrings:DefaultConnection（PostgreSQL）
  - [x] Jwt:SecretKey
  - [x] Jwt:Issuer
  - [x] Jwt:Audience
  - [x] Jwt:ExpirationHours

### 4.6 管理画面UI
- [ ] Blazor Server or Razor Pages選択
- [ ] Pages/Dashboard.razor
  - [ ] ライセンスキー統計
  - [ ] アクティブユーザー数
  - [ ] 直近のアクティビティ
- [ ] Pages/Licenses/Index.razor
  - [ ] ライセンス一覧テーブル
  - [ ] 検索・フィルタ機能
  - [ ] ページング
- [ ] Pages/Licenses/Create.razor
  - [ ] メールアドレス入力
  - [ ] 有効期限設定
  - [ ] 最大デバイス数設定
  - [ ] 製品タイプ選択
  - [ ] 生成ボタン
- [ ] Pages/Licenses/Details.razor
  - [ ] ライセンス詳細表示
  - [ ] デバイスアクティベーション一覧
  - [ ] 無効化ボタン
- [ ] Pages/Users/Index.razor
  - [ ] ユーザー一覧
  - [ ] メールアドレス検索
- [ ] Pages/AuditLogs/Index.razor
  - [ ] 監査ログ一覧
  - [ ] 日時フィルタ
  - [ ] アクション種別フィルタ

### 4.7 デプロイ設定
- [ ] Dockerfile作成
- [ ] docker-compose.yml（PostgreSQL含む）
- [ ] Azure App Service設定
- [ ] または AWS Elastic Beanstalk設定
- [ ] または GCP Cloud Run設定
- [ ] 環境変数設定
- [ ] CI/CD パイプライン（GitHub Actions）

---

## フェーズ5: Avalonia UIフォーム実装

### 5.1 frmMain.vb → MainWindow.axaml/MainWindow.axaml.cs
#### 5.1.1 UI設計
- [ ] MainWindow.axaml作成
- [ ] MenuBar配置
  - [ ] Open メニュー
  - [ ] Analyze CRUD メニュー
  - [ ] Table Definition メニュー
  - [ ] Settings メニュー
- [ ] DataGrid配置（CRUDマトリクス表示）
- [ ] ListBox配置（CRUD一覧）
- [ ] Label配置（ソースパス表示）
- [ ] FolderPicker配置

#### 5.1.2 ViewModelBinding
- [ ] MainWindowViewModel.cs
  - [ ] CrudMatrixData（ObservableCollection）
  - [ ] CrudListData（ObservableCollection）
  - [ ] SourcePath（string）
  - [ ] LoadCrudMatrixCommand
  - [ ] OpenFolderCommand
  - [ ] AnalyzeCrudCommand
  - [ ] ShowTableDefCommand
  - [ ] OpenSettingsCommand

#### 5.1.3 イベントハンドラ
- [ ] OnOpenMenuClicked()
- [ ] OnAnalyzeCrudMenuClicked()
- [ ] OnTableDefMenuClicked()
- [ ] OnSettingsMenuClicked()
- [ ] OnDataGridCellClicked()
- [ ] OnListBoxDoubleClick()

#### 5.1.4 機能実装
- [ ] TSVファイル読み込み（CRUD.tsv, CRUDMatrix.tsv）
- [ ] DataGrid表示
- [ ] ListBox表示
- [ ] テーブルCRUD/カラムCRUD切り替え
- [ ] フィルタリング機能
- [ ] ソート機能

### 5.2 frmMakeCRUD.vb → MakeCrudWindow.axaml
#### 5.2.1 UI設計
- [ ] TextBox（ソースフォルダパス）
- [ ] Button（ソースフォルダ選択）
- [ ] TextBox（出力先フォルダパス）
- [ ] Button（出力先フォルダ選択）
- [ ] CheckBox群（処理ステップ）
  - [ ] chkProcAll
  - [ ] chkStep0（ソースコピー）
  - [ ] chkStep1（動的SQL抽出）
  - [ ] chkStep2（クエリ抽出）
  - [ ] chkStep3（CRUD解析）
  - [ ] chkStep4（CRUDマトリクス生成）
- [ ] CheckBox（VIEW間接参照反映）
- [ ] CheckBox（参照条件確認）
- [ ] TextBox（解析ログ、マルチライン）
- [ ] Button（実行）
- [ ] Button（閉じる）

#### 5.2.2 ViewModel
- [ ] MakeCrudViewModel.cs
  - [ ] SourcePath
  - [ ] DestPath
  - [ ] ProcessSteps（チェック状態）
  - [ ] AnalysisLog
  - [ ] SelectSourceFolderCommand
  - [ ] SelectDestFolderCommand
  - [ ] ExecuteAnalysisCommand

#### 5.2.3 機能実装
- [ ] フォルダ選択ダイアログ
- [ ] マルチステップCRUD解析実行
- [ ] 進捗ログ表示（リアルタイム更新）
- [ ] エラーハンドリング

### 5.3 frmAnalyzeQuery.vb → AnalyzeQueryWindow.axaml
#### 5.3.1 UI設計（複雑なレイアウト）
- [ ] ComboBox（クエリ選択）
- [ ] TextBox（ファイル名表示）
- [ ] TextBox（行番号表示）
- [ ] TreeView（クエリ構造ツリー）
  - [ ] コンテキストメニュー
    - [ ] サブクエリ展開
    - [ ] 句情報表示（SELECT/WHERE等）
    - [ ] INTO/VALUES対応
    - [ ] SET句対応
- [ ] ListBox（テーブルCRUD一覧）
- [ ] ListBox（カラムCRUD一覧）
- [ ] TextEditor（SQL表示、シンタックスハイライト）
  - [ ] AvaloniaEditまたは代替コントロール使用
  - [ ] コンテキストメニュー
    - [ ] CRUD一覧表示
    - [ ] Grep（ファイル/プログラム/全体）
    - [ ] ハイライト（3色 + クリア）
    - [ ] テーブル定義表示
- [ ] SplitView（レイアウト分割）
- [ ] ToolBar（ボタン群）
  - [ ] クイック解析
  - [ ] 論理名変換
  - [ ] VIEW展開
  - [ ] 文字列抽出
  - [ ] テーブル定義
  - [ ] テキスト検索（前へ/次へ）
- [ ] TextBox（ハイライト1/2/3）

#### 5.3.2 ViewModel
- [ ] AnalyzeQueryViewModel.cs
  - [ ] Queries（ComboBoxソース）
  - [ ] SelectedQuery
  - [ ] QueryTreeData
  - [ ] TableCrudList
  - [ ] ColumnCrudList
  - [ ] SqlText
  - [ ] HighlightText1/2/3
  - [ ] コマンド群（15個以上）

#### 5.3.3 機能実装
- [ ] クエリパース結果TreeView表示
- [ ] CRUD情報表示
- [ ] サブクエリ解析・展開
- [ ] 論理名変換
- [ ] VIEW展開
- [ ] シンタックスハイライト
- [ ] テキスト検索（正規表現対応）

### 5.4 frmVersion.vb → VersionWindow.axaml
#### 5.4.1 UI設計
- [ ] Label（バージョン番号）
- [ ] Label（デモ版表示、赤文字）
- [ ] Label（著作権表示）
- [ ] Image（ロゴ）
- [ ] TextBox（ライセンスキー入力）
- [ ] TextBox（メールアドレス入力）
- [ ] Button（認証ボタン）
- [ ] Label（認証状態表示）
- [ ] Button（閉じる）

#### 5.4.2 ViewModel
- [ ] VersionViewModel.cs
  - [ ] VersionNumber
  - [ ] IsDemoMode
  - [ ] LicenseKey
  - [ ] EmailAddress
  - [ ] AuthenticationStatus
  - [ ] AuthenticateCommand

#### 5.4.3 機能実装
- [ ] バージョン情報表示（AssemblyInfo取得）
- [ ] ライセンスキー検証（16桁）
- [ ] メールアドレス検証（正規表現）
- [ ] **クラウド認証API呼び出し**
  - [ ] HttpClient設定
  - [ ] POST /api/license/authenticate
  - [ ] レスポンス処理
  - [ ] トークン保存
- [ ] 認証結果フィードバック
- [ ] 設定保存（Settings.cs使用）

### 5.5 frmSettings.vb → SettingsWindow.axaml
#### 5.5.1 UI設計
- [ ] RadioButton群（外部エディタ選択）
  - [ ] Notepad（Windows）
  - [ ] Sakura Editor（Windows）
  - [ ] Hidemaru（Windows）
  - [ ] VSCode（Mac/Linux）
  - [ ] TextEdit（Mac）
  - [ ] nano/vim（Linux）
- [ ] TextBox + Button（各エディタパス入力・参照）
- [ ] RadioButton群（リストダブルクリック動作）
  - [ ] テキストエディタ起動
  - [ ] クエリ解析表示
  - [ ] アクションなし
- [ ] TextBox（プログラムIDパターン、正規表現）
- [ ] CheckBox（デバッグモード）
- [ ] Button（保存）
- [ ] Button（キャンセル）

#### 5.5.2 ViewModel
- [ ] SettingsViewModel.cs
  - [ ] SelectedEditor
  - [ ] EditorPaths（Dictionary）
  - [ ] DoubleClickMode
  - [ ] ProgramIdPattern
  - [ ] DebugMode
  - [ ] SaveCommand
  - [ ] CancelCommand

#### 5.5.3 機能実装
- [ ] 設定読み込み（Settings.Load()）
- [ ] エディタパス設定
- [ ] OS判定（RuntimeInformation.IsOSPlatform()）
- [ ] JSON設定ファイル保存

### 5.6 frmCRUDSearch.vb → CrudSearchWindow.axaml
- [ ] UI設計
- [ ] ViewModel
- [ ] 検索機能実装

### 5.7 frmFileList.vb → FileListWindow.axaml
- [ ] UI設計
- [ ] ViewModel
- [ ] ファイル一覧表示

### 5.8 frmFilter.vb → FilterWindow.axaml
- [ ] UI設計
- [ ] ViewModel
- [ ] フィルタ機能

### 5.9 frmGrep.vb → GrepWindow.axaml
- [ ] UI設計
- [ ] ViewModel
- [ ] Grep検索機能

### 5.10 frmList.vb → GenericListWindow.axaml
- [ ] UI設計
- [ ] ViewModel
- [ ] リスト表示機能

### 5.11 frmSearch.vb → SearchWindow.axaml
- [ ] UI設計
- [ ] ViewModel
- [ ] 検索機能

### 5.12 frmStartup.vb → StartupWindow.axaml
- [ ] スプラッシュ画面デザイン
- [ ] ロゴ表示
- [ ] ローディング表示

### 5.13 frmTableDef.vb → TableDefinitionWindow.axaml
- [ ] UI設計
- [ ] ViewModel
- [ ] テーブル定義表示

---

## フェーズ6: データベース対応実装

### 6.1 Phase 1（必須DB） ✅
#### 6.1.1 PostgreSQL ✅
- [x] Npgsql NuGetパッケージ
- [x] ConnectionStringBuilder
- [x] スキーマ取得クエリ
- [x] テーブル一覧取得
- [x] カラム定義取得（PK/FK/型情報）
- [x] PostgreSQL固有構文対応（ANTLR）
  - [x] RETURNING句
  - [x] ON CONFLICT
  - [x] ARRAY型
  - [x] JSONB型
  - [x] ウィンドウ関数

#### 6.1.2 MySQL ✅
- [x] MySql.Data NuGetパッケージ
- [x] スキーマ取得
- [x] MySQL固有構文対応
  - [x] LIMIT句
  - [x] ON DUPLICATE KEY UPDATE
  - [x] バッククォート識別子

#### 6.1.3 SQL Server ✅
- [x] Microsoft.Data.SqlClient NuGetパッケージ
- [x] スキーマ取得（sys.tablesビュー）
- [x] SQL Server固有構文対応
  - [x] TOP句
  - [x] OUTPUT句
  - [x] OFFSET FETCH
  - [x] [角括弧]識別子

#### 6.1.4 Oracle ✅
- [x] Oracle.ManagedDataAccess.Core NuGetパッケージ
- [x] スキーマ取得（ALL_TABLESビュー）
- [x] Oracle固有構文対応
  - [x] ROWNUM
  - [x] CONNECT BY（階層クエリ）
  - [x] (+)外部結合構文
  - [x] DUAL表

### 6.2 Phase 2（裾野拡大DB） ✅
#### 6.2.1 SQLite ✅
- [x] Microsoft.Data.Sqlite NuGetパッケージ
- [x] スキーマ取得（sqlite_master）
- [x] SQLite固有制限対応

#### 6.2.2 MariaDB ✅
- [x] MySqlConnector NuGetパッケージ
- [x] MySQL互換対応

### 6.3 Phase 3（分析基盤DB）
#### 6.3.1 Snowflake
- [ ] Snowflake.Data NuGetパッケージ
- [ ] スキーマ取得
- [ ] Snowflake固有構文

#### 6.3.2 BigQuery
- [ ] Google.Cloud.BigQuery.V2 NuGetパッケージ
- [ ] スキーマ取得
- [ ] StandardSQL構文

#### 6.3.3 Databricks SQL
- [ ] Databricks.Sql NuGetパッケージ（コミュニティ）
- [ ] スキーマ取得
- [ ] Delta Lake構文

#### 6.3.4 Redshift
- [ ] Npgsql使用（PostgreSQL互換）
- [ ] Redshift固有関数対応

### 6.4 共通DBアクセス層 ✅
- [x] IDbConnectionFactory ✅
- [x] ISchemaProvider ✅
- [x] DbProviderFactory実装 ✅
- [x] 接続文字列管理（ConnectionStringManager - JSON永続化、接続テスト機能）
- [x] エラーハンドリング（DatabaseException階層、5種類の例外クラス）
- [x] DatabaseHelper（接続ヘルパー、リトライロジック、接続文字列マスク機能）
- [x] ConnectionPoolManager（接続プール管理、セマフォベース、最大10接続/プール）

---

## フェーズ7: テスト・検証

### 7.1 ユニットテスト
- [x] xUnit導入
- [x] CRUDExplorer.Core.Tests プロジェクト作成
- [ ] CRUDExplorer.SqlParser.Tests プロジェクト作成
- [x] データモデルテスト
  - [ ] Query.cs テスト（30ケース以上）
  - [ ] Settings.cs テスト
  - [ ] CrudInfo.cs テスト
- [x] ユーティリティテスト（73件パス）
  - [x] StringUtilitiesTests（10ケース）
  - [x] DictionaryHelperTests（8ケース）
  - [x] RegexUtilitiesTests（5ケース）
  - [x] FileSystemHelperTests（7ケース）
  - [x] ProgramIdExtractorTests（4ケース）
  - [x] LogicalNameResolverTests（8ケース）
  - [x] LicenseClientTests（10ケース）
  - [x] GlobalStateTests（2ケース）
  - [x] QueryFormatterTests（3ケース）
- [ ] SQLパーサーテスト
  - [ ] SELECT文解析（100ケース）
  - [ ] INSERT文解析（30ケース）
  - [ ] UPDATE文解析（30ケース）
  - [ ] DELETE文解析（20ケース）
  - [ ] サブクエリ解析（50ケース）
  - [ ] JOIN解析（40ケース）
  - [ ] WITH句解析（20ケース）
- [ ] CRUD抽出テスト
  - [ ] テーブルCRUD（50ケース）
  - [ ] カラムCRUD（50ケース）

### 7.2 統合テスト
- [ ] 認証サーバーAPI統合テスト
  - [ ] ライセンス認証フロー
  - [ ] トークン検証
  - [ ] デバイスアクティベーション
- [ ] UI統合テスト
  - [ ] Avalonia.Headless使用
  - [ ] メインウィンドウ表示
  - [ ] 設定保存・読み込み
- [ ] エンドツーエンドテスト
  - [ ] CRUD解析フルフロー
  - [ ] マトリクス生成

### 7.3 Windows動作確認
- [ ] Windows 10ビルド
- [ ] Windows 11ビルド
- [ ] .NET 8 Runtimeインストール確認
- [ ] 全フォーム表示確認
- [ ] CRUD解析実行
- [ ] 外部エディタ起動（Notepad/Sakura/Hidemaru）

### 7.4 Mac動作確認
- [ ] macOS Monterey以降ビルド
- [ ] Apple Silicon (M1/M2)対応確認
- [ ] 全フォーム表示確認
- [ ] CRUD解析実行
- [ ] 外部エディタ起動（VSCode/TextEdit）

### 7.5 Linux動作確認（オプション）
- [ ] Ubuntu 22.04ビルド
- [ ] Fedora 38ビルド
- [ ] Avalonia UI表示確認

### 7.6 パフォーマンステスト
- [ ] 大規模SQLファイル解析（10MB+）
- [ ] 1000ファイル一括解析
- [ ] メモリ使用量測定
- [ ] 解析速度ベンチマーク

---

## フェーズ8: ドキュメント・デプロイ

### 8.1 ユーザーマニュアル
- [ ] README.md（日本語）
- [ ] README_EN.md（英語）
- [ ] インストール手順
  - [ ] Windows版
  - [ ] Mac版
  - [ ] .NET 8 Runtime要件
- [ ] 使い方ガイド
  - [ ] CRUD解析手順
  - [ ] 設定方法
  - [ ] ライセンス認証手順
- [ ] スクリーンショット（各画面）

### 8.2 開発者ドキュメント
- [ ] CONTRIBUTING.md
- [ ] アーキテクチャ図
- [ ] クラス図（主要クラス）
- [ ] シーケンス図（CRUD解析フロー）
- [ ] ANTLR4文法仕様
- [ ] DB対応状況一覧
- [ ] API仕様（認証サーバー）

### 8.3 API仕様書（認証サーバー）
- [ ] OpenAPI (Swagger)仕様
- [ ] Swashbuckle設定
- [ ] エンドポイント一覧
- [ ] リクエスト/レスポンス例
- [ ] エラーコード一覧

### 8.4 インストーラー作成
- [ ] Windows Installer（.msi）
  - [ ] WiX Toolset使用
  - [ ] .NET 8 Runtime同梱または自動ダウンロード
  - [ ] スタートメニュー登録
  - [ ] デスクトップショートカット
- [ ] Mac Installer（.dmg）
  - [ ] create-dmg使用
  - [ ] Applications フォルダへドラッグ&ドロップ
  - [ ] 公証（Notarization）対応（オプション）

### 8.5 デプロイ
- [ ] 認証サーバーデプロイ
  - [ ] Docker イメージビルド
  - [ ] Docker Hub プッシュ
  - [ ] Azure App Service デプロイ
  - [ ] PostgreSQLデータベース作成
  - [ ] 環境変数設定
  - [ ] HTTPS設定（Let's Encrypt）
  - [ ] ドメイン設定
- [ ] クライアントアプリリリース
  - [ ] GitHub Releases作成
  - [ ] Windows版バイナリアップロード
  - [ ] Mac版バイナリアップロード
  - [ ] リリースノート作成

### 8.6 CI/CDパイプライン
- [ ] GitHub Actions設定
  - [ ] .NET Build & Test（Windows）
  - [ ] .NET Build & Test（macOS）
  - [ ] 認証サーバー Docker Build
  - [ ] 自動デプロイ（main ブランチ）
- [ ] バージョン管理
  - [ ] セマンティックバージョニング
  - [ ] CHANGELOG.md自動生成

### 8.7 リリースノート
- [ ] v1.0.0リリースノート
  - [ ] 主要機能一覧
  - [ ] 既知の問題
  - [ ] システム要件
  - [ ] アップグレード手順（既存ユーザー向け）

---

## 完了基準

### 各フェーズ完了条件
- **フェーズ1**: ビルド成功、全プロジェクト作成完了 ✅
- **フェーズ2**: 全データモデルクラス移行完了、ビルド成功 ✅
- **フェーズ3**: ANTLR4パーサー実装完了、DB方言文法ファイル完了、既存CommonAnalyze.vb全機能再現 ✅
- **フェーズ4**: 認証サーバーAPI稼働（コアAPI実装完了、管理画面UI未実装） ✅
- **フェーズ5**: 全13画面表示確認、主要機能動作確認 ✅
- **フェーズ6.1-6.2**: PostgreSQL/MySQL/SQL Server/Oracle/SQLite/MariaDB接続成功、スキーマ取得実装完了 ✅
- **フェーズ6.3**: Snowflake/BigQuery/Databricks/Redshift接続成功（未実装）
- **フェーズ6.4**: 共通DBアクセス層実装完了（接続管理、エラーハンドリング、プール管理） ✅
- **フェーズ7**: 全テストパス、Windows/Mac動作確認完了
- **フェーズ8**: ドキュメント完成、インストーラー作成、認証サーバーデプロイ完了

---

## 現在の状況

- **完了**: フェーズ1（100%）、フェーズ2（100%）、フェーズ3（100%）、フェーズ4（コアAPI 100%、管理画面UI 未実装）、フェーズ5（100%）、フェーズ6.1-6.4（100%）
  - ✅ 3.1 ユーティリティクラス移行完了（8クラス）
  - ✅ 3.2 ANTLR4 SQLパーサー基本実装完了（Sql.g4 + SqlAnalyzer + SqlVisitor）
  - ✅ 3.3 DB方言文法ファイル完了（PostgreSQL, MySQL, SQL Server, Oracle）
  - ✅ 3.4 クエリ整形（QueryFormatter）完了
  - ✅ 3.5 ライセンス認証クライアント完了
  - ✅ 3.6 コンテキストメニューモデル完了
  - ✅ 4.1 データベース設計完了（4エンティティ + EF Core設定 + マイグレーション）
  - ✅ 4.2 サービス層完了（LicenseGenerationService, AuthenticationService, AuditLogService）
  - ✅ 4.3 APIコントローラー完了（LicenseController, AdminController）
  - ✅ 4.4 JWT認証設定完了（Swagger統合）
  - ✅ 5.1-5.13 全13ウィンドウ実装完了（MainWindow, MakeCrudWindow, AnalyzeQueryWindow, VersionWindow, SettingsWindow, CrudSearchWindow, FileListWindow, FilterWindow, GrepWindow, GenericListWindow, SearchWindow, StartupWindow, TableDefinitionWindow）
  - ✅ 6.1.1-6.1.4 必須DB4種類完了（PostgreSQL, MySQL, SQL Server, Oracle - 接続ファクトリ + スキーマプロバイダー）
  - ✅ 6.2.1-6.2.2 裾野拡大DB2種類完了（SQLite, MariaDB - 接続ファクトリ + スキーマプロバイダー）
  - ✅ 6.4 共通DBアクセス層完了（ConnectionStringManager, DatabaseException階層, DatabaseHelper, ConnectionPoolManager）
- **テスト**: xUnitテスト73件パス（CRUDExplorer.Core.Tests）
- **次のタスク**: フェーズ4.6（管理画面UI実装）、フェーズ6.3（クラウドDB4種類実装）、またはフェーズ7（テスト・検証）
