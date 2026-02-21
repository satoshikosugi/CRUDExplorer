<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTableDef
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTableDef))
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.pnlRelate = New System.Windows.Forms.Panel
        Me.cmbInsText = New System.Windows.Forms.ComboBox
        Me.btnInsConma = New System.Windows.Forms.Button
        Me.btnInsEQ = New System.Windows.Forms.Button
        Me.btnInsOr = New System.Windows.Forms.Button
        Me.btnInsAnd = New System.Windows.Forms.Button
        Me.bntInsEndKakko = New System.Windows.Forms.Button
        Me.btnInsBeginKakko = New System.Windows.Forms.Button
        Me.bntInsEnter = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnInsTable = New System.Windows.Forms.Button
        Me.btnInsTableColumn = New System.Windows.Forms.Button
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.cmdClose = New System.Windows.Forms.Button
        Me.lstTableDef = New System.Windows.Forms.ListView
        Me.chNo = New System.Windows.Forms.ColumnHeader
        Me.chColumnName = New System.Windows.Forms.ColumnHeader
        Me.chAttributeName = New System.Windows.Forms.ColumnHeader
        Me.chPK = New System.Windows.Forms.ColumnHeader
        Me.chFK = New System.Windows.Forms.ColumnHeader
        Me.chRequired = New System.Windows.Forms.ColumnHeader
        Me.chDateType = New System.Windows.Forms.ColumnHeader
        Me.chDigit = New System.Windows.Forms.ColumnHeader
        Me.chAccuracy = New System.Windows.Forms.ColumnHeader
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.テーブルにToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmTableAccess = New System.Windows.Forms.ToolStripMenuItem
        Me.tsmColumnAccess = New System.Windows.Forms.ToolStripMenuItem
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.lstTable = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.btnClearFilter = New System.Windows.Forms.Button
        Me.btnApplyFilter = New System.Windows.Forms.Button
        Me.txtFilter = New System.Windows.Forms.TextBox
        Me.Panel1.SuspendLayout()
        Me.pnlRelate.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel1.Controls.Add(Me.pnlRelate)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 517)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1008, 33)
        Me.Panel1.TabIndex = 3
        '
        'pnlRelate
        '
        Me.pnlRelate.Controls.Add(Me.cmbInsText)
        Me.pnlRelate.Controls.Add(Me.btnInsConma)
        Me.pnlRelate.Controls.Add(Me.btnInsEQ)
        Me.pnlRelate.Controls.Add(Me.btnInsOr)
        Me.pnlRelate.Controls.Add(Me.btnInsAnd)
        Me.pnlRelate.Controls.Add(Me.bntInsEndKakko)
        Me.pnlRelate.Controls.Add(Me.btnInsBeginKakko)
        Me.pnlRelate.Controls.Add(Me.bntInsEnter)
        Me.pnlRelate.Controls.Add(Me.Label1)
        Me.pnlRelate.Controls.Add(Me.btnInsTable)
        Me.pnlRelate.Controls.Add(Me.btnInsTableColumn)
        Me.pnlRelate.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlRelate.Location = New System.Drawing.Point(0, 0)
        Me.pnlRelate.Name = "pnlRelate"
        Me.pnlRelate.Size = New System.Drawing.Size(679, 29)
        Me.pnlRelate.TabIndex = 4
        '
        'cmbInsText
        '
        Me.cmbInsText.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbInsText.FormattingEnabled = True
        Me.cmbInsText.Items.AddRange(New Object() {"SELECT ", " FROM ", " INNER JOIN ", " LEFT OUTER JOIN ", " RIGHT OUTER JOIN ", " ON ", " WHERE ", " GROUP BY ", " ORDER BY ", "UPDATE ", " SET ", " INSERT ", " INTO ", "DELETE ", " != ", " < ", " > ", " <= ", " >= ", " BETWEEN ", " MAX (", " MIN (", " SUM (", " AVR (", " DISTINCT ", " NOT", " EXIST", " IS ", " NULL "})
        Me.cmbInsText.Location = New System.Drawing.Point(567, 6)
        Me.cmbInsText.Name = "cmbInsText"
        Me.cmbInsText.Size = New System.Drawing.Size(95, 20)
        Me.cmbInsText.TabIndex = 8
        '
        'btnInsConma
        '
        Me.btnInsConma.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnInsConma.Location = New System.Drawing.Point(270, 3)
        Me.btnInsConma.Name = "btnInsConma"
        Me.btnInsConma.Size = New System.Drawing.Size(31, 25)
        Me.btnInsConma.TabIndex = 7
        Me.btnInsConma.Text = ","
        Me.btnInsConma.UseVisualStyleBackColor = True
        '
        'btnInsEQ
        '
        Me.btnInsEQ.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnInsEQ.Location = New System.Drawing.Point(530, 3)
        Me.btnInsEQ.Name = "btnInsEQ"
        Me.btnInsEQ.Size = New System.Drawing.Size(31, 25)
        Me.btnInsEQ.TabIndex = 7
        Me.btnInsEQ.Text = "="
        Me.btnInsEQ.UseVisualStyleBackColor = True
        '
        'btnInsOr
        '
        Me.btnInsOr.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnInsOr.Location = New System.Drawing.Point(480, 3)
        Me.btnInsOr.Name = "btnInsOr"
        Me.btnInsOr.Size = New System.Drawing.Size(44, 25)
        Me.btnInsOr.TabIndex = 6
        Me.btnInsOr.Text = "OR"
        Me.btnInsOr.UseVisualStyleBackColor = True
        '
        'btnInsAnd
        '
        Me.btnInsAnd.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnInsAnd.Location = New System.Drawing.Point(430, 3)
        Me.btnInsAnd.Name = "btnInsAnd"
        Me.btnInsAnd.Size = New System.Drawing.Size(44, 25)
        Me.btnInsAnd.TabIndex = 5
        Me.btnInsAnd.Text = "AND"
        Me.btnInsAnd.UseVisualStyleBackColor = True
        '
        'bntInsEndKakko
        '
        Me.bntInsEndKakko.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bntInsEndKakko.Location = New System.Drawing.Point(394, 3)
        Me.bntInsEndKakko.Name = "bntInsEndKakko"
        Me.bntInsEndKakko.Size = New System.Drawing.Size(30, 25)
        Me.bntInsEndKakko.TabIndex = 4
        Me.bntInsEndKakko.Text = ")"
        Me.bntInsEndKakko.UseVisualStyleBackColor = True
        '
        'btnInsBeginKakko
        '
        Me.btnInsBeginKakko.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnInsBeginKakko.Location = New System.Drawing.Point(358, 3)
        Me.btnInsBeginKakko.Name = "btnInsBeginKakko"
        Me.btnInsBeginKakko.Size = New System.Drawing.Size(30, 25)
        Me.btnInsBeginKakko.TabIndex = 3
        Me.btnInsBeginKakko.Text = "("
        Me.btnInsBeginKakko.UseVisualStyleBackColor = True
        '
        'bntInsEnter
        '
        Me.bntInsEnter.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.bntInsEnter.Location = New System.Drawing.Point(307, 3)
        Me.bntInsEnter.Name = "bntInsEnter"
        Me.bntInsEnter.Size = New System.Drawing.Size(45, 25)
        Me.bntInsEnter.TabIndex = 2
        Me.bntInsEnter.Text = "改行"
        Me.bntInsEnter.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(10, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 12)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "クエリビューに挿入"
        '
        'btnInsTable
        '
        Me.btnInsTable.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnInsTable.Location = New System.Drawing.Point(203, 3)
        Me.btnInsTable.Name = "btnInsTable"
        Me.btnInsTable.Size = New System.Drawing.Size(61, 25)
        Me.btnInsTable.TabIndex = 1
        Me.btnInsTable.Text = "テーブル"
        Me.btnInsTable.UseVisualStyleBackColor = True
        '
        'btnInsTableColumn
        '
        Me.btnInsTableColumn.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnInsTableColumn.Location = New System.Drawing.Point(104, 3)
        Me.btnInsTableColumn.Name = "btnInsTableColumn"
        Me.btnInsTableColumn.Size = New System.Drawing.Size(94, 25)
        Me.btnInsTableColumn.TabIndex = 0
        Me.btnInsTableColumn.Text = "テーブル.カラム"
        Me.btnInsTableColumn.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.cmdClose)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel2.Location = New System.Drawing.Point(903, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(101, 29)
        Me.Panel2.TabIndex = 5
        '
        'cmdClose
        '
        Me.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdClose.Location = New System.Drawing.Point(3, 3)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(94, 25)
        Me.cmdClose.TabIndex = 0
        Me.cmdClose.Text = "閉じる(&C)"
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'lstTableDef
        '
        Me.lstTableDef.AllowColumnReorder = True
        Me.lstTableDef.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lstTableDef.CheckBoxes = True
        Me.lstTableDef.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chNo, Me.chColumnName, Me.chAttributeName, Me.chPK, Me.chFK, Me.chRequired, Me.chDateType, Me.chDigit, Me.chAccuracy})
        Me.lstTableDef.ContextMenuStrip = Me.ContextMenuStrip1
        Me.lstTableDef.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstTableDef.FullRowSelect = True
        Me.lstTableDef.GridLines = True
        Me.lstTableDef.HideSelection = False
        Me.lstTableDef.Location = New System.Drawing.Point(0, 0)
        Me.lstTableDef.Name = "lstTableDef"
        Me.lstTableDef.Size = New System.Drawing.Size(698, 517)
        Me.lstTableDef.TabIndex = 1
        Me.lstTableDef.TileSize = New System.Drawing.Size(1, 1)
        Me.lstTableDef.UseCompatibleStateImageBehavior = False
        Me.lstTableDef.View = System.Windows.Forms.View.Details
        '
        'chNo
        '
        Me.chNo.Text = "No"
        Me.chNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.chNo.Width = 36
        '
        'chColumnName
        '
        Me.chColumnName.Text = "カラム名"
        Me.chColumnName.Width = 250
        '
        'chAttributeName
        '
        Me.chAttributeName.Text = "属性名"
        Me.chAttributeName.Width = 169
        '
        'chPK
        '
        Me.chPK.Text = "PK"
        Me.chPK.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.chPK.Width = 48
        '
        'chFK
        '
        Me.chFK.Text = "FK"
        Me.chFK.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.chFK.Width = 45
        '
        'chRequired
        '
        Me.chRequired.Text = "必須"
        Me.chRequired.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.chRequired.Width = 51
        '
        'chDateType
        '
        Me.chDateType.Text = "データ型"
        Me.chDateType.Width = 100
        '
        'chDigit
        '
        Me.chDigit.Text = "桁数"
        Me.chDigit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'chAccuracy
        '
        Me.chAccuracy.Text = "精度"
        Me.chAccuracy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.テーブルにToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(132, 26)
        '
        'テーブルにToolStripMenuItem
        '
        Me.テーブルにToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmTableAccess, Me.tsmColumnAccess})
        Me.テーブルにToolStripMenuItem.Name = "テーブルにToolStripMenuItem"
        Me.テーブルにToolStripMenuItem.Size = New System.Drawing.Size(131, 22)
        Me.テーブルにToolStripMenuItem.Text = "CRUDサーチ"
        '
        'tsmTableAccess
        '
        Me.tsmTableAccess.Name = "tsmTableAccess"
        Me.tsmTableAccess.Size = New System.Drawing.Size(232, 22)
        Me.tsmTableAccess.Text = "このテーブルにアクセスしている処理"
        '
        'tsmColumnAccess
        '
        Me.tsmColumnAccess.Name = "tsmColumnAccess"
        Me.tsmColumnAccess.Size = New System.Drawing.Size(232, 22)
        Me.tsmColumnAccess.Text = "このカラムにアクセスしている処理"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Panel4)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Panel3)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lstTableDef)
        Me.SplitContainer1.Size = New System.Drawing.Size(1008, 517)
        Me.SplitContainer1.SplitterDistance = 306
        Me.SplitContainer1.TabIndex = 0
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.lstTable)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(0, 32)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(306, 485)
        Me.Panel4.TabIndex = 1
        '
        'lstTable
        '
        Me.lstTable.AllowColumnReorder = True
        Me.lstTable.CheckBoxes = True
        Me.lstTable.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
        Me.lstTable.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstTable.FullRowSelect = True
        Me.lstTable.GridLines = True
        Me.lstTable.HideSelection = False
        Me.lstTable.Location = New System.Drawing.Point(0, 0)
        Me.lstTable.MultiSelect = False
        Me.lstTable.Name = "lstTable"
        Me.lstTable.Size = New System.Drawing.Size(306, 485)
        Me.lstTable.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lstTable.TabIndex = 0
        Me.lstTable.UseCompatibleStateImageBehavior = False
        Me.lstTable.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "テーブル名"
        Me.ColumnHeader1.Width = 172
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "エンティティ名"
        Me.ColumnHeader2.Width = 265
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.btnClearFilter)
        Me.Panel3.Controls.Add(Me.btnApplyFilter)
        Me.Panel3.Controls.Add(Me.txtFilter)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(306, 32)
        Me.Panel3.TabIndex = 0
        '
        'btnClearFilter
        '
        Me.btnClearFilter.Location = New System.Drawing.Point(220, 3)
        Me.btnClearFilter.Name = "btnClearFilter"
        Me.btnClearFilter.Size = New System.Drawing.Size(79, 25)
        Me.btnClearFilter.TabIndex = 6
        Me.btnClearFilter.Text = "フィルタクリア"
        Me.btnClearFilter.UseVisualStyleBackColor = True
        '
        'btnApplyFilter
        '
        Me.btnApplyFilter.Location = New System.Drawing.Point(135, 3)
        Me.btnApplyFilter.Name = "btnApplyFilter"
        Me.btnApplyFilter.Size = New System.Drawing.Size(79, 25)
        Me.btnApplyFilter.TabIndex = 5
        Me.btnApplyFilter.Text = "フィルタ適用"
        Me.btnApplyFilter.UseVisualStyleBackColor = True
        '
        'txtFilter
        '
        Me.txtFilter.Location = New System.Drawing.Point(3, 6)
        Me.txtFilter.Name = "txtFilter"
        Me.txtFilter.Size = New System.Drawing.Size(126, 19)
        Me.txtFilter.TabIndex = 4
        '
        'frmTableDef
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdClose
        Me.ClientSize = New System.Drawing.Size(1008, 550)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmTableDef"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "テーブル定義"
        Me.Panel1.ResumeLayout(False)
        Me.pnlRelate.ResumeLayout(False)
        Me.pnlRelate.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents lstTableDef As System.Windows.Forms.ListView
    Friend WithEvents chColumnName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chAttributeName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chPK As System.Windows.Forms.ColumnHeader
    Friend WithEvents chFK As System.Windows.Forms.ColumnHeader
    Friend WithEvents chRequired As System.Windows.Forms.ColumnHeader
    Friend WithEvents chDateType As System.Windows.Forms.ColumnHeader
    Friend WithEvents chDigit As System.Windows.Forms.ColumnHeader
    Friend WithEvents chAccuracy As System.Windows.Forms.ColumnHeader
    Friend WithEvents chNo As System.Windows.Forms.ColumnHeader
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents テーブルにToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmTableAccess As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tsmColumnAccess As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents lstTable As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents pnlRelate As System.Windows.Forms.Panel
    Friend WithEvents bntInsEnter As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnInsTableColumn As System.Windows.Forms.Button
    Friend WithEvents cmbInsText As System.Windows.Forms.ComboBox
    Friend WithEvents btnInsEQ As System.Windows.Forms.Button
    Friend WithEvents btnInsOr As System.Windows.Forms.Button
    Friend WithEvents btnInsAnd As System.Windows.Forms.Button
    Friend WithEvents bntInsEndKakko As System.Windows.Forms.Button
    Friend WithEvents btnInsBeginKakko As System.Windows.Forms.Button
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents btnClearFilter As System.Windows.Forms.Button
    Friend WithEvents btnApplyFilter As System.Windows.Forms.Button
    Friend WithEvents txtFilter As System.Windows.Forms.TextBox
    Friend WithEvents btnInsTable As System.Windows.Forms.Button
    Friend WithEvents btnInsConma As System.Windows.Forms.Button
End Class
