Public Class frmStartup

    Private Sub frmStartup_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lblVersion.Text = Application.ProductVersion

        If bolDemoFlag Then
            lblDemo.Visible = True
        Else
            lblDemo.Visible = False
        End If
    End Sub
End Class