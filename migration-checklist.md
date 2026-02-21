# 移植チェックリスト（CRUDExplorer）

## ビルド状況
- 実行: `dotnet build original/CRUDExplorer2011/CRUDExplorer.vbproj`（Linux 環境） → .NET Framework v4.0 の参照アセンブリ不足で失敗。クロスプラットフォーム向けにプロジェクト再作成が必要。

## 画面・コントロール洗い出し
- 既存 WinForms 画面のコントロール名（WithEvents 宣言）とイベントハンドラ一覧。移植時は同名・同イベントを対応付ける。

### frmMain（CRUD マトリクス）
- コントロール: MenuStrip1, tsmOpen, Panel1, OpenFileDialog1, FolderBrowserDialog1, SplitContainer1, lblSourcePath, Label1, Panel4, Panel3, マトリックスをToolStripMenuItem, ToolStripMenuItem1, Panel7, txtFilterTableName, txtFilterProgramId, Label5, Label4, dgvMatrix, btnClearFilter, btnApplyFilter, Panel8, cmdAnalyzeQuery, cmdRunHidemaru, lstCRUD, chFile, chTableName, chLineNo, chCRUD, chFuncProc, chAlt, AnalyzeCRUDToolStripMenuItem, SettingsToolStripMenuItem, chEntityName, テーブルCRUDToolStripMenuItem, カラムCRUDToolStripMenuItem, Panel9, btnToggleDisplayedName, btnTableDef, chkD, chkU, chkR, chkC, TableDefToolStripMenuItem, ソースファイル一覧ToolStripMenuItem, クエリ解析ToolStripMenuItem, CRUDは開かないToolStripMenuItem, cmdDetailFilter, cmsMatrix, tsmCopyClipBoard, tsmTableDef, chProgram, サポートサイトToolStripMenuItem, ToolStripMenuItem2, バージョン情報ToolStripMenuItem
- イベント: テーブルCRUDToolStripMenuItem_Click, カラムCRUDToolStripMenuItem_Click, dgvMatrix_CellClick/CellEnter/CellPainting/ColumnAdded/Scroll, lstCRUD_DoubleClick/ColumnClick/KeyDown, frmMain_Load/FormClosing/FormClosed, マトリックスをToolStripMenuItem_Click, btnApplyFilter_Click, btnClearFilter_Click, txtFilterTableName_KeyPress, cmdRunHidemaru_Click, cmdAnalyzeQuery_Click, AnalyzeCRUDToolStripMenuItem_Click, SettingsToolStripMenuItem_Click, dgvMatrix_KeyDown, btnToggleDisplayedName_Click, btnTableDef_Click, TableDefToolStripMenuItem_Click, ソースファイル一覧ToolStripMenuItem_Click, クエリ解析ToolStripMenuItem_Click, CRUDは開かないToolStripMenuItem_Click, cmdDetailFilter_Click, tsmCopyClipBoard_Click, tsmTableDef_Click, バージョン情報ToolStripMenuItem_Click, サポートサイトToolStripMenuItem_Click

### frmAnalyzeQuery（SQL 解析ウィンドウ）
- コントロール: txtFileName, Label5, txtAltName, Label3, txtTableName, Label2, cmdRunHidemaru, cmdClose, tvQuery, rtbQuery, Panel5, txtHighLight3, txtHighLight2, txtHighLight1, Label4, lstCRUD, chTableName, chEntityName, chAltName, Panel7, lstColumnCRUD, ccTableName, ccEntityName, ccCRUD, ccColumnName, ccAttributeName, Panel11, txtGuide, SplitContainer5, SplitContainer1, SplitContainer6, Panel6, Panel1, Panel2, Panel4, Panel12, btnQuickAnalyze, btnDivideStrings, cmsTextBox, tsmOtherCRUD, cmbQuery, GrepToolStripMenuItem, GrepFileToolStripMenuItem, GrepProgramToolStripMenuItem, GrepAllToolStripMenuItem, cmdExpandView, cmdExchangeLogicalName, ハイライトToolStripMenuItem, HighLight1ToolStripMenuItem, HighLight2ToolStripMenuItem, HighLight3ToolStripMenuItem, HighLightClearToolStripMenuItem, cmsTableList, 全選択ToolStripMenuItem, 全解除ToolStripMenuItem, テーブル定義ToolStripMenuItem, cmsColumnList, ToolStripMenuItem1, ToolStripMenuItem2, cmsTreeView, tsmExpandSubQuery, テーブル定義ToolStripMenuItem1, 句の情報ToolStripMenuItem, tsmSelect, tsmWhere, tsmGroupBy, tsmOrderBy, tsmHaving, tsmUpdate, tsmDelete, tsmInsert, tsmSetCond, tsmAllKu, ToolStripMenuItem3, tsmIntoValues, CRUDサーチToolStripMenuItem, このテーブルにアクセスしている処理ToolStripMenuItem, CRUDサーチToolStripMenuItem1, このカラムにアクセスしている処理ToolStripMenuItem, このテーブルにアクセスしている処理ToolStripMenuItem1, txtLineNo, Label1, tsmSetValues, btnTableDef, tsmSelects, tsmTableDef, btnTextSearch, btnSearchPrev, btnSearchNext, chCRUD
- イベント: tvQuery_AfterSelect/KeyDown, lstCRUD_ItemChecked/ItemSelectionChanged, cmdRunHidemaru_Click, cmdClose_Click, frmAnalyzeQuery_Activated/Load, rtbQuery_DoubleClick/KeyDown/KeyUp/KeyPress/Click, txtHighLight*_TextChanged, lstColumnCRUD_ItemChecked, btnQuickAnalyze_Click, btnDivideStrings_Click, tsmOtherCRUD_Click, cmbQuery_SelectedIndexChanged, Grep* メニュー Click, cmdExpandView_Click, cmdExchangeLogicalName_Click, HighLight*ToolStripMenuItem_Click, HighLightClearToolStripMenuItem_Click, 全選択/全解除/ToolStripMenuItem1/ToolStripMenuItem2_Click, テーブル定義*MenuItem_Click, tsmExpandSubQuery_Click, tsmSelect/Where/GroupBy/OrderBy/Having/Insert/Update/Delete/SetCond/AllKu/IntoValues/SetValues/Selects_Click, 句の情報ToolStripMenuItem_Paint, このテーブル/カラムにアクセスしている処理*_Click, btnTableDef_Click_1, txtGuide_Click, tsmTableDef_Click, btnTextSearch_Click, btnSearchNext_Click, btnSearchPrev_Click

### frmCRUDSearch（他 CRUD サーチ）
- コントロール: lstQuerys, chLineNo, cmdAnalyzeQuery, cmdRunHidemaru, chFuncProc, cmdAnalyzeQueryNew, Panel7, cmdClose, chFilename, chProgram, Panel1, Panel2, chCRUD
- イベント: cmdRunHidemaru_Click, cmdAnalyzeQuery_Click, cmdAnalyzeQueryNew_Click, lstQuerys_DoubleClick/ColumnClick/KeyDown, cmdClose_Click, frmOtherCRUD_Load

### frmFileList（ファイル別クエリ一覧）
- コントロール: SplitContainer1, lstFiles, chFile, chProgramId, lstQuerys, chQuery, chLineNo, Panel1, cmdAnalyzeQuery, cmdRunHidemaru, chFuncProc, cmdAnalyzeQueryNew, Panel7, cmdClose, pngGrep, pnlList, Label1, Panel2, chkRegular, txtSearch, Panel3, Panel4, Panel5, btnGrep, Panel8, Panel6, Panel9, chFilename, Panel10, cmsFiles, 全選択ToolStripMenuItem, 全解除ToolStripMenuItem
- イベント: lstFiles_Click/SelectedIndexChanged/KeyDown, cmdRunHidemaru_Click, cmdAnalyzeQuery_Click, cmdAnalyzeQueryNew_Click, lstQuerys_DoubleClick/ColumnClick/KeyDown, cmdClose_Click, btnGrep_Click, txtSearch_KeyDown, frmFileList_Activated/FormClosed/Load, 全選択ToolStripMenuItem_Click, 全解除ToolStripMenuItem_Click

### frmFilter（フィルタ条件）
- コントロール: GroupBox1/2/3, Label1, cmbProgramAccess, cmdCancel, cmdApply, lstProgram (chTableName, chEntityName), lstTable (ColumnHeader1/2), cmsProgram（全選択/全解除）, cmsTable（ToolStripMenuItem1/2）, Label2, cmbTableAccess, btnAllClear
- イベント: 全選択ToolStripMenuItem_Click, 全解除ToolStripMenuItem_Click, ToolStripMenuItem1_Click, ToolStripMenuItem2_Click, cmbProgramAccess_SelectedIndexChanged, cmbTableAccess_SelectedIndexChanged, cmdCancel_Click, cmdApply_Click, btnAllClear_Click

### frmGrep（テキスト grep）
- コントロール: Panel2, Panel7, cmdClose, cmdAnalyzeQueryNew, cmdAnalyzeQuery, cmdRunHidemaru, lstQuerys, chQuery, chLineNo, chFuncProc, chFilename, chProgram
- イベント: cmdRunHidemaru_Click, cmdAnalyzeQuery_Click, cmdAnalyzeQueryNew_Click, lstQuerys_DoubleClick/ColumnClick/KeyDown, cmdClose_Click, frmGrep_Load

### frmList（汎用リスト）
- コントロール: Panel1, Panel2, cmdClose, Panel3, lstList
- イベント: lstList_ColumnClick, cmdClose_Click, frmList_Activated

### frmMakeCRUD（CRUD 自動生成）
- コントロール: Label1, btnSelectSourceFolder, chkReference, btnAnalyzeCRUD, grpState, Label2, txtLog, lblPhase, GroupBox1, chkStep4/3/2/1/0, chkProcAll, FolderBrowserDialog1, btnClose, pnlControl, Label4, chkReferenceCond, cmdAnalyzeQuery, cmdRunHidemaru, btnSelectDestFolder, txtDestPath, Label3, txtSourcePath
- イベント: chkProcAll_CheckedChanged, btnAnalyzeCRUD_Click, btnSelectSourceFolder_Click, btnSelectDestFolder_Click, btnClose_Click, frmMakeCRUD_Activated/Load, txtLog_DoubleClick, cmdRunHidemaru_Click, cmdAnalyzeQuery_Click

### frmSearch（単純検索）
- コントロール: Label1, txtSearch, chkRegular, btnSearch, btnCancel
- イベント: btnCancel_Click, btnSearch_Click, frmSearch_Activated

### frmSettings（設定）
- コントロール: btnSelectHidemaruPath, Label1, txtHidemaruPath, OpenFileDialog1, cmdCancel, cmdSave, GroupBox1 (rdoListDblClickMode0/1/2), chkDebugMode, Label2, txtProgramIdPattern, Label3, grpTextEditor (rdoEditorSakura/Hidemaru/Notepad), txtNotepadPath, Label7, btnSelectNotepadPath, txtSakuraPath, Label6, btnSelectSakuraPath
- イベント: frmSettings_Load, cmdCancel_Click, cmdSave_Click, btnSelectHidemaruPath_Click, btnSelectNotepadPath_Click, btnSelectSakuraPath_Click

### frmStartup（起動スプラッシュ）
- コントロール: Label2, PictureBox1, PictureBox2, lblDemo, Label1, lblVersion
- イベント: frmStartup_Load

### frmTableDef（テーブル定義参照）
- コントロール: Panel1, cmdClose, Panel2, lstTableDef (chColumnName, chAttributeName, chPK, chFK, chRequired, chDateType, chDigit, chAccuracy, chNo), ContextMenuStrip1（テーブルにToolStripMenuItem, tsmTableAccess, tsmColumnAccess）, SplitContainer1, lstTable (ColumnHeader1/2), pnlRelate, bntInsEnter, Label1, btnInsTableColumn, cmbInsText, btnInsEQ, btnInsOr, btnInsAnd, bntInsEndKakko, btnInsBeginKakko, Panel4, Panel3, btnClearFilter, btnApplyFilter, txtFilter, btnInsTable, btnInsConma
- イベント: cmdClose_Click, frmTableDef_Load/FormClosed, tsmTableAccess_Click, tsmColumnAccess_Click, テーブルにToolStripMenuItem_Paint, lstTable_SelectedIndexChanged, lstTableDef_DoubleClick, btnInsTableColumn_Click, btnInsTable_Click, bntInsEnter_Click, btnInsConma_Click, btnInsBeginKakko_Click, bntInsEndKakko_Click, btnInsAnd_Click, btnInsOr_Click, btnInsEQ_Click, cmbInsText_SelectedIndexChanged, txtFilter_KeyPress, btnApplyFilter_Click, btnClearFilter_Click

### frmVersion（バージョン/ライセンス）
- コントロール: lblVersion, Label1, cmdClose, Label2, lblDemo, lblCopyright, PictureBox1, GroupBox1, txtLicenseKey, Label3, btnAuthentication, txtEMailAddr, Label4, lblAccept
- イベント: frmVersion_Load, btnAuthentication_Click

## 共通モジュール・クラスの主な責務
- CommonModule.vb: 汎用ユーティリティ（Win32 SendMessage, 正規表現ヘルパー、辞書読み込み、エディタ起動、ListView 共通コンテキストメニュー構成、テーブル定義表示、論理名解決）。RSA 鍵文字列もここで保持。
- CommonAnalyze.vb: 独自 SQL 解析ロジック（CRUD 抽出、句の分割、JOIN 分解、列 CRUD 追加、論理名変換、値/句の解析、サブクエリ展開、カンマ分割、整形）。
- CommonLicence.vb: RSA 鍵生成・暗号化/復号、ライセンスキー交換/復号ロジック。
- clsQuery/clsColumn/clsTableDef/clsView/clsCRUD 他: SQL オブジェクトモデルと比較器。ColumnCollection, ViewCollection, CRUD 表現、ListViewItemComparer など。
- clsArrangeQuery.vb: クエリ整形（インデント管理）。
- clsSettings.vb: 設定保存/読み込み、フォームサイズ保持、エディタパス・ダブルクリック動作・ライセンス情報管理。
- MenuContents.vb: ListView 用コンテキストメニュー生成ヘルパー。

## 移行タスクチェックリスト（未着手）
- [ ] 画面 UI 再構成（各フォームのコントロール・イベントを 1:1 で再現）  
      - [ ] frmMain / [ ] frmAnalyzeQuery / [ ] frmCRUDSearch / [ ] frmFileList / [ ] frmFilter / [ ] frmGrep / [ ] frmList / [ ] frmMakeCRUD / [ ] frmSearch / [ ] frmSettings / [ ] frmStartup / [ ] frmTableDef / [ ] frmVersion
- [ ] テキストエディタ置換（Azuki 相当の構文ハイライト/行番号付きエディタをクロスプラットフォームで選定・組込み）
- [ ] SQL 解析を ANTLR ベースに置換（現行 CommonAnalyze のルールを ANTLR grammar へ移植）
- [ ] DB 接続対応  
      - [ ] Phase1: PostgreSQL / MySQL / SQL Server / Oracle  
      - [ ] Phase2: SQLite / MariaDB  
      - [ ] Phase3: Snowflake / BigQuery / Databricks SQL / Redshift
- [ ] ライセンス認証クラウドサーバ実装（メール+ライセンスコード認証、管理 UI: ライセンス生成・メール紐付け）※現行 CommonLicence のローカル暗号化方式を置換
- [ ] クロスプラットフォームビルドパイプライン整備（Windows/Mac 向けパッケージング、ランタイム依存確認）
- [ ] 旧外部エディタ連携（Hidemaru/Sakura/Notepad）代替の検討と設定画面置換

## メモ（移植時の注意）
- 現行は Windows Forms 依存・.NET Framework 4.0。Mac 対応には .NET 6+（または MAUI/ Avalonia 等）への再設計が必要。
- SQL 解析や CRUD 抽出の独自処理が多く、ANTLR 置換時は CommonAnalyze/clsQuery/clsColumn のデータ構造と互換を保つ必要あり。
- ライセンス鍵（RSA 文字列）がソース内にベタ書きされているため、新認証サーバでは安全な保管・発行フローに変更する。
