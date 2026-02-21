<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFilter
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFilter))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.lstProgram = New System.Windows.Forms.ListView
        Me.chTableName = New System.Windows.Forms.ColumnHeader
        Me.chEntityName = New System.Windows.Forms.ColumnHeader
        Me.cmsProgram = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.全選択ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.全解除ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.lstTable = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.cmsTable = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.cmbTableAccess = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.cmbProgramAccess = New System.Windows.Forms.ComboBox
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdApply = New System.Windows.Forms.Button
        Me.btnAllClear = New System.Windows.Forms.Button
        Me.GroupBox1.SuspendLayout()
        Me.cmsProgram.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.cmsTable.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lstProgram)
        Me.GroupBox1.Location = New System.Drawing.Point(7, 10)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(489, 436)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "プログラム"
        '
        'lstProgram
        '
        Me.lstProgram.AllowColumnReorder = True
        Me.lstProgram.CheckBoxes = True
        Me.lstProgram.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chTableName, Me.chEntityName})
        Me.lstProgram.ContextMenuStrip = Me.cmsProgram
        Me.lstProgram.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstProgram.FullRowSelect = True
        Me.lstProgram.GridLines = True
        Me.lstProgram.HideSelection = False
        Me.lstProgram.Location = New System.Drawing.Point(3, 15)
        Me.lstProgram.Name = "lstProgram"
        Me.lstProgram.Size = New System.Drawing.Size(483, 418)
        Me.lstProgram.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lstProgram.TabIndex = 4
        Me.lstProgram.UseCompatibleStateImageBehavior = False
        Me.lstProgram.View = System.Windows.Forms.View.Details
        '
        'chTableName
        '
        Me.chTableName.Text = "プログラムID"
        Me.chTableName.Width = 172
        '
        'chEntityName
        '
        Me.chEntityName.Text = "プログラム名"
        Me.chEntityName.Width = 265
        '
        'cmsProgram
        '
        Me.cmsProgram.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.全選択ToolStripMenuItem, Me.全解除ToolStripMenuItem})
        Me.cmsProgram.Name = "cmsFiles"
        Me.cmsProgram.Size = New System.Drawing.Size(107, 48)
        '
        '全選択ToolStripMenuItem
        '
        Me.全選択ToolStripMenuItem.Name = "全選択ToolStripMenuItem"
        Me.全選択ToolStripMenuItem.Size = New System.Drawing.Size(106, 22)
        Me.全選択ToolStripMenuItem.Text = "全選択"
        '
        '全解除ToolStripMenuItem
        '
        Me.全解除ToolStripMenuItem.Name = "全解除ToolStripMenuItem"
        Me.全解除ToolStripMenuItem.Size = New System.Drawing.Size(106, 22)
        Me.全解除ToolStripMenuItem.Text = "全解除"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lstTable)
        Me.GroupBox2.Location = New System.Drawing.Point(502, 10)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(489, 436)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "テーブル"
        '
        'lstTable
        '
        Me.lstTable.AllowColumnReorder = True
        Me.lstTable.CheckBoxes = True
        Me.lstTable.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
        Me.lstTable.ContextMenuStrip = Me.cmsTable
        Me.lstTable.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstTable.FullRowSelect = True
        Me.lstTable.GridLines = True
        Me.lstTable.HideSelection = False
        Me.lstTable.Location = New System.Drawing.Point(3, 15)
        Me.lstTable.Name = "lstTable"
        Me.lstTable.Size = New System.Drawing.Size(483, 418)
        Me.lstTable.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lstTable.TabIndex = 5
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
        'cmsTable
        '
        Me.cmsTable.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.ToolStripMenuItem2})
        Me.cmsTable.Name = "cmsFiles"
        Me.cmsTable.Size = New System.Drawing.Size(107, 48)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(106, 22)
        Me.ToolStripMenuItem1.Text = "全選択"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(106, 22)
        Me.ToolStripMenuItem2.Text = "全解除"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label2)
        Me.GroupBox3.Controls.Add(Me.cmbTableAccess)
        Me.GroupBox3.Controls.Add(Me.Label1)
        Me.GroupBox3.Controls.Add(Me.cmbProgramAccess)
        Me.GroupBox3.Location = New System.Drawing.Point(7, 456)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(984, 125)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "その他のフィルタ"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(508, 33)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(218, 12)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "このテーブルにアクセスしているプログラムに絞る"
        '
        'cmbTableAccess
        '
        Me.cmbTableAccess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbTableAccess.FormattingEnabled = True
        Me.cmbTableAccess.Location = New System.Drawing.Point(535, 58)
        Me.cmbTableAccess.Name = "cmbTableAccess"
        Me.cmbTableAccess.Size = New System.Drawing.Size(430, 20)
        Me.cmbTableAccess.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(16, 33)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(219, 12)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "このプログラムがアクセスしているテーブルに絞る"
        '
        'cmbProgramAccess
        '
        Me.cmbProgramAccess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbProgramAccess.FormattingEnabled = True
        Me.cmbProgramAccess.Location = New System.Drawing.Point(43, 58)
        Me.cmbProgramAccess.Name = "cmbProgramAccess"
        Me.cmbProgramAccess.Size = New System.Drawing.Size(430, 20)
        Me.cmbProgramAccess.TabIndex = 0
        '
        'cmdCancel
        '
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Location = New System.Drawing.Point(894, 589)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(94, 25)
        Me.cmdCancel.TabIndex = 9
        Me.cmdCancel.Text = "キャンセル"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdApply
        '
        Me.cmdApply.Location = New System.Drawing.Point(794, 589)
        Me.cmdApply.Name = "cmdApply"
        Me.cmdApply.Size = New System.Drawing.Size(94, 25)
        Me.cmdApply.TabIndex = 8
        Me.cmdApply.Text = "適用(&A)"
        Me.cmdApply.UseVisualStyleBackColor = True
        '
        'btnAllClear
        '
        Me.btnAllClear.Location = New System.Drawing.Point(694, 589)
        Me.btnAllClear.Name = "btnAllClear"
        Me.btnAllClear.Size = New System.Drawing.Size(94, 25)
        Me.btnAllClear.TabIndex = 10
        Me.btnAllClear.Text = "全てクリア"
        Me.btnAllClear.UseVisualStyleBackColor = True
        '
        'frmFilter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(999, 619)
        Me.Controls.Add(Me.btnAllClear)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.cmdApply)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmFilter"
        Me.Text = "詳細フィルタ設定"
        Me.GroupBox1.ResumeLayout(False)
        Me.cmsProgram.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.cmsTable.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmbProgramAccess As System.Windows.Forms.ComboBox
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdApply As System.Windows.Forms.Button
    Friend WithEvents lstProgram As System.Windows.Forms.ListView
    Friend WithEvents chTableName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chEntityName As System.Windows.Forms.ColumnHeader
    Friend WithEvents lstTable As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents cmsProgram As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents 全選択ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 全解除ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmsTable As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmbTableAccess As System.Windows.Forms.ComboBox
    Friend WithEvents btnAllClear As System.Windows.Forms.Button
End Class
