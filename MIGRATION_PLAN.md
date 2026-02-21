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

## フェーズ3: 共通モジュール移行 🔄

### 3.1 ユーティリティクラス（CommonModule.vb → 複数C#クラス）
- [ ] StringUtilities.cs
  - [ ] GetRight()（VB.NET Right関数相当）
  - [ ] LenB()（バイト長取得）
  - [ ] EscapeRegular()（正規表現エスケープ）
- [ ] DictionaryHelper.cs
  - [ ] GetDictValue<T>()（大文字小文字区別なし取得）
  - [ ] GetDictObject<T>()
  - [ ] DictExists()
  - [ ] DictAdd()（重複チェック付き）
  - [ ] SortDictionary()（キーでソート）
- [ ] RegexUtilities.cs
  - [ ] RegMatch()（正規表現マッチング）
  - [ ] RegMatchI()（大文字小文字区別なし）
- [ ] FileSystemHelper.cs
  - [ ] ReadDictionary()（TSVファイル読み込み）
  - [ ] ReadTableDef()（テーブル定義読み込み）
  - [ ] ファイル一覧取得
- [ ] ExternalEditorLauncher.cs
  - [ ] RunTextEditor()（Notepad/Sakura/Hidemaru起動）
  - [ ] Mac/Linux対応（VSCode/TextEdit/nano/vim）
  - [ ] ProcessStartInfo設定
- [ ] GlobalState.cs
  - [ ] objSettings（Settings型）
  - [ ] dctTableName（Dictionary<string, string>）
  - [ ] dctTableDef（Dictionary<string, TableDefinition>）
  - [ ] dctProgramName（Dictionary<string, string>）
  - [ ] dctCRUDProgram（Dictionary<string, object>）
  - [ ] dctCRUDTable（Dictionary<string, object>）
  - [ ] dctReferenceCond（Dictionary<string, object>）
  - [ ] dctQueryList（Dictionary<string, Query>）
  - [ ] dctFiles（Dictionary<string, object>）
  - [ ] colViews（ViewCollection）
  - [ ] bolDemoFlag（bool）
- [ ] ProgramIdExtractor.cs
  - [ ] GetProgramId()（正規表現でプログラムID抽出）

### 3.2 SQL解析モジュール（CommonAnalyze.vb → ANTLR4 + C#）

#### 3.2.1 ANTLR4文法ファイル作成
- [ ] Sql.g4（基本SQL文法）
  - [ ] レキサールール（トークン定義）
  - [ ] パーサールール
  - [ ] SELECT文
  - [ ] INSERT文
  - [ ] UPDATE文
  - [ ] DELETE文
  - [ ] FROM句（JOIN対応）
  - [ ] WHERE句
  - [ ] GROUP BY句
  - [ ] ORDER BY句
  - [ ] HAVING句
  - [ ] WITH句（CTE）
  - [ ] サブクエリ
  - [ ] UNION/MINUS/INTERSECT
- [ ] PostgreSqlDialect.g4（PostgreSQL固有）
  - [ ] PostgreSQL型
  - [ ] PostgreSQL関数
  - [ ] PostgreSQL演算子
- [ ] MySqlDialect.g4（MySQL固有）
- [ ] SqlServerDialect.g4（SQL Server固有）
- [ ] OracleDialect.g4（Oracle固有）

#### 3.2.2 ANTLR4パーサー生成
- [ ] .g4ファイルからC#コード生成設定
- [ ] ビルドタスク設定（Antlr4BuildTasks）
- [ ] 生成コード確認

#### 3.2.3 SQL解析クラス実装
- [ ] SqlAnalyzer.cs（メインエントリポイント）
  - [ ] AnalyzeCRUD()（CommonAnalyze.vbのAnalyzeCRUD()置き換え）
  - [ ] Parse()（ANTLR4パーサー呼び出し）
  - [ ] Queryオブジェクト構築
- [ ] SqlVisitor.cs（ANTLRビジターパターン）
  - [ ] VisitSelectStatement()
  - [ ] VisitInsertStatement()
  - [ ] VisitUpdateStatement()
  - [ ] VisitDeleteStatement()
  - [ ] VisitFromClause()
  - [ ] VisitJoinClause()
  - [ ] VisitWhereClause()
  - [ ] VisitSubquery()
- [ ] CrudExtractor.cs
  - [ ] ExtractTableCRUD()（テーブルCRUD抽出）
  - [ ] ExtractColumnCRUD()（カラムCRUD抽出）
  - [ ] AddTableCRUD()（VB.NETのAddTableCRUD()移植）
  - [ ] AddColumnCRUD()（VB.NETのAddColumnCRUD()移植）
  - [ ] AddColumnCRUD2()（VB.NETのAddColumnCRUD2()移植）
- [ ] SubQueryAnalyzer.cs
  - [ ] DivideWith()（WITH句分割）
  - [ ] DivideUnion()（UNION/MINUS/INTERSECT分割）
  - [ ] DivideSubQuery()（サブクエリ抽出）
- [ ] ClauseSplitter.cs
  - [ ] SplitKu()（VB.NETのSplitKu()をANTLR4で実装）
  - [ ] SplitWords()（複数区切り文字で分割）
  - [ ] SplitKanma()（カンマ分割、括弧考慮）
  - [ ] SplitJOIN()（JOIN句分割）
- [ ] TableColumnResolver.cs
  - [ ] ConvertRealName()（別名→実名変換）
  - [ ] ResolveTableAlias()（テーブル別名解決）
  - [ ] ResolveColumnFullName()（カラムフルネーム解決）
- [ ] SelectClauseAnalyzer.cs
  - [ ] AnalyzeSelectKu()（SELECT句解析）
  - [ ] ExtractAliases()（別名抽出）
- [ ] FromClauseAnalyzer.cs
  - [ ] AnalyzeFrom()（FROM句解析）
  - [ ] ParseJoins()（JOIN解析）
- [ ] SetClauseAnalyzer.cs
  - [ ] AnalyzeSetKu()（SET句解析）
- [ ] HelperFunctions.cs
  - [ ] DeleteYobunKakko()（余分な括弧削除）
  - [ ] FixCursorForLoop()（カーソルFORループ修正）
  - [ ] ReplaceDelimToSpace()（区切り文字→空白）

### 3.3 クエリ整形（clsArrangeQuery.vb → C#）
- [ ] QueryFormatter.cs
  - [ ] CArrange()（SQL整形メイン）
  - [ ] キーワードインデント設定
  - [ ] SELECT句インデント
  - [ ] FROM句インデント
  - [ ] WHERE句インデント
  - [ ] JOIN句インデント
  - [ ] サブクエリインデント

### 3.4 ライセンス・認証（CommonLicence.vb → 新認証システム）
- [ ] LicenseClient.cs（クライアント側）
  - [ ] AuthenticateAsync()（認証API呼び出し）
  - [ ] ValidateOffline()（オフライン検証）
  - [ ] CacheLicense()（ライセンスキャッシュ）
- [ ] LicenseValidator.cs（検証ロジック）
  - [ ] ValidateFormat()（16桁フォーマット検証）
  - [ ] ValidateEmail()（メールアドレス検証）
  - [ ] CheckExpiration()（有効期限確認）

### 3.5 コンテキストメニュー（MenuContents.vb → C#）
- [ ] ContextMenuManager.cs
  - [ ] InitializeCommonMenus()
  - [ ] CreateCRUDSearchMenu()
  - [ ] CreateTableDefMenu()
  - [ ] CreateFileNavigationMenu()

---

## フェーズ4: 認証サーバー実装

### 4.1 データベース設計
- [ ] マイグレーション初期化
- [ ] Usersテーブル
  - [ ] Id（Guid、PK）
  - [ ] EmailAddress（string、Unique）
  - [ ] CreatedAt（DateTime）
  - [ ] UpdatedAt（DateTime）
  - [ ] IsActive（bool）
- [ ] LicenseKeysテーブル
  - [ ] Id（Guid、PK）
  - [ ] UserId（Guid、FK → Users）
  - [ ] LicenseKey（string、16桁、Unique）
  - [ ] IssuedAt（DateTime）
  - [ ] ExpiresAt（DateTime）
  - [ ] IsActive（bool）
  - [ ] MaxDevices（int）
  - [ ] ProductType（string）
- [ ] DeviceActivationsテーブル
  - [ ] Id（Guid、PK）
  - [ ] LicenseKeyId（Guid、FK → LicenseKeys）
  - [ ] DeviceId（string）
  - [ ] ActivatedAt（DateTime）
  - [ ] LastSeenAt（DateTime）
- [ ] AuditLogsテーブル
  - [ ] Id（Guid、PK）
  - [ ] UserId（Guid、FK → Users、nullable）
  - [ ] Action（string）
  - [ ] Timestamp（DateTime）
  - [ ] IpAddress（string）
  - [ ] Details（string、JSON）

### 4.2 Entity Framework Core設定
- [ ] AuthDbContext.cs
  - [ ] DbSet<User>
  - [ ] DbSet<LicenseKey>
  - [ ] DbSet<DeviceActivation>
  - [ ] DbSet<AuditLog>
  - [ ] OnModelCreating()（リレーション設定）
- [ ] エンティティクラス
  - [ ] User.cs
  - [ ] LicenseKey.cs
  - [ ] DeviceActivation.cs
  - [ ] AuditLog.cs

### 4.3 サービス層
- [ ] Services/LicenseGenerationService.cs
  - [ ] GenerateLicenseKey()（16桁キー生成）
  - [ ] ValidateKeyFormat()
  - [ ] EncryptKey()（暗号化）
- [ ] Services/EmailValidationService.cs
  - [ ] ValidateEmailFormat()
  - [ ] CheckEmailDuplicate()
  - [ ] SendActivationEmail()（オプション）
- [ ] Services/AuthenticationService.cs
  - [ ] AuthenticateAsync()
  - [ ] GenerateJwtToken()
  - [ ] ValidateLicenseKeyAsync()
  - [ ] ActivateDeviceAsync()
- [ ] Services/AuditLogService.cs
  - [ ] LogActionAsync()

### 4.4 コントローラー
- [ ] Controllers/LicenseController.cs
  - [ ] POST /api/license/authenticate
    - [ ] リクエスト: { emailAddress, licenseKey, deviceId }
    - [ ] レスポンス: { token, expiresAt, isValid }
  - [ ] POST /api/license/validate
    - [ ] リクエスト: { token }
    - [ ] レスポンス: { isValid, expiresAt }
  - [ ] GET /api/license/status
    - [ ] 認証必須
    - [ ] レスポンス: { licenseKey, expiresAt, activeDevices, maxDevices }
- [ ] Controllers/AdminController.cs
  - [ ] GET /api/admin/licenses（一覧）
  - [ ] POST /api/admin/licenses（新規作成）
  - [ ] PUT /api/admin/licenses/{id}（更新）
  - [ ] DELETE /api/admin/licenses/{id}（削除）
  - [ ] GET /api/admin/users（ユーザー一覧）
  - [ ] POST /api/admin/users（ユーザー作成）
  - [ ] GET /api/admin/audit-logs（監査ログ）

### 4.5 JWT認証設定
- [ ] Program.cs設定
  - [ ] AddAuthentication()
  - [ ] AddJwtBearer()
  - [ ] 秘密鍵設定（appsettings.json）
- [ ] JwtOptions.cs
  - [ ] SecretKey
  - [ ] Issuer
  - [ ] Audience
  - [ ] ExpirationMinutes

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

### 6.1 Phase 1（必須DB）
#### 6.1.1 PostgreSQL
- [ ] Npgsql NuGetパッケージ
- [ ] ConnectionStringBuilder
- [ ] スキーマ取得クエリ
- [ ] テーブル一覧取得
- [ ] カラム定義取得（PK/FK/型情報）
- [ ] PostgreSQL固有構文対応（ANTLR）
  - [ ] RETURNING句
  - [ ] ON CONFLICT
  - [ ] ARRAY型
  - [ ] JSONB型
  - [ ] ウィンドウ関数

#### 6.1.2 MySQL
- [ ] MySql.Data NuGetパッケージ
- [ ] スキーマ取得
- [ ] MySQL固有構文対応
  - [ ] LIMIT句
  - [ ] ON DUPLICATE KEY UPDATE
  - [ ] バッククォート識別子

#### 6.1.3 SQL Server
- [ ] Microsoft.Data.SqlClient NuGetパッケージ
- [ ] スキーマ取得（sys.tablesビュー）
- [ ] SQL Server固有構文対応
  - [ ] TOP句
  - [ ] OUTPUT句
  - [ ] OFFSET FETCH
  - [ ] [角括弧]識別子

#### 6.1.4 Oracle
- [ ] Oracle.ManagedDataAccess.Core NuGetパッケージ
- [ ] スキーマ取得（ALL_TABLESビュー）
- [ ] Oracle固有構文対応
  - [ ] ROWNUM
  - [ ] CONNECT BY（階層クエリ）
  - [ ] (+)外部結合構文
  - [ ] DUAL表

### 6.2 Phase 2（裾野拡大DB）
#### 6.2.1 SQLite
- [ ] Microsoft.Data.Sqlite NuGetパッケージ
- [ ] スキーマ取得（sqlite_master）
- [ ] SQLite固有制限対応

#### 6.2.2 MariaDB
- [ ] MySqlConnector NuGetパッケージ
- [ ] MySQL互換対応

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

### 6.4 共通DBアクセス層
- [ ] IDbConnectionFactory
- [ ] ISchemaProvider
- [ ] DbProviderFactory実装
- [ ] 接続文字列管理
- [ ] エラーハンドリング

---

## フェーズ7: テスト・検証

### 7.1 ユニットテスト
- [ ] xUnit or NUnit導入
- [ ] CRUDExplorer.Core.Tests プロジェクト作成
- [ ] CRUDExplorer.SqlParser.Tests プロジェクト作成
- [ ] データモデルテスト
  - [ ] Query.cs テスト（30ケース以上）
  - [ ] Settings.cs テスト
  - [ ] CrudInfo.cs テスト
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
- [ ] ユーティリティテスト
  - [ ] DictionaryHelper（10ケース）
  - [ ] StringUtilities（15ケース）

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
- **フェーズ3**: ANTLR4パーサー実装完了、既存CommonAnalyze.vb全機能再現
- **フェーズ4**: 認証サーバーAPI稼働、管理画面動作確認
- **フェーズ5**: 全13画面表示確認、主要機能動作確認
- **フェーズ6**: PostgreSQL/MySQL/SQL Server/Oracle接続成功
- **フェーズ7**: 全テストパス、Windows/Mac動作確認完了
- **フェーズ8**: ドキュメント完成、インストーラー作成、認証サーバーデプロイ完了

---

## 現在の状況

- **完了**: フェーズ1（100%）、フェーズ2（100%）
- **進行中**: フェーズ3（0%）
- **次のタスク**: ANTLR4 SQL文法ファイル作成開始
