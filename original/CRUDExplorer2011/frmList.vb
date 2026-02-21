Public Class frmList
    Private bolSortOrderAsc As Boolean = False
    Public strSourcePath As String
    Public strFileName As String

    Public Sub ShowForm(ByVal strTitle As String, ByVal strData As String, ByVal strPath As String, ByVal strFile As String)
        Dim strLiens As String() = Split(strData, vbCrLf)
        Dim intLine As Integer
        Dim intCol As Integer
        Dim intTableIdIdx As Integer = -1

        Me.Text = strTitle
        strSourcePath = strPath
        strFileName = strFile
        For intLine = 0 To UBound(strLiens) - 1
            Dim strCols As String() = Split(strLiens(intLine), vbTab)
            If intLine = 0 Then
                For intCol = 0 To UBound(strCols)
                    lstList.Columns.Add(strCols(intCol))
                    If strCols(intCol) = "ÉeÅ[ÉuÉãñº" Then intTableIdIdx = intCol
                Next
            Else
                Dim objItem As ListViewItem = lstList.Items.Add(strCols(0))
                For intCol = 1 To UBound(strCols)
                    objItem.SubItems.Add(strCols(intCol))
                Next
            End If
        Next

        lstList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        Dim intListWidth As Integer = 0
        For intCol = 0 To lstList.Columns.Count - 1
            intListWidth += lstList.Columns(intCol).Width
        Next
        If intListWidth + 40 <= 1024 Then
            Me.Width = intListWidth + 40
        Else
            Me.Width = 1024
        End If
        InitCommonContextMenu(lstList, strSourcePath, strFileName, intTableIdIdx)

        Me.Show()
        lstList.Sorting = SortOrder.Ascending
        lstList.Sort()
    End Sub

    Private Sub lstList_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles lstList.ColumnClick
        Dim lngOrder As Long

        On Error Resume Next
        bolSortOrderAsc = Not bolSortOrderAsc
        If bolSortOrderAsc Then
            lngOrder = 1
        Else
            lngOrder = -1
        End If

        lstList.ListViewItemSorter = New ListViewItemComparerCommonList(e.Column, lngOrder)

    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

    Private Sub frmList_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        lstList.Focus()
    End Sub
End Class