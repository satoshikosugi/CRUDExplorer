Public Class frmFilter
    Private strSourcePath As String
    Private bolInit As Boolean = True
    Public bolCancel As Boolean = True

    Public Sub ShowForm(ByVal strPath As String)
        Dim strKey As String
        Dim objItem As ListViewItem

        strSourcePath = strPath

        If Not bolInit Then
            Me.ShowDialog()
            Exit Sub
        End If

        InitCommonContextMenu(lstProgram, strSourcePath)
        InitCommonContextMenu(lstTable, strSourcePath, "", 0)

        cmbProgramAccess.Items.Add("")
        cmbTableAccess.Items.Add("")
        For Each strKey In dctCRUDProgram.Keys
            objItem = lstProgram.Items.Add(strKey)
            objItem.SubItems.Add(dctCRUDProgram(strKey))
            cmbProgramAccess.Items.Add(strKey & " (" & dctCRUDProgram(strKey) & ")")
        Next

        For Each strKey In dctCRUDTable.Keys
            objItem = lstTable.Items.Add(strKey)
            objItem.SubItems.Add(GetLogicalName(strKey))
            '            cmbTableAccess.Items.Add(strKey & " (" & dctCRUDTable(strKey) & ")")
            cmbTableAccess.Items.Add(strKey & " (" & GetLogicalName(strKey) & ")")
        Next

        bolInit = False

        Me.ShowDialog()
    End Sub

    'Private Sub frmFilter_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    '    Dim strKey As String
    '    Dim objItem As ListViewItem

    '    If Not bolInit Then Exit Sub

    '    InitCommonContextMenu(lstProgram, Str)

    '    cmbProgramAccess.Items.Add("")
    '    cmbTableAccess.Items.Add("")
    '    For Each strKey In dctCRUDProgram.Keys
    '        objItem = lstProgram.Items.Add(strKey)
    '        objItem.SubItems.Add(dctCRUDProgram(strKey))
    '        cmbProgramAccess.Items.Add(strKey & " (" & dctCRUDProgram(strKey) & ")")
    '    Next

    '    For Each strKey In dctCRUDTable.Keys
    '        objItem = lstTable.Items.Add(strKey)
    '        objItem.SubItems.Add(GetLogicalName(strKey))
    '        '            cmbTableAccess.Items.Add(strKey & " (" & dctCRUDTable(strKey) & ")")
    '        cmbTableAccess.Items.Add(strKey & " (" & GetLogicalName(strKey) & ")")
    '    Next

    '    bolInit = False
    'End Sub

    Private Sub 全選択ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 全選択ToolStripMenuItem.Click
        ListItemCheckAll(lstProgram, True)
    End Sub

    Private Sub 全解除ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 全解除ToolStripMenuItem.Click
        ListItemCheckAll(lstProgram, False)
    End Sub

    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
        ListItemCheckAll(lstTable, True)
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        ListItemCheckAll(lstTable, False)
    End Sub

    Private Sub cmbProgramAccess_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbProgramAccess.SelectedIndexChanged
        If cmbProgramAccess.Text <> "" Then
            ListItemCheckAll(lstTable, False)
        End If
    End Sub

    Private Sub cmbTableAccess_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbTableAccess.SelectedIndexChanged
        If cmbTableAccess.Text <> "" Then
            ListItemCheckAll(lstProgram, False)
        End If
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        bolCancel = True
        Me.Close()
    End Sub

    Private Sub cmdApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdApply.Click
        bolCancel = False
        Me.Close()
    End Sub

    Private Sub btnAllClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllClear.Click
        Dim intCnt As Integer

        For intCnt = 0 To lstProgram.Items.Count - 1
            lstProgram.Items(intCnt).Checked = False
        Next
        For intCnt = 0 To lstTable.Items.Count - 1
            lstTable.Items(intCnt).Checked = False
        Next
        cmbProgramAccess.Text = ""
        cmbTableAccess.Text = ""
    End Sub
End Class