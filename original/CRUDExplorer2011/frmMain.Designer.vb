<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.tsmOpen = New System.Windows.Forms.ToolStripMenuItem
        Me.テーブルCRUDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.カラムCRUDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CRUDは開かないToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AnalyzeCRUDToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ソースファイル一覧ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.マトリックスをToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TableDefToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.クエリ解析ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.サポートサイトToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator
        Me.バージョン情報ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.lblSourcePath = New System.Windows.Forms.Label
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.Label1 = New System.Windows.Forms.Label
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.dgvMatrix = New System.Windows.Forms.DataGridView
        Me.cmsMatrix = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmCopyClipBoard = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmTableDef = New System.Windows.Forms.ToolStripMenuItem
        Me.Panel7 = New System.Windows.Forms.Panel
        Me.cmdDetailFilter = New System.Windows.Forms.Button
        Me.chkD = New System.Windows.Forms.CheckBox
        Me.chkU = New System.Windows.Forms.CheckBox
        Me.chkR = New System.Windows.Forms.CheckBox
        Me.chkC = New System.Windows.Forms.CheckBox
        Me.Panel9 = New System.Windows.Forms.Panel
        Me.btnToggleDisplayedName = New System.Windows.Forms.Button
        Me.btnClearFilter = New System.Windows.Forms.Button
        Me.btnApplyFilter = New System.Windows.Forms.Button
        Me.txtFilterTableName = New System.Windows.Forms.TextBox
        Me.txtFilterProgramId = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.lstCRUD = New System.Windows.Forms.ListView
        Me.chFile = New System.Windows.Forms.ColumnHeader
        Me.chLineNo = New System.Windows.Forms.ColumnHeader
        Me.chProgram = New System.Windows.Forms.ColumnHeader
        Me.chEntityName = New System.Windows.Forms.ColumnHeader
        Me.chTableName = New System.Windows.Forms.ColumnHeader
        Me.chCRUD = New System.Windows.Forms.ColumnHeader
        Me.chFuncProc = New System.Windows.Forms.ColumnHeader
        Me.chAlt = New System.Windows.Forms.ColumnHeader
        Me.Panel8 = New System.Windows.Forms.Panel
        Me.btnTableDef = New System.Windows.Forms.Button
        Me.cmdAnalyzeQuery = New System.Windows.Forms.Button
        Me.cmdRunHidemaru = New System.Windows.Forms.Button
        Me.MenuStrip1.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.dgvMatrix, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.cmsMatrix.SuspendLayout()
        Me.Panel7.SuspendLayout()
        Me.Panel9.SuspendLayout()
        Me.Panel8.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmOpen, Me.AnalyzeCRUDToolStripMenuItem, Me.ソースファイル一覧ToolStripMenuItem, Me.マトリックスをToolStripMenuItem, Me.TableDefToolStripMenuItem, Me.クエリ解析ToolStripMenuItem, Me.SettingsToolStripMenuItem, Me.ToolStripMenuItem1})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1008, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'tsmOpen
        '
        Me.tsmOpen.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.テーブルCRUDToolStripMenuItem, Me.カラムCRUDToolStripMenuItem, Me.CRUDは開かないToolStripMenuItem})
        Me.tsmOpen.Name = "tsmOpen"
        Me.tsmOpen.Size = New System.Drawing.Size(51, 20)
        Me.tsmOpen.Text = "開く(&O)"
        '
        'テーブルCRUDToolStripMenuItem
        '
        Me.テーブルCRUDToolStripMenuItem.Name = "テーブルCRUDToolStripMenuItem"
        Me.テーブルCRUDToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.テーブルCRUDToolStripMenuItem.Text = "テーブルCRUD"
        '
        'カラムCRUDToolStripMenuItem
        '
        Me.カラムCRUDToolStripMenuItem.Name = "カラムCRUDToolStripMenuItem"
        Me.カラムCRUDToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.カラムCRUDToolStripMenuItem.Text = "カラムCRUD"
        '
        'CRUDは開かないToolStripMenuItem
        '
        Me.CRUDは開かないToolStripMenuItem.Name = "CRUDは開かないToolStripMenuItem"
        Me.CRUDは開かないToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.CRUDは開かないToolStripMenuItem.Text = "CRUDは開かない"
        '
        'AnalyzeCRUDToolStripMenuItem
        '
        Me.AnalyzeCRUDToolStripMenuItem.Name = "AnalyzeCRUDToolStripMenuItem"
        Me.AnalyzeCRUDToolStripMenuItem.Size = New System.Drawing.Size(89, 20)
        Me.AnalyzeCRUDToolStripMenuItem.Text = "CRUD解析(&A)"
        '
        'ソースファイル一覧ToolStripMenuItem
        '
        Me.ソースファイル一覧ToolStripMenuItem.Name = "ソースファイル一覧ToolStripMenuItem"
        Me.ソースファイル一覧ToolStripMenuItem.Size = New System.Drawing.Size(121, 20)
        Me.ソースファイル一覧ToolStripMenuItem.Text = "ソースファイル一覧(&L) "
        '
        'マトリックスをToolStripMenuItem
        '
        Me.マトリックスをToolStripMenuItem.Name = "マトリックスをToolStripMenuItem"
        Me.マトリックスをToolStripMenuItem.Size = New System.Drawing.Size(152, 20)
        Me.マトリックスをToolStripMenuItem.Text = "マトリクスをクリップボードに(&C)"
        '
        'TableDefToolStripMenuItem
        '
        Me.TableDefToolStripMenuItem.Name = "TableDefToolStripMenuItem"
        Me.TableDefToolStripMenuItem.Size = New System.Drawing.Size(118, 20)
        Me.TableDefToolStripMenuItem.Text = "テーブル定義参照(&T)"
        '
        'クエリ解析ToolStripMenuItem
        '
        Me.クエリ解析ToolStripMenuItem.Name = "クエリ解析ToolStripMenuItem"
        Me.クエリ解析ToolStripMenuItem.Size = New System.Drawing.Size(124, 20)
        Me.クエリ解析ToolStripMenuItem.Text = "クエリ解析ウィンドウ(&Q)"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(56, 20)
        Me.SettingsToolStripMenuItem.Text = "設定(&X)"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.サポートサイトToolStripMenuItem, Me.ToolStripMenuItem2, Me.バージョン情報ToolStripMenuItem})
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(46, 20)
        Me.ToolStripMenuItem1.Text = "ヘルプ"
        '
        'サポートサイトToolStripMenuItem
        '
        Me.サポートサイトToolStripMenuItem.Name = "サポートサイトToolStripMenuItem"
        Me.サポートサイトToolStripMenuItem.Size = New System.Drawing.Size(139, 22)
        Me.サポートサイトToolStripMenuItem.Text = "サポートサイト"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(136, 6)
        '
        'バージョン情報ToolStripMenuItem
        '
        Me.バージョン情報ToolStripMenuItem.Name = "バージョン情報ToolStripMenuItem"
        Me.バージョン情報ToolStripMenuItem.Size = New System.Drawing.Size(139, 22)
        Me.バージョン情報ToolStripMenuItem.Text = "バージョン情報"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Panel4)
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 24)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1008, 23)
        Me.Panel1.TabIndex = 1
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.lblSourcePath)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(125, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(883, 23)
        Me.Panel4.TabIndex = 4
        '
        'lblSourcePath
        '
        Me.lblSourcePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblSourcePath.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblSourcePath.Location = New System.Drawing.Point(0, 0)
        Me.lblSourcePath.Name = "lblSourcePath"
        Me.lblSourcePath.Size = New System.Drawing.Size(883, 23)
        Me.lblSourcePath.TabIndex = 1
        Me.lblSourcePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.Label1)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(125, 23)
        Me.Panel3.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label1.Location = New System.Drawing.Point(0, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.Label1.Size = New System.Drawing.Size(125, 23)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "解析結果フォルダ　"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 47)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.dgvMatrix)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Panel7)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lstCRUD)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Panel8)
        Me.SplitContainer1.Size = New System.Drawing.Size(1008, 655)
        Me.SplitContainer1.SplitterDistance = 463
        Me.SplitContainer1.TabIndex = 3
        Me.SplitContainer1.TabStop = False
        '
        'dgvMatrix
        '
        Me.dgvMatrix.AllowUserToAddRows = False
        Me.dgvMatrix.AllowUserToDeleteRows = False
        Me.dgvMatrix.AllowUserToOrderColumns = True
        Me.dgvMatrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMatrix.ContextMenuStrip = Me.cmsMatrix
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.Font = New System.Drawing.Font("MS UI Gothic", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvMatrix.DefaultCellStyle = DataGridViewCellStyle1
        Me.dgvMatrix.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvMatrix.Location = New System.Drawing.Point(0, 32)
        Me.dgvMatrix.Name = "dgvMatrix"
        Me.dgvMatrix.ReadOnly = True
        Me.dgvMatrix.RowHeadersWidth = 300
        Me.dgvMatrix.RowTemplate.Height = 21
        Me.dgvMatrix.Size = New System.Drawing.Size(1008, 431)
        Me.dgvMatrix.TabIndex = 4
        '
        'cmsMatrix
        '
        Me.cmsMatrix.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmCopyClipBoard, Me.tsmTableDef})
        Me.cmsMatrix.Name = "cmsMatrix"
        Me.cmsMatrix.Size = New System.Drawing.Size(224, 48)
        '
        'tsmCopyClipBoard
        '
        Me.tsmCopyClipBoard.Name = "tsmCopyClipBoard"
        Me.tsmCopyClipBoard.Size = New System.Drawing.Size(223, 22)
        Me.tsmCopyClipBoard.Text = "マトリックスをクリップボードにコピー"
        '
        'tsmTableDef
        '
        Me.tsmTableDef.Name = "tsmTableDef"
        Me.tsmTableDef.Size = New System.Drawing.Size(223, 22)
        Me.tsmTableDef.Text = "テーブル定義"
        '
        'Panel7
        '
        Me.Panel7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel7.Controls.Add(Me.cmdDetailFilter)
        Me.Panel7.Controls.Add(Me.chkD)
        Me.Panel7.Controls.Add(Me.chkU)
        Me.Panel7.Controls.Add(Me.chkR)
        Me.Panel7.Controls.Add(Me.chkC)
        Me.Panel7.Controls.Add(Me.Panel9)
        Me.Panel7.Controls.Add(Me.btnClearFilter)
        Me.Panel7.Controls.Add(Me.btnApplyFilter)
        Me.Panel7.Controls.Add(Me.txtFilterTableName)
        Me.Panel7.Controls.Add(Me.txtFilterProgramId)
        Me.Panel7.Controls.Add(Me.Label5)
        Me.Panel7.Controls.Add(Me.Label4)
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel7.Location = New System.Drawing.Point(0, 0)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(1008, 32)
        Me.Panel7.TabIndex = 3
        '
        'cmdDetailFilter
        '
        Me.cmdDetailFilter.Location = New System.Drawing.Point(777, 2)
        Me.cmdDetailFilter.Name = "cmdDetailFilter"
        Me.cmdDetailFilter.Size = New System.Drawing.Size(79, 25)
        Me.cmdDetailFilter.TabIndex = 9
        Me.cmdDetailFilter.Text = "詳細フィルタ"
        Me.cmdDetailFilter.UseVisualStyleBackColor = True
        '
        'chkD
        '
        Me.chkD.AutoSize = True
        Me.chkD.Checked = True
        Me.chkD.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkD.Location = New System.Drawing.Point(569, 7)
        Me.chkD.Name = "chkD"
        Me.chkD.Size = New System.Drawing.Size(32, 16)
        Me.chkD.TabIndex = 8
        Me.chkD.Text = "D"
        Me.chkD.UseVisualStyleBackColor = True
        '
        'chkU
        '
        Me.chkU.AutoSize = True
        Me.chkU.Checked = True
        Me.chkU.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkU.Location = New System.Drawing.Point(531, 7)
        Me.chkU.Name = "chkU"
        Me.chkU.Size = New System.Drawing.Size(32, 16)
        Me.chkU.TabIndex = 7
        Me.chkU.Text = "U"
        Me.chkU.UseVisualStyleBackColor = True
        '
        'chkR
        '
        Me.chkR.AutoSize = True
        Me.chkR.Checked = True
        Me.chkR.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkR.Location = New System.Drawing.Point(493, 7)
        Me.chkR.Name = "chkR"
        Me.chkR.Size = New System.Drawing.Size(32, 16)
        Me.chkR.TabIndex = 6
        Me.chkR.Text = "R"
        Me.chkR.UseVisualStyleBackColor = True
        '
        'chkC
        '
        Me.chkC.AutoSize = True
        Me.chkC.Checked = True
        Me.chkC.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkC.Location = New System.Drawing.Point(455, 7)
        Me.chkC.Name = "chkC"
        Me.chkC.Size = New System.Drawing.Size(32, 16)
        Me.chkC.TabIndex = 5
        Me.chkC.Text = "C"
        Me.chkC.UseVisualStyleBackColor = True
        '
        'Panel9
        '
        Me.Panel9.Controls.Add(Me.btnToggleDisplayedName)
        Me.Panel9.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel9.Location = New System.Drawing.Point(894, 0)
        Me.Panel9.Name = "Panel9"
        Me.Panel9.Size = New System.Drawing.Size(110, 28)
        Me.Panel9.TabIndex = 4
        '
        'btnToggleDisplayedName
        '
        Me.btnToggleDisplayedName.Dock = System.Windows.Forms.DockStyle.Right
        Me.btnToggleDisplayedName.Location = New System.Drawing.Point(1, 0)
        Me.btnToggleDisplayedName.Name = "btnToggleDisplayedName"
        Me.btnToggleDisplayedName.Size = New System.Drawing.Size(109, 28)
        Me.btnToggleDisplayedName.TabIndex = 4
        Me.btnToggleDisplayedName.Tag = "1"
        Me.btnToggleDisplayedName.Text = "物理名に切り替え"
        Me.btnToggleDisplayedName.UseVisualStyleBackColor = True
        '
        'btnClearFilter
        '
        Me.btnClearFilter.Location = New System.Drawing.Point(692, 2)
        Me.btnClearFilter.Name = "btnClearFilter"
        Me.btnClearFilter.Size = New System.Drawing.Size(79, 25)
        Me.btnClearFilter.TabIndex = 3
        Me.btnClearFilter.Text = "フィルタクリア"
        Me.btnClearFilter.UseVisualStyleBackColor = True
        '
        'btnApplyFilter
        '
        Me.btnApplyFilter.Location = New System.Drawing.Point(607, 2)
        Me.btnApplyFilter.Name = "btnApplyFilter"
        Me.btnApplyFilter.Size = New System.Drawing.Size(79, 25)
        Me.btnApplyFilter.TabIndex = 2
        Me.btnApplyFilter.Text = "フィルタ適用"
        Me.btnApplyFilter.UseVisualStyleBackColor = True
        '
        'txtFilterTableName
        '
        Me.txtFilterTableName.Location = New System.Drawing.Point(115, 5)
        Me.txtFilterTableName.Name = "txtFilterTableName"
        Me.txtFilterTableName.Size = New System.Drawing.Size(126, 19)
        Me.txtFilterTableName.TabIndex = 0
        '
        'txtFilterProgramId
        '
        Me.txtFilterProgramId.Location = New System.Drawing.Point(314, 5)
        Me.txtFilterProgramId.Name = "txtFilterProgramId"
        Me.txtFilterProgramId.Size = New System.Drawing.Size(126, 19)
        Me.txtFilterProgramId.TabIndex = 1
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 8)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(100, 12)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "テーブル名・カラム名"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(247, 8)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(61, 12)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "プログラムID"
        '
        'lstCRUD
        '
        Me.lstCRUD.AllowColumnReorder = True
        Me.lstCRUD.CheckBoxes = True
        Me.lstCRUD.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chFile, Me.chLineNo, Me.chProgram, Me.chEntityName, Me.chTableName, Me.chCRUD, Me.chFuncProc, Me.chAlt})
        Me.lstCRUD.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstCRUD.FullRowSelect = True
        Me.lstCRUD.GridLines = True
        Me.lstCRUD.HideSelection = False
        Me.lstCRUD.Location = New System.Drawing.Point(0, 0)
        Me.lstCRUD.MultiSelect = False
        Me.lstCRUD.Name = "lstCRUD"
        Me.lstCRUD.Size = New System.Drawing.Size(1008, 150)
        Me.lstCRUD.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lstCRUD.TabIndex = 5
        Me.lstCRUD.UseCompatibleStateImageBehavior = False
        Me.lstCRUD.View = System.Windows.Forms.View.Details
        '
        'chFile
        '
        Me.chFile.Text = "ファイル"
        Me.chFile.Width = 219
        '
        'chLineNo
        '
        Me.chLineNo.Text = "行番号"
        Me.chLineNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'chProgram
        '
        Me.chProgram.Text = "プログラム"
        Me.chProgram.Width = 150
        '
        'chEntityName
        '
        Me.chEntityName.Text = "エンティティ名"
        Me.chEntityName.Width = 169
        '
        'chTableName
        '
        Me.chTableName.Text = "テーブル名"
        Me.chTableName.Width = 135
        '
        'chCRUD
        '
        Me.chCRUD.Text = "CURD"
        Me.chCRUD.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.chCRUD.Width = 46
        '
        'chFuncProc
        '
        Me.chFuncProc.Text = "関数名/カーソル名"
        Me.chFuncProc.Width = 152
        '
        'chAlt
        '
        Me.chAlt.Text = "代替"
        Me.chAlt.Width = 53
        '
        'Panel8
        '
        Me.Panel8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel8.Controls.Add(Me.btnTableDef)
        Me.Panel8.Controls.Add(Me.cmdAnalyzeQuery)
        Me.Panel8.Controls.Add(Me.cmdRunHidemaru)
        Me.Panel8.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel8.Location = New System.Drawing.Point(0, 150)
        Me.Panel8.Name = "Panel8"
        Me.Panel8.Size = New System.Drawing.Size(1008, 38)
        Me.Panel8.TabIndex = 4
        '
        'btnTableDef
        '
        Me.btnTableDef.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnTableDef.Location = New System.Drawing.Point(203, 5)
        Me.btnTableDef.Name = "btnTableDef"
        Me.btnTableDef.Size = New System.Drawing.Size(94, 25)
        Me.btnTableDef.TabIndex = 9
        Me.btnTableDef.Text = "テーブル定義"
        Me.btnTableDef.UseVisualStyleBackColor = True
        '
        'cmdAnalyzeQuery
        '
        Me.cmdAnalyzeQuery.Location = New System.Drawing.Point(103, 5)
        Me.cmdAnalyzeQuery.Name = "cmdAnalyzeQuery"
        Me.cmdAnalyzeQuery.Size = New System.Drawing.Size(94, 25)
        Me.cmdAnalyzeQuery.TabIndex = 7
        Me.cmdAnalyzeQuery.Text = "クエリ分析(&A)"
        Me.cmdAnalyzeQuery.UseVisualStyleBackColor = True
        '
        'cmdRunHidemaru
        '
        Me.cmdRunHidemaru.Location = New System.Drawing.Point(3, 5)
        Me.cmdRunHidemaru.Name = "cmdRunHidemaru"
        Me.cmdRunHidemaru.Size = New System.Drawing.Size(94, 25)
        Me.cmdRunHidemaru.TabIndex = 6
        Me.cmdRunHidemaru.Text = "エディタ起動(&E)"
        Me.cmdRunHidemaru.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1008, 702)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "CRUD Explorer"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.dgvMatrix, System.ComponentModel.ISupportInitialize).EndInit()
        Me.cmsMatrix.ResumeLayout(False)
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        Me.Panel9.ResumeLayout(False)
        Me.Panel8.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents tsmOpen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents lblSourcePath As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents マトリックスをToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents txtFilterTableName As System.Windows.Forms.TextBox
    Friend WithEvents txtFilterProgramId As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents dgvMatrix As System.Windows.Forms.DataGridView
    Friend WithEvents btnClearFilter As System.Windows.Forms.Button
    Friend WithEvents btnApplyFilter As System.Windows.Forms.Button
    Friend WithEvents Panel8 As System.Windows.Forms.Panel
    Friend WithEvents cmdAnalyzeQuery As System.Windows.Forms.Button
    Friend WithEvents cmdRunHidemaru As System.Windows.Forms.Button
    Friend WithEvents lstCRUD As System.Windows.Forms.ListView
    Friend WithEvents chFile As System.Windows.Forms.ColumnHeader
    Friend WithEvents chTableName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chLineNo As System.Windows.Forms.ColumnHeader
    Friend WithEvents chCRUD As System.Windows.Forms.ColumnHeader
    Friend WithEvents chFuncProc As System.Windows.Forms.ColumnHeader
    Friend WithEvents chAlt As System.Windows.Forms.ColumnHeader
    Friend WithEvents AnalyzeCRUDToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chEntityName As System.Windows.Forms.ColumnHeader
    Friend WithEvents テーブルCRUDToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents カラムCRUDToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Panel9 As System.Windows.Forms.Panel
    Friend WithEvents btnToggleDisplayedName As System.Windows.Forms.Button
    Friend WithEvents btnTableDef As System.Windows.Forms.Button
    Friend WithEvents chkD As System.Windows.Forms.CheckBox
    Friend WithEvents chkU As System.Windows.Forms.CheckBox
    Friend WithEvents chkR As System.Windows.Forms.CheckBox
    Friend WithEvents chkC As System.Windows.Forms.CheckBox
    Friend WithEvents TableDefToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ソースファイル一覧ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents クエリ解析ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CRUDは開かないToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmdDetailFilter As System.Windows.Forms.Button
    Friend WithEvents cmsMatrix As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmCopyClipBoard As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmTableDef As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents chProgram As System.Windows.Forms.ColumnHeader
    Friend WithEvents サポートサイトToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents バージョン情報ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
