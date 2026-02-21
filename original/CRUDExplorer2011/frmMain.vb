Imports System.Text.RegularExpressions

Public Class frmMain

    Private regkey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\CRUDExplorer")
    Private strSourcePath As String
    Private MatrixKind As enmMatrixKind
    Private dctCRUD As New Scripting.Dictionary
    Private dctCRUDMatrix As New Scripting.Dictionary
    Private strSelectTableName As String
    Private strSelectProgramId As String
    Private strMatrix As String
    Private bolSortOrderAsc As Boolean = False
    Private bolCellClick As Boolean = False
    Private fso As Scripting.FileSystemObject = New Scripting.FileSystemObject
    Private objFrmFilter As frmFilter = Nothing

    Enum enmMatrixKind
        TableCRUD = 0
        ColumnCRUD = 1
    End Enum


    Private Sub テーブルCRUDToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles テーブルCRUDToolStripMenuItem.Click
        Dim strPath As String

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        strPath = SelectSourcePath()
        If strPath = "" Then Exit Sub
        strSourcePath = strPath
        lstCRUD.Columns(3).Text = "エンティティ名"
        lstCRUD.Columns(4).Text = "テーブル名"

        MatrixKind = enmMatrixKind.TableCRUD
        OpenMatrix(strPath)

        System.Windows.Forms.Cursor.Current = Nothing

    End Sub

    Private Sub カラムCRUDToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles カラムCRUDToolStripMenuItem.Click
        Dim strPath As String

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        strPath = SelectSourcePath()
        If strPath = "" Then Exit Sub
        strSourcePath = strPath
        lstCRUD.Columns(3).Text = "エンティティ名．属性名"
        lstCRUD.Columns(4).Text = "テーブル名．カラム名"

        MatrixKind = enmMatrixKind.ColumnCRUD
        OpenMatrix(strPath)

        System.Windows.Forms.Cursor.Current = Nothing

    End Sub

    Private Sub OpenMatrix(ByVal strPath As String)
        strSourcePath = ""
        strMatrix = ""

        'ソースフォルダのチェック
        If fso.FolderExists(strPath & "\querys") = False Then
            MsgBox("querysサブフォルダが見つかりません。" & vbCrLf & "CRUD解析を実行していない場合は、まず「CRUD解析」を実行し、解析結果フォルダを開いてください。")
            Exit Sub
        End If

        If MatrixKind = enmMatrixKind.TableCRUD Then
            If fso.FileExists(strPath & "\querys\CRUD.tsv") = False Then
                MsgBox("querys\CRUD.tsvファイルが見つかりません。")
                Exit Sub
            End If
            If fso.FileExists(strPath & "\querys\CRUDMatrix.tsv") = False Then
                MsgBox("querys\CRUDMatrix.tsvファイルが見つかりません。")
                Exit Sub
            End If
        Else
            If fso.FileExists(strPath & "\querys\CRUDColumns.tsv") = False Then
                MsgBox("querys\CRUDColumns.tsvファイルが見つかりません。")
                Exit Sub
            End If
            If fso.FileExists(strPath & "\querys\CRUDColumnsMatrix.tsv") = False Then
                MsgBox("querys\CRUDColumnsMatrix.tsvファイルが見つかりません。")
                Exit Sub
            End If
        End If
        strSourcePath = strPath
        regkey.SetValue("DestFolder", strSourcePath)

        lblSourcePath.Text = strSourcePath
        Me.Text = Mid(strSourcePath, InStrRev(strSourcePath, "\") + 1) & " ～ CRUD Explorer"

        dgvMatrix.Rows.Clear()
        dgvMatrix.Columns.Clear()

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
        dgvMatrix.Visible = False
        If MatrixKind = enmMatrixKind.TableCRUD Then
            LoadCRUD(strSourcePath & "\querys\CRUD.tsv")
            LoadCRUDMatrix(strSourcePath & "\querys\CRUDMatrix.tsv")
        Else
            LoadCRUD(strSourcePath & "\querys\CRUDColumns.tsv")
            LoadCRUDMatrix(strSourcePath & "\querys\CRUDColumnsMatrix.tsv")
        End If

        '無駄な行、列を削除
        Dim intRow As Integer, intCol As Integer
        Dim bolEmpty As Boolean
        For intRow = dgvMatrix.RowCount - 1 To 0 Step -1
            bolEmpty = True
            For intCol = dgvMatrix.ColumnCount - 2 To 0 Step -1
                If dgvMatrix.Rows(intRow).Cells(intCol).Value <> "" Then
                    bolEmpty = False
                    Exit For
                End If
            Next
            If bolEmpty Then
                dgvMatrix.Rows.RemoveAt(intRow)
            End If
        Next
        For intCol = dgvMatrix.ColumnCount - 2 To 0 Step -1
            bolEmpty = True
            For intRow = dgvMatrix.RowCount - 1 To 0 Step -1
                bolEmpty = True
                If dgvMatrix.Rows(intRow).Cells(intCol).Value <> "" Then
                    bolEmpty = False
                    Exit For
                End If
            Next
            If bolEmpty Then
                dgvMatrix.Columns.RemoveAt(intCol)
            End If
        Next


        dgvMatrix.Visible = True
        System.Windows.Forms.Cursor.Current = Nothing
        dgvMatrix.Focus()

    End Sub

    Function SelectSourcePath()
        SelectSourcePath = ""
        colViews.Clear()
        objFrmFilter = Nothing
        dctCRUDProgram.RemoveAll()
        dctCRUDTable.RemoveAll()
        dctCRUD.RemoveAll()
        dctFiles.RemoveAll()
        FolderBrowserDialog1.SelectedPath = regkey.GetValue("DestFolder")

        FolderBrowserDialog1.Description = "CRUDを出力した解析結果フォルダを指定"
        If FolderBrowserDialog1.ShowDialog() <> Windows.Forms.DialogResult.OK Then
            Exit Function
        End If
        SelectSourcePath = FolderBrowserDialog1.SelectedPath
        dctQueryList.RemoveAll()
        If fso.FolderExists(SelectSourcePath & "\querys") Then
            Dim objFolder As Scripting.Folder = fso.GetFolder(SelectSourcePath & "\querys")
            Dim objFile As Scripting.File
            For Each objFile In objFolder.Files
                If objFile.Size > 0 AndAlso GetRight(objFile.Name, 6) = ".query" Then
                    DictAdd(dctFiles, "K" & dctFiles.Count + 1, Mid(objFile.Name, 1, objFile.Name.Length - 6))
                    Dim strLines As String() = Split(objFile.OpenAsTextStream().ReadAll(), vbCrLf)
                    Dim intCnt As Integer
                    For intCnt = LBound(strLines) To UBound(strLines)
                        If strLines(intCnt) <> "" Then
                            Dim strCols As String() = Split(strLines(intCnt), vbTab)
                            On Error Resume Next
                            dctQueryList.Add(Mid(objFile.Name, 1, objFile.Name.Length - 6) & vbTab & strCols(0), strCols(1) & vbTab & strCols(2))
                            On Error GoTo 0
                        End If
                    Next
                End If
            Next
        End If

        'ビュー情報の読み込み
        If fso.FileExists(SelectSourcePath & "\querys\views.txt") Then
            Dim t As Scripting.TextStream
            t = fso.OpenTextFile(SelectSourcePath & "\querys\views.txt")
            Dim strLine As String
            Do Until t.AtEndOfStream
                strLine = t.ReadLine
                Dim strCols As String() = Split(strLine, vbTab)
                colViews.add(New clsView(strCols(0), strCols(1), strCols(2), strCols(3)))
            Loop
        End If

        lstCRUD.ContextMenu = Nothing
        InitCommonContextMenu(lstCRUD, FolderBrowserDialog1.SelectedPath, , 4)

    End Function

    Sub LoadCRUD(ByVal strCRUDFile As String)
        Dim t As Scripting.TextStream
        Dim strFile As String
        Dim strLines As String()
        Dim strCols As String()
        Dim lngRow As Long
        Dim strProgramId As String
        Dim strSourceFile As String
        Dim strLineNo As String
        Dim strTableName As String
        Dim strCRUD As String
        Dim dctCRUDDetail As Scripting.Dictionary
        Dim objCRUD As clsCRUD

        If fso.GetFile(strCRUDFile).Size = 0 Then Exit Sub
        t = fso.OpenTextFile(strCRUDFile)
        strFile = t.ReadAll
        strLines = Split(strFile, vbCrLf)

        For lngRow = LBound(strLines) To UBound(strLines)
            If strLines(lngRow) <> "" Then
                strCols = Split(strLines(lngRow), vbTab)
                strSourceFile = strCols(0)
                strProgramId = UCase(strCols(1))

                '辞書に追加
                Dim strPGDict As String = strProgramId
                If strPGDict = "" Then strPGDict = strSourceFile
                strPGDict = UCase(strPGDict)
                If Not DictExists(dctCRUDProgram, strPGDict) Then
                    dctCRUDProgram.Add(strPGDict, GetDictValue(dctProgramName, strPGDict))
                End If

                If strProgramId = "" Then strProgramId = strSourceFile
                strLineNo = strCols(2)
                strTableName = Trim(strCols(3))
                strCRUD = strCols(4)

                '辞書に追加
                Dim strDBDict As String = strTableName
                If Not DictExists(dctCRUDTable, strDBDict) Then
                    DictAdd(dctCRUDTable, strDBDict, GetDictValue(dctTableName, strDBDict))
                End If


                If DictExists(dctCRUD, strProgramId & ":" & strTableName) Then
                    dctCRUDDetail = GetDictObject(dctCRUD, strProgramId & ":" & strTableName)
                Else
                    dctCRUDDetail = New Scripting.Dictionary
                    DictAdd(dctCRUD, strProgramId & ":" & strTableName, dctCRUDDetail)
                End If
                If DictExists(dctCRUDDetail, strSourceFile & ":" & strLineNo) Then
                    objCRUD = GetDictObject(dctCRUDDetail, strSourceFile & ":" & strLineNo)
                Else
                    objCRUD = New clsCRUD
                    DictAdd(dctCRUDDetail, strSourceFile & ":" & strLineNo, objCRUD)
                End If
                objCRUD.TableName = strTableName
                objCRUD.FuncProcName = strCols(5)
                objCRUD.AltName = strCols(6)
                Select Case strCRUD
                    Case "C"
                        objCRUD.C = True
                    Case "R"
                        objCRUD.R = True
                    Case "U"
                        objCRUD.U = True
                    Case "D"
                        objCRUD.D = True
                End Select
            End If
        Next

    End Sub

    Sub LoadCRUDMatrix(ByVal strCRUDMatrix As String)
        Dim t As Scripting.TextStream
        Dim strLines As String()
        Dim strCols As String()
        Dim lngRow As Long
        Dim intCol As Integer
        Dim lngRowIdx As Long
        Dim intColIdx As Integer
        Dim arrColShow As Boolean() = Nothing

        '        On Error Resume Next

        dctCRUDMatrix.RemoveAll()
        dgvMatrix.RowTemplate.Height = 20
        lstCRUD.Items.Clear()
        t = fso.OpenTextFile(strCRUDMatrix)
        strMatrix = t.ReadAll
        dgvMatrix.Visible = False
        dgvMatrix.SuspendLayout()
        'dgvMatrix.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        strLines = Split(strMatrix, vbCrLf)
        '       dgvMatrix.RowCount = UBound(strLines) - LBound(strLines) - 2
        dgvMatrix.Rows.Clear()
        lngRowIdx = 0
        For lngRow = 0 To UBound(strLines) - 1
            strCols = Split(strLines(lngRow), vbTab)
            dctCRUDMatrix.Add("L" & lngRow, strCols)
            Select Case lngRow
                Case 0  'ヘッダ行
                    dgvMatrix.ColumnCount = UBound(strCols) - 2
                    intColIdx = 0
                    ReDim arrColShow(UBound(strCols) - 3)
                    For intCol = 3 To UBound(strCols)
                        arrColShow(intCol - 3) = False
                        If txtFilterProgramId.Text <> "" Then
                            If RegMatch(strCols(intCol), txtFilterProgramId.Text, RegexOptions.IgnoreCase) Then
                                arrColShow(intCol - 3) = True
                            End If
                        Else
                            arrColShow(intCol - 3) = True
                        End If
                        If arrColShow(intCol - 3) = True Then
                            dgvMatrix.Columns(intColIdx).Width = 50
                            dgvMatrix.Columns(intColIdx).Tag = strCols(intCol)
                            intColIdx += 1
                        End If
                    Next
                    dgvMatrix.ColumnCount = intColIdx
                    If intColIdx = 0 Then Exit For
                Case 1  '計算式の行
                Case Else 'データ行
                    If txtFilterTableName.Text = "" OrElse RegMatch(strCols(0), txtFilterTableName.Text, RegexOptions.IgnoreCase) Then
                        lngRowIdx = dgvMatrix.Rows.Add()
                        If strLines(lngRow) <> "" Then
                            dgvMatrix.Rows(lngRowIdx).Tag = strCols(0)
                        End If
                        intColIdx = 0
                        For intCol = 3 To UBound(strCols)
                            If arrColShow(intCol - 3) = True Then
                                dgvMatrix.Rows(lngRowIdx).Cells(intColIdx).Value = FilterCRUD(strCols(intCol))
                                intColIdx += 1
                            End If
                        Next
                        'lngRowIdx += 1

                    End If
            End Select
        Next
        ChangeHeaderText()
        dgvMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        dgvMatrix.ColumnHeadersHeight = 100

        Dim intCnt As Integer
        For intCnt = 0 To dgvMatrix.ColumnCount - 1
            dgvMatrix.Columns(intCnt).SortMode = DataGridViewColumnSortMode.NotSortable
        Next
        dgvMatrix.ColumnCount += 1
        dgvMatrix.Columns(dgvMatrix.ColumnCount - 1).Visible = False

        'ソート
        For intCnt = 0 To dgvMatrix.RowCount - 1
            If MatrixKind = enmMatrixKind.TableCRUD Then
                dgvMatrix.Rows(intCnt).Cells(dgvMatrix.ColumnCount - 1).Value = dgvMatrix.Rows(intCnt).Tag
            Else
                Dim objColumnDef As clsColumnDef = GetColumnDef(dgvMatrix.Rows(intCnt).Tag)
                Dim strArr As String() = Split(dgvMatrix.Rows(intCnt).Tag, ".")
                Dim strSeq As String = ""
                If Not objColumnDef Is Nothing Then
                    strSeq = CStr(objColumnDef.SEQ)
                Else
                    strSeq = "9999"
                End If
                Dim strSeqSpace As New String(" ", 4 - strSeq.Length)
                dgvMatrix.Rows(intCnt).Cells(dgvMatrix.ColumnCount - 1).Value = strArr(0) & strSeqSpace & strSeq & strArr(1)
            End If
        Next
        dgvMatrix.Sort(dgvMatrix.Columns(dgvMatrix.ColumnCount - 1), System.ComponentModel.ListSortDirection.Ascending)

        dgvMatrix.ResumeLayout()
        dgvMatrix.Visible = True
    End Sub

    Private Function FilterCRUD(ByVal strCRUD) As String
        Dim strRet As String = strCRUD
        If chkC.Checked = False Then strRet = Replace(strRet, "(C)", "") : strRet = Replace(strRet, "C", "")
        If chkR.Checked = False Then strRet = Replace(strRet, "(R)", "") : strRet = Replace(strRet, "R", "")
        If chkU.Checked = False Then strRet = Replace(strRet, "(U)", "") : strRet = Replace(strRet, "U", "")
        If chkD.Checked = False Then strRet = Replace(strRet, "(D)", "") : strRet = Replace(strRet, "D", "")

        FilterCRUD = strRet
    End Function

    Private Sub dgvMatrix_CellClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvMatrix.CellClick
        If bolCellClick Then Exit Sub
        bolCellClick = True
        lstCRUD.Sorting = SortOrder.None
        dgvMatrix_CellEnter(sender, e)
        lstCRUD.Sorting = SortOrder.Ascending
        lstCRUD.ListViewItemSorter = New ListViewItemComparer(0, 1)

        bolCellClick = False
    End Sub

    Private Sub dgvMatrix_CellEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvMatrix.CellEnter
        Dim dctCRUDDetail As Scripting.Dictionary
        Dim objCRUD As clsCRUD
        Dim strKey As String
        Dim strKey2 As String
        Dim strWork As String()
        Dim strArray As String()
        Dim objItem As ListViewItem
        Dim strProgramName As String

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
        lstCRUD.Visible = False
        lstCRUD.Items.Clear()

        If e.ColumnIndex = -1 And e.RowIndex = -1 Then
            dgvMatrix.ClearSelection()
            'ソート
            Dim intCnt As Integer
            For intCnt = 0 To dgvMatrix.RowCount - 1
                If MatrixKind = enmMatrixKind.TableCRUD Then
                    dgvMatrix.Rows(intCnt).Cells(dgvMatrix.ColumnCount - 1).Value = dgvMatrix.Rows(intCnt).Tag
                Else
                    Dim objColumnDef As clsColumnDef = GetColumnDef(dgvMatrix.Rows(intCnt).Tag)
                    Dim strArr As String() = Split(dgvMatrix.Rows(intCnt).Tag, ".")
                    Dim strSeq As String = ""
                    If Not objColumnDef Is Nothing Then
                        strSeq = CStr(objColumnDef.SEQ)
                    Else
                        strSeq = "9999"
                    End If
                    Dim strSeqSpace As New String(" ", 4 - strSeq.Length)
                    dgvMatrix.Rows(intCnt).Cells(dgvMatrix.ColumnCount - 1).Value = strArr(0) & strSeqSpace & strSeq & strArr(1)
                End If
            Next
            dgvMatrix.Sort(dgvMatrix.Columns(dgvMatrix.ColumnCount - 1), System.ComponentModel.ListSortDirection.Ascending)
            lstCRUD.Items.Clear()

        ElseIf e.ColumnIndex <> -1 And e.RowIndex <> -1 Then        'セルクリック
            strSelectTableName = dgvMatrix.Rows(e.RowIndex).Tag
            strSelectProgramId = dgvMatrix.Columns(e.ColumnIndex).Tag
            If DictExists(dctCRUD, strSelectProgramId & ":" & strSelectTableName) = False Then Exit Sub
            dctCRUDDetail = GetDictObject(dctCRUD, strSelectProgramId & ":" & strSelectTableName)

            For Each strKey In dctCRUDDetail.Keys
                strArray = Split(strKey, ":")
                objCRUD = GetDictObject(dctCRUDDetail, strKey)
                If CheckFilter(strSelectProgramId, objCRUD) Then
                    objItem = lstCRUD.Items.Add(strArray(0))
                    objItem.SubItems.Add(strArray(1))
                    strProgramName = GetDictValue(dctProgramName, strSelectProgramId, False)
                    If strProgramName <> "" Then
                        objItem.SubItems.Add(strProgramName & "(" & strSelectProgramId & ")")
                    Else
                        objItem.SubItems.Add(strSelectProgramId)
                    End If
                    objItem.SubItems.Add(GetEntityAttrName(strSelectTableName))
                    objItem.SubItems.Add(strSelectTableName)
                    objItem.SubItems.Add(objCRUD.GetCRUD)
                    objItem.SubItems.Add(objCRUD.FuncProcName)
                    objItem.SubItems.Add(objCRUD.AltName)
                End If
            Next
        ElseIf e.ColumnIndex <> -1 And e.RowIndex = -1 Then     '列ヘッダクリック
            dgvMatrix.ClearSelection()
            'ソート
            Dim intCnt As Integer
            For intCnt = 0 To dgvMatrix.RowCount - 1
                dgvMatrix.Rows(intCnt).Cells(dgvMatrix.ColumnCount - 1).Value = IIf(dgvMatrix.Rows(intCnt).Cells(e.ColumnIndex).Value <> "", "0", "1") & dgvMatrix.Rows(intCnt).Tag
            Next
            dgvMatrix.Sort(dgvMatrix.Columns(dgvMatrix.ColumnCount - 1), System.ComponentModel.ListSortDirection.Ascending)

            strSelectTableName = ""
            strSelectProgramId = dgvMatrix.Columns(e.ColumnIndex).Tag

            lstCRUD.Items.Clear()
            For Each strKey2 In dctCRUD.Keys
                strWork = Split(strKey2, ":")
                strSelectTableName = strWork(1)
                If UCase(strSelectProgramId) = UCase(strWork(0)) Then
                    dctCRUDDetail = GetDictObject(dctCRUD, strKey2)
                    For Each strKey In dctCRUDDetail.Keys
                        strArray = Split(strKey, ":")
                        objCRUD = GetDictObject(dctCRUDDetail, strKey)
                        If CheckFilter(strSelectProgramId, objCRUD) Then
                            objItem = lstCRUD.Items.Add(strArray(0))
                            objItem.SubItems.Add(strArray(1))
                            strProgramName = GetDictValue(dctProgramName, strSelectProgramId, False)
                            If strProgramName <> "" Then
                                objItem.SubItems.Add(strProgramName & "(" & strSelectProgramId & ")")
                            Else
                                objItem.SubItems.Add(strSelectProgramId)
                            End If
                            objItem.SubItems.Add(GetEntityAttrName(strSelectTableName))
                            objItem.SubItems.Add(strWork(1))
                            objItem.SubItems.Add(objCRUD.GetCRUD)
                            objItem.SubItems.Add(objCRUD.FuncProcName)
                            objItem.SubItems.Add(objCRUD.AltName)
                        End If
                    Next
                End If
            Next
            dgvMatrix.FirstDisplayedScrollingRowIndex = 0

        Else    '行ヘッダクリック
            strSelectTableName = dgvMatrix.Rows(e.RowIndex).Tag
            strSelectProgramId = ""

            For Each strKey2 In dctCRUD.Keys
                strWork = Split(strKey2, ":")
                If UCase(strSelectTableName) = UCase(strWork(1)) Then
                    dctCRUDDetail = GetDictObject(dctCRUD, strKey2)
                    For Each strKey In dctCRUDDetail.Keys
                        strArray = Split(strKey, ":")
                        strSelectProgramId = GetProgramId(strArray(0))
                        objCRUD = GetDictObject(dctCRUDDetail, strKey)
                        If CheckFilter(strSelectProgramId, objCRUD) Then
                            objItem = lstCRUD.Items.Add(strArray(0))
                            objItem.SubItems.Add(strArray(1))
                            strProgramName = GetDictValue(dctProgramName, strSelectProgramId, False)
                            If strProgramName <> "" Then
                                objItem.SubItems.Add(strProgramName & "(" & strSelectProgramId & ")")
                            Else
                                objItem.SubItems.Add(strSelectProgramId)
                            End If
                            objItem.SubItems.Add(GetEntityAttrName(strSelectTableName))
                            objItem.SubItems.Add(strSelectTableName)
                            objItem.SubItems.Add(objCRUD.GetCRUD)
                            objItem.SubItems.Add(objCRUD.FuncProcName)
                            objItem.SubItems.Add(objCRUD.AltName)
                        End If
                    Next
                End If
            Next
        End If
        ''''        lstCRUD.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        lstCRUD.Visible = True
        System.Windows.Forms.Cursor.Current = Nothing
    End Sub

    Private Function CheckFilter(ByVal strProgramId As String, ByVal objCRUD As clsCRUD) As Boolean
        Dim strFilterTableName = Trim(txtFilterTableName.Text)
        Dim strFilterProgramId = Trim(txtFilterProgramId.Text)

        If (strFilterTableName = "" OrElse RegMatch(objCRUD.TableName, strFilterTableName, RegexOptions.IgnoreCase)) And _
              (strFilterProgramId = "" OrElse RegMatch(strProgramId, strFilterProgramId, RegexOptions.IgnoreCase)) And _
              ((chkC.Checked AndAlso objCRUD.C) OrElse _
               (chkR.Checked AndAlso objCRUD.R) OrElse _
               (chkU.Checked AndAlso objCRUD.U) OrElse _
               (chkD.Checked AndAlso objCRUD.D)) Then
            CheckFilter = True
        Else
            CheckFilter = False
        End If
    End Function

    Private Function GetEntityAttrName(ByVal strTableColumnName As String) As String
        If MatrixKind = enmMatrixKind.TableCRUD Then
            GetEntityAttrName = GetDictValue(dctTableName, strTableColumnName)
        Else
            Dim arrTableColumn As String() = Split(strTableColumnName, ".")
            GetEntityAttrName = GetDictValue(dctTableName, arrTableColumn(0))
            GetEntityAttrName &= " ．"
            If DictExists(dctTableDef, arrTableColumn(0)) Then
                Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, arrTableColumn(0))
                If DictExists(objTableDef.dctColumnts, arrTableColumn(1)) Then
                    GetEntityAttrName &= GetDictObject(objTableDef.dctColumnts, arrTableColumn(1)).AttributeName
                Else
                    GetEntityAttrName &= arrTableColumn(1)
                End If
            Else
                GetEntityAttrName &= arrTableColumn(1)
            End If
        End If

    End Function

    Private Sub lstCRUD_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstCRUD.DoubleClick
        Select Case objSettings.ListDblClickMode
            Case clsSettings.enmListDblClickMode.ExecTextEditor
                cmdRunHidemaru_Click(Nothing, Nothing)
            Case clsSettings.enmListDblClickMode.AnalyzeQuery
                cmdAnalyzeQuery_Click(Nothing, Nothing)
        End Select
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim objForm As New frmStartup

        If regkey.GetValue("FirstStartupDate") = "" Then regkey.SetValue("FirstStartupDate", Now().Year & "/" & Now().Month & "/" & Now.Day)

        objSettings.LoadSettings()
        If objSettings.LicenseKey.Length = 16 Then
            Dim strWork As String = DeExchageKey(objSettings.LicenseKey)
            Dim strNo As String = Mid(strWork, 1, 8)
            Dim strEnc = Encrypt(strNo)
            If strWork = (strNo & strEnc) Then
                bolDemoFlag = False
            End If
        End If

        Dim objFolder As Scripting.Folder = fso.GetFolder(System.IO.Directory.GetCurrentDirectory)
        If bolDemoFlag And objFolder.DateCreated.AddDays(31) < Now() Then
            MsgBox("評価版の試用期限が切れています。")
            Me.Dispose()
            Exit Sub
        End If

        If bolShowStartup Then objForm.Show()
        My.Application.DoEvents()
        System.Threading.Thread.Sleep(2000)

        objSettings.LoadFormSize(Me)
        dctTableName = ReadDictionary(System.IO.Directory.GetCurrentDirectory & "\TableNameDictionary.txt")
        dctProgramName = ReadDictionary(System.IO.Directory.GetCurrentDirectory & "\ProgramNameDictionary.txt")
        dctReferenceCond = ReadDictionary(System.IO.Directory.GetCurrentDirectory & "\ReferenceCondDictionary.txt")
        ReadTableDef(System.IO.Directory.GetCurrentDirectory & "\TableDefDictionary.txt", dctTableDef)
        If bolShowStartup Then objForm.Close()

        If RegMatch(System.Reflection.Assembly.GetExecutingAssembly().Location, "QueryAnalyzer[.]exe", RegexOptions.IgnoreCase) Then
            frmAnalyzeQuery.ShowDialog()
            Me.Close()
        Else
            Me.Focus()

            If System.Environment.GetCommandLineArgs.Length > 1 Then
                MatrixKind = enmMatrixKind.TableCRUD
                OpenMatrix(System.Environment.GetCommandLineArgs(1))
            End If
        End If

        If bolDemoFlag Then
            MsgBox("CRUD Explorerをダウンロード頂き、誠にありがとうございます。" & vbCrLf & "ライセンス登録されていないため、評価版として動作します。" & vbCrLf & "評価版は、解析ソースファイルが３ファイル、１０クエリに制限されています。" & vbCrLf & "また、一部の機能についてもご利用頂くことができませんので、予めご了承ください。" & vbCrLf & vbCrLf & "評価版の試用期限は３０日間となっています。" & vbCrLf & vbCrLf & "ライセンス登録を行うには、「バージョン情報」から登録してください。")
        End If

    End Sub

    Private Sub マトリックスをToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles マトリックスをToolStripMenuItem.Click
        tsmCopyClipBoard_Click(Nothing, Nothing)
    End Sub

    Private Sub lstCRUD_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles lstCRUD.ColumnClick
        Dim lngOrder As Long

        On Error Resume Next
        bolSortOrderAsc = Not bolSortOrderAsc
        If bolSortOrderAsc Then
            lngOrder = 1
        Else
            lngOrder = -1
        End If

        lstCRUD.ListViewItemSorter = New ListViewItemComparer(e.Column, lngOrder)
    End Sub


    Private Sub btnApplyFilter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApplyFilter.Click
        If strSourcePath = "" Then
            MsgBox("先に解析結果フォルダを開いてください")
            Exit Sub
        End If
        OpenMatrix(strSourcePath)
    End Sub

    Private Sub btnClearFilter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearFilter.Click
        txtFilterProgramId.Text = ""
        txtFilterTableName.Text = ""
        chkC.Checked = True
        chkR.Checked = True
        chkU.Checked = True
        chkD.Checked = True
        OpenMatrix(strSourcePath)
    End Sub

    Private Sub txtFilterTableName_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtFilterTableName.KeyPress, txtFilterProgramId.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Return) Then
            btnApplyFilter_Click(Nothing, Nothing)
            e.Handled = False
        End If
    End Sub

    Private Sub cmdRunHidemaru_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRunHidemaru.Click
        If lstCRUD.SelectedItems.Count > 0 Then
            RunTextEditor(strSourcePath & "\" & lstCRUD.SelectedItems(0).Text, lstCRUD.SelectedItems(0).SubItems(1).Text, lstCRUD.SelectedItems(0).SubItems(4).Text, lstCRUD.SelectedItems(0).SubItems(7).Text)
        End If
    End Sub

    Private Sub cmdAnalyzeQuery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAnalyzeQuery.Click
        If lstCRUD.SelectedItems.Count > 0 Then
            Dim objForm As New frmAnalyzeQuery
            objForm.ShowForm(strSourcePath, lstCRUD.SelectedItems(0).SubItems(0).Text, lstCRUD.SelectedItems(0).SubItems(1).Text, lstCRUD.SelectedItems(0).SubItems(4).Text, lstCRUD.SelectedItems(0).SubItems(7).Text)
        End If
    End Sub

    Private Sub AnalyzeCRUDToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AnalyzeCRUDToolStripMenuItem.Click
        Dim objForm As New frmMakeCRUD()
        objForm.Show()
    End Sub

    Private Sub SettingsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingsToolStripMenuItem.Click
        frmSettings.ShowDialog()
    End Sub

    Private Sub dgvMatrix_CellPainting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellPaintingEventArgs) Handles dgvMatrix.CellPainting
        Dim dv As DataGridView = sender
        If e.RowIndex < 0 Then
            If e.ColumnIndex >= 0 Then
                dv.Columns(e.ColumnIndex).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                e.Paint(e.ClipBounds, DataGridViewPaintParts.Background Or DataGridViewPaintParts.Border)
                Dim sf As New StringFormat
                sf.FormatFlags = StringFormatFlags.DirectionVertical Or StringFormatFlags.NoWrap
                sf.Alignment = StringAlignment.Near
                sf.LineAlignment = StringAlignment.Center
                Dim rect As Rectangle = e.CellBounds
                rect.Inflate(-2, -2)
                Dim text As String = dv.Columns(e.ColumnIndex).HeaderText
                Dim font As Font = dv.ColumnHeadersDefaultCellStyle.Font
                Dim foreBrush As Brush = New SolidBrush(dv.ColumnHeadersDefaultCellStyle.ForeColor)
                e.Graphics.DrawString(text, font, foreBrush, rect, sf)
                e.Handled = True
                '                dv.Columns(e.ColumnIndex).Width = 50
            End If
        End If
    End Sub

    Private Sub dgvMatrix_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles dgvMatrix.KeyDown
        If e.KeyData = Keys.Enter Or e.KeyData = Keys.Tab Then
            e.Handled = True
            lstCRUD.Focus()
            If lstCRUD.SelectedItems.Count = 0 AndAlso lstCRUD.Items.Count > 0 Then
                lstCRUD.Items(0).Selected = True
            End If
        ElseIf e.KeyData = Keys.T Then
            tsmTableDef_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub lstCRUD_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lstCRUD.KeyDown
        If e.KeyData = Keys.Enter Then
            lstCRUD_DoubleClick(Nothing, Nothing)
        End If
    End Sub

    Private Sub dgvMatrix_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles dgvMatrix.Scroll
        '       dgvMatrix.SuspendLayout()

    End Sub

    Private Sub btnListToClip_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If lstCRUD.Items.Count > 0 Then
            On Error Resume Next
            Clipboard.Clear()
            Dim strWork As String = ""
            Dim intRow As Integer, intCol As Integer
            For intRow = 0 To lstCRUD.Items.Count
                For intCol = 0 To lstCRUD.Items(intRow).SubItems.Count
                    If intCol <> 0 Then strWork &= vbTab
                    strWork &= lstCRUD.Items(intRow).SubItems(intCol).Text
                Next
                strWork &= vbCrLf
            Next
            Clipboard.SetText(strWork)
            MsgBox("クリップボードに格納しました")
        End If
    End Sub

    Private Sub btnToggleDisplayedName_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnToggleDisplayedName.Click
        If btnToggleDisplayedName.Tag = "1" Then
            btnToggleDisplayedName.Tag = "2"
            btnToggleDisplayedName.Text = "論理名に切り替え"
        Else
            btnToggleDisplayedName.Tag = "1"
            btnToggleDisplayedName.Text = "物理名に切り替え"
        End If

        ChangeHeaderText()

    End Sub

    Private Sub ChangeHeaderText()
        Dim intCnt As Integer

        For intCnt = 0 To dgvMatrix.Columns.Count - 1
            Dim strProgramID As String = dgvMatrix.Columns(intCnt).Tag
            Dim strProgramName As String = strProgramID
            If DictExists(dctProgramName, strProgramID) Then strProgramName = GetDictValue(dctProgramName, strProgramID)
            If btnToggleDisplayedName.Tag = "1" Then
                dgvMatrix.Columns(intCnt).HeaderText = strProgramName
                dgvMatrix.Columns(intCnt).ToolTipText = strProgramID
            Else
                dgvMatrix.Columns(intCnt).HeaderText = strProgramID
                dgvMatrix.Columns(intCnt).ToolTipText = strProgramName
            End If
        Next

        For intCnt = 0 To dgvMatrix.Rows.Count - 1
            If dgvMatrix.Rows(intCnt).Tag Is Nothing Then Exit For
            Dim strTableColumnName As String = dgvMatrix.Rows(intCnt).Tag
            Dim strTableName As String
            Dim strColumnName As String
            Dim strEntityName As String
            Dim strAttributeName As String
            If MatrixKind = enmMatrixKind.TableCRUD Then
                strTableName = strTableColumnName
                strEntityName = strTableColumnName
                If DictExists(dctTableName, strTableName) Then strEntityName = GetDictValue(dctTableName, strTableName)
                If btnToggleDisplayedName.Tag = "1" Then
                    dgvMatrix.Rows(intCnt).HeaderCell.Value = strEntityName
                    dgvMatrix.Rows(intCnt).HeaderCell.ToolTipText = strTableName
                Else
                    dgvMatrix.Rows(intCnt).HeaderCell.Value = strTableName
                    dgvMatrix.Rows(intCnt).HeaderCell.ToolTipText = strEntityName
                End If
            Else
                Dim strWord As String() = Split(strTableColumnName, ".")
                strTableName = strWord(0)
                strEntityName = strWord(0)
                strColumnName = strWord(1)
                strAttributeName = strWord(1)
                If DictExists(dctTableName, strTableName) Then strEntityName = GetDictValue(dctTableName, strTableName)
                If DictExists(dctTableDef, strTableName) Then
                    Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, strTableName)
                    If DictExists(objTableDef.dctColumnts, strColumnName) Then
                        strAttributeName = GetDictObject(objTableDef.dctColumnts, strColumnName).AttributeName
                    End If
                End If
                If btnToggleDisplayedName.Tag = "1" Then
                    dgvMatrix.Rows(intCnt).HeaderCell.Value = strEntityName & "．" & strAttributeName
                    dgvMatrix.Rows(intCnt).HeaderCell.ToolTipText = strTableName & "．" & strColumnName
                Else
                    dgvMatrix.Rows(intCnt).HeaderCell.Value = strTableName & "．" & strColumnName
                    dgvMatrix.Rows(intCnt).HeaderCell.ToolTipText = strEntityName & "．" & strAttributeName
                End If
            End If
        Next
    End Sub

    Private Sub btnTableDef_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTableDef.Click
        If lstCRUD.SelectedItems.Count > 0 Then
            ShowTableDef(lstCRUD.SelectedItems(0).SubItems(4).Text, strSourcePath)
        End If

    End Sub

    Private Sub TableDefToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TableDefToolStripMenuItem.Click
        frmTableDef.ShowForm(strSourcePath)
    End Sub

    Private Sub ソースファイル一覧ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ソースファイル一覧ToolStripMenuItem.Click
        If strSourcePath = "" Then Exit Sub
        frmFileList.ShowForm(strSourcePath)
    End Sub

    Private Sub クエリ解析ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles クエリ解析ToolStripMenuItem.Click
        Dim objForm As New frmAnalyzeQuery
        objForm.ShowForm(strSourcePath, "", 0, "", "")
    End Sub

    Private Sub CRUDは開かないToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CRUDは開かないToolStripMenuItem.Click
        Dim strPath As String

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        strPath = SelectSourcePath()
        If strPath = "" Then Exit Sub
        strSourcePath = strPath

        dgvMatrix.Rows.Clear()
        dgvMatrix.Columns.Clear()

        lblSourcePath.Text = strSourcePath

        System.Windows.Forms.Cursor.Current = Nothing

    End Sub

    Private Sub cmdDetailFilter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDetailFilter.Click
        If objFrmFilter Is Nothing Then objFrmFilter = New frmFilter

        If strSourcePath = "" Then Exit Sub
        objFrmFilter.ShowForm(strSourcePath)
        Dim strWork As String = ""
        Dim intCnt As Integer
        If Not objFrmFilter.bolCancel Then
            strWork = ""
            For intCnt = 0 To objFrmFilter.lstProgram.Items.Count - 1
                If objFrmFilter.lstProgram.Items(intCnt).Checked Then
                    If strWork <> "" Then strWork &= "|"
                    strWork &= objFrmFilter.lstProgram.Items(intCnt).SubItems(0).Text
                End If
            Next
            txtFilterProgramId.Text = strWork

            strWork = ""
            For intCnt = 0 To objFrmFilter.lstTable.Items.Count - 1
                If objFrmFilter.lstTable.Items(intCnt).Checked Then
                    If strWork <> "" Then strWork &= "|"
                    strWork &= objFrmFilter.lstTable.Items(intCnt).SubItems(0).Text
                End If
            Next
            txtFilterTableName.Text = strWork

            If objFrmFilter.cmbProgramAccess.Text <> "" Then
                Dim strProgramId As String = Split(objFrmFilter.cmbProgramAccess.Text, " ")(0)
                strWork = ""
                Dim intPGIdx As Integer = -1
                For intCnt = 2 To UBound(dctCRUDMatrix.Items(0))
                    If dctCRUDMatrix.Items(0)(intCnt) = strProgramId Then intPGIdx = intCnt : Exit For
                Next

                If intPGIdx > 0 Then
                    For intCnt = 2 To dctCRUDMatrix.Count - 1
                        If intCnt > 2 Then
                            If dctCRUDMatrix.Items(intCnt)(intPGIdx) <> "" Then
                                If strWork <> "" Then strWork &= "|"
                                strWork &= EscapeRegular(dctCRUDMatrix.Items(intCnt)(0))
                            End If
                        End If
                    Next
                End If
                txtFilterTableName.Text = strWork
            End If

            If objFrmFilter.cmbTableAccess.Text <> "" Then
                Dim strTableName As String = Split(objFrmFilter.cmbTableAccess.Text, " ")(0)
                strWork = ""
                Dim intTBIdx As Integer = -1
                For intCnt = 2 To dctCRUDMatrix.Count - 1
                    If dctCRUDMatrix.Items(intCnt)(0) = strTableName Then intTBIdx = intCnt : Exit For
                Next

                If intTBIdx > 0 Then
                    For intCnt = 2 To UBound(dctCRUDMatrix.Items(0))
                        If intCnt > 2 Then
                            If dctCRUDMatrix.Items(intTBIdx)(intCnt) <> "" Then
                                If strWork <> "" Then strWork &= "|"
                                strWork &= dctCRUDMatrix.Items(0)(intCnt)
                            End If
                        End If
                    Next
                End If
                txtFilterProgramId.Text = strWork
            End If

        End If
        OpenMatrix(strSourcePath)
    End Sub

    Private Sub tsmCopyClipBoard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmCopyClipBoard.Click
        If dgvMatrix.RowCount = 0 Then Exit Sub

        '        Clipboard.Clear()

        Dim strClip As String = ""
        Dim intRow As Integer, intCol As Integer
        Dim strHeader1 As String = vbTab
        Dim strHeader2 As String = "テーブル名" & vbTab & "エンティティ名"

        Try
            For intCol = 0 To dgvMatrix.Columns.Count - 1
                strHeader1 &= vbTab & dgvMatrix.Columns(intCol).Tag
                strHeader2 &= vbTab & GetDictValue(dctProgramName, dgvMatrix.Columns(intCol).Tag)
            Next

            strClip = strHeader1 & vbCrLf & strHeader2 & vbCrLf
            For intRow = 0 To dgvMatrix.RowCount - 1
                '                strClip &= dgvMatrix.Rows(intRow).Tag & vbTab & GetDictValue(dctTableName, dgvMatrix.Rows(intRow).Tag)

                strClip &= dgvMatrix.Rows(intRow).Tag & vbTab & GetLogicalName(dgvMatrix.Rows(intRow).Tag)
                For intCol = 0 To dgvMatrix.ColumnCount - 2
                    strClip &= vbTab & dgvMatrix.Rows(intRow).Cells(intCol).Value
                Next
                strClip &= vbCrLf
            Next
            Clipboard.SetText(strClip)
            MsgBox("クリップボードに格納しました")
        Catch
        End Try
    End Sub

    Private Sub tsmTableDef_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmTableDef.Click
        If dgvMatrix.CurrentCell Is Nothing Then Exit Sub
        ShowTableDef(dgvMatrix.Rows(dgvMatrix.CurrentCell.RowIndex).Tag, strSourcePath)
    End Sub

    Private Sub ToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If lstCRUD.SelectedItems.Count > 0 Then
            Dim strTableColumn As String() = Split(lstCRUD.SelectedItems(0).SubItems(4).Text, ".")
            frmTableDef.ShowForm(strSourcePath, strTableColumn(0))
        End If
    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If Application.OpenForms.Count > 1 Then
            If MsgBox("他の画面を起動中です。" & vbCrLf & "CRUD Explorerを終了してもよろしいですか？", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub frmMain_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        objSettings.SaveFormSize(Me)
    End Sub

    Private Sub バージョン情報ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles バージョン情報ToolStripMenuItem.Click
        frmVersion.ShowDialog()
    End Sub

    Private Sub サポートサイトToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles サポートサイトToolStripMenuItem.Click
        If MsgBox("ブラウザでCRUD Explorerサポートページを開きますか？", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        System.Diagnostics.Process.Start("http://crudexplorer.coresv.com")
    End Sub

    Private Sub dgvMatrix_ColumnAdded(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewColumnEventArgs) Handles dgvMatrix.ColumnAdded
        e.Column.FillWeight = 1
    End Sub
End Class
