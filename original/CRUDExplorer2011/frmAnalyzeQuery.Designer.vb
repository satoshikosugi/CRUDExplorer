<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAnalyzeQuery
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAnalyzeQuery))
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.cmbQuery = New System.Windows.Forms.ComboBox
        Me.Panel7 = New System.Windows.Forms.Panel
        Me.cmdClose = New System.Windows.Forms.Button
        Me.cmdRunHidemaru = New System.Windows.Forms.Button
        Me.txtAltName = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtTableName = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtLineNo = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtFileName = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.SplitContainer5 = New System.Windows.Forms.SplitContainer
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.tvQuery = New System.Windows.Forms.TreeView
        Me.cmsTreeView = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmExpandSubQuery = New System.Windows.Forms.ToolStripMenuItem
        Me.句の情報ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmSelect = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmWhere = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmGroupBy = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmOrderBy = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmHaving = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmInsert = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmUpdate = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmDelete = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmSetCond = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmAllKu = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator
        Me.tsmIntoValues = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmSetValues = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmSelects = New System.Windows.Forms.ToolStripMenuItem
        Me.SplitContainer6 = New System.Windows.Forms.SplitContainer
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.lstCRUD = New System.Windows.Forms.ListView
        Me.chTableName = New System.Windows.Forms.ColumnHeader
        Me.chEntityName = New System.Windows.Forms.ColumnHeader
        Me.chAltName = New System.Windows.Forms.ColumnHeader
        Me.cmsTableList = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.全選択ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.全解除ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.テーブル定義ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.CRUDサーチToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.このテーブルにアクセスしている処理ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.Panel12 = New System.Windows.Forms.Panel
        Me.lstColumnCRUD = New System.Windows.Forms.ListView
        Me.ccTableName = New System.Windows.Forms.ColumnHeader
        Me.ccEntityName = New System.Windows.Forms.ColumnHeader
        Me.ccColumnName = New System.Windows.Forms.ColumnHeader
        Me.ccAttributeName = New System.Windows.Forms.ColumnHeader
        Me.ccCRUD = New System.Windows.Forms.ColumnHeader
        Me.cmsColumnList = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.テーブル定義ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.CRUDサーチToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.このカラムにアクセスしている処理ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.このテーブルにアクセスしている処理ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.Panel6 = New System.Windows.Forms.Panel
        Me.rtbQuery = New System.Windows.Forms.RichTextBox
        Me.cmsTextBox = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.tsmOtherCRUD = New System.Windows.Forms.ToolStripMenuItem
        Me.GrepToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.GrepFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.GrepProgramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.GrepAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ハイライトToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HighLight1ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HighLight2ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HighLight3ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.HighLightClearToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmTableDef = New System.Windows.Forms.ToolStripMenuItem
        Me.Panel11 = New System.Windows.Forms.Panel
        Me.txtGuide = New System.Windows.Forms.TextBox
        Me.Panel5 = New System.Windows.Forms.Panel
        Me.btnSearchPrev = New System.Windows.Forms.Button
        Me.btnSearchNext = New System.Windows.Forms.Button
        Me.btnTextSearch = New System.Windows.Forms.Button
        Me.btnTableDef = New System.Windows.Forms.Button
        Me.cmdExpandView = New System.Windows.Forms.Button
        Me.cmdExchangeLogicalName = New System.Windows.Forms.Button
        Me.btnQuickAnalyze = New System.Windows.Forms.Button
        Me.txtHighLight3 = New System.Windows.Forms.TextBox
        Me.btnDivideStrings = New System.Windows.Forms.Button
        Me.txtHighLight2 = New System.Windows.Forms.TextBox
        Me.txtHighLight1 = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.chCRUD = New System.Windows.Forms.ColumnHeader
        Me.Panel1.SuspendLayout()
        Me.Panel7.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SplitContainer5.Panel1.SuspendLayout()
        Me.SplitContainer5.Panel2.SuspendLayout()
        Me.SplitContainer5.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.cmsTreeView.SuspendLayout()
        Me.SplitContainer6.Panel1.SuspendLayout()
        Me.SplitContainer6.Panel2.SuspendLayout()
        Me.SplitContainer6.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.cmsTableList.SuspendLayout()
        Me.Panel12.SuspendLayout()
        Me.cmsColumnList.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.cmsTextBox.SuspendLayout()
        Me.Panel11.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel1.Controls.Add(Me.cmbQuery)
        Me.Panel1.Controls.Add(Me.Panel7)
        Me.Panel1.Controls.Add(Me.txtAltName)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.txtTableName)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.txtLineNo)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.txtFileName)
        Me.Panel1.Controls.Add(Me.Label5)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1008, 33)
        Me.Panel1.TabIndex = 2
        '
        'cmbQuery
        '
        Me.cmbQuery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbQuery.FormattingEnabled = True
        Me.cmbQuery.Location = New System.Drawing.Point(8, 6)
        Me.cmbQuery.Name = "cmbQuery"
        Me.cmbQuery.Size = New System.Drawing.Size(244, 20)
        Me.cmbQuery.TabIndex = 1
        '
        'Panel7
        '
        Me.Panel7.Controls.Add(Me.cmdClose)
        Me.Panel7.Controls.Add(Me.cmdRunHidemaru)
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel7.Location = New System.Drawing.Point(803, 0)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(201, 29)
        Me.Panel7.TabIndex = 12
        '
        'cmdClose
        '
        Me.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdClose.Location = New System.Drawing.Point(104, 2)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(94, 25)
        Me.cmdClose.TabIndex = 6
        Me.cmdClose.Text = "閉じる(&C)"
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'cmdRunHidemaru
        '
        Me.cmdRunHidemaru.Location = New System.Drawing.Point(4, 2)
        Me.cmdRunHidemaru.Name = "cmdRunHidemaru"
        Me.cmdRunHidemaru.Size = New System.Drawing.Size(94, 25)
        Me.cmdRunHidemaru.TabIndex = 5
        Me.cmdRunHidemaru.Text = "エディタ起動(&E)"
        Me.cmdRunHidemaru.UseVisualStyleBackColor = True
        '
        'txtAltName
        '
        Me.txtAltName.Location = New System.Drawing.Point(875, 6)
        Me.txtAltName.Name = "txtAltName"
        Me.txtAltName.ReadOnly = True
        Me.txtAltName.Size = New System.Drawing.Size(42, 19)
        Me.txtAltName.TabIndex = 11
        Me.txtAltName.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(840, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 12)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "代替"
        '
        'txtTableName
        '
        Me.txtTableName.Location = New System.Drawing.Point(685, 5)
        Me.txtTableName.Name = "txtTableName"
        Me.txtTableName.ReadOnly = True
        Me.txtTableName.Size = New System.Drawing.Size(149, 19)
        Me.txtTableName.TabIndex = 4
        Me.txtTableName.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(624, 8)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 12)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "テーブル名"
        '
        'txtLineNo
        '
        Me.txtLineNo.Location = New System.Drawing.Point(561, 5)
        Me.txtLineNo.Name = "txtLineNo"
        Me.txtLineNo.ReadOnly = True
        Me.txtLineNo.Size = New System.Drawing.Size(43, 19)
        Me.txtLineNo.TabIndex = 3
        Me.txtLineNo.TabStop = False
        Me.txtLineNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(514, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 12)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "行番号"
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(329, 5)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.ReadOnly = True
        Me.txtFileName.Size = New System.Drawing.Size(168, 19)
        Me.txtFileName.TabIndex = 2
        Me.txtFileName.TabStop = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(268, 8)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(51, 12)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "ファイル名"
        '
        'Panel2
        '
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel2.Controls.Add(Me.SplitContainer5)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 33)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1008, 629)
        Me.Panel2.TabIndex = 3
        '
        'SplitContainer5
        '
        Me.SplitContainer5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer5.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer5.Name = "SplitContainer5"
        '
        'SplitContainer5.Panel1
        '
        Me.SplitContainer5.Panel1.Controls.Add(Me.SplitContainer1)
        '
        'SplitContainer5.Panel2
        '
        Me.SplitContainer5.Panel2.Controls.Add(Me.Panel6)
        Me.SplitContainer5.Panel2.Controls.Add(Me.Panel11)
        Me.SplitContainer5.Panel2.Controls.Add(Me.Panel5)
        Me.SplitContainer5.Size = New System.Drawing.Size(1004, 625)
        Me.SplitContainer5.SplitterDistance = 330
        Me.SplitContainer5.TabIndex = 2
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.tvQuery)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer6)
        Me.SplitContainer1.Size = New System.Drawing.Size(330, 625)
        Me.SplitContainer1.SplitterDistance = 116
        Me.SplitContainer1.TabIndex = 1
        '
        'tvQuery
        '
        Me.tvQuery.ContextMenuStrip = Me.cmsTreeView
        Me.tvQuery.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvQuery.HideSelection = False
        Me.tvQuery.Location = New System.Drawing.Point(0, 0)
        Me.tvQuery.Name = "tvQuery"
        Me.tvQuery.Size = New System.Drawing.Size(330, 116)
        Me.tvQuery.TabIndex = 2
        '
        'cmsTreeView
        '
        Me.cmsTreeView.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmExpandSubQuery, Me.句の情報ToolStripMenuItem})
        Me.cmsTreeView.Name = "cmsFiles"
        Me.cmsTreeView.Size = New System.Drawing.Size(221, 48)
        '
        'tsmExpandSubQuery
        '
        Me.tsmExpandSubQuery.Name = "tsmExpandSubQuery"
        Me.tsmExpandSubQuery.Size = New System.Drawing.Size(220, 22)
        Me.tsmExpandSubQuery.Text = "サブクエリを展開"
        '
        '句の情報ToolStripMenuItem
        '
        Me.句の情報ToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmSelect, Me.tsmWhere, Me.tsmGroupBy, Me.tsmOrderBy, Me.tsmHaving, Me.tsmInsert, Me.tsmUpdate, Me.tsmDelete, Me.tsmSetCond, Me.tsmAllKu, Me.ToolStripMenuItem3, Me.tsmIntoValues, Me.tsmSetValues, Me.tsmSelects})
        Me.句の情報ToolStripMenuItem.Name = "句の情報ToolStripMenuItem"
        Me.句の情報ToolStripMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.句の情報ToolStripMenuItem.Text = "句毎のカラムアクセス一覧"
        '
        'tsmSelect
        '
        Me.tsmSelect.Name = "tsmSelect"
        Me.tsmSelect.Size = New System.Drawing.Size(229, 22)
        Me.tsmSelect.Text = "Select"
        '
        'tsmWhere
        '
        Me.tsmWhere.Name = "tsmWhere"
        Me.tsmWhere.Size = New System.Drawing.Size(229, 22)
        Me.tsmWhere.Text = "Where"
        '
        'tsmGroupBy
        '
        Me.tsmGroupBy.Name = "tsmGroupBy"
        Me.tsmGroupBy.Size = New System.Drawing.Size(229, 22)
        Me.tsmGroupBy.Text = "GroupBy"
        '
        'tsmOrderBy
        '
        Me.tsmOrderBy.Name = "tsmOrderBy"
        Me.tsmOrderBy.Size = New System.Drawing.Size(229, 22)
        Me.tsmOrderBy.Text = "OrderBy"
        '
        'tsmHaving
        '
        Me.tsmHaving.Name = "tsmHaving"
        Me.tsmHaving.Size = New System.Drawing.Size(229, 22)
        Me.tsmHaving.Text = "Having"
        '
        'tsmInsert
        '
        Me.tsmInsert.Name = "tsmInsert"
        Me.tsmInsert.Size = New System.Drawing.Size(229, 22)
        Me.tsmInsert.Text = "Insert"
        '
        'tsmUpdate
        '
        Me.tsmUpdate.Name = "tsmUpdate"
        Me.tsmUpdate.Size = New System.Drawing.Size(229, 22)
        Me.tsmUpdate.Text = "Update"
        '
        'tsmDelete
        '
        Me.tsmDelete.Name = "tsmDelete"
        Me.tsmDelete.Size = New System.Drawing.Size(229, 22)
        Me.tsmDelete.Text = "Delete"
        '
        'tsmSetCond
        '
        Me.tsmSetCond.Name = "tsmSetCond"
        Me.tsmSetCond.Size = New System.Drawing.Size(229, 22)
        Me.tsmSetCond.Text = "SetCond"
        '
        'tsmAllKu
        '
        Me.tsmAllKu.Name = "tsmAllKu"
        Me.tsmAllKu.Size = New System.Drawing.Size(229, 22)
        Me.tsmAllKu.Text = "全て"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(226, 6)
        '
        'tsmIntoValues
        '
        Me.tsmIntoValues.Name = "tsmIntoValues"
        Me.tsmIntoValues.Size = New System.Drawing.Size(229, 22)
        Me.tsmIntoValues.Text = "INTO句とVALUES句の対応"
        '
        'tsmSetValues
        '
        Me.tsmSetValues.Name = "tsmSetValues"
        Me.tsmSetValues.Size = New System.Drawing.Size(229, 22)
        Me.tsmSetValues.Text = "SET句のカラムと値の対応"
        '
        'tsmSelects
        '
        Me.tsmSelects.Name = "tsmSelects"
        Me.tsmSelects.Size = New System.Drawing.Size(229, 22)
        Me.tsmSelects.Text = "SELECT句の別名と式の対応"
        '
        'SplitContainer6
        '
        Me.SplitContainer6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer6.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer6.Name = "SplitContainer6"
        Me.SplitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer6.Panel1
        '
        Me.SplitContainer6.Panel1.Controls.Add(Me.Panel4)
        '
        'SplitContainer6.Panel2
        '
        Me.SplitContainer6.Panel2.Controls.Add(Me.Panel12)
        Me.SplitContainer6.Size = New System.Drawing.Size(330, 505)
        Me.SplitContainer6.SplitterDistance = 262
        Me.SplitContainer6.TabIndex = 0
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.lstCRUD)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(0, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(330, 262)
        Me.Panel4.TabIndex = 5
        '
        'lstCRUD
        '
        Me.lstCRUD.AllowColumnReorder = True
        Me.lstCRUD.CheckBoxes = True
        Me.lstCRUD.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chTableName, Me.chEntityName, Me.chAltName, Me.chCRUD})
        Me.lstCRUD.ContextMenuStrip = Me.cmsTableList
        Me.lstCRUD.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstCRUD.FullRowSelect = True
        Me.lstCRUD.GridLines = True
        Me.lstCRUD.HideSelection = False
        Me.lstCRUD.Location = New System.Drawing.Point(0, 0)
        Me.lstCRUD.Name = "lstCRUD"
        Me.lstCRUD.Size = New System.Drawing.Size(330, 262)
        Me.lstCRUD.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lstCRUD.TabIndex = 3
        Me.lstCRUD.UseCompatibleStateImageBehavior = False
        Me.lstCRUD.View = System.Windows.Forms.View.Details
        '
        'chTableName
        '
        Me.chTableName.Text = "テーブル名"
        Me.chTableName.Width = 120
        '
        'chEntityName
        '
        Me.chEntityName.Text = "エンティティ名"
        Me.chEntityName.Width = 120
        '
        'chAltName
        '
        Me.chAltName.Text = "代替"
        Me.chAltName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.chAltName.Width = 50
        '
        'cmsTableList
        '
        Me.cmsTableList.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.全選択ToolStripMenuItem, Me.全解除ToolStripMenuItem, Me.テーブル定義ToolStripMenuItem, Me.CRUDサーチToolStripMenuItem})
        Me.cmsTableList.Name = "cmsFiles"
        Me.cmsTableList.Size = New System.Drawing.Size(149, 92)
        '
        '全選択ToolStripMenuItem
        '
        Me.全選択ToolStripMenuItem.Name = "全選択ToolStripMenuItem"
        Me.全選択ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.全選択ToolStripMenuItem.Text = "全選択"
        '
        '全解除ToolStripMenuItem
        '
        Me.全解除ToolStripMenuItem.Name = "全解除ToolStripMenuItem"
        Me.全解除ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.全解除ToolStripMenuItem.Text = "全解除"
        '
        'テーブル定義ToolStripMenuItem
        '
        Me.テーブル定義ToolStripMenuItem.Name = "テーブル定義ToolStripMenuItem"
        Me.テーブル定義ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.テーブル定義ToolStripMenuItem.Text = "テーブル定義"
        '
        'CRUDサーチToolStripMenuItem
        '
        Me.CRUDサーチToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.このテーブルにアクセスしている処理ToolStripMenuItem})
        Me.CRUDサーチToolStripMenuItem.Name = "CRUDサーチToolStripMenuItem"
        Me.CRUDサーチToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.CRUDサーチToolStripMenuItem.Text = "CRUDサーチ"
        '
        'このテーブルにアクセスしている処理ToolStripMenuItem
        '
        Me.このテーブルにアクセスしている処理ToolStripMenuItem.Name = "このテーブルにアクセスしている処理ToolStripMenuItem"
        Me.このテーブルにアクセスしている処理ToolStripMenuItem.Size = New System.Drawing.Size(280, 22)
        Me.このテーブルにアクセスしている処理ToolStripMenuItem.Text = "このテーブルにアクセスしている処理"
        '
        'Panel12
        '
        Me.Panel12.Controls.Add(Me.lstColumnCRUD)
        Me.Panel12.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel12.Location = New System.Drawing.Point(0, 0)
        Me.Panel12.Name = "Panel12"
        Me.Panel12.Size = New System.Drawing.Size(330, 239)
        Me.Panel12.TabIndex = 6
        '
        'lstColumnCRUD
        '
        Me.lstColumnCRUD.AllowColumnReorder = True
        Me.lstColumnCRUD.CheckBoxes = True
        Me.lstColumnCRUD.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ccTableName, Me.ccEntityName, Me.ccColumnName, Me.ccAttributeName, Me.ccCRUD})
        Me.lstColumnCRUD.ContextMenuStrip = Me.cmsColumnList
        Me.lstColumnCRUD.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstColumnCRUD.FullRowSelect = True
        Me.lstColumnCRUD.GridLines = True
        Me.lstColumnCRUD.HideSelection = False
        Me.lstColumnCRUD.Location = New System.Drawing.Point(0, 0)
        Me.lstColumnCRUD.Name = "lstColumnCRUD"
        Me.lstColumnCRUD.Size = New System.Drawing.Size(330, 239)
        Me.lstColumnCRUD.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lstColumnCRUD.TabIndex = 4
        Me.lstColumnCRUD.UseCompatibleStateImageBehavior = False
        Me.lstColumnCRUD.View = System.Windows.Forms.View.Details
        '
        'ccTableName
        '
        Me.ccTableName.Text = "テーブル名"
        Me.ccTableName.Width = 120
        '
        'ccEntityName
        '
        Me.ccEntityName.Text = "エンティティ名"
        Me.ccEntityName.Width = 120
        '
        'ccColumnName
        '
        Me.ccColumnName.Text = "カラム名"
        Me.ccColumnName.Width = 120
        '
        'ccAttributeName
        '
        Me.ccAttributeName.Text = "属性名"
        Me.ccAttributeName.Width = 120
        '
        'ccCRUD
        '
        Me.ccCRUD.Text = "CRUD"
        Me.ccCRUD.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.ccCRUD.Width = 50
        '
        'cmsColumnList
        '
        Me.cmsColumnList.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.ToolStripMenuItem2, Me.テーブル定義ToolStripMenuItem1, Me.CRUDサーチToolStripMenuItem1})
        Me.cmsColumnList.Name = "cmsFiles"
        Me.cmsColumnList.Size = New System.Drawing.Size(149, 92)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(148, 22)
        Me.ToolStripMenuItem1.Text = "全選択"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(148, 22)
        Me.ToolStripMenuItem2.Text = "全解除"
        '
        'テーブル定義ToolStripMenuItem1
        '
        Me.テーブル定義ToolStripMenuItem1.Name = "テーブル定義ToolStripMenuItem1"
        Me.テーブル定義ToolStripMenuItem1.Size = New System.Drawing.Size(148, 22)
        Me.テーブル定義ToolStripMenuItem1.Text = "テーブル定義"
        '
        'CRUDサーチToolStripMenuItem1
        '
        Me.CRUDサーチToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.このカラムにアクセスしている処理ToolStripMenuItem, Me.このテーブルにアクセスしている処理ToolStripMenuItem1})
        Me.CRUDサーチToolStripMenuItem1.Name = "CRUDサーチToolStripMenuItem1"
        Me.CRUDサーチToolStripMenuItem1.Size = New System.Drawing.Size(148, 22)
        Me.CRUDサーチToolStripMenuItem1.Text = "CRUDサーチ"
        '
        'このカラムにアクセスしている処理ToolStripMenuItem
        '
        Me.このカラムにアクセスしている処理ToolStripMenuItem.Name = "このカラムにアクセスしている処理ToolStripMenuItem"
        Me.このカラムにアクセスしている処理ToolStripMenuItem.Size = New System.Drawing.Size(280, 22)
        Me.このカラムにアクセスしている処理ToolStripMenuItem.Text = "このカラムにアクセスしている処理"
        '
        'このテーブルにアクセスしている処理ToolStripMenuItem1
        '
        Me.このテーブルにアクセスしている処理ToolStripMenuItem1.Name = "このテーブルにアクセスしている処理ToolStripMenuItem1"
        Me.このテーブルにアクセスしている処理ToolStripMenuItem1.Size = New System.Drawing.Size(280, 22)
        Me.このテーブルにアクセスしている処理ToolStripMenuItem1.Text = "このテーブルにアクセスしている処理"
        '
        'Panel6
        '
        Me.Panel6.Controls.Add(Me.rtbQuery)
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel6.Location = New System.Drawing.Point(0, 58)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(670, 546)
        Me.Panel6.TabIndex = 8
        '
        'rtbQuery
        '
        Me.rtbQuery.ContextMenuStrip = Me.cmsTextBox
        Me.rtbQuery.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtbQuery.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.rtbQuery.Location = New System.Drawing.Point(0, 0)
        Me.rtbQuery.Name = "rtbQuery"
        Me.rtbQuery.Size = New System.Drawing.Size(670, 546)
        Me.rtbQuery.TabIndex = 6
        Me.rtbQuery.Text = ""
        '
        'cmsTextBox
        '
        Me.cmsTextBox.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmOtherCRUD, Me.GrepToolStripMenuItem, Me.ハイライトToolStripMenuItem, Me.tsmTableDef})
        Me.cmsTextBox.Name = "cmsTextBox"
        Me.cmsTextBox.Size = New System.Drawing.Size(265, 92)
        '
        'tsmOtherCRUD
        '
        Me.tsmOtherCRUD.Name = "tsmOtherCRUD"
        Me.tsmOtherCRUD.Size = New System.Drawing.Size(264, 22)
        Me.tsmOtherCRUD.Text = "このテーブル(カラム)のCRUD一覧"
        '
        'GrepToolStripMenuItem
        '
        Me.GrepToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GrepFileToolStripMenuItem, Me.GrepProgramToolStripMenuItem, Me.GrepAllToolStripMenuItem})
        Me.GrepToolStripMenuItem.Name = "GrepToolStripMenuItem"
        Me.GrepToolStripMenuItem.Size = New System.Drawing.Size(264, 22)
        Me.GrepToolStripMenuItem.Text = "Grep"
        '
        'GrepFileToolStripMenuItem
        '
        Me.GrepFileToolStripMenuItem.Name = "GrepFileToolStripMenuItem"
        Me.GrepFileToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.GrepFileToolStripMenuItem.Text = "このファイルから"
        '
        'GrepProgramToolStripMenuItem
        '
        Me.GrepProgramToolStripMenuItem.Name = "GrepProgramToolStripMenuItem"
        Me.GrepProgramToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.GrepProgramToolStripMenuItem.Text = "このプログラムから"
        '
        'GrepAllToolStripMenuItem
        '
        Me.GrepAllToolStripMenuItem.Name = "GrepAllToolStripMenuItem"
        Me.GrepAllToolStripMenuItem.Size = New System.Drawing.Size(196, 22)
        Me.GrepAllToolStripMenuItem.Text = "全てのプログラムから"
        '
        'ハイライトToolStripMenuItem
        '
        Me.ハイライトToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HighLight1ToolStripMenuItem, Me.HighLight2ToolStripMenuItem, Me.HighLight3ToolStripMenuItem, Me.HighLightClearToolStripMenuItem})
        Me.ハイライトToolStripMenuItem.Name = "ハイライトToolStripMenuItem"
        Me.ハイライトToolStripMenuItem.Size = New System.Drawing.Size(264, 22)
        Me.ハイライトToolStripMenuItem.Text = "ハイライト"
        '
        'HighLight1ToolStripMenuItem
        '
        Me.HighLight1ToolStripMenuItem.Name = "HighLight1ToolStripMenuItem"
        Me.HighLight1ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.HighLight1ToolStripMenuItem.Text = "ハイライト１"
        '
        'HighLight2ToolStripMenuItem
        '
        Me.HighLight2ToolStripMenuItem.Name = "HighLight2ToolStripMenuItem"
        Me.HighLight2ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.HighLight2ToolStripMenuItem.Text = "ハイライト２"
        '
        'HighLight3ToolStripMenuItem
        '
        Me.HighLight3ToolStripMenuItem.Name = "HighLight3ToolStripMenuItem"
        Me.HighLight3ToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.HighLight3ToolStripMenuItem.Text = "ハイライト３"
        '
        'HighLightClearToolStripMenuItem
        '
        Me.HighLightClearToolStripMenuItem.Name = "HighLightClearToolStripMenuItem"
        Me.HighLightClearToolStripMenuItem.Size = New System.Drawing.Size(148, 22)
        Me.HighLightClearToolStripMenuItem.Text = "クリア"
        '
        'tsmTableDef
        '
        Me.tsmTableDef.Name = "tsmTableDef"
        Me.tsmTableDef.Size = New System.Drawing.Size(264, 22)
        Me.tsmTableDef.Text = "テーブル定義"
        '
        'Panel11
        '
        Me.Panel11.Controls.Add(Me.txtGuide)
        Me.Panel11.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel11.Location = New System.Drawing.Point(0, 604)
        Me.Panel11.Name = "Panel11"
        Me.Panel11.Size = New System.Drawing.Size(670, 21)
        Me.Panel11.TabIndex = 7
        '
        'txtGuide
        '
        Me.txtGuide.BackColor = System.Drawing.SystemColors.Control
        Me.txtGuide.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtGuide.Location = New System.Drawing.Point(10, 4)
        Me.txtGuide.Name = "txtGuide"
        Me.txtGuide.Size = New System.Drawing.Size(897, 12)
        Me.txtGuide.TabIndex = 0
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.btnSearchPrev)
        Me.Panel5.Controls.Add(Me.btnSearchNext)
        Me.Panel5.Controls.Add(Me.btnTextSearch)
        Me.Panel5.Controls.Add(Me.btnTableDef)
        Me.Panel5.Controls.Add(Me.cmdExpandView)
        Me.Panel5.Controls.Add(Me.cmdExchangeLogicalName)
        Me.Panel5.Controls.Add(Me.btnQuickAnalyze)
        Me.Panel5.Controls.Add(Me.txtHighLight3)
        Me.Panel5.Controls.Add(Me.btnDivideStrings)
        Me.Panel5.Controls.Add(Me.txtHighLight2)
        Me.Panel5.Controls.Add(Me.txtHighLight1)
        Me.Panel5.Controls.Add(Me.Label4)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel5.Location = New System.Drawing.Point(0, 0)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(670, 58)
        Me.Panel5.TabIndex = 7
        '
        'btnSearchPrev
        '
        Me.btnSearchPrev.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnSearchPrev.Location = New System.Drawing.Point(503, 4)
        Me.btnSearchPrev.Name = "btnSearchPrev"
        Me.btnSearchPrev.Size = New System.Drawing.Size(28, 25)
        Me.btnSearchPrev.TabIndex = 6
        Me.btnSearchPrev.Text = "前"
        Me.btnSearchPrev.UseVisualStyleBackColor = True
        '
        'btnSearchNext
        '
        Me.btnSearchNext.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnSearchNext.Location = New System.Drawing.Point(637, 4)
        Me.btnSearchNext.Name = "btnSearchNext"
        Me.btnSearchNext.Size = New System.Drawing.Size(28, 25)
        Me.btnSearchNext.TabIndex = 8
        Me.btnSearchNext.Text = "次"
        Me.btnSearchNext.UseVisualStyleBackColor = True
        '
        'btnTextSearch
        '
        Me.btnTextSearch.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnTextSearch.Location = New System.Drawing.Point(537, 4)
        Me.btnTextSearch.Name = "btnTextSearch"
        Me.btnTextSearch.Size = New System.Drawing.Size(94, 25)
        Me.btnTextSearch.TabIndex = 7
        Me.btnTextSearch.Text = "テキスト検索"
        Me.btnTextSearch.UseVisualStyleBackColor = True
        '
        'btnTableDef
        '
        Me.btnTableDef.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnTableDef.Location = New System.Drawing.Point(403, 4)
        Me.btnTableDef.Name = "btnTableDef"
        Me.btnTableDef.Size = New System.Drawing.Size(94, 25)
        Me.btnTableDef.TabIndex = 5
        Me.btnTableDef.Text = "テーブル定義"
        Me.btnTableDef.UseVisualStyleBackColor = True
        '
        'cmdExpandView
        '
        Me.cmdExpandView.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdExpandView.Location = New System.Drawing.Point(203, 4)
        Me.cmdExpandView.Name = "cmdExpandView"
        Me.cmdExpandView.Size = New System.Drawing.Size(94, 25)
        Me.cmdExpandView.TabIndex = 3
        Me.cmdExpandView.Text = "Viewを展開"
        Me.cmdExpandView.UseVisualStyleBackColor = True
        '
        'cmdExchangeLogicalName
        '
        Me.cmdExchangeLogicalName.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdExchangeLogicalName.Location = New System.Drawing.Point(103, 4)
        Me.cmdExchangeLogicalName.Name = "cmdExchangeLogicalName"
        Me.cmdExchangeLogicalName.Size = New System.Drawing.Size(94, 25)
        Me.cmdExchangeLogicalName.TabIndex = 2
        Me.cmdExchangeLogicalName.Text = "論理名変換"
        Me.cmdExchangeLogicalName.UseVisualStyleBackColor = True
        '
        'btnQuickAnalyze
        '
        Me.btnQuickAnalyze.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnQuickAnalyze.Location = New System.Drawing.Point(3, 4)
        Me.btnQuickAnalyze.Name = "btnQuickAnalyze"
        Me.btnQuickAnalyze.Size = New System.Drawing.Size(94, 25)
        Me.btnQuickAnalyze.TabIndex = 1
        Me.btnQuickAnalyze.Text = "SQLを解析"
        Me.btnQuickAnalyze.UseVisualStyleBackColor = True
        '
        'txtHighLight3
        '
        Me.txtHighLight3.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.txtHighLight3.Location = New System.Drawing.Point(453, 34)
        Me.txtHighLight3.Name = "txtHighLight3"
        Me.txtHighLight3.Size = New System.Drawing.Size(189, 19)
        Me.txtHighLight3.TabIndex = 11
        '
        'btnDivideStrings
        '
        Me.btnDivideStrings.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnDivideStrings.Location = New System.Drawing.Point(303, 4)
        Me.btnDivideStrings.Name = "btnDivideStrings"
        Me.btnDivideStrings.Size = New System.Drawing.Size(94, 25)
        Me.btnDivideStrings.TabIndex = 4
        Me.btnDivideStrings.Text = "文字列抽出"
        Me.btnDivideStrings.UseVisualStyleBackColor = True
        '
        'txtHighLight2
        '
        Me.txtHighLight2.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.txtHighLight2.Location = New System.Drawing.Point(258, 34)
        Me.txtHighLight2.Name = "txtHighLight2"
        Me.txtHighLight2.Size = New System.Drawing.Size(189, 19)
        Me.txtHighLight2.TabIndex = 10
        '
        'txtHighLight1
        '
        Me.txtHighLight1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.txtHighLight1.Location = New System.Drawing.Point(63, 34)
        Me.txtHighLight1.Name = "txtHighLight1"
        Me.txtHighLight1.Size = New System.Drawing.Size(189, 19)
        Me.txtHighLight1.TabIndex = 9
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(8, 37)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(49, 12)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "ハイライト"
        '
        'chCRUD
        '
        Me.chCRUD.Text = "CRUD"
        Me.chCRUD.Width = 50
        '
        'frmAnalyzeQuery
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdClose
        Me.ClientSize = New System.Drawing.Size(1008, 662)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmAnalyzeQuery"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "クエリの分析"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel7.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.SplitContainer5.Panel1.ResumeLayout(False)
        Me.SplitContainer5.Panel2.ResumeLayout(False)
        Me.SplitContainer5.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.cmsTreeView.ResumeLayout(False)
        Me.SplitContainer6.Panel1.ResumeLayout(False)
        Me.SplitContainer6.Panel2.ResumeLayout(False)
        Me.SplitContainer6.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.cmsTableList.ResumeLayout(False)
        Me.Panel12.ResumeLayout(False)
        Me.cmsColumnList.ResumeLayout(False)
        Me.Panel6.ResumeLayout(False)
        Me.cmsTextBox.ResumeLayout(False)
        Me.Panel11.ResumeLayout(False)
        Me.Panel11.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtAltName As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtTableName As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmdRunHidemaru As System.Windows.Forms.Button
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents tvQuery As System.Windows.Forms.TreeView
    Friend WithEvents rtbQuery As System.Windows.Forms.RichTextBox
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents txtHighLight3 As System.Windows.Forms.TextBox
    Friend WithEvents txtHighLight2 As System.Windows.Forms.TextBox
    Friend WithEvents txtHighLight1 As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lstCRUD As System.Windows.Forms.ListView
    Friend WithEvents chTableName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEntityName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chAltName As System.Windows.Forms.ColumnHeader
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents lstColumnCRUD As System.Windows.Forms.ListView
    Friend WithEvents ccTableName As System.Windows.Forms.ColumnHeader
    Friend WithEvents ccEntityName As System.Windows.Forms.ColumnHeader
    Friend WithEvents ccCRUD As System.Windows.Forms.ColumnHeader
    Friend WithEvents ccColumnName As System.Windows.Forms.ColumnHeader
    Friend WithEvents ccAttributeName As System.Windows.Forms.ColumnHeader
    Friend WithEvents Panel11 As System.Windows.Forms.Panel
    Friend WithEvents txtGuide As System.Windows.Forms.TextBox
    Friend WithEvents SplitContainer5 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer6 As System.Windows.Forms.SplitContainer
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel12 As System.Windows.Forms.Panel
    Friend WithEvents btnQuickAnalyze As System.Windows.Forms.Button
    Friend WithEvents btnDivideStrings As System.Windows.Forms.Button
    Friend WithEvents cmsTextBox As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmOtherCRUD As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmbQuery As System.Windows.Forms.ComboBox
    Friend WithEvents GrepToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GrepFileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GrepProgramToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GrepAllToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmdExpandView As System.Windows.Forms.Button
    Friend WithEvents cmdExchangeLogicalName As System.Windows.Forms.Button
    Friend WithEvents ハイライトToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HighLight1ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HighLight2ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HighLight3ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HighLightClearToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmsTableList As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents 全選択ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 全解除ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents テーブル定義ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmsColumnList As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmsTreeView As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents tsmExpandSubQuery As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents テーブル定義ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 句の情報ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSelect As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmWhere As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmGroupBy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmOrderBy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmHaving As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmUpdate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmInsert As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmSetCond As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmAllKu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tsmIntoValues As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CRUDサーチToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents このテーブルにアクセスしている処理ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CRUDサーチToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents このカラムにアクセスしている処理ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents このテーブルにアクセスしている処理ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents txtLineNo As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tsmSetValues As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnTableDef As System.Windows.Forms.Button
    Friend WithEvents tsmSelects As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmTableDef As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnTextSearch As System.Windows.Forms.Button
    Friend WithEvents btnSearchPrev As System.Windows.Forms.Button
    Friend WithEvents btnSearchNext As System.Windows.Forms.Button
    Friend WithEvents chCRUD As System.Windows.Forms.ColumnHeader
End Class
