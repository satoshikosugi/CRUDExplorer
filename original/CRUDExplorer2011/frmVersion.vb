Imports System.Text.RegularExpressions

Public Class frmVersion

    Private Sub frmVersion_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblVersion.Text = Application.ProductVersion
        If Not bolDemoFlag Then
            lblDemo.Visible = False
            lblAccept.Visible = True
            btnAuthentication.Enabled = False
        End If
        txtLicenseKey.Text = objSettings.LicenseKey
        txtEMailAddr.Text = objSettings.EMailAddr
    End Sub

    Private Sub btnAuthentication_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAuthentication.Click
        If txtLicenseKey.Text.Length <> 16 Then
            MsgBox("ライセンスキーに誤りがあります。")
            txtLicenseKey.Focus()
            Exit Sub
        End If

        If txtEMailAddr.Text.Length = 0 Then
            MsgBox("登録メールアドレスを指定してください。")
            txtEMailAddr.Focus()
            Exit Sub
        End If

        If Not RegMatch(txtEMailAddr.Text, "^([a-zA-Z0-9])+([a-zA-Z0-9\._-])*@([a-zA-Z0-9_-])+([a-zA-Z0-9\._-]+)+$", RegexOptions.IgnoreCase) Then
            MsgBox("登録メールアドレスの形式に誤りがあります。")
            txtEMailAddr.Focus()
            Exit Sub
        End If

        Dim strWork As String = DeExchageKey(txtLicenseKey.Text)
        Dim strNo As String = Mid(strWork, 1, 8)
        Dim strEnc = Encrypt(strNo)
        If strWork = (strNo & strEnc) Then
            MsgBox("ライセンス認証に成功しました。" & vbCrLf & "ご登録ありがとうございます！")
            bolDemoFlag = False
            objSettings.LicenseKey = txtLicenseKey.Text
            objSettings.EMailAddr = txtEMailAddr.Text
            objSettings.SaveSettings()
            Me.Close()
        Else
            MsgBox("ライセンスキーに誤りがあります。")
            txtLicenseKey.Focus()
        End If


    End Sub
End Class