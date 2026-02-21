<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFileList
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFileList))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.Panel10 = New System.Windows.Forms.Panel
        Me.lstFiles = New System.Windows.Forms.ListView
        Me.chProgramId = New System.Windows.Forms.ColumnHeader
        Me.chFile = New System.Windows.Forms.ColumnHeader
        Me.cmsFiles = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.全選択ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.全解除ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.pngGrep = New System.Windows.Forms.Panel
        Me.Panel3 = New System.Windows.Forms.Panel
        Me.Panel4 = New System.Windows.Forms.Panel
        Me.Panel9 = New System.Windows.Forms.Panel
        Me.txtSearch = New System.Windows.Forms.TextBox
        Me.Panel8 = New System.Windows.Forms.Panel
        Me.Panel6 = New System.Windows.Forms.Panel
        Me.Panel5 = New System.Windows.Forms.Panel
        Me.btnGrep = New System.Windows.Forms.Button
        Me.chkRegular = New System.Windows.Forms.CheckBox
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.Label1 = New System.Windows.Forms.Label
        Me.pnlList = New System.Windows.Forms.Panel
        Me.lstQuerys = New System.Windows.Forms.ListView
        Me.chFilename = New System.Windows.Forms.ColumnHeader
        Me.chLineNo = New System.Windows.Forms.ColumnHeader
        Me.chFuncProc = New System.Windows.Forms.ColumnHeader
        Me.chQuery = New System.Windows.Forms.ColumnHeader
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.Panel7 = New System.Windows.Forms.Panel
        Me.cmdClose = New System.Windows.Forms.Button
        Me.cmdAnalyzeQueryNew = New System.Windows.Forms.Button
        Me.cmdAnalyzeQuery = New System.Windows.Forms.Button
        Me.cmdRunHidemaru = New System.Windows.Forms.Button
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.Panel10.SuspendLayout()
        Me.cmsFiles.SuspendLayout()
        Me.pngGrep.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel9.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.pnlList.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel7.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Panel10)
        Me.SplitContainer1.Panel1.Controls.Add(Me.pngGrep)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlList)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Panel1)
        Me.SplitContainer1.Size = New System.Drawing.Size(1008, 662)
        Me.SplitContainer1.SplitterDistance = 377
        Me.SplitContainer1.TabIndex = 1
        Me.SplitContainer1.TabStop = False
        '
        'Panel10
        '
        Me.Panel10.Controls.Add(Me.lstFiles)
        Me.Panel10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel10.Location = New System.Drawing.Point(0, 34)
        Me.Panel10.Name = "Panel10"
        Me.Panel10.Size = New System.Drawing.Size(377, 628)
        Me.Panel10.TabIndex = 4
        '
        'lstFiles
        '
        Me.lstFiles.AllowColumnReorder = True
        Me.lstFiles.CheckBoxes = True
        Me.lstFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chProgramId, Me.chFile})
        Me.lstFiles.ContextMenuStrip = Me.cmsFiles
        Me.lstFiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstFiles.FullRowSelect = True
        Me.lstFiles.GridLines = True
        Me.lstFiles.HideSelection = False
        Me.lstFiles.Location = New System.Drawing.Point(0, 0)
        Me.lstFiles.MultiSelect = False
        Me.lstFiles.Name = "lstFiles"
        Me.lstFiles.Size = New System.Drawing.Size(377, 628)
        Me.lstFiles.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lstFiles.TabIndex = 4
        Me.lstFiles.UseCompatibleStateImageBehavior = False
        Me.lstFiles.View = System.Windows.Forms.View.Details
        '
        'chProgramId
        '
        Me.chProgramId.Text = "プログラムID"
        Me.chProgramId.Width = 136
        '
        'chFile
        '
        Me.chFile.Text = "ファイル名"
        Me.chFile.Width = 216
        '
        'cmsFiles
        '
        Me.cmsFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.全選択ToolStripMenuItem, Me.全解除ToolStripMenuItem})
        Me.cmsFiles.Name = "cmsFiles"
        Me.cmsFiles.Size = New System.Drawing.Size(113, 48)
        '
        '全選択ToolStripMenuItem
        '
        Me.全選択ToolStripMenuItem.Name = "全選択ToolStripMenuItem"
        Me.全選択ToolStripMenuItem.Size = New System.Drawing.Size(112, 22)
        Me.全選択ToolStripMenuItem.Text = "全選択"
        '
        '全解除ToolStripMenuItem
        '
        Me.全解除ToolStripMenuItem.Name = "全解除ToolStripMenuItem"
        Me.全解除ToolStripMenuItem.Size = New System.Drawing.Size(112, 22)
        Me.全解除ToolStripMenuItem.Text = "全解除"
        '
        'pngGrep
        '
        Me.pngGrep.Controls.Add(Me.Panel3)
        Me.pngGrep.Controls.Add(Me.Panel2)
        Me.pngGrep.Dock = System.Windows.Forms.DockStyle.Top
        Me.pngGrep.Location = New System.Drawing.Point(0, 0)
        Me.pngGrep.Name = "pngGrep"
        Me.pngGrep.Size = New System.Drawing.Size(377, 34)
        Me.pngGrep.TabIndex = 0
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.Panel4)
        Me.Panel3.Controls.Add(Me.Panel5)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel3.Location = New System.Drawing.Point(74, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(303, 34)
        Me.Panel3.TabIndex = 2
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.Panel9)
        Me.Panel4.Controls.Add(Me.Panel8)
        Me.Panel4.Controls.Add(Me.Panel6)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel4.Location = New System.Drawing.Point(0, 0)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(115, 34)
        Me.Panel4.TabIndex = 6
        '
        'Panel9
        '
        Me.Panel9.Controls.Add(Me.txtSearch)
        Me.Panel9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel9.Location = New System.Drawing.Point(0, 10)
        Me.Panel9.Name = "Panel9"
        Me.Panel9.Size = New System.Drawing.Size(115, 16)
        Me.Panel9.TabIndex = 4
        '
        'txtSearch
        '
        Me.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtSearch.Location = New System.Drawing.Point(0, 0)
        Me.txtSearch.Margin = New System.Windows.Forms.Padding(3, 10, 3, 3)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(115, 19)
        Me.txtSearch.TabIndex = 1
        '
        'Panel8
        '
        Me.Panel8.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel8.Location = New System.Drawing.Point(0, 26)
        Me.Panel8.Name = "Panel8"
        Me.Panel8.Size = New System.Drawing.Size(115, 8)
        Me.Panel8.TabIndex = 3
        '
        'Panel6
        '
        Me.Panel6.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel6.Location = New System.Drawing.Point(0, 0)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(115, 10)
        Me.Panel6.TabIndex = 2
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.btnGrep)
        Me.Panel5.Controls.Add(Me.chkRegular)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel5.Location = New System.Drawing.Point(115, 0)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(188, 34)
        Me.Panel5.TabIndex = 5
        '
        'btnGrep
        '
        Me.btnGrep.Location = New System.Drawing.Point(91, 4)
        Me.btnGrep.Name = "btnGrep"
        Me.btnGrep.Size = New System.Drawing.Size(94, 25)
        Me.btnGrep.TabIndex = 3
        Me.btnGrep.Text = "クエリGrep"
        Me.btnGrep.UseVisualStyleBackColor = True
        '
        'chkRegular
        '
        Me.chkRegular.AutoSize = True
        Me.chkRegular.Checked = True
        Me.chkRegular.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkRegular.Location = New System.Drawing.Point(6, 10)
        Me.chkRegular.Name = "chkRegular"
        Me.chkRegular.Size = New System.Drawing.Size(88, 16)
        Me.chkRegular.TabIndex = 2
        Me.chkRegular.Text = "正規表現(&R)"
        Me.chkRegular.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Label1)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(74, 34)
        Me.Panel2.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(5, 11)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 12)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "検索文字列"
        '
        'pnlList
        '
        Me.pnlList.Controls.Add(Me.lstQuerys)
        Me.pnlList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlList.Location = New System.Drawing.Point(0, 34)
        Me.pnlList.Name = "pnlList"
        Me.pnlList.Size = New System.Drawing.Size(627, 628)
        Me.pnlList.TabIndex = 6
        '
        'lstQuerys
        '
        Me.lstQuerys.AllowColumnReorder = True
        Me.lstQuerys.CheckBoxes = True
        Me.lstQuerys.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chFilename, Me.chLineNo, Me.chFuncProc, Me.chQuery})
        Me.lstQuerys.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstQuerys.FullRowSelect = True
        Me.lstQuerys.GridLines = True
        Me.lstQuerys.HideSelection = False
        Me.lstQuerys.Location = New System.Drawing.Point(0, 0)
        Me.lstQuerys.MultiSelect = False
        Me.lstQuerys.Name = "lstQuerys"
        Me.lstQuerys.Size = New System.Drawing.Size(627, 628)
        Me.lstQuerys.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lstQuerys.TabIndex = 5
        Me.lstQuerys.UseCompatibleStateImageBehavior = False
        Me.lstQuerys.View = System.Windows.Forms.View.Details
        '
        'chFilename
        '
        Me.chFilename.Text = "ファイル名"
        Me.chFilename.Width = 145
        '
        'chLineNo
        '
        Me.chLineNo.Text = "行番号"
        Me.chLineNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.chLineNo.Width = 57
        '
        'chFuncProc
        '
        Me.chFuncProc.Text = "関数名/カーソル名"
        Me.chFuncProc.Width = 169
        '
        'chQuery
        '
        Me.chQuery.Text = "クエリ"
        Me.chQuery.Width = 300
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Panel7)
        Me.Panel1.Controls.Add(Me.cmdAnalyzeQueryNew)
        Me.Panel1.Controls.Add(Me.cmdAnalyzeQuery)
        Me.Panel1.Controls.Add(Me.cmdRunHidemaru)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(627, 34)
        Me.Panel1.TabIndex = 8
        '
        'Panel7
        '
        Me.Panel7.Controls.Add(Me.cmdClose)
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel7.Location = New System.Drawing.Point(528, 0)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(99, 34)
        Me.Panel7.TabIndex = 13
        '
        'cmdClose
        '
        Me.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdClose.Location = New System.Drawing.Point(0, 4)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(94, 25)
        Me.cmdClose.TabIndex = 9
        Me.cmdClose.Text = "閉じる(&C)"
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'cmdAnalyzeQueryNew
        '
        Me.cmdAnalyzeQueryNew.Location = New System.Drawing.Point(203, 4)
        Me.cmdAnalyzeQueryNew.Name = "cmdAnalyzeQueryNew"
        Me.cmdAnalyzeQueryNew.Size = New System.Drawing.Size(147, 25)
        Me.cmdAnalyzeQueryNew.TabIndex = 8
        Me.cmdAnalyzeQueryNew.Text = "クエリ分析(別ウィンドウ)(&N)"
        Me.cmdAnalyzeQueryNew.UseVisualStyleBackColor = True
        '
        'cmdAnalyzeQuery
        '
        Me.cmdAnalyzeQuery.Location = New System.Drawing.Point(103, 4)
        Me.cmdAnalyzeQuery.Name = "cmdAnalyzeQuery"
        Me.cmdAnalyzeQuery.Size = New System.Drawing.Size(94, 25)
        Me.cmdAnalyzeQuery.TabIndex = 7
        Me.cmdAnalyzeQuery.Text = "クエリ分析(&A)"
        Me.cmdAnalyzeQuery.UseVisualStyleBackColor = True
        '
        'cmdRunHidemaru
        '
        Me.cmdRunHidemaru.Location = New System.Drawing.Point(3, 4)
        Me.cmdRunHidemaru.Name = "cmdRunHidemaru"
        Me.cmdRunHidemaru.Size = New System.Drawing.Size(94, 25)
        Me.cmdRunHidemaru.TabIndex = 6
        Me.cmdRunHidemaru.Text = "エディタ起動(&E)"
        Me.cmdRunHidemaru.UseVisualStyleBackColor = True
        '
        'frmFileList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdClose
        Me.ClientSize = New System.Drawing.Size(1008, 662)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmFileList"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ソースファイル一覧"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.Panel10.ResumeLayout(False)
        Me.cmsFiles.ResumeLayout(False)
        Me.pngGrep.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel9.ResumeLayout(False)
        Me.Panel9.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.pnlList.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel7.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents lstFiles As System.Windows.Forms.ListView
    Friend WithEvents chFile As System.Windows.Forms.ColumnHeader
    Friend WithEvents chProgramId As System.Windows.Forms.ColumnHeader
    Friend WithEvents lstQuerys As System.Windows.Forms.ListView
    Friend WithEvents chQuery As System.Windows.Forms.ColumnHeader
    Friend WithEvents chLineNo As System.Windows.Forms.ColumnHeader
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents cmdAnalyzeQuery As System.Windows.Forms.Button
    Friend WithEvents cmdRunHidemaru As System.Windows.Forms.Button
    Friend WithEvents chFuncProc As System.Windows.Forms.ColumnHeader
    Friend WithEvents cmdAnalyzeQueryNew As System.Windows.Forms.Button
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents pngGrep As System.Windows.Forms.Panel
    Friend WithEvents pnlList As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents chkRegular As System.Windows.Forms.CheckBox
    Friend WithEvents txtSearch As System.Windows.Forms.TextBox
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents btnGrep As System.Windows.Forms.Button
    Friend WithEvents Panel8 As System.Windows.Forms.Panel
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents Panel9 As System.Windows.Forms.Panel
    Friend WithEvents chFilename As System.Windows.Forms.ColumnHeader
    Friend WithEvents Panel10 As System.Windows.Forms.Panel
    Friend WithEvents cmsFiles As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents 全選択ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents 全解除ToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
