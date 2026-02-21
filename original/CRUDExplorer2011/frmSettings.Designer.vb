<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSettings))
        Me.btnSelectHidemaruPath = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtHidemaruPath = New System.Windows.Forms.TextBox
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdSave = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.rdoListDblClickMode2 = New System.Windows.Forms.RadioButton
        Me.rdoListDblClickMode1 = New System.Windows.Forms.RadioButton
        Me.rdoListDblClickMode0 = New System.Windows.Forms.RadioButton
        Me.chkDebugMode = New System.Windows.Forms.CheckBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtProgramIdPattern = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.grpTextEditor = New System.Windows.Forms.GroupBox
        Me.rdoEditorNotepad = New System.Windows.Forms.RadioButton
        Me.txtNotepadPath = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.btnSelectNotepadPath = New System.Windows.Forms.Button
        Me.txtSakuraPath = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.btnSelectSakuraPath = New System.Windows.Forms.Button
        Me.rdoEditorSakura = New System.Windows.Forms.RadioButton
        Me.rdoEditorHidemaru = New System.Windows.Forms.RadioButton
        Me.GroupBox1.SuspendLayout()
        Me.grpTextEditor.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSelectHidemaruPath
        '
        Me.btnSelectHidemaruPath.Location = New System.Drawing.Point(894, 90)
        Me.btnSelectHidemaruPath.Name = "btnSelectHidemaruPath"
        Me.btnSelectHidemaruPath.Size = New System.Drawing.Size(30, 25)
        Me.btnSelectHidemaruPath.TabIndex = 8
        Me.btnSelectHidemaruPath.Text = "…"
        Me.btnSelectHidemaruPath.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label1.Location = New System.Drawing.Point(120, 93)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.Label1.Size = New System.Drawing.Size(137, 22)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "秀丸のパス"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtHidemaruPath
        '
        Me.txtHidemaruPath.Location = New System.Drawing.Point(246, 93)
        Me.txtHidemaruPath.Name = "txtHidemaruPath"
        Me.txtHidemaruPath.Size = New System.Drawing.Size(642, 19)
        Me.txtHidemaruPath.TabIndex = 7
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'cmdCancel
        '
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Location = New System.Drawing.Point(858, 364)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(94, 25)
        Me.cmdCancel.TabIndex = 4
        Me.cmdCancel.Text = "キャンセル"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdSave
        '
        Me.cmdSave.Location = New System.Drawing.Point(758, 364)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.Size = New System.Drawing.Size(94, 25)
        Me.cmdSave.TabIndex = 3
        Me.cmdSave.Text = "設定"
        Me.cmdSave.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.rdoListDblClickMode2)
        Me.GroupBox1.Controls.Add(Me.rdoListDblClickMode1)
        Me.GroupBox1.Controls.Add(Me.rdoListDblClickMode0)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 153)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(469, 75)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "メイン画面のリストダブルクリック時の動作"
        '
        'rdoListDblClickMode2
        '
        Me.rdoListDblClickMode2.AutoSize = True
        Me.rdoListDblClickMode2.Location = New System.Drawing.Point(317, 30)
        Me.rdoListDblClickMode2.Name = "rdoListDblClickMode2"
        Me.rdoListDblClickMode2.Size = New System.Drawing.Size(73, 16)
        Me.rdoListDblClickMode2.TabIndex = 2
        Me.rdoListDblClickMode2.TabStop = True
        Me.rdoListDblClickMode2.Text = "何もしない"
        Me.rdoListDblClickMode2.UseVisualStyleBackColor = True
        '
        'rdoListDblClickMode1
        '
        Me.rdoListDblClickMode1.AutoSize = True
        Me.rdoListDblClickMode1.Location = New System.Drawing.Point(173, 30)
        Me.rdoListDblClickMode1.Name = "rdoListDblClickMode1"
        Me.rdoListDblClickMode1.Size = New System.Drawing.Size(128, 16)
        Me.rdoListDblClickMode1.TabIndex = 1
        Me.rdoListDblClickMode1.TabStop = True
        Me.rdoListDblClickMode1.Text = "クエリ分析画面を表示"
        Me.rdoListDblClickMode1.UseVisualStyleBackColor = True
        '
        'rdoListDblClickMode0
        '
        Me.rdoListDblClickMode0.AutoSize = True
        Me.rdoListDblClickMode0.Location = New System.Drawing.Point(24, 30)
        Me.rdoListDblClickMode0.Name = "rdoListDblClickMode0"
        Me.rdoListDblClickMode0.Size = New System.Drawing.Size(126, 16)
        Me.rdoListDblClickMode0.TabIndex = 0
        Me.rdoListDblClickMode0.TabStop = True
        Me.rdoListDblClickMode0.Text = "テキストエディタを起動"
        Me.rdoListDblClickMode0.UseVisualStyleBackColor = True
        '
        'chkDebugMode
        '
        Me.chkDebugMode.AutoSize = True
        Me.chkDebugMode.Location = New System.Drawing.Point(20, 334)
        Me.chkDebugMode.Name = "chkDebugMode"
        Me.chkDebugMode.Size = New System.Drawing.Size(88, 16)
        Me.chkDebugMode.TabIndex = 11
        Me.chkDebugMode.Text = "デバッグモード"
        Me.chkDebugMode.UseVisualStyleBackColor = True
        Me.chkDebugMode.Visible = False
        '
        'Label2
        '
        Me.Label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label2.Location = New System.Drawing.Point(12, 241)
        Me.Label2.Name = "Label2"
        Me.Label2.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.Label2.Size = New System.Drawing.Size(138, 24)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "プログラムID抽出パターン"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtProgramIdPattern
        '
        Me.txtProgramIdPattern.Location = New System.Drawing.Point(229, 244)
        Me.txtProgramIdPattern.Name = "txtProgramIdPattern"
        Me.txtProgramIdPattern.Size = New System.Drawing.Size(140, 19)
        Me.txtProgramIdPattern.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.ForeColor = System.Drawing.Color.Blue
        Me.Label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label3.Location = New System.Drawing.Point(375, 244)
        Me.Label3.Name = "Label3"
        Me.Label3.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.Label3.Size = New System.Drawing.Size(561, 32)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "ソースファイル名からプログラムIDを識別するための正規表現を指定します。例えば、ソースファイル名の先頭５文字がプログラムIDの場合、^(.{5})と指定します。"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'grpTextEditor
        '
        Me.grpTextEditor.Controls.Add(Me.rdoEditorNotepad)
        Me.grpTextEditor.Controls.Add(Me.txtNotepadPath)
        Me.grpTextEditor.Controls.Add(Me.Label7)
        Me.grpTextEditor.Controls.Add(Me.btnSelectNotepadPath)
        Me.grpTextEditor.Controls.Add(Me.txtSakuraPath)
        Me.grpTextEditor.Controls.Add(Me.Label6)
        Me.grpTextEditor.Controls.Add(Me.btnSelectSakuraPath)
        Me.grpTextEditor.Controls.Add(Me.rdoEditorSakura)
        Me.grpTextEditor.Controls.Add(Me.rdoEditorHidemaru)
        Me.grpTextEditor.Controls.Add(Me.txtHidemaruPath)
        Me.grpTextEditor.Controls.Add(Me.Label1)
        Me.grpTextEditor.Controls.Add(Me.btnSelectHidemaruPath)
        Me.grpTextEditor.Location = New System.Drawing.Point(12, 12)
        Me.grpTextEditor.Name = "grpTextEditor"
        Me.grpTextEditor.Size = New System.Drawing.Size(940, 125)
        Me.grpTextEditor.TabIndex = 0
        Me.grpTextEditor.TabStop = False
        Me.grpTextEditor.Text = "外部テキストエディタ"
        '
        'rdoEditorNotepad
        '
        Me.rdoEditorNotepad.AutoSize = True
        Me.rdoEditorNotepad.Location = New System.Drawing.Point(24, 28)
        Me.rdoEditorNotepad.Name = "rdoEditorNotepad"
        Me.rdoEditorNotepad.Size = New System.Drawing.Size(52, 16)
        Me.rdoEditorNotepad.TabIndex = 0
        Me.rdoEditorNotepad.TabStop = True
        Me.rdoEditorNotepad.Text = "メモ帳"
        Me.rdoEditorNotepad.UseVisualStyleBackColor = True
        '
        'txtNotepadPath
        '
        Me.txtNotepadPath.Location = New System.Drawing.Point(246, 25)
        Me.txtNotepadPath.Name = "txtNotepadPath"
        Me.txtNotepadPath.Size = New System.Drawing.Size(642, 19)
        Me.txtNotepadPath.TabIndex = 3
        '
        'Label7
        '
        Me.Label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label7.Location = New System.Drawing.Point(116, 25)
        Me.Label7.Name = "Label7"
        Me.Label7.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.Label7.Size = New System.Drawing.Size(141, 22)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "メモ帳(Notepad)のパス"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectNotepadPath
        '
        Me.btnSelectNotepadPath.Location = New System.Drawing.Point(894, 22)
        Me.btnSelectNotepadPath.Name = "btnSelectNotepadPath"
        Me.btnSelectNotepadPath.Size = New System.Drawing.Size(30, 25)
        Me.btnSelectNotepadPath.TabIndex = 4
        Me.btnSelectNotepadPath.Text = "…"
        Me.btnSelectNotepadPath.UseVisualStyleBackColor = True
        '
        'txtSakuraPath
        '
        Me.txtSakuraPath.Location = New System.Drawing.Point(246, 59)
        Me.txtSakuraPath.Name = "txtSakuraPath"
        Me.txtSakuraPath.Size = New System.Drawing.Size(642, 19)
        Me.txtSakuraPath.TabIndex = 5
        '
        'Label6
        '
        Me.Label6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label6.Location = New System.Drawing.Point(118, 59)
        Me.Label6.Name = "Label6"
        Me.Label6.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.Label6.Size = New System.Drawing.Size(139, 22)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "サクラエディタのパス"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectSakuraPath
        '
        Me.btnSelectSakuraPath.Location = New System.Drawing.Point(894, 56)
        Me.btnSelectSakuraPath.Name = "btnSelectSakuraPath"
        Me.btnSelectSakuraPath.Size = New System.Drawing.Size(30, 25)
        Me.btnSelectSakuraPath.TabIndex = 6
        Me.btnSelectSakuraPath.Text = "…"
        Me.btnSelectSakuraPath.UseVisualStyleBackColor = True
        '
        'rdoEditorSakura
        '
        Me.rdoEditorSakura.AutoSize = True
        Me.rdoEditorSakura.Location = New System.Drawing.Point(24, 62)
        Me.rdoEditorSakura.Name = "rdoEditorSakura"
        Me.rdoEditorSakura.Size = New System.Drawing.Size(83, 16)
        Me.rdoEditorSakura.TabIndex = 1
        Me.rdoEditorSakura.TabStop = True
        Me.rdoEditorSakura.Text = "サクラエディタ"
        Me.rdoEditorSakura.UseVisualStyleBackColor = True
        '
        'rdoEditorHidemaru
        '
        Me.rdoEditorHidemaru.AutoSize = True
        Me.rdoEditorHidemaru.Location = New System.Drawing.Point(24, 96)
        Me.rdoEditorHidemaru.Name = "rdoEditorHidemaru"
        Me.rdoEditorHidemaru.Size = New System.Drawing.Size(47, 16)
        Me.rdoEditorHidemaru.TabIndex = 2
        Me.rdoEditorHidemaru.TabStop = True
        Me.rdoEditorHidemaru.Text = "秀丸"
        Me.rdoEditorHidemaru.UseVisualStyleBackColor = True
        '
        'frmSettings
        '
        Me.AcceptButton = Me.cmdSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(961, 393)
        Me.Controls.Add(Me.grpTextEditor)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtProgramIdPattern)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.chkDebugMode)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdSave)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSettings"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "CRUD Explorerの設定"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.grpTextEditor.ResumeLayout(False)
        Me.grpTextEditor.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnSelectHidemaruPath As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtHidemaruPath As System.Windows.Forms.TextBox
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdSave As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents rdoListDblClickMode1 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoListDblClickMode0 As System.Windows.Forms.RadioButton
    Friend WithEvents rdoListDblClickMode2 As System.Windows.Forms.RadioButton
    Friend WithEvents chkDebugMode As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtProgramIdPattern As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents grpTextEditor As System.Windows.Forms.GroupBox
    Friend WithEvents rdoEditorSakura As System.Windows.Forms.RadioButton
    Friend WithEvents rdoEditorHidemaru As System.Windows.Forms.RadioButton
    Friend WithEvents rdoEditorNotepad As System.Windows.Forms.RadioButton
    Friend WithEvents txtNotepadPath As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents btnSelectNotepadPath As System.Windows.Forms.Button
    Friend WithEvents txtSakuraPath As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btnSelectSakuraPath As System.Windows.Forms.Button
End Class
