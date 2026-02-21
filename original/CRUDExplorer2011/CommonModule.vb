Imports System.Text.RegularExpressions

Module CommonModule
    Public Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As String) As Integer
    Public Const EM_GETLINECOUNT = &HBA
    Public Const EM_LINEFROMCHAR = &HC9
    Public Const EM_LINEINDEX = &HBB
    Public Const EM_GETSEL = &HB0
    Public bolDummyFlag As Boolean = True           '評価版
    Public bolShowStartup As Boolean = True         '開始画面表示
    Public objSettings As New clsSettings
    Public dctTableName As Scripting.Dictionary
    Public dctTableDef As New Scripting.Dictionary
    Public dctProgramName As Scripting.Dictionary
    Public dctCRUDProgram As New Scripting.Dictionary
    Public dctCRUDTable As New Scripting.Dictionary
    Public dctReferenceCond As New Scripting.Dictionary
    Public dctQueryList As New Scripting.Dictionary
    Public dctFiles As New Scripting.Dictionary
    Public colViews As New ViewCollection
    Public bolDemoFlag As Boolean = bolDummyFlag
    Public strPubKey As String = "<RSAKeyValue><Modulus>q53+M7Brxnifa3hw7Bn5PuEiX+QpYPE33OAWDKmy3vPllmkRuxyfjquOeLVbHkTSaoA1qPYirxfayRwRUoCwvWGSqz84qQpE4fhU1STem201M5RnJx3fxXiUduL4Nxs3tk5XrlDVY1lQWXKDomGBiKS+3T6B+ljH4HpQmF973yU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"
    Public strPriKey As String = "<RSAKeyValue><Modulus>q53+M7Brxnifa3hw7Bn5PuEiX+QpYPE33OAWDKmy3vPllmkRuxyfjquOeLVbHkTSaoA1qPYirxfayRwRUoCwvWGSqz84qQpE4fhU1STem201M5RnJx3fxXiUduL4Nxs3tk5XrlDVY1lQWXKDomGBiKS+3T6B+ljH4HpQmF973yU=</Modulus><Exponent>AQAB</Exponent><P>2AB9nfkOs/3xIIJAZbfBAfNvP09mucBHC2HE/crCmEzxPHCjjLhfdg2zORmg1Ua4t0zIpVBFMv3yx8EJjgRZBQ==</P><Q>y2V0nmLRQB55ns9hi2jKN/C/QrZWWQdCm7yrgv30Z9PBGAQve3frWt+p81byKUQ0yNu02jKXKAPerf39iOnHoQ==</Q><DP>h9EHK2WHETYDf+VmiI7aFVf0A2LxvKpiAY4gR1ROt2Tp6o8Ix1rG63wBzU2IC5LEYr0tDIVEfaOgHGoMj/e74Q==</DP><DQ>fqCWPvkkbvfKHe3cO6+snbEbUcw0685SUKTgXnf+jhlOEMaiTQr2kqfGpcGOl9RnzFjEOkfexUHLg6UqD/ADoQ==</DQ><InverseQ>wA9PtlSGjzJqV5JmGnrpMN3B0BEXxvxS15tQr1HB8I86KN1klLesJoB90vpuawoAR+5BUHHFz3RGXghpnHuDNg==</InverseQ><D>Py4ACol1c/CuSANkFxeM0eBSJlk5/o1vUmpQ08KZrki+CfyOYYMtHnn8DmY9sEwH5ttiZdyPckRm8Ejb+7KS3dsqXcY2//SfWxzrF6uB2jb6KdLVeBOzjToUqE73S8MtWvJg2PfKh9iWEGe/1pdJUF5M2dzX0PO/OYImEQVT4YE=</D></RSAKeyValue>"

    Public Function GetRight(ByVal stTarget As String, ByVal iLength As Integer) As String
        If iLength <= stTarget.Length Then
            Return stTarget.Substring(stTarget.Length - iLength)
        End If

        Return stTarget
    End Function

    Public Function LenB(ByVal stTarget As String) As Integer
        Return System.Text.Encoding.GetEncoding("Shift_JIS").GetByteCount(stTarget)
    End Function

    Public Function RegMatch(ByVal input As String, ByVal pattern As String, Optional ByVal opt As RegexOptions = RegexOptions.None) As Boolean
        Try
            Dim objMatch As MatchCollection = Regex.Matches(input, pattern, opt)
            RegMatch = False
            If objMatch.Count > 0 Then
                RegMatch = True
            End If
        Catch
            RegMatch = False
        End Try
    End Function

    Public Function RegMatchI(ByVal input As String, ByVal pattern As String, Optional ByVal opt As RegexOptions = RegexOptions.None) As Boolean
        RegMatchI = RegMatch(input, pattern, RegexOptions.IgnoreCase)
    End Function

    Public Function ReadDictionary(ByVal strDictFile As String) As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        Dim fso As New Scripting.FileSystemObject
        Dim arrLine As String()
        Dim arrCols As String()
        Dim lngCnt As Long
        Dim t As Scripting.TextStream

        If fso.FileExists(strDictFile) Then
            t = fso.OpenTextFile(strDictFile)
            arrLine = Split(Replace(t.ReadAll, vbCr, ""), vbLf)
            For lngCnt = LBound(arrLine) To UBound(arrLine)
                arrCols = Split(arrLine(lngCnt), vbTab)
                If (UBound(arrCols) - LBound(arrCols)) = 1 Then
                    On Error Resume Next
                    dct.Add(UCase(arrCols(0)), arrCols(1))
                    On Error GoTo 0
                End If
            Next
        End If
        ReadDictionary = dct
    End Function

    Public Sub ReadTableDef(ByVal strDictFile As String, ByRef dct As Scripting.Dictionary)
        Dim fso As New Scripting.FileSystemObject
        Dim arrLine As String()
        Dim arrCols As String()
        Dim lngCnt As Long
        Dim t As Scripting.TextStream
        Dim objTable As clsTableDef

        On Error Resume Next
        If fso.FileExists(strDictFile) Then
            t = fso.OpenTextFile(strDictFile)
            arrLine = Split(Replace(t.ReadAll, vbCr, ""), vbLf)
            For lngCnt = LBound(arrLine) To UBound(arrLine)
                arrCols = Split(arrLine(lngCnt), vbTab)
                Dim objCol As New clsColumnDef
                objCol.TableName = UCase(arrCols(0))
                objCol.ColumnName = UCase(arrCols(1))
                objCol.AttributeName = arrCols(2)
                objCol.SEQ = arrCols(3)
                objCol.PK = arrCols(4)
                objCol.FK = arrCols(5)
                objCol.Required = arrCols(6)
                objCol.DataType = arrCols(7)
                objCol.Digits = arrCols(8)
                objCol.Accuracy = arrCols(9)
                If DictExists(dct, objCol.TableName) Then
                    objTable = dct(objCol.TableName)
                Else
                    objTable = New clsTableDef
                    dct.Add(objCol.TableName, objTable)
                End If
                If Not DictExists(objTable.dctColumnts, objCol.ColumnName) Then
                    objTable.dctColumnts.Add(objCol.ColumnName, objCol)
                End If
            Next
        End If
    End Sub


    Public Function GetDictValue(ByRef dct As Scripting.Dictionary, ByVal strKey As String, Optional ByVal bolMissingReternKey As Boolean = True) As String
        If DictExists(dct, strKey) Then
            GetDictValue = dct(UCase(strKey))
        Else
            If bolMissingReternKey Then         '見つからない場合はキーを返す
                GetDictValue = strKey
            Else
                GetDictValue = ""
            End If
        End If
    End Function

    Public Function GetDictObject(ByRef dct As Scripting.Dictionary, ByVal strKey As String, Optional ByVal bolMissingReternKey As Boolean = True) As Object
        If DictExists(dct, strKey) Then
            GetDictObject = dct(UCase(strKey))
        Else
            GetDictObject = Nothing
        End If
    End Function

    Public Function DictExists(ByRef dct As Scripting.Dictionary, ByVal strKey As String) As Boolean
        DictExists = dct.Exists(UCase(strKey))
    End Function

    Public Sub DictAdd(ByRef dct As Scripting.Dictionary, ByVal strKey As String, ByVal objVal As Object)
        dct.Add(UCase(strKey), objVal)
    End Sub

    Public Function EscapeRegular(ByVal strValue As String) As String
        Dim lngCnt As Long
        Dim strRet As String = ""

        For lngCnt = 1 To Len(strValue)
            Dim strChar As String = Mid(strValue, lngCnt, 1)
            Dim objMaches As MatchCollection = Regex.Matches(strChar, "[a-z0-9]", RegexOptions.IgnoreCase)
            If objMaches.Count = 0 Then
                strRet &= "[" & Mid(strValue, lngCnt, 1) & "]"
            Else
                strRet &= strChar
            End If
        Next
        EscapeRegular = strRet
    End Function

    Public Function GetProgramId(ByVal strModuleId As String) As String
        Dim objMatches As MatchCollection

        On Error Resume Next

        objMatches = Regex.Matches(strModuleId, objSettings.ProgramIdPattern, RegexOptions.IgnoreCase)
        If objMatches.Count > 0 Then
            If objMatches(0).Groups.Count > 0 Then
                GetProgramId = objMatches(0).Groups(0).Value
            Else
                GetProgramId = objMatches(0).Value
            End If
        Else
            GetProgramId = ""
        End If
    End Function

    Public Sub RunTextEditor(ByVal strPath As String, Optional ByVal intLineNo As Integer = 0, Optional ByVal strSearch1 As String = "", Optional ByVal strSearch2 As String = "")
        Dim fso As Scripting.FileSystemObject = New Scripting.FileSystemObject
        Dim t As Scripting.TextStream
        Dim strSearch As String = ""
        Dim strMacroFile As String = ""

        Select Case objSettings.TextEditor
            Case "sakura"
                'サクラエディタのマクロを作成
                strMacroFile = "C:\_sakura.mac"
                '                strMacroFile = System.IO.Directory.GetCurrentDirectory & "\sakura.mac"
                t = fso.OpenTextFile(strMacroFile, Scripting.IOMode.ForWriting, True)
                If strSearch1 <> "" Then
                    strSearch = EscapeRegular(strSearch1)
                End If
                If strSearch2 <> "" Then
                    If strSearch <> "" Then strSearch &= "|"
                    strSearch &= EscapeRegular(strSearch2)
                End If
                If intLineNo <> 1 Then t.WriteLine("S_Jump(" & intLineNo & ", 1);")
                If strSearch <> "" Then t.WriteLine("S_SearchNext(""" & strSearch & """,7);")
                If intLineNo <> 1 Then t.WriteLine("S_Jump(" & intLineNo & ", 1);")
                t.Close()

                Try
                    Shell("""" & objSettings.SakuraPath & """ -M=" & strMacroFile & " """ & strPath & """", AppWinStyle.NormalFocus)
                Catch
                    If MsgBox("テキストエディタを起動できません。設定を変更しますか？", MsgBoxStyle.YesNo) = vbYes Then
                        frmSettings.ShowDialog()
                    End If
                End Try
            Case "hidemaru"
                '秀丸のマクロを作成
                strMacroFile = System.IO.Directory.GetCurrentDirectory & "\hidemaru.mac"
                t = fso.OpenTextFile(strMacroFile, Scripting.IOMode.ForWriting, True)
                If strSearch1 <> "" Then
                    strSearch = EscapeRegular(strSearch1)
                End If
                If strSearch2 <> "" Then
                    If strSearch <> "" Then strSearch &= "|"
                    strSearch &= EscapeRegular(strSearch2)
                End If
                If strSearch <> "" Then t.WriteLine("searchdown """ & strSearch & """,regular,nocasesense,hilight;")
                If intLineNo <> 1 Then t.WriteLine("movetolineno 1, " & intLineNo & "; ")
                t.WriteLine("hilightfound 1 ;")
                t.Close()

                Try
                    Shell("""" & objSettings.HidemaruPath & """ /x""" & strMacroFile & """ """ & strPath & """", AppWinStyle.NormalFocus)
                Catch
                    If MsgBox("テキストエディタを起動できません。設定を変更しますか？", MsgBoxStyle.YesNo) = vbYes Then
                        frmSettings.ShowDialog()
                    End If
                End Try
            Case Else
                Try
                    Shell("""" & objSettings.NotepadPath & """ """ & strPath & """", AppWinStyle.NormalFocus)
                Catch
                    If MsgBox("テキストエディタを起動できません。設定を変更しますか？", MsgBoxStyle.YesNo) = vbYes Then
                        frmSettings.ShowDialog()
                    End If
                End Try
        End Select


    End Sub

    '----------------------------------------------------------------
    'コメント(単一行、複数行）を削除する
    '引数
    ' strSrc        : ソース
    ' bolKeepLineNo : True(行番号を維持するために改行を付加)
    '                 False(コメント部分を完全に除去)
    '戻り値         : コメント抜きソース
    '----------------------------------------------------------------
    Function DeleteComment(ByVal strSrc As String, ByVal bolKeepLineNo As Boolean) As String
        Dim intPos As Integer
        Dim lngSrcLen As Long
        Dim strMultiLineCommentSpace As String = ""
        Dim intHitPos As Integer, intEndPos As Integer
        Dim strRet As String = ""
        Dim strDel As String
        Dim strArr As String()

        intPos = 1
        strSrc = Replace(strSrc, vbCr, "")
        lngSrcLen = Len(strSrc)

        Do
            My.Application.DoEvents()
            intHitPos = InStr(intPos, strSrc, "/*")
            If intHitPos > 0 Then
                strRet &= Mid(strSrc, intPos, intHitPos - intPos)
                intEndPos = InStr(intHitPos + 2, strSrc, "*/")
                If intEndPos < 1 Then   'コメントの終了が見つからない場合、そのまま残りを追加
                    strRet &= Mid(strSrc, intHitPos)
                    Exit Do
                Else
                    If bolKeepLineNo Then
                        strDel = Mid(strSrc, intHitPos, intEndPos - intHitPos + 2)
                        strArr = Split(strDel, vbLf)
                        Dim strRepStr As New String(vbLf, UBound(strArr))
                        strRet &= strRepStr
                    End If
                    intPos = intEndPos + 2
                End If
            Else
                strRet &= Mid(strSrc, intPos)
                Exit Do
            End If
        Loop
        strRet = Regex.Replace(strRet, "--.*\n", vbLf, RegexOptions.Multiline)
        DeleteComment = strRet
    End Function

    Public Function DeleteFormsPropertyInfo(ByVal strSrc As String) As String
        Dim strRet As String = ""

        strRet = Replace(strSrc, "* レコード・グループ問合せ", "")
        strRet = Regex.Replace(strRet, "^[ ]+[*-o][ ][^ ]+[ ]+(SELECT.*)" & vbLf, "$1" & vbLf, RegexOptions.Multiline Or RegexOptions.IgnoreCase)
        strRet = Regex.Replace(strRet, "^[ ]+[*-o\^][ ]オブジェクト・グループの子オブジェクトが指す実オブジェクト\n", ";" & vbLf, RegexOptions.Multiline Or RegexOptions.IgnoreCase)
        strRet = Regex.Replace(strRet, "^[ ]+[*-o\^][ ][^ ]+.*\n", ";" & vbLf, RegexOptions.Multiline Or RegexOptions.IgnoreCase)

        DeleteFormsPropertyInfo = strRet
    End Function

    Public Sub ListItemCheckAll(ByVal objList As ListView, Optional ByVal bolSelect As Boolean = True)
        Dim intCnt As Integer
        For intCnt = 0 To objList.Items.Count - 1
            objList.Items(intCnt).Checked = bolSelect
        Next
    End Sub

    Public Sub InitCommonContextMenu(ByRef objListView As ListView, ByVal strPath As String, Optional ByVal strFile As String = "", Optional ByVal intTableDefIdx As Integer = -1)
        Dim objContainer As New ToolStripMenuItem
        Dim objGroup As ToolStripMenuItem
        Dim objItem As ToolStripItem

        If objListView.ContextMenuStrip Is Nothing Then
            objListView.ContextMenuStrip = New ContextMenuStrip
        End If

        'メニューが初期化済みの場合、初期化を中断する
        For Each objItem In objListView.ContextMenuStrip.Items
            If objItem.Text = "コピー" Then
                Exit Sub
            End If
        Next

        'コピー
        objContainer.Text = "コピー"
        objContainer = objListView.ContextMenuStrip.Items.Add("コピー")
        objItem = objContainer.DropDownItems.Add("選択行")
        objItem.Tag = New MenuContents(objListView, "line", strPath)
        AddHandler objItem.Click, AddressOf CopyClip
        objItem = objContainer.DropDownItems.Add("チェックを付けた行")
        objItem.Tag = New MenuContents(objListView, "checked", strPath)
        AddHandler objItem.Click, AddressOf CopyClip
        objItem = objContainer.DropDownItems.Add("全行")
        objItem.Tag = New MenuContents(objListView, "all", strPath)
        AddHandler objItem.Click, AddressOf CopyClip
        objItem = objContainer.DropDownItems.Add("-")

        Dim intCnt As Integer
        For intCnt = 0 To objListView.Columns.Count - 1
            objItem = objContainer.DropDownItems.Add(objListView.Columns(intCnt).Text)
            objItem.Tag = New MenuContents(objListView, intCnt, strPath)
            AddHandler objItem.Click, AddressOf CopyClip
        Next

        If strPath <> "" Then
            'Grep
            objContainer = New ToolStripMenuItem
            objContainer.Text = "クエリGrep"
            objContainer = objListView.ContextMenuStrip.Items.Add("クエリGrep")
            If strFile <> "" Then
                objGroup = objContainer.DropDownItems.Add("このファイルから")
                For intCnt = 0 To objListView.Columns.Count - 1
                    objItem = objGroup.DropDownItems.Add(objListView.Columns(intCnt).Text)
                    objItem.Tag = New MenuContents(objListView, "file" & vbTab & intCnt, strPath, strFile)
                    AddHandler objItem.Click, AddressOf ListGrep
                Next
                objGroup = objContainer.DropDownItems.Add("このプログラムから")
                For intCnt = 0 To objListView.Columns.Count - 1
                    objItem = objGroup.DropDownItems.Add(objListView.Columns(intCnt).Text)
                    objItem.Tag = New MenuContents(objListView, "program" & vbTab & intCnt, strPath, strFile)
                    AddHandler objItem.Click, AddressOf ListGrep
                Next
            End If
            objGroup = objContainer.DropDownItems.Add("全てのプログラムから")
            For intCnt = 0 To objListView.Columns.Count - 1
                objItem = objGroup.DropDownItems.Add(objListView.Columns(intCnt).Text)
                objItem.Tag = New MenuContents(objListView, "all" & vbTab & intCnt, strPath)
                AddHandler objItem.Click, AddressOf ListGrep
            Next
        End If

        If intTableDefIdx <> -1 Then
            objItem = objListView.ContextMenuStrip.Items.Add("テーブル定義")
            objItem.Tag = New MenuContents(objListView, intTableDefIdx, strPath)
            AddHandler objItem.Click, AddressOf TableDef
        End If

        ''フィルタ
        'objContainer = objListView.ContextMenuStrip.Items.Add("フィルタ")
        'For intCnt = 0 To objListView.Columns.Count - 1
        '    objGroup = objContainer.DropDownItems.Add(objListView.Columns(intCnt).Text)
        '    Dim objStripTextBox As New ToolStripTextBox()
        '    objGroup.DropDownItems.Add(objStripTextBox)
        '    objStripTextBox.Tag = New MenuContents(objListView, intCnt, strPath)
        '    AddHandler objStripTextBox.TextChanged, AddressOf FilterTextChanged
        'Next
        'objGroup = objContainer.DropDownItems.Add("フィルタ解除")
        'objGroup.Tag = New MenuContents(objListView, "", strPath)
        'AddHandler objGroup.Click, AddressOf FilterClear

    End Sub

    Private Sub FilterTextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim objItem As ToolStripItem = sender
        Dim objMenuContents As MenuContents = objItem.Tag
        Dim objListView As ListView = objMenuContents.objListView
        Dim intRow As Integer

        For intRow = 0 To objListView.Items.Count - 1
            If RegMatch(objListView.Items(intRow).SubItems(CInt(objMenuContents.strMenuKind)).Text, sender.text, RegexOptions.IgnoreCase) Then
                objListView.Items(intRow).ForeColor = Color.Black
            Else
                objListView.Items(intRow).ForeColor = Color.White
            End If
        Next

    End Sub

    Private Sub FilterClear(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim objItem As ToolStripItem = sender
        Dim objMenuContents As MenuContents = objItem.Tag
        Dim objListView As ListView = objMenuContents.objListView
        Dim intCnt As Integer

        For intCnt = 0 To objListView.Items.Count - 1
            objListView.Items(intCnt).ForeColor = Color.Black
        Next

    End Sub


    Private Sub CopyClip(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim objItem As ToolStripItem = sender
        Dim objMenuContents As MenuContents = objItem.Tag
        Dim objListView As ListView = objMenuContents.objListView
        Dim intRow As Integer, intCol As Integer
        Dim strWork As String = ""

        On Error Resume Next

        Clipboard.Clear()

        If objMenuContents.strMenuKind = "all" OrElse objMenuContents.strMenuKind = "checked" Then
            For intCol = 0 To objListView.Columns.Count - 1
                If intCol <> 0 Then strWork &= vbTab
                strWork &= objListView.Columns(intCol).Text
            Next
            strWork &= vbCrLf
            For intRow = 0 To objListView.Items.Count - 1
                If objMenuContents.strMenuKind = "all" OrElse objListView.Items(intRow).Checked Then
                    For intCol = 0 To objListView.Columns.Count - 1
                        If intCol <> 0 Then strWork &= vbTab
                        strWork &= objListView.Items(intRow).SubItems(intCol).Text
                    Next
                    strWork &= vbCrLf
                End If
            Next
            Clipboard.SetText(strWork)

        ElseIf objMenuContents.strMenuKind = "line" Then
            If objListView.SelectedItems.Count = 0 Then Exit Sub
            Dim objListItem As ListViewItem
            For Each objListItem In objListView.SelectedItems
                For intCol = 0 To objListView.Columns.Count - 1
                    If intCol <> 0 Then strWork &= vbTab
                    strWork &= objListItem.SubItems(intCol).Text
                Next
                strWork &= vbCrLf
            Next
            Clipboard.SetText(strWork)
        ElseIf IsNumeric(objMenuContents.strMenuKind) Then
                If objListView.SelectedItems.Count = 0 Then Exit Sub
                Clipboard.SetText(objListView.SelectedItems(0).SubItems(CInt(objMenuContents.strMenuKind)).Text)
        End If

    End Sub

    Private Sub ListGrep(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim objItem As ToolStripItem = sender
        Dim objMenuContents As MenuContents = objItem.Tag
        Dim objListView As ListView = objMenuContents.objListView
        Dim intRow As Integer, intCol As Integer
        Dim strWork As String = ""

        On Error Resume Next

        If objListView.SelectedItems.Count = 0 Then Exit Sub
        Dim objForm As New frmGrep
        Dim strArr As String() = Split(objMenuContents.strMenuKind, vbTab)
        Dim strKind As String = strArr(0)
        Dim intIndex As Integer = CInt(strArr(1))
        Select Case strKind
            Case "all"
                objForm.ShowForm(objMenuContents.strSourcePath, "", objListView.SelectedItems(0).SubItems(intIndex).Text, frmGrep.enmGrepScope.AllPrograms)
            Case "program"
                objForm.ShowForm(objMenuContents.strSourcePath, objMenuContents.strFileName, objListView.SelectedItems(0).SubItems(intIndex).Text, frmGrep.enmGrepScope.ThisProgram)
            Case "file"
                objForm.ShowForm(objMenuContents.strSourcePath, objMenuContents.strFileName, objListView.SelectedItems(0).SubItems(intIndex).Text, frmGrep.enmGrepScope.ThisFile)
        End Select
    End Sub

    Private Sub TableDef(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim objItem As ToolStripItem = sender
        Dim objMenuContents As MenuContents = objItem.Tag
        Dim objListView As ListView = objMenuContents.objListView

        If objListView.SelectedItems.Count > 0 Then
            ShowTableDef(objListView.SelectedItems(0).SubItems(CInt(objMenuContents.strMenuKind)).Text, objMenuContents.strSourcePath)
        End If
    End Sub

    Public Sub ShowTableDef(ByVal strTableColumn As String, ByVal strPath As String)
        Dim strArr As String() = Split(strTableColumn, ".")
        If strArr.Length = 2 Then
            frmTableDef.ShowForm(strPath, strArr(0), strArr(1))
        Else
            frmTableDef.ShowForm(strPath, strArr(0))
        End If
    End Sub

    Public Function GetStringArrayByIndex(ByRef strArray As String(), ByVal intIdex As Integer) As String
        GetStringArrayByIndex = ""
        On Error Resume Next
        GetStringArrayByIndex = strArray(intIdex)
    End Function

    Function GetTableDef(ByVal strTableName As String) As clsTableDef
        Dim objTableDef As clsTableDef = Nothing

        If DictExists(dctTableDef, strTableName) Then
            objTableDef = GetDictObject(dctTableDef, strTableName)
        End If
        GetTableDef = objTableDef
    End Function

    Function GetColumnDef(ByVal strTableColumn As String) As clsColumnDef
        Dim strArr As String() = Split(strTableColumn, ".")
        Dim objTableDef As clsTableDef = GetTableDef(strArr(0))
        Dim objColumnDef As clsColumnDef = Nothing

        If Not objTableDef Is Nothing Then
            If DictExists(objTableDef.dctColumnts, strArr(1)) Then
                objColumnDef = GetDictObject(objTableDef.dctColumnts, strArr(1))
            End If
        End If

        GetColumnDef = objColumnDef
    End Function

    Public Function GetLogicalName(ByVal strTableColumnName As String) As String
        Dim strArr As String() = Split(strTableColumnName, ".")
        Dim strRet As String = ""
        Dim strTableName As String = strArr(0)

        If DictExists(dctTableName, strTableName) Then
            strRet = GetDictValue(dctTableName, strTableName)
        Else
            strRet = strTableName
        End If

        If UBound(strArr) > 0 Then
            Dim strColumnName As String = strArr(1)
            Dim strAttributeName As String = strColumnName
            If DictExists(dctTableDef, strTableName) Then
                Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, strTableName)
                If DictExists(objTableDef.dctColumnts, strColumnName) Then
                    strAttributeName = GetDictObject(objTableDef.dctColumnts, strColumnName).AttributeName
                End If
            End If
            strRet &= "." & strAttributeName
        End If
        GetLogicalName = strRet
    End Function

    Public Sub GetLogicalName(ByVal strTableName As String, ByVal strColumnName As String, ByRef strEntityName As String, ByRef strAttributeName As String)
        Dim strEntityAttributeName As String = GetLogicalName(strTableName & "." & strColumnName)

        Dim strArr As String() = Split(strEntityAttributeName, ".")
        strEntityName = strArr(0)
        strAttributeName = strArr(1)
    End Sub


End Module
