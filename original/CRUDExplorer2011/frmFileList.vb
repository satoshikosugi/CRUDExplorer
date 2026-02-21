Imports System.Text.RegularExpressions

Public Class frmFileList
    Private bolInit As Boolean = True
    Private strSourcePath As String
    Private fso As New Scripting.FileSystemObject
    Private bolSortOrderAsc As Boolean = False
    Private objForm As New frmAnalyzeQuery

    Public Sub ShowForm(ByVal strPath As String)
        strSourcePath = strPath

        lstFiles.Items.Clear()

        Dim strKey As String

        For Each strKey In dctFiles.Keys
            Dim strSourceFileName As String = GetDictValue(dctFiles, strKey)
            Dim strProgramId As String = GetProgramId(strSourceFileName)
            If DictExists(dctProgramName, strProgramId) Then strProgramId &= "(" & GetDictValue(dctProgramName, strProgramId) & ")"
            Dim objItem As ListViewItem = lstFiles.Items.Add(strProgramId)
            objItem.SubItems.Add(strSourceFileName)
        Next
        If lstFiles.Items.Count > 0 Then
            lstQuerys.ListViewItemSorter = New ListViewItemComparerQueryList(1, 1)
            lstFiles.Items(0).Selected = True
        End If

        Me.Show()
    End Sub

    Private Sub lstFiles_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstFiles.Click
        Dim strKey As String

        If bolInit Then Exit Sub
        If lstFiles.SelectedItems.Count = 0 Then Exit Sub

        lstQuerys.SuspendLayout()
        lstQuerys.Items.Clear()
        For Each strKey In dctQueryList.Keys
            Dim strCols As String() = Split(strKey, vbTab)
            Dim strFileName As String = strCols(0)
            Dim strLineNo As String = strCols(1)
            If lstFiles.SelectedItems(0).SubItems(1).Text = strCols(0) Then
                Dim strArr2 As String() = Split(dctQueryList(strKey), vbTab)
                Dim strQuery As String = strArr2(0)
                Dim strFuncProcName As String = GetStringArrayByIndex(strArr2, 1)
                Dim objItem As ListViewItem = lstQuerys.Items.Add(strFileName)
                objItem.SubItems.Add(strLineNo)
                objItem.SubItems.Add(strFuncProcName)
                objItem.SubItems.Add(Mid(strQuery, 1, 40) & "...")
            End If
        Next

        If lstQuerys.Items.Count > 0 Then
            lstQuerys.ListViewItemSorter = New ListViewItemComparerQueryList(1, 1)
            lstQuerys.Items(0).Selected = True
        End If
        lstQuerys.ResumeLayout()
    End Sub

    Private Sub lstFiles_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstFiles.SelectedIndexChanged
        'Dim strKey As String

        'If bolInit Then Exit Sub
        'If lstFiles.SelectedItems.Count = 0 Then Exit Sub

        'lstQuerys.SuspendLayout()
        'lstQuerys.Items.Clear()
        'For Each strKey In dctQueryList.Keys
        '    Dim strCols As String() = Split(strKey, vbTab)
        '    Dim strFileName As String = strCols(0)
        '    Dim strLineNo As String = strCols(1)
        '    If lstFiles.SelectedItems(0).SubItems(1).Text = strCols(0) Then
        '        Dim strArr2 As String() = Split(dctQueryList(strKey), vbTab)
        '        Dim strQuery As String = strArr2(0)
        '        Dim strFuncProcName As String = GetStringArrayByIndex(strArr2, 1)
        '        Dim objItem As ListViewItem = lstQuerys.Items.Add(strFileName)
        '        objItem.SubItems.Add(strLineNo)
        '        objItem.SubItems.Add(strFuncProcName)
        '        objItem.SubItems.Add(Mid(strQuery, 1, 40) & "...")
        '    End If
        'Next

        'If lstQuerys.Items.Count > 0 Then
        '    lstQuerys.ListViewItemSorter = New ListViewItemComparerQueryList(1, 1)
        '    lstQuerys.Items(0).Selected = True
        'End If
        'lstQuerys.ResumeLayout()

    End Sub

    Private Sub cmdRunHidemaru_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRunHidemaru.Click
        If lstQuerys.SelectedItems.Count > 0 Then
            RunTextEditor(strSourcePath & "\" & lstQuerys.SelectedItems(0).SubItems(0).Text, lstQuerys.SelectedItems(0).SubItems(1).Text, txtSearch.Text)
        End If
    End Sub

    Private Sub cmdAnalyzeQuery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAnalyzeQuery.Click
        If lstQuerys.SelectedItems.Count > 0 Then
            If objForm.IsDisposed Then objForm = New frmAnalyzeQuery
            objForm.ShowForm(strSourcePath, lstQuerys.SelectedItems(0).SubItems(0).Text, lstQuerys.SelectedItems(0).SubItems(1).Text, "", "", txtSearch.Text)
        End If
    End Sub

    Private Sub cmdAnalyzeQueryNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAnalyzeQueryNew.Click
        If lstQuerys.SelectedItems.Count > 0 Then
            Dim objFormNew As New frmAnalyzeQuery
            objFormNew.ShowForm(strSourcePath, lstQuerys.SelectedItems(0).SubItems(0).Text, lstQuerys.SelectedItems(0).SubItems(1).Text, "", "", txtSearch.Text)
        End If
    End Sub

    Private Sub lstQuerys_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstQuerys.DoubleClick
        Select Case objSettings.ListDblClickMode
            Case clsSettings.enmListDblClickMode.ExecTextEditor
                cmdRunHidemaru_Click(Nothing, Nothing)
            Case clsSettings.enmListDblClickMode.AnalyzeQuery
                cmdAnalyzeQuery_Click(Nothing, Nothing)
        End Select

    End Sub

    Private Sub lstQuerys_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles lstQuerys.ColumnClick
        Dim lngOrder As Long

        On Error Resume Next
        bolSortOrderAsc = Not bolSortOrderAsc
        If bolSortOrderAsc Then
            lngOrder = 1
        Else
            lngOrder = -1
        End If

        lstQuerys.ListViewItemSorter = New ListViewItemComparerQueryList(e.Column, lngOrder)

    End Sub

    Private Sub lstFiles_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lstFiles.KeyDown
        If e.KeyCode = Keys.Enter Then lstQuerys.Focus()
    End Sub

    Private Sub lstQuerys_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lstQuerys.KeyDown
        If e.KeyCode = Keys.Enter Then lstQuerys_DoubleClick(Nothing, Nothing)
    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

    Private Sub btnGrep_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGrep.Click
        Dim strSource As String = ""
        Dim intCnt As Integer
        Dim bolAllDecheck As Boolean = True

        txtSearch.Focus()
        If Trim(txtSearch.Text) = "" Then Exit Sub

        If Not fso.FolderExists(strSourcePath & "\querys") Then Exit Sub

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        '全部チェックが外れているか
        For intCnt = 0 To lstFiles.Items.Count - 1
            If lstFiles.Items(intCnt).Checked = True Then
                bolAllDecheck = False
                Exit For
            End If
        Next

        For intCnt = 0 To lstFiles.Items.Count - 1
            If bolAllDecheck OrElse lstFiles.Items(intCnt).Checked Then
                If fso.FileExists(strSourcePath & "\querys\" & lstFiles.Items(intCnt).SubItems(1).Text & ".query") Then
                    Dim t As Scripting.TextStream = fso.OpenTextFile(strSourcePath & "\querys\" & lstFiles.Items(intCnt).SubItems(1).Text & ".query")
                    If Not t.AtEndOfStream Then
                        Dim strLines As String() = Split(t.ReadAll, vbCrLf)
                        t.Close()
                        Dim intLine As Integer
                        For intLine = LBound(strLines) To UBound(strLines)
                            If strLines(intLine) <> "" Then strSource &= lstFiles.Items(intCnt).SubItems(1).Text & vbTab & strLines(intLine) & vbCrLf
                        Next
                    End If
                End If
            End If
        Next

        Dim objMatches As MatchCollection
        Dim strSearch As String
        If chkRegular.Checked Then
            strSearch = "^(.*)\t([0-9]+)\t(.*)(" & txtSearch.Text & ")(.*)\n"
        Else
            strSearch = "^(.*)\t([0-9]+)\t(.*)(" & EscapeRegular(txtSearch.Text) & ")(.*)\n"
        End If
        Try
            objMatches = Regex.Matches(strSource, strSearch, RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Catch
            MsgBox("正規表現が不正です。")
            System.Windows.Forms.Cursor.Current = Nothing
            Exit Sub
        End Try

        lstQuerys.Items.Clear()

        For intCnt = 0 To objMatches.Count - 1
            Dim strArr As String() = Split(objMatches(intCnt).Groups(3).Value & objMatches(intCnt).Groups(4).Value & objMatches(intCnt).Groups(5).Value, vbTab)

            Dim objItem As ListViewItem = lstQuerys.Items.Add(objMatches(intCnt).Groups(1).Value)
            objItem.SubItems.Add(objMatches(intCnt).Groups(2).Value)
            If UBound(strArr) > 0 Then
                objItem.SubItems.Add(strArr(1))
            Else
                objItem.SubItems.Add("")
            End If
            objItem.SubItems.Add(GetRight(objMatches(intCnt).Groups(3).Value, 20) & objMatches(intCnt).Groups(4).Value & Mid(objMatches(intCnt).Groups(5).Value, 1, 20))
        Next
        If lstQuerys.Items.Count > 0 Then
            lstQuerys.Items(0).Selected = True
            lstQuerys.Focus()
        Else
            txtSearch.Focus()
        End If
        System.Windows.Forms.Cursor.Current = Nothing
    End Sub

    Private Sub txtSearch_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtSearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            btnGrep_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub frmFileList_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        If bolInit Then
            txtSearch.Focus()
            bolInit = False
        End If
    End Sub

    Private Sub 全選択ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 全選択ToolStripMenuItem.Click
        ListItemCheckAll(lstFiles, True)
    End Sub

    Private Sub 全解除ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 全解除ToolStripMenuItem.Click
        ListItemCheckAll(lstFiles, False)
    End Sub

    Private Sub frmFileList_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        objSettings.SaveFormSize(Me)
    End Sub

    Private Sub frmFileList_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objSettings.LoadFormSize(Me)
        InitCommonContextMenu(lstFiles, strSourcePath)
        InitCommonContextMenu(lstQuerys, strSourcePath)
    End Sub
End Class