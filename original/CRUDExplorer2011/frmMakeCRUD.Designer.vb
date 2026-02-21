<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMakeCRUD
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMakeCRUD))
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnSelectSourceFolder = New System.Windows.Forms.Button
        Me.chkReference = New System.Windows.Forms.CheckBox
        Me.btnAnalyzeCRUD = New System.Windows.Forms.Button
        Me.grpState = New System.Windows.Forms.GroupBox
        Me.cmdAnalyzeQuery = New System.Windows.Forms.Button
        Me.cmdRunHidemaru = New System.Windows.Forms.Button
        Me.lblPhase = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtLog = New System.Windows.Forms.TextBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.chkStep4 = New System.Windows.Forms.CheckBox
        Me.chkStep3 = New System.Windows.Forms.CheckBox
        Me.chkStep2 = New System.Windows.Forms.CheckBox
        Me.chkStep1 = New System.Windows.Forms.CheckBox
        Me.chkProcAll = New System.Windows.Forms.CheckBox
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.btnClose = New System.Windows.Forms.Button
        Me.pnlControl = New System.Windows.Forms.Panel
        Me.btnSelectDestFolder = New System.Windows.Forms.Button
        Me.txtDestPath = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtSourcePath = New System.Windows.Forms.TextBox
        Me.chkReferenceCond = New System.Windows.Forms.CheckBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.chkStep0 = New System.Windows.Forms.CheckBox
        Me.grpState.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.pnlControl.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label1.Location = New System.Drawing.Point(7, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.Label1.Size = New System.Drawing.Size(90, 22)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "ソースフォルダ"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnSelectSourceFolder
        '
        Me.btnSelectSourceFolder.Location = New System.Drawing.Point(950, 38)
        Me.btnSelectSourceFolder.Name = "btnSelectSourceFolder"
        Me.btnSelectSourceFolder.Size = New System.Drawing.Size(30, 25)
        Me.btnSelectSourceFolder.TabIndex = 1
        Me.btnSelectSourceFolder.Text = "…"
        Me.btnSelectSourceFolder.UseVisualStyleBackColor = True
        '
        'chkReference
        '
        Me.chkReference.AutoSize = True
        Me.chkReference.Checked = True
        Me.chkReference.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkReference.Location = New System.Drawing.Point(545, 149)
        Me.chkReference.Name = "chkReference"
        Me.chkReference.Size = New System.Drawing.Size(226, 16)
        Me.chkReference.TabIndex = 10
        Me.chkReference.Text = "Viewによる間接参照をマトリクスに反映する"
        Me.chkReference.UseVisualStyleBackColor = True
        '
        'btnAnalyzeCRUD
        '
        Me.btnAnalyzeCRUD.Location = New System.Drawing.Point(868, 187)
        Me.btnAnalyzeCRUD.Name = "btnAnalyzeCRUD"
        Me.btnAnalyzeCRUD.Size = New System.Drawing.Size(112, 25)
        Me.btnAnalyzeCRUD.TabIndex = 12
        Me.btnAnalyzeCRUD.Text = "CRUD解析開始"
        Me.btnAnalyzeCRUD.UseVisualStyleBackColor = True
        '
        'grpState
        '
        Me.grpState.Controls.Add(Me.cmdAnalyzeQuery)
        Me.grpState.Controls.Add(Me.cmdRunHidemaru)
        Me.grpState.Controls.Add(Me.lblPhase)
        Me.grpState.Controls.Add(Me.Label2)
        Me.grpState.Controls.Add(Me.txtLog)
        Me.grpState.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.grpState.Location = New System.Drawing.Point(0, 266)
        Me.grpState.Name = "grpState"
        Me.grpState.Size = New System.Drawing.Size(1008, 296)
        Me.grpState.TabIndex = 8
        Me.grpState.TabStop = False
        Me.grpState.Text = "解析状況"
        '
        'cmdAnalyzeQuery
        '
        Me.cmdAnalyzeQuery.Location = New System.Drawing.Point(910, 17)
        Me.cmdAnalyzeQuery.Name = "cmdAnalyzeQuery"
        Me.cmdAnalyzeQuery.Size = New System.Drawing.Size(94, 25)
        Me.cmdAnalyzeQuery.TabIndex = 12
        Me.cmdAnalyzeQuery.Text = "クエリ分析(&A)"
        Me.cmdAnalyzeQuery.UseVisualStyleBackColor = True
        '
        'cmdRunHidemaru
        '
        Me.cmdRunHidemaru.Location = New System.Drawing.Point(810, 17)
        Me.cmdRunHidemaru.Name = "cmdRunHidemaru"
        Me.cmdRunHidemaru.Size = New System.Drawing.Size(94, 25)
        Me.cmdRunHidemaru.TabIndex = 11
        Me.cmdRunHidemaru.Text = "エディタ起動(&E)"
        Me.cmdRunHidemaru.UseVisualStyleBackColor = True
        '
        'lblPhase
        '
        Me.lblPhase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblPhase.Location = New System.Drawing.Point(94, 26)
        Me.lblPhase.Name = "lblPhase"
        Me.lblPhase.Size = New System.Drawing.Size(574, 17)
        Me.lblPhase.TabIndex = 4
        Me.lblPhase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(14, 23)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(90, 22)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "解析フェーズ"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtLog
        '
        Me.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.txtLog.Location = New System.Drawing.Point(3, 46)
        Me.txtLog.Margin = New System.Windows.Forms.Padding(100, 3, 100, 3)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLog.Size = New System.Drawing.Size(1002, 247)
        Me.txtLog.TabIndex = 10
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.chkStep0)
        Me.GroupBox1.Controls.Add(Me.chkStep4)
        Me.GroupBox1.Controls.Add(Me.chkStep3)
        Me.GroupBox1.Controls.Add(Me.chkStep2)
        Me.GroupBox1.Controls.Add(Me.chkStep1)
        Me.GroupBox1.Controls.Add(Me.chkProcAll)
        Me.GroupBox1.Location = New System.Drawing.Point(103, 99)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(392, 147)
        Me.GroupBox1.TabIndex = 10
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "処理選択"
        '
        'chkStep4
        '
        Me.chkStep4.AutoSize = True
        Me.chkStep4.Checked = True
        Me.chkStep4.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStep4.Enabled = False
        Me.chkStep4.Location = New System.Drawing.Point(108, 116)
        Me.chkStep4.Name = "chkStep4"
        Me.chkStep4.Size = New System.Drawing.Size(154, 16)
        Me.chkStep4.TabIndex = 9
        Me.chkStep4.Text = "Step4 CRUDマトリクス生成"
        Me.chkStep4.UseVisualStyleBackColor = True
        '
        'chkStep3
        '
        Me.chkStep3.AutoSize = True
        Me.chkStep3.Checked = True
        Me.chkStep3.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStep3.Enabled = False
        Me.chkStep3.Location = New System.Drawing.Point(108, 94)
        Me.chkStep3.Name = "chkStep3"
        Me.chkStep3.Size = New System.Drawing.Size(149, 16)
        Me.chkStep3.TabIndex = 8
        Me.chkStep3.Text = "Step3 CRUDアクセス分析"
        Me.chkStep3.UseVisualStyleBackColor = True
        '
        'chkStep2
        '
        Me.chkStep2.AutoSize = True
        Me.chkStep2.Checked = True
        Me.chkStep2.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStep2.Enabled = False
        Me.chkStep2.Location = New System.Drawing.Point(108, 72)
        Me.chkStep2.Name = "chkStep2"
        Me.chkStep2.Size = New System.Drawing.Size(125, 16)
        Me.chkStep2.TabIndex = 7
        Me.chkStep2.Text = "Step2 クエリーの抽出"
        Me.chkStep2.UseVisualStyleBackColor = True
        '
        'chkStep1
        '
        Me.chkStep1.AutoSize = True
        Me.chkStep1.Checked = True
        Me.chkStep1.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStep1.Enabled = False
        Me.chkStep1.Location = New System.Drawing.Point(108, 50)
        Me.chkStep1.Name = "chkStep1"
        Me.chkStep1.Size = New System.Drawing.Size(136, 16)
        Me.chkStep1.TabIndex = 6
        Me.chkStep1.Text = "Step1 動的SQLの抽出"
        Me.chkStep1.UseVisualStyleBackColor = True
        '
        'chkProcAll
        '
        Me.chkProcAll.AutoSize = True
        Me.chkProcAll.Checked = True
        Me.chkProcAll.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkProcAll.Location = New System.Drawing.Point(22, 28)
        Me.chkProcAll.Name = "chkProcAll"
        Me.chkProcAll.Size = New System.Drawing.Size(45, 16)
        Me.chkProcAll.TabIndex = 4
        Me.chkProcAll.Text = "全て"
        Me.chkProcAll.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Location = New System.Drawing.Point(868, 221)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(112, 25)
        Me.btnClose.TabIndex = 13
        Me.btnClose.Text = "閉じる"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'pnlControl
        '
        Me.pnlControl.Controls.Add(Me.btnSelectDestFolder)
        Me.pnlControl.Controls.Add(Me.txtDestPath)
        Me.pnlControl.Controls.Add(Me.Label3)
        Me.pnlControl.Controls.Add(Me.txtSourcePath)
        Me.pnlControl.Controls.Add(Me.chkReferenceCond)
        Me.pnlControl.Controls.Add(Me.Label1)
        Me.pnlControl.Controls.Add(Me.btnClose)
        Me.pnlControl.Controls.Add(Me.btnSelectSourceFolder)
        Me.pnlControl.Controls.Add(Me.GroupBox1)
        Me.pnlControl.Controls.Add(Me.chkReference)
        Me.pnlControl.Controls.Add(Me.Label4)
        Me.pnlControl.Controls.Add(Me.btnAnalyzeCRUD)
        Me.pnlControl.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlControl.Location = New System.Drawing.Point(0, 0)
        Me.pnlControl.Name = "pnlControl"
        Me.pnlControl.Size = New System.Drawing.Size(1008, 264)
        Me.pnlControl.TabIndex = 11
        '
        'btnSelectDestFolder
        '
        Me.btnSelectDestFolder.Location = New System.Drawing.Point(950, 71)
        Me.btnSelectDestFolder.Name = "btnSelectDestFolder"
        Me.btnSelectDestFolder.Size = New System.Drawing.Size(30, 25)
        Me.btnSelectDestFolder.TabIndex = 3
        Me.btnSelectDestFolder.Text = "…"
        Me.btnSelectDestFolder.UseVisualStyleBackColor = True
        '
        'txtDestPath
        '
        Me.txtDestPath.Location = New System.Drawing.Point(125, 74)
        Me.txtDestPath.Name = "txtDestPath"
        Me.txtDestPath.Size = New System.Drawing.Size(819, 19)
        Me.txtDestPath.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label3.Location = New System.Drawing.Point(7, 72)
        Me.Label3.Name = "Label3"
        Me.Label3.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.Label3.Size = New System.Drawing.Size(112, 22)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "解析結果フォルダ"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtSourcePath
        '
        Me.txtSourcePath.Location = New System.Drawing.Point(125, 41)
        Me.txtSourcePath.Name = "txtSourcePath"
        Me.txtSourcePath.Size = New System.Drawing.Size(819, 19)
        Me.txtSourcePath.TabIndex = 0
        '
        'chkReferenceCond
        '
        Me.chkReferenceCond.AutoSize = True
        Me.chkReferenceCond.Location = New System.Drawing.Point(545, 171)
        Me.chkReferenceCond.Name = "chkReferenceCond"
        Me.chkReferenceCond.Size = New System.Drawing.Size(203, 16)
        Me.chkReferenceCond.TabIndex = 11
        Me.chkReferenceCond.Text = "テーブルの必須参照条件チェックを行う"
        Me.chkReferenceCond.UseVisualStyleBackColor = True
        Me.chkReferenceCond.Visible = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(7, 9)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(286, 12)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "■ソースフォルダを指定して、SQLのCRUD解析を行ないます"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'chkStep0
        '
        Me.chkStep0.AutoSize = True
        Me.chkStep0.Checked = True
        Me.chkStep0.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkStep0.Enabled = False
        Me.chkStep0.Location = New System.Drawing.Point(108, 28)
        Me.chkStep0.Name = "chkStep0"
        Me.chkStep0.Size = New System.Drawing.Size(247, 16)
        Me.chkStep0.TabIndex = 5
        Me.chkStep0.Text = "Step0 ソースファイルを解析結果フォルダにコピー"
        Me.chkStep0.UseVisualStyleBackColor = True
        '
        'frmMakeCRUD
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(1008, 562)
        Me.Controls.Add(Me.pnlControl)
        Me.Controls.Add(Me.grpState)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(1024, 600)
        Me.MinimumSize = New System.Drawing.Size(1024, 600)
        Me.Name = "frmMakeCRUD"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "CRUD解析"
        Me.grpState.ResumeLayout(False)
        Me.grpState.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.pnlControl.ResumeLayout(False)
        Me.pnlControl.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnSelectSourceFolder As System.Windows.Forms.Button
    Friend WithEvents chkReference As System.Windows.Forms.CheckBox
    Friend WithEvents btnAnalyzeCRUD As System.Windows.Forms.Button
    Friend WithEvents grpState As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtLog As System.Windows.Forms.TextBox
    Friend WithEvents lblPhase As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents chkStep4 As System.Windows.Forms.CheckBox
    Friend WithEvents chkStep3 As System.Windows.Forms.CheckBox
    Friend WithEvents chkStep2 As System.Windows.Forms.CheckBox
    Friend WithEvents chkStep1 As System.Windows.Forms.CheckBox
    Friend WithEvents chkProcAll As System.Windows.Forms.CheckBox
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents pnlControl As System.Windows.Forms.Panel
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents chkReferenceCond As System.Windows.Forms.CheckBox
    Friend WithEvents cmdAnalyzeQuery As System.Windows.Forms.Button
    Friend WithEvents cmdRunHidemaru As System.Windows.Forms.Button
    Friend WithEvents btnSelectDestFolder As System.Windows.Forms.Button
    Friend WithEvents txtDestPath As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtSourcePath As System.Windows.Forms.TextBox
    Friend WithEvents chkStep0 As System.Windows.Forms.CheckBox
End Class
