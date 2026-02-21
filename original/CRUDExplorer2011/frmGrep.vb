Imports System.Text.RegularExpressions

Public Class frmGrep
    Public Enum enmGrepScope
        ThisFile
        ThisProgram
        AllPrograms
    End Enum

    Private strSourcePath As String
    Private fso As New Scripting.FileSystemObject
    Private bolSortOrderAsc As Boolean = False
    Private objForm As New frmAnalyzeQuery
    Private strGrepWord As String

    Public Sub ShowForm(ByVal strPath As String, ByVal strSourceFile As String, ByVal strWord As String, ByVal GrepScope As enmGrepScope)
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
        ExecGrep(strPath, strSourceFile, strWord, GrepScope)
        System.Windows.Forms.Cursor.Current = Nothing
    End Sub

    Public Sub ExecGrep(ByVal strPath As String, ByVal strSourceFile As String, ByVal strWord As String, ByVal GrepScope As enmGrepScope)
        Dim strSource As String = ""
        Dim intCnt As Integer
        Dim bolAllDecheck As Boolean = True
        Dim strProgramId As String = GetProgramId(strSourceFile)
        Dim strKey As String
        Dim strCols As String()

        strSourcePath = strPath
        strGrepWord = Trim(strWord)
        If Not fso.FolderExists(strSourcePath & "\querys") Then Exit Sub

        Select Case GrepScope
            Case enmGrepScope.ThisFile
                Me.Text = strSourceFile & " からGrep [" & strGrepWord & "]"
            Case enmGrepScope.ThisProgram
                Me.Text = strProgramId & " からGrep [" & strGrepWord & "]"
            Case enmGrepScope.AllPrograms
                Me.Text = "全てのプログラムからGrep [" & strGrepWord & "]"
        End Select


        For Each strKey In dctQueryList.Keys
            strCols = Split(strKey, vbTab)
            Select Case GrepScope
                Case enmGrepScope.ThisFile
                    If UCase(strSourceFile) = UCase(strCols(0)) Then
                        strSource &= strCols(0) & vbTab & strCols(1) & vbTab & dctQueryList(strKey) & vbCrLf
                    End If
                Case enmGrepScope.ThisProgram
                    If UCase(strProgramId) = UCase(GetProgramId(strCols(0))) Then
                        strSource &= strCols(0) & vbTab & strCols(1) & vbTab & dctQueryList(strKey) & vbCrLf
                    End If
                Case enmGrepScope.AllPrograms
                    strSource &= strCols(0) & vbTab & strCols(1) & vbTab & dctQueryList(strKey) & vbCrLf
            End Select
        Next

        If strSource = "" Then Exit Sub

        Dim objMatches As MatchCollection
        Dim strSearch As String
        strSearch = "^(.*)\t([0-9]+)\t(.*)(" & EscapeRegular(strGrepWord) & ")(.*)\n"
        Try
            objMatches = Regex.Matches(strSource, strSearch, RegexOptions.IgnoreCase Or RegexOptions.Multiline)
        Catch
            'System.Windows.Forms.Cursor.Current = Nothing
            Exit Sub
        End Try

        lstQuerys.Items.Clear()

        For intCnt = 0 To objMatches.Count - 1
            Dim strArr As String() = Split(objMatches(intCnt).Groups(3).Value & objMatches(intCnt).Groups(4).Value & objMatches(intCnt).Groups(5).Value, vbTab)

            Dim objItem As ListViewItem = lstQuerys.Items.Add(objMatches(intCnt).Groups(1).Value)
            '            Dim objItem As ListViewItem = lstQuerys.Items.Add(GetRight(objMatches(intCnt).Groups(3).Value, 20) & objMatches(intCnt).Groups(4).Value & Mid(objMatches(intCnt).Groups(5).Value, 1, 20))
            objItem.SubItems.Add(objMatches(intCnt).Groups(2).Value)
            Dim strSelectedProgramId As String = GetProgramId(objMatches(intCnt).Groups(1).Value)
            Dim strProgramName As String = GetDictValue(dctProgramName, strSelectedProgramId)
            If strProgramName <> "" Then
                objItem.SubItems.Add(strProgramName & "(" & strSelectedProgramId & ")")
            Else
                objItem.SubItems.Add(strSelectedProgramId)
            End If
            If UBound(strArr) > 0 Then
                objItem.SubItems.Add(strArr(1))
            Else
                objItem.SubItems.Add("")
            End If
            '         objItem.SubItems.Add(objMatches(intCnt).Groups(1).Value)
            objItem.SubItems.Add(GetRight(objMatches(intCnt).Groups(3).Value, 20) & objMatches(intCnt).Groups(4).Value & Mid(objMatches(intCnt).Groups(5).Value, 1, 20))
        Next

        If lstQuerys.Items.Count = 0 Then
            MsgBox("[" & strWord & "]がみつかりません")
            Exit Sub
        End If

        lstQuerys.Items(0).Selected = True
        lstQuerys.Focus()

        'リスト、フィームサイズの自動調整
        lstQuerys.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        Dim intListWidth As Integer = 0
        Dim intCol As Integer
        For intCol = 0 To lstQuerys.Columns.Count - 1
            intListWidth += lstQuerys.Columns(intCol).Width
        Next
        If intListWidth + 40 <= 1024 Then
            Me.Width = intListWidth + 40
        Else
            Me.Width = 1024
        End If

        Me.Show()
    End Sub

    Private Sub cmdRunHidemaru_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRunHidemaru.Click
        If lstQuerys.SelectedItems.Count > 0 Then
            RunTextEditor(strSourcePath & "\" & lstQuerys.SelectedItems(0).SubItems(0).Text, lstQuerys.SelectedItems(0).SubItems(1).Text, EscapeRegular(strGrepWord))
        End If
    End Sub

    Private Sub cmdAnalyzeQuery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAnalyzeQuery.Click
        If lstQuerys.SelectedItems.Count > 0 Then
            If objForm.IsDisposed Then objForm = New frmAnalyzeQuery
            objForm.ShowForm(strSourcePath, lstQuerys.SelectedItems(0).SubItems(0).Text, lstQuerys.SelectedItems(0).SubItems(1).Text, "", "", EscapeRegular(strGrepWord))
        End If
    End Sub

    Private Sub cmdAnalyzeQueryNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAnalyzeQueryNew.Click
        If lstQuerys.SelectedItems.Count > 0 Then
            Dim objFormNew As New frmAnalyzeQuery
            objFormNew.ShowForm(strSourcePath, lstQuerys.SelectedItems(0).SubItems(0).Text, lstQuerys.SelectedItems(0).SubItems(1).Text, "", "", EscapeRegular(strGrepWord))
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

    Private Sub lstQuerys_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lstQuerys.KeyDown
        If e.KeyCode = Keys.Enter Then lstQuerys_DoubleClick(Nothing, Nothing)
    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

    Private Sub frmGrep_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitCommonContextMenu(lstQuerys, strSourcePath)
    End Sub
End Class