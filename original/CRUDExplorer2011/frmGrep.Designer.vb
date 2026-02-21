<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGrep
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGrep))
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.Panel7 = New System.Windows.Forms.Panel
        Me.cmdClose = New System.Windows.Forms.Button
        Me.cmdAnalyzeQueryNew = New System.Windows.Forms.Button
        Me.cmdAnalyzeQuery = New System.Windows.Forms.Button
        Me.cmdRunHidemaru = New System.Windows.Forms.Button
        Me.lstQuerys = New System.Windows.Forms.ListView
        Me.chQuery = New System.Windows.Forms.ColumnHeader
        Me.chLineNo = New System.Windows.Forms.ColumnHeader
        Me.chFuncProc = New System.Windows.Forms.ColumnHeader
        Me.chFilename = New System.Windows.Forms.ColumnHeader
        Me.chProgram = New System.Windows.Forms.ColumnHeader
        Me.Panel2.SuspendLayout()
        Me.Panel7.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Panel7)
        Me.Panel2.Controls.Add(Me.cmdAnalyzeQueryNew)
        Me.Panel2.Controls.Add(Me.cmdAnalyzeQuery)
        Me.Panel2.Controls.Add(Me.cmdRunHidemaru)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.Location = New System.Drawing.Point(0, 397)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(974, 34)
        Me.Panel2.TabIndex = 11
        '
        'Panel7
        '
        Me.Panel7.Controls.Add(Me.cmdClose)
        Me.Panel7.Dock = System.Windows.Forms.DockStyle.Right
        Me.Panel7.Location = New System.Drawing.Point(857, 0)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(117, 34)
        Me.Panel7.TabIndex = 14
        '
        'cmdClose
        '
        Me.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdClose.Location = New System.Drawing.Point(20, 4)
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
        Me.cmdAnalyzeQueryNew.TabIndex = 11
        Me.cmdAnalyzeQueryNew.Text = "クエリ分析(別ウィンドウ)(&N)"
        Me.cmdAnalyzeQueryNew.UseVisualStyleBackColor = True
        '
        'cmdAnalyzeQuery
        '
        Me.cmdAnalyzeQuery.Location = New System.Drawing.Point(103, 3)
        Me.cmdAnalyzeQuery.Name = "cmdAnalyzeQuery"
        Me.cmdAnalyzeQuery.Size = New System.Drawing.Size(94, 25)
        Me.cmdAnalyzeQuery.TabIndex = 10
        Me.cmdAnalyzeQuery.Text = "クエリ分析(&A)"
        Me.cmdAnalyzeQuery.UseVisualStyleBackColor = True
        '
        'cmdRunHidemaru
        '
        Me.cmdRunHidemaru.Location = New System.Drawing.Point(3, 3)
        Me.cmdRunHidemaru.Name = "cmdRunHidemaru"
        Me.cmdRunHidemaru.Size = New System.Drawing.Size(94, 25)
        Me.cmdRunHidemaru.TabIndex = 9
        Me.cmdRunHidemaru.Text = "エディタ起動(&E)"
        Me.cmdRunHidemaru.UseVisualStyleBackColor = True
        '
        'lstQuerys
        '
        Me.lstQuerys.AllowColumnReorder = True
        Me.lstQuerys.CheckBoxes = True
        Me.lstQuerys.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chFilename, Me.chLineNo, Me.chProgram, Me.chFuncProc, Me.chQuery})
        Me.lstQuerys.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstQuerys.FullRowSelect = True
        Me.lstQuerys.GridLines = True
        Me.lstQuerys.HideSelection = False
        Me.lstQuerys.Location = New System.Drawing.Point(0, 0)
        Me.lstQuerys.MultiSelect = False
        Me.lstQuerys.Name = "lstQuerys"
        Me.lstQuerys.Size = New System.Drawing.Size(974, 397)
        Me.lstQuerys.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lstQuerys.TabIndex = 12
        Me.lstQuerys.UseCompatibleStateImageBehavior = False
        Me.lstQuerys.View = System.Windows.Forms.View.Details
        '
        'chQuery
        '
        Me.chQuery.Text = "クエリ"
        Me.chQuery.Width = 308
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
        Me.chFuncProc.Width = 200
        '
        'chFilename
        '
        Me.chFilename.Text = "ファイル名"
        Me.chFilename.Width = 213
        '
        'chProgram
        '
        Me.chProgram.Text = "プログラム"
        Me.chProgram.Width = 171
        '
        'frmGrep
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdClose
        Me.ClientSize = New System.Drawing.Size(974, 431)
        Me.Controls.Add(Me.lstQuerys)
        Me.Controls.Add(Me.Panel2)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmGrep"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "frmGrep"
        Me.Panel2.ResumeLayout(False)
        Me.Panel7.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents cmdAnalyzeQueryNew As System.Windows.Forms.Button
    Friend WithEvents cmdAnalyzeQuery As System.Windows.Forms.Button
    Friend WithEvents cmdRunHidemaru As System.Windows.Forms.Button
    Friend WithEvents lstQuerys As System.Windows.Forms.ListView
    Friend WithEvents chQuery As System.Windows.Forms.ColumnHeader
    Friend WithEvents chLineNo As System.Windows.Forms.ColumnHeader
    Friend WithEvents chFuncProc As System.Windows.Forms.ColumnHeader
    Friend WithEvents chFilename As System.Windows.Forms.ColumnHeader
    Friend WithEvents chProgram As System.Windows.Forms.ColumnHeader
End Class
