Public Class frmSearch
    Public bolAccept As Boolean

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        bolAccept = False
        Me.Close()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        bolAccept = True
        Me.Close()
    End Sub

    Private Sub frmSearch_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        If txtSearch.Text <> "" Then
            txtSearch.SelectionStart = 0
            txtSearch.SelectionLength = txtSearch.Text.Length
        End If
    End Sub
End Class