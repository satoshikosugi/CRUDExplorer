<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmVersion
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmVersion))
        Me.lblVersion = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.cmdClose = New System.Windows.Forms.Button
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblDemo = New System.Windows.Forms.Label
        Me.lblCopyright = New System.Windows.Forms.Label
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnAuthentication = New System.Windows.Forms.Button
        Me.txtEMailAddr = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.txtLicenseKey = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.lblAccept = New System.Windows.Forms.Label
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblVersion
        '
        Me.lblVersion.AutoSize = True
        Me.lblVersion.Location = New System.Drawing.Point(298, 53)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(56, 12)
        Me.lblVersion.TabIndex = 10
        Me.lblVersion.Text = "バージョン："
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(236, 53)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(56, 12)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "バージョン："
        '
        'cmdClose
        '
        Me.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdClose.Location = New System.Drawing.Point(478, 265)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(94, 25)
        Me.cmdClose.TabIndex = 11
        Me.cmdClose.Text = "閉じる(&C)"
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(236, 20)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(79, 12)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "CRUD Exploer"
        '
        'lblDemo
        '
        Me.lblDemo.AutoSize = True
        Me.lblDemo.ForeColor = System.Drawing.Color.Red
        Me.lblDemo.Location = New System.Drawing.Point(321, 20)
        Me.lblDemo.Name = "lblDemo"
        Me.lblDemo.Size = New System.Drawing.Size(41, 12)
        Me.lblDemo.TabIndex = 13
        Me.lblDemo.Text = "評価版"
        '
        'lblCopyright
        '
        Me.lblCopyright.AutoSize = True
        Me.lblCopyright.Location = New System.Drawing.Point(236, 85)
        Me.lblCopyright.Name = "lblCopyright"
        Me.lblCopyright.Size = New System.Drawing.Size(131, 12)
        Me.lblCopyright.TabIndex = 14
        Me.lblCopyright.Text = "Copyright (C) 2011  SEK"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(-1, 20)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(200, 200)
        Me.PictureBox1.TabIndex = 15
        Me.PictureBox1.TabStop = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lblAccept)
        Me.GroupBox1.Controls.Add(Me.btnAuthentication)
        Me.GroupBox1.Controls.Add(Me.txtEMailAddr)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.txtLicenseKey)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Location = New System.Drawing.Point(222, 122)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(350, 131)
        Me.GroupBox1.TabIndex = 16
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "ライセンス登録"
        '
        'btnAuthentication
        '
        Me.btnAuthentication.Location = New System.Drawing.Point(250, 95)
        Me.btnAuthentication.Name = "btnAuthentication"
        Me.btnAuthentication.Size = New System.Drawing.Size(94, 25)
        Me.btnAuthentication.TabIndex = 17
        Me.btnAuthentication.Text = "ライセンス認証"
        Me.btnAuthentication.UseVisualStyleBackColor = True
        '
        'txtEMailAddr
        '
        Me.txtEMailAddr.Location = New System.Drawing.Point(128, 60)
        Me.txtEMailAddr.Name = "txtEMailAddr"
        Me.txtEMailAddr.Size = New System.Drawing.Size(216, 19)
        Me.txtEMailAddr.TabIndex = 16
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(14, 63)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(99, 12)
        Me.Label4.TabIndex = 15
        Me.Label4.Text = "登録メールアドレス："
        '
        'txtLicenseKey
        '
        Me.txtLicenseKey.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtLicenseKey.Location = New System.Drawing.Point(128, 20)
        Me.txtLicenseKey.Name = "txtLicenseKey"
        Me.txtLicenseKey.Size = New System.Drawing.Size(216, 22)
        Me.txtLicenseKey.TabIndex = 14
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(14, 26)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(108, 12)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "ライセンスキー(16桁)："
        '
        'lblAccept
        '
        Me.lblAccept.AutoSize = True
        Me.lblAccept.ForeColor = System.Drawing.Color.Blue
        Me.lblAccept.Location = New System.Drawing.Point(14, 101)
        Me.lblAccept.Name = "lblAccept"
        Me.lblAccept.Size = New System.Drawing.Size(52, 12)
        Me.lblAccept.TabIndex = 18
        Me.lblAccept.Text = "認証済み"
        Me.lblAccept.Visible = False
        '
        'frmVersion
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.cmdClose
        Me.ClientSize = New System.Drawing.Size(584, 302)
        Me.ControlBox = False
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.lblCopyright)
        Me.Controls.Add(Me.lblDemo)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cmdClose)
        Me.Controls.Add(Me.lblVersion)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximumSize = New System.Drawing.Size(600, 340)
        Me.MinimumSize = New System.Drawing.Size(600, 340)
        Me.Name = "frmVersion"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "バージョン情報"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblDemo As System.Windows.Forms.Label
    Friend WithEvents lblCopyright As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents txtLicenseKey As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnAuthentication As System.Windows.Forms.Button
    Friend WithEvents txtEMailAddr As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblAccept As System.Windows.Forms.Label
End Class
