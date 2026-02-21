Imports System.Text.RegularExpressions

Public Class frmCRUDSearch
    Private strSourcePath As String
    Private fso As New Scripting.FileSystemObject
    Private objForm As New frmAnalyzeQuery
    Private bolSortOrderAsc As Boolean = False
    Private strSearch As String
    Private strTableName As String
    Private strColumnName As String

    Public Sub ShowForm(ByVal strPath As String, ByVal strTN As String, Optional ByVal strCN As String = "")
        strSourcePath = strPath

        Dim t As Scripting.TextStream
        Dim strFileName As String
        Dim strSource As String
        Dim strRegular As String

        lstQuerys.Items.Clear()

        strSourcePath = strPath
        strTableName = strTN
        strColumnName = strCN

        If strColumnName <> "" Then
            strFileName = strSourcePath & "\querys\CRUDColumns.tsv"
            strRegular = "^(.*)\t(.*)\t(.*)\t(" & EscapeRegular(strTableName & "." & strColumnName) & ")\t(.*)\t(.*)\t(.*)\n"
            strSearch = strColumnName
            Me.Text = "CRUD一覧 テーブル[" & strTableName & "] カラム[" & strColumnName & "]"
        Else
            strFileName = strSourcePath & "\querys\CRUD.tsv"
            strRegular = "^(.*)\t(.*)\t(.*)\t(" & EscapeRegular(strTableName) & ")\t(.*)\t(.*)\t(.*)\n"
            strSearch = strTableName
            Me.Text = "CRUD一覧 テーブル[" & strTableName & "]"
        End If
        If Not fso.FileExists(strFileName) Then
            MsgBox(strFileName & vbCrLf & "がみつかりません")
            Me.Close()
            Exit Sub
        End If
        t = fso.OpenTextFile(strFileName)
        strSource = t.ReadAll
        t.Close()

        Dim objMatches As MatchCollection

        objMatches = Regex.Matches(strSource, strRegular, RegexOptions.Multiline Or RegexOptions.IgnoreCase)

        Dim intCnt As Integer

        If objMatches.Count = 0 Then
            MsgBox("テーブル[" & strTN & "] " & "カラム[" & strCN & "]にアクセスしている処理はみつかりませんでした。")
            Exit Sub
        End If

        For intCnt = 0 To objMatches.Count - 1
            Dim strProgram As String = ""
            strProgram = objMatches(intCnt).Groups(2).Value
            If DictExists(dctProgramName, strProgram) Then strProgram &= "(" & GetDictValue(dctProgramName, strProgram) & ")"
            Dim objItem As ListViewItem = lstQuerys.Items.Add(strProgram)   'プログラム
            objItem.SubItems.Add(objMatches(intCnt).Groups(1).Value)            'ファイル名 
            objItem.SubItems.Add(objMatches(intCnt).Groups(3).Value)            '行番号
            objItem.SubItems.Add(objMatches(intCnt).Groups(5).Value)            'CRUD
            objItem.SubItems.Add(objMatches(intCnt).Groups(6).Value)            '関数名/カーソル名
        Next

        Me.Show()
    End Sub

    Private Sub cmdRunHidemaru_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRunHidemaru.Click
        If lstQuerys.SelectedItems.Count > 0 Then
            RunTextEditor(strSourcePath & "\" & lstQuerys.SelectedItems(0).SubItems(1).Text, lstQuerys.SelectedItems(0).SubItems(2).Text, strSearch)
        End If
    End Sub

    Private Sub cmdAnalyzeQuery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAnalyzeQuery.Click
        If lstQuerys.SelectedItems.Count > 0 Then
            If objForm.IsDisposed Then objForm = New frmAnalyzeQuery
            objForm.ShowForm(strSourcePath, lstQuerys.SelectedItems(0).SubItems(1).Text, lstQuerys.SelectedItems(0).SubItems(2).Text, strTableName, "", strSearch)
        End If
    End Sub

    Private Sub cmdAnalyzeQueryNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAnalyzeQueryNew.Click
        If lstQuerys.SelectedItems.Count > 0 Then
            Dim objFormNew As New frmAnalyzeQuery
            objFormNew.ShowForm(strSourcePath, lstQuerys.SelectedItems(0).SubItems(1).Text, lstQuerys.SelectedItems(0).SubItems(2).Text, strTableName, "", strSearch)
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

        lstQuerys.ListViewItemSorter = New ListViewItemComparerOtherList(e.Column, lngOrder)

    End Sub

    Private Sub lstFiles_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.Enter Then lstQuerys.Focus()
    End Sub

    Private Sub lstQuerys_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lstQuerys.KeyDown
        If e.KeyCode = Keys.Enter Then lstQuerys_DoubleClick(Nothing, Nothing)
    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

    Private Sub frmOtherCRUD_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitCommonContextMenu(lstQuerys, strSourcePath)
    End Sub

End Class