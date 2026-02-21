Imports System.Text.RegularExpressions

Public Class frmAnalyzeQuery
    Private strSourcePath As String
    Private strFileName As String
    Private strTableName As String
    Private strAltName As String
    Private lngLineNo As String
    Private ToolTip1 = New ToolTip()
    Private bolInitialyze As Boolean = False
    Private bolDisableDecoText As Boolean = True
    Private bolSelectionChanged As Boolean = False
    Private strBeforeDecoTextTime As String
    Private bolTvBuzy As Boolean = False
    Private bolInitMenu As Boolean = False
    Private intKakkoDecoPos As Integer = -1
    Private objSearch As New frmSearch
    Private strSearchText As String = ""
    Private objSearchMatches As MatchCollection = Nothing
    Private intSearchIdx As Integer

    Public Sub ShowForm(ByVal strPath As String, ByVal strFN As String, ByVal lngLine As Long, ByVal strTable As String, ByVal strAlt As String, Optional ByVal strHighLight1 As String = "", Optional ByVal strHighLight2 As String = "", Optional ByVal strHighLight3 As String = "")
        Dim fso As Scripting.FileSystemObject = New Scripting.FileSystemObject
        Dim t As Scripting.TextStream
        Dim strFile As String
        Dim strLines As String()
        Dim strCols As String()
        Dim lngCnt As Long

        bolInitialyze = True
        strSourcePath = strPath
        strFileName = strFN
        lngLineNo = lngLine
        strTableName = strTable
        strAltName = strAlt
        frmAnalyzeQuery_Load(Nothing, Nothing)
        tvQuery.Nodes.Clear()
        If lngLineNo = 0 Then
            bolInitialyze = False
            Me.Show()
            Exit Sub
        End If

        bolDisableDecoText = True
        Me.Text = strFN & "(" & lngLine & ") ～ クエリの分析"
        txtFileName.Text = strFileName
        txtLineNo.Text = lngLineNo
        txtTableName.Text = strTableName
        txtAltName.Text = strAlt
        txtHighLight1.Text = strHighLight1
        txtHighLight2.Text = strHighLight2
        txtHighLight3.Text = strHighLight3
        cmbQuery.Text = strFN & ":" & lngLine

        t = fso.OpenTextFile(strSourcePath & "\querys\" & strFileName & ".query")
        strFile = t.ReadAll
        strLines = Split(strFile, vbCrLf)
        'クエリを探す
        For lngCnt = LBound(strLines) To UBound(strLines)
            strCols = Split(strLines(lngCnt), vbTab)
            If UBound(strCols) > 0 Then     'クエリ本体の行
                If lngLineNo = strCols(0) Then  '行番号が一致
                    Dim strSQL As String = strCols(1)
                    Dim strFuncProc As String = ""
                    If UBound(strCols) >= 2 Then
                        strFuncProc = strCols(2)
                    End If
                    AnalyzeQuery(strSQL, strFuncProc)
                    Exit Sub
                End If
            End If
        Next
        MsgBox("サブクエリの情報が見つかりませんでした。")
    End Sub

    Private Sub AnalyzeQuery(ByVal strQuery As String, ByVal strFuncProc As String, Optional ByVal bolExpandView As Boolean = False)
        bolTvBuzy = True

        tvQuery.Nodes.Clear()

        strQuery = Replace(strQuery, ",", " , ")
        Dim objRoot As TreeNode = tvQuery.Nodes.Add("〔全体@" & strFuncProc & "〕")
        Dim objRootQuery As New clsQuery
        objRootQuery.Query = strQuery
        objRootQuery.Query = Replace(objRootQuery.Query, ",", " , ")
        objRootQuery.Query = Regex.Replace(objRootQuery.Query, "[ ]+", " ")

        Dim objQuery As New clsQuery
        objRootQuery.dctSubQuerys.Add("%0%", objQuery)
        objQuery.Parent = objRootQuery
        Dim dctE As New Scripting.Dictionary
        AnalyzeCRUD(objRootQuery.Query, objQuery, dctE, "", Nothing, True, bolExpandView)
        objRoot.Tag = objRootQuery

        AddSubQueryNode(objRoot, objQuery)

        objRootQuery.TableC = objQuery.AllTableC
        objRootQuery.TableR = objQuery.AllTableR
        objRootQuery.TableU = objQuery.AllTableU
        objRootQuery.TableD = objQuery.AllTableD
        objRootQuery.ColumnC = objQuery.AllColumnC
        objRootQuery.ColumnR = objQuery.AllColumnR
        objRootQuery.ColumnU = objQuery.AllColumnU
        objRootQuery.ColumnD = objQuery.AllColumnD

        lstCRUD.Visible = False
        AddCRUDList(objQuery.AllTableC, "C")
        AddCRUDList(objQuery.AllTableR, "R")
        AddCRUDList(objQuery.AllTableU, "U")
        AddCRUDList(objQuery.AllTableD, "D")
        lstCRUD.Visible = True

        lstColumnCRUD.Visible = False
        AddColumnCRUDList(objQuery.AllColumnC, "C")
        AddColumnCRUDList(objQuery.AllColumnR, "R")
        AddColumnCRUDList(objQuery.AllColumnU, "U")
        AddColumnCRUDList(objQuery.AllColumnD, "D")
        lstColumnCRUD.Visible = True

        tvQuery.ExpandAll()
        bolTvBuzy = False
        tvQuery.SelectedNode = objRoot

        Me.Show()
        bolDisableDecoText = False
        bolInitialyze = False
        Dim intCnt As Integer
        For intCnt = 0 To lstCRUD.Items.Count - 1
            If lstCRUD.Items(intCnt).SubItems(0).Text = txtTableName.Text Then
                lstCRUD.Items(intCnt).Checked = True
            End If
        Next
        DecoText()
    End Sub

    Private Sub AddSubQueryNode(ByVal objParent As TreeNode, ByVal objQuery As clsQuery)
        Dim strKey As String, strTitle As String = ""
        Dim objChild As TreeNode
        Dim dctAllTable As Scripting.Dictionary = objQuery.AllTable

        If objQuery.AltName <> "" Then
            strTitle = "〔" & objQuery.AltName & "〕 " & objQuery.QueryKind & " "
        ElseIf objQuery.SubQueryIndex = "0" Then
            strTitle = "〔本体〕 " & objQuery.QueryKind & " "
        Else
            strTitle = "〔%" & objQuery.SubQueryIndex & "%〕 " & objQuery.QueryKind & " "
        End If
        For Each strKey In dctAllTable.Keys
            Dim strCol As String() = Split(dctAllTable(strKey), vbTab)
            strTitle &= "【" & GetLogicalName(strCol(0)) & "】"
        Next
        objChild = objParent.Nodes.Add(strTitle)
        objChild.Tag = objQuery

        For Each strKey In objQuery.dctSubQuerys
            AddSubQueryNode(objChild, objQuery.dctSubQuerys(strKey))
        Next
    End Sub

    Public Function FindParentNode(ByVal objNode As TreeNode, ByVal strKey As String) As TreeNode
        Dim objChild As TreeNode

        FindParentNode = Nothing
        If InStr(objNode.Text, strKey) > 0 Then
            FindParentNode = objNode
            Exit Function
        End If
        For Each objChild In objNode.Nodes
            FindParentNode = FindParentNode(objChild, strKey)
            If Not FindParentNode Is Nothing Then Exit For
        Next
    End Function

    Private Sub tvQuery_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvQuery.AfterSelect
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        If bolTvBuzy Then Exit Sub 'ツリービュー構築中はイベント処理しない

        If objQuery.Parent Is Nothing AndAlso objQuery.dctSubQuerys.Count > 0 Then
            ShowQuery(objQuery.dctSubQuerys(objQuery.dctSubQuerys.Keys(0)), True)
        Else
            ShowQuery(objQuery, False)
        End If
    End Sub

    Private Sub ShowQuery(ByVal objQuery As clsQuery, ByVal bolExpand As Boolean)
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        rtbQuery.Visible = False
        rtbQuery.Text = objQuery.Arrange(bolExpand)
        rtbQuery.SelectionStart = 0
        rtbQuery.ScrollToCaret()

        bolDisableDecoText = True
        lstCRUD.Items.Clear()
        lstColumnCRUD.Items.Clear()

        If bolExpand Then
            AddCRUDList(objQuery.AllTableC, "C")
            AddCRUDList(objQuery.AllTableR, "R")
            AddCRUDList(objQuery.AllTableU, "U")
            AddCRUDList(objQuery.AllTableD, "D")

            AddColumnCRUDList(objQuery.AllColumnC, "C")
            AddColumnCRUDList(objQuery.AllColumnR, "R")
            AddColumnCRUDList(objQuery.AllColumnU, "U")
            AddColumnCRUDList(objQuery.AllColumnD, "D")
        Else
            AddCRUDList(objQuery.TableC, "C")
            AddCRUDList(objQuery.TableR, "R")
            AddCRUDList(objQuery.TableU, "U")
            AddCRUDList(objQuery.TableD, "D")

            AddColumnCRUDList(objQuery.ColumnC, "C")
            AddColumnCRUDList(objQuery.ColumnR, "R")
            AddColumnCRUDList(objQuery.ColumnU, "U")
            AddColumnCRUDList(objQuery.ColumnD, "D")
        End If

        bolDisableDecoText = False

        Dim intCnt As Integer
        For intCnt = 0 To lstCRUD.Items.Count - 1
            If lstCRUD.Items(intCnt).SubItems(0).Text = txtTableName.Text Then
                lstCRUD.Items(intCnt).Checked = True
            End If
        Next

        DecoText()
        rtbQuery.Visible = True
        System.Windows.Forms.Cursor.Current = Nothing
        '        rtbQuery.Focus()
    End Sub

    Private Function GetAllNodes(ByVal Nodes As TreeNodeCollection) As Generic.List(Of TreeNode)
        Dim Ar As New Generic.List(Of TreeNode)
        Dim Node As TreeNode

        For Each Node In Nodes
            Ar.Add(Node)
            If Node.GetNodeCount(False) > 0 Then
                Ar.AddRange(GetAllNodes(Node.Nodes))
            End If
        Next

        Return Ar

    End Function

    Sub AddCRUDList(ByVal dct As Scripting.Dictionary, ByVal strCRUD As String)
        Dim strKey As String
        Dim strArr As String()
        Dim strTable As String
        Dim objItem As ListViewItem

        For Each strKey In dct.Items
            strArr = Split(strKey, vbTab)
            strTable = strArr(0)
            objItem = lstCRUD.Items.Add(strTable)
            objItem.Tag = strKey & vbTab & strCRUD
            If DictExists(dctTableName, strTable) Then
                objItem.SubItems.Add(GetDictValue(dctTableName, strTable))
            Else
                objItem.SubItems.Add("")
            End If
            If UBound(strArr) > 0 Then
                objItem.SubItems.Add(strArr(1))
            Else
                objItem.SubItems.Add("")
            End If
            objItem.SubItems.Add(strCRUD)
        Next
    End Sub

    Sub AddColumnCRUDList(ByVal dct As Scripting.Dictionary, ByVal strCRUD As String)
        Dim strKey As String
        Dim strArr As String()
        Dim strTable As String
        Dim strColumn As String
        Dim objItem As ListViewItem

        For Each strKey In dct.Items
            strArr = Split(strKey, ".")
            strTable = strArr(0)
            strColumn = strArr(1)
            objItem = lstColumnCRUD.Items.Add(strTable)
            objItem.Tag = strKey & vbTab & strCRUD
            objItem.SubItems.Add(GetDictValue(dctTableName, strTable))
            objItem.SubItems.Add(strColumn)
            If DictExists(dctTableDef, strTable) Then
                Dim objTalbeDef As clsTableDef = GetDictObject(dctTableDef, strTable)
                If DictExists(objTalbeDef.dctColumnts, strColumn) Then
                    Dim objColumnDef As clsColumnDef = GetDictObject(objTalbeDef.dctColumnts, strColumn)
                    objItem.SubItems.Add(objColumnDef.AttributeName)
                Else
                    objItem.SubItems.Add(strColumn)
                End If
            Else
                objItem.SubItems.Add(strColumn)
            End If
            objItem.SubItems.Add(strCRUD)
        Next
    End Sub

    Sub EnableCRUDList(ByVal dct As Scripting.Dictionary, ByVal strCRUD As String)
        Dim strKey As String
        Dim intCnt As Integer

        For Each strKey In dct.Items
            For intCnt = 0 To lstCRUD.Items.Count - 1
                If lstCRUD.Items(intCnt).Tag = strKey & vbTab & strCRUD Then
                    lstCRUD.Items(intCnt).BackColor = Nothing
                End If
            Next
        Next
    End Sub


    'サブクエリ分解
    Function DivideSubQuery(ByVal sql, ByVal dctSubQuerys)
        Dim lngPos, lngSelectPos, lngWorkPos
        Dim strSubQuery
        Dim lngDepth, lngStartIdx
        Dim lngCnt
        Dim objReg

        objReg = CreateObject("VBScript.RegExp")
        objReg.IgnoreCase = True
        objReg.Global = True

        sql = Trim(sql)
        lngPos = 1
        lngStartIdx = dctSubQuerys.Count

        Do
            lngSelectPos = InStr(lngPos, UCase(sql), " SELECT ")
            If lngSelectPos = 0 Then
                Exit Do
            Else
                'サブクエリの終端を検索
                lngDepth = 1
                lngWorkPos = lngSelectPos + Len(" SELECT ")
                Do
                    Select Case Mid(sql, lngWorkPos, 1)
                        Case "("
                            lngDepth = lngDepth + 1
                        Case ")"
                            lngDepth = lngDepth - 1
                    End Select

                    If lngDepth = 0 Then
                        Exit Do
                    End If

                    If lngWorkPos = Len(sql) Then
                        Exit Do
                    End If

                    lngWorkPos = lngWorkPos + 1
                Loop
                strSubQuery = Trim(Mid(sql, lngSelectPos + 1, lngWorkPos - lngSelectPos))
                Call dctSubQuerys.Add(dctSubQuerys.Count + 1, strSubQuery)

                sql = Mid(sql, 1, lngSelectPos - 2) & "%" & dctSubQuerys.Count & "%" & Mid(sql, lngWorkPos + 1)
                If GetRight(sql, 1) = ")" Then sql = Trim(Mid(sql, 1, Len(sql) - 1))
            End If
        Loop

        'サブクエリ置換部分の補正 TABLE%1%→TABLE %1%
        objReg.Pattern = "(%[0-9]+%)" : sql = objReg.Replace(sql, " $1")


        'サブクエリ内のサブクエリを分解
        For lngCnt = lngStartIdx + 1 To dctSubQuerys.Count
            dctSubQuerys(lngCnt) = DivideSubQuery(dctSubQuerys(lngCnt), dctSubQuerys)
        Next

        DivideSubQuery = sql
    End Function

    '文字列を検索し、みかった場合は探した文字列の次の位置を返す
    Function InStrEx(ByVal lngStart, ByVal strSrc, ByVal strSearch)
        Dim lngPos
        lngPos = InStr(lngStart, strSrc, strSearch)
        If lngPos > 0 Then
            InStrEx = lngPos + Len(strSearch)
        Else
            InStrEx = 0
        End If
    End Function

    Private Sub lstCRUD_ItemChecked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckedEventArgs) Handles lstCRUD.ItemChecked
        If bolInitialyze Or bolDisableDecoText Then Exit Sub

        DecoText()

    End Sub

    Sub DecoText()
        Dim objArrangeQuery As New clsArrangeQuery
        Dim intSelStart As Integer = rtbQuery.SelectionStart
        Dim intSelLength As Integer = rtbQuery.SelectionLength
        Dim strTime = Now.ToString

        If bolDisableDecoText Then Exit Sub
        If strBeforeDecoTextTime = strTime Then Exit Sub

        On Error Resume Next


        rtbQuery.SuspendLayout()
        rtbQuery.SelectionStart = 0
        rtbQuery.SelectionLength = rtbQuery.Text.Length
        Dim fnt As New Font(rtbQuery.SelectionFont.FontFamily, rtbQuery.SelectionFont.Size)
        rtbQuery.SelectionColor = Color.Black : rtbQuery.SelectionBackColor = Color.White : rtbQuery.SelectionFont = fnt

        '        rtbQuery.Text = objArrangeQuery.CArrange(tvQuery.SelectedNode.Tag)
        DecoReserve()
        DecoSelectTable()
        DecoHighLight(Trim(txtHighLight1.Text), txtHighLight1.BackColor)
        DecoHighLight(Trim(txtHighLight2.Text), txtHighLight2.BackColor)
        DecoHighLight(Trim(txtHighLight3.Text), txtHighLight3.BackColor)
        DecoSelectColumn()
        DecoSubquery()

        rtbQuery.SelectionStart = intSelStart
        rtbQuery.SelectionLength = intSelLength

        rtbQuery.ResumeLayout()
    End Sub

    Sub DecoReserve()
        Dim strPattern As String = "\b+ALL\b+|\b+ALTER\b+|\b+AND\b+|\b+ANY\b+|\b+ARRAY\b+|\b+ARROW\b+|\b+AS\b+|\b+ASC\b+|\b+AT\b+|\b+BEGIN\b+|\b+BETWEEN\b+|\b+BY\b+|\b+CASE\b+|\b+CHECK\b+|\b+CLUSTERS\b+|\b+CLUSTER\b+|\b+COLAUTH\b+|\b+COLUMNS\b+|\b+COMPRESS\b+|\b+CONNECT\b+|\b+CRASH\b+|\b+CREATE\b+|\b+CURRENT\b+|\b+DECODE\b+|\b+DECIMAL\b+|\b+DECLARE\b+|\b+DEFAULT\b+|\b+DELETE\b+|\b+DESC\b+|\b+DISTINCT\b+|\b+DROP\b+|\b+ELSE\b+|\b+END\b+|\b+EXCEPTION\b+|\b+EXCLUSIVE\b+|\b+EXISTS\b+|\b+FETCH\b+|\b+FORM\b+|\b+FOR\b+|\b+FROM\b+|\b+GOTO\b+|\b+GRANT\b+|\b+GROUP\b+|\b+HAVING\b+|\b+IDENTIFIED\b+|\b+IF\b+|\b+IN\b+|\b+INDEXES\b+|\b+INDEX\b+|\b+INSERT\b+|\b+INTERSECT\b+|\b+INTO\b+|\b+IS\b+|\b+LIKE\b+|\b+LOCK\b+|\b+MINUS\b+|\b+MODE\b+|\b+NOCOMPRESS\b+|\b+NOT\b+|\b+NOWAIT\b+|\b+NULL\b+|\b+OF\b+|\b+ON\b+|\b+OPTION\b+|\b+OR\b+|\b+ORDER\b+|\b+OVERLAPS\b+|\b+PRIOR\b+|\b+PROCEDURE\b+|\b+PUBLIC\b+|\b+RANGE\b+|\b+RECORD\b+|\b+RESOURCE\b+|\b+REVOKE\b+|\b+SELECT\b+|\b+SET\b+|\b+SHARE\b+|\b+SIZE\b+|\b+SQL\b+|\b+START\b+|\b+SUBTYPE\b+|\b+TABAUTH\b+|\b+TABLE\b+|\b+THEN\b+|\b+TO\b+|\b+TO_CHAR\b+|\b+TO_DATE\b+|\b+TYPE\b+|\b+UNION\b+|\b+UNIQUE\b+|\b+UPDATE\b+|\b+USE\b+|\b+VALUES\b+|\b+VIEW\b+|\b+VIEWS\b+|\b+WHEN\b+|\b+WHERE\b+|\b+WITH\b+|\b+ROWNUM\b+|\b+COUNT\b+|\b+MAX\b+|\b+MIN\b+|\b+SUM\b+|\b+AVR\b+|\b+NVL\b+|\b+SUBSTR\b+|\b+SYSDATE\b+"
        Dim objMatches As MatchCollection
        Dim intCnt As Integer

        On Error Resume Next
        objMatches = Regex.Matches(rtbQuery.Text, strPattern, RegexOptions.IgnoreCase)
        For intCnt = 0 To objMatches.Count - 1
            rtbQuery.SelectionStart = objMatches(intCnt).Index : rtbQuery.SelectionLength = objMatches(intCnt).Length : rtbQuery.SelectionColor = Color.Blue
        Next
    End Sub

    Sub DecoSelectTable()
        Dim objArrangeQuery As New clsArrangeQuery
        Dim objReg As Regex
        Dim m As System.Text.RegularExpressions.MatchCollection
        Dim strSearch As String
        Dim intChk As Integer
        Dim lngCnt As Long
        Dim col As Color

        'rtbQuery.Text = objArrangeQuery.CArrange(tvQuery.SelectedNode.Tag)
        On Error Resume Next
        For intChk = 0 To lstCRUD.Items.Count - 1
            lstCRUD.Items(intChk).ForeColor = Color.Black
        Next
        For intChk = 0 To lstCRUD.CheckedItems.Count - 1
            col = GetColor(intChk)
            lstCRUD.CheckedItems(intChk).ForeColor = col

            strSearch = "\b+(" & lstCRUD.CheckedItems(intChk).SubItems(0).Text & ")\b+"
            m = Regex.Matches(rtbQuery.Text, strSearch, RegexOptions.IgnoreCase)
            For lngCnt = 0 To m.Count - 1
                Dim fnt As New Font(rtbQuery.SelectionFont.FontFamily, rtbQuery.SelectionFont.Size, rtbQuery.SelectionFont.Style Or FontStyle.Bold)
                rtbQuery.SelectionStart = m(lngCnt).Groups(1).Index : rtbQuery.SelectionLength = m(lngCnt).Groups(1).Length : rtbQuery.SelectionColor = col : rtbQuery.SelectionFont = fnt
            Next

            If lstCRUD.CheckedItems(intChk).SubItems(2).Text <> "" Then
                strSearch = "\b*(" & lstCRUD.CheckedItems(intChk).SubItems(2).Text & ")\b*"
                m = Regex.Matches(rtbQuery.Text, strSearch, RegexOptions.IgnoreCase)
                For lngCnt = 0 To m.Count - 1
                    Dim fnt As Font = New Font(rtbQuery.SelectionFont.FontFamily, rtbQuery.SelectionFont.Size, rtbQuery.SelectionFont.Style Or FontStyle.Bold)
                    rtbQuery.SelectionStart = m(lngCnt).Groups(1).Index : rtbQuery.SelectionLength = m(lngCnt).Groups(1).Length : rtbQuery.SelectionColor = col : rtbQuery.SelectionFont = fnt
                Next

            End If
        Next

    End Sub

    Sub DecoSelectColumn()
        Dim objArrangeQuery As New clsArrangeQuery
        Dim objReg As Regex
        Dim m As System.Text.RegularExpressions.MatchCollection
        Dim strSearch As String
        Dim intChk As Integer
        Dim lngCnt As Long

        'rtbQuery.Text = objArrangeQuery.CArrange(tvQuery.SelectedNode.Tag)

        For intChk = 0 To lstColumnCRUD.CheckedItems.Count - 1
            strSearch = lstColumnCRUD.CheckedItems(intChk).SubItems(2).Text
            objReg = New System.Text.RegularExpressions.Regex(" (" & strSearch & ") ", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            m = Regex.Matches(rtbQuery.Text, strSearch)
            For lngCnt = 0 To m.Count - 1
                rtbQuery.SelectionStart = m(lngCnt).Groups(0).Index : rtbQuery.SelectionLength = Len(strSearch) : rtbQuery.SelectionColor = Color.White : rtbQuery.SelectionBackColor = Color.Black
            Next
        Next

    End Sub

    Sub DecoHighLight(ByVal strPattern As String, ByVal col As Color)
        Dim objMatches As MatchCollection
        Dim intCnt As Integer

        If strPattern = "" Then Exit Sub
        On Error Resume Next
        objMatches = Regex.Matches(rtbQuery.Text, strPattern, RegexOptions.IgnoreCase)
        For intCnt = 0 To objMatches.Count - 1
            rtbQuery.SelectionStart = objMatches(intCnt).Index : rtbQuery.SelectionLength = objMatches(intCnt).Length : rtbQuery.SelectionBackColor = col
        Next
    End Sub

    Sub DecoSubquery()
        Dim objMatches As MatchCollection
        Dim intCnt As Integer

        On Error Resume Next
        objMatches = Regex.Matches(rtbQuery.Text, "[%][0-9]+[%]", RegexOptions.IgnoreCase)
        Dim fnt As Font = New Font(rtbQuery.SelectionFont.FontFamily, rtbQuery.SelectionFont.Size, rtbQuery.SelectionFont.Style Or FontStyle.Underline)
        For intCnt = 0 To objMatches.Count - 1
            rtbQuery.SelectionStart = objMatches(intCnt).Index : rtbQuery.SelectionLength = objMatches(intCnt).Length : rtbQuery.SelectionColor = Color.Blue : rtbQuery.SelectionFont = fnt
        Next
    End Sub
    Function GetColor(ByVal lngIndex As Long) As Color
        Dim col As Color
        Select Case lngIndex Mod 20
            Case 0 : col = Color.Red
            Case 1 : col = Color.Green
            Case 2 : col = Color.Blue
            Case 3 : col = Color.Brown
            Case 4 : col = Color.Magenta
            Case 5 : col = Color.BlueViolet
            Case 6 : col = Color.CadetBlue
            Case 7 : col = Color.DarkOrange
            Case 8 : col = Color.DarkTurquoise
            Case 9 : col = Color.DarkSlateBlue
            Case 10 : col = Color.OliveDrab
            Case 11 : col = Color.DarkCyan
            Case 12 : col = Color.Indigo
            Case 13 : col = Color.DarkMagenta
            Case 14 : col = Color.OrangeRed
            Case 15 : col = Color.DimGray
            Case 16 : col = Color.Chocolate
            Case 17 : col = Color.RoyalBlue
            Case 18 : col = Color.DeepPink
            Case 19 : col = Color.SpringGreen
        End Select
        GetColor = col
    End Function

    Private Sub cmdRunHidemaru_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRunHidemaru.Click
        If txtFileName.Text = "" Then Exit Sub

        RunTextEditor(strSourcePath & "\" & txtFileName.Text, txtLineNo.Text, txtTableName.Text, txtAltName.Text)
    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

    Private Sub cmdSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim objItem As ListViewItem
        bolDisableDecoText = True
        For Each objItem In lstCRUD.Items
            objItem.Checked = True
        Next
        bolDisableDecoText = False
        DecoText()
    End Sub

    Private Sub btnDeselectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim objItem As ListViewItem
        bolDisableDecoText = True
        For Each objItem In lstCRUD.Items
            objItem.Checked = False
        Next
        bolDisableDecoText = False
        DecoText()
    End Sub

    Private Sub DecoKakko(ByVal intKakkoPos As Integer)
        Dim objMathes As MatchCollection = Regex.Matches(rtbQuery.Text, "[(]|[)]")
        Dim intCnt As Integer
        Dim intDepth As Integer = 0
        Dim intStart As Integer
        Dim intTo As Integer
        Dim intStep As Integer
        Dim intFind As Integer

        For intCnt = 0 To objMathes.Count - 1
            If objMathes(intCnt).Value = "(" Then
                intDepth += 1
                intStep = 1
                intTo = objMathes.Count - 1
            Else
                intDepth -= 1
                intStep = -1
                intTo = 0
            End If

            If objMathes(intCnt).Index = intKakkoPos Then
                intStart = intCnt + intStep
                intFind = intDepth + (intStep * -1)
                Exit For
            End If
        Next

        For intCnt = intStart To intTo Step intStep
            If objMathes(intCnt).Value = "(" Then
                intDepth += 1
            Else
                intDepth -= 1
            End If
            If intDepth = intFind Then
                Dim intSelStart As Integer = rtbQuery.SelectionStart
                Dim intSelLength As Integer = rtbQuery.SelectionLength

                rtbQuery.SuspendLayout()
                rtbQuery.SelectionStart = objMathes(intCnt).Index
                rtbQuery.SelectionLength = 1
                rtbQuery.SelectionBackColor = Color.Cyan
                rtbQuery.SelectionStart = intSelStart
                rtbQuery.SelectionLength = intSelLength
                rtbQuery.ResumeLayout()
                intKakkoDecoPos = objMathes(intCnt).Index
                Exit For
            End If
        Next
    End Sub

    Private Function GetCursorTableName() As String
        Dim lngStart As Long, lngEnd As Long, lngWork As Long
        Dim strWord As String
        Dim strArr As String()
        Dim strTableName As String
        Dim objItem As ListViewItem

        GetCursorTableName = ""
        On Error Resume Next
        If rtbQuery.SelectionStart = 0 Then Exit Function
        If lstCRUD.SelectedItems.Count Then lstCRUD.SelectedItems(0).Selected = False
        For Each objItem In lstCRUD.Items
            objItem.BackColor = Nothing
        Next

        '単語の開始位置を探す
        For lngWork = rtbQuery.SelectionStart To 0 Step -1
            Select Case Mid(rtbQuery.Text, lngWork, 1)
                Case " ", ",", "(", ")", "|", "+", "-", "*", "/", "<", ">", "=", ";", vbCr, vbLf
                    lngStart = lngWork + 1
                    Exit For
                Case Else
            End Select
        Next

        '単語の終了位置を探す
        lngEnd = Len(rtbQuery.Text)
        For lngWork = rtbQuery.SelectionStart + 1 To Len(rtbQuery.Text) Step 1
            Select Case Mid(rtbQuery.Text, lngWork, 1)
                Case " ", ",", "(", ")", "|", "+", "-", "*", "/", "<", ">", "=", ";", vbCr, vbLf
                    lngEnd = lngWork - 1
                    Exit For
                Case Else
            End Select
        Next
        If lngStart >= lngEnd Then Exit Function

        strWord = Mid(rtbQuery.Text, lngStart, lngEnd - lngStart + 1)
        strArr = Split(strWord, ".")
        If UBound(strArr) > 0 Then
            strTableName = strArr(0)
        Else
            strTableName = strWord
        End If

        GetCursorTableName = strTableName

    End Function

    Private Sub GetCursorTableColumnName(ByRef strTableName As String, ByRef strColumnName As String, Optional ByVal bolSelect As Boolean = True)
        Dim lngStart As Long, lngEnd As Long, lngWork As Long
        Dim strWord As String
        Dim strArr As String()
        Dim objItem As ListViewItem

        On Error Resume Next
        If rtbQuery.SelectionStart < 0 Then Exit Sub
        If lstCRUD.SelectedItems.Count Then lstCRUD.SelectedItems(0).Selected = False
        For Each objItem In lstCRUD.Items
            objItem.BackColor = Nothing
        Next

        '単語の開始位置を探す
        lngStart = 1
        For lngWork = rtbQuery.SelectionStart To 0 Step -1
            Select Case Mid(rtbQuery.Text, lngWork, 1)
                Case " ", ",", "(", ")", "|", "+", "-", "*", "/", "<", ">", "=", ";", vbCr, vbLf
                    lngStart = lngWork + 1
                    Exit For
                Case Else
            End Select
        Next

        '単語の終了位置を探す
        lngEnd = Len(rtbQuery.Text)
        For lngWork = rtbQuery.SelectionStart + 1 To Len(rtbQuery.Text) Step 1
            Select Case Mid(rtbQuery.Text, lngWork, 1)
                Case " ", ",", "(", ")", "|", "+", "-", "*", "/", "<", ">", "=", ";", vbCr, vbLf
                    lngEnd = lngWork - 1
                    Exit For
                Case Else
            End Select
        Next
        If lngStart >= lngEnd Then Exit Sub

        If bolSelect Then
            rtbQuery.SelectionStart = lngStart - 1
            rtbQuery.SelectionLength = lngEnd - lngStart + 1
        End If

        strWord = Mid(rtbQuery.Text, lngStart, lngEnd - lngStart + 1)
        strArr = Split(strWord, ".")
        If UBound(strArr) > 0 Then
            strTableName = strArr(0)
            strColumnName = strArr(1)
        Else
            strTableName = strWord
            strColumnName = ""
        End If

    End Sub

    Private Function GetCursorWord() As String
        Dim lngStart As Long, lngEnd As Long, lngWork As Long
        Dim strWord As String
        Dim strArr As String()
        Dim objItem As ListViewItem

        On Error Resume Next
        If rtbQuery.SelectionLength > 0 Then
            GetCursorWord = rtbQuery.SelectedText
            Exit Function
        End If

        '単語の開始位置を探す
        For lngWork = rtbQuery.SelectionStart To 0 Step -1
            Select Case Mid(rtbQuery.Text, lngWork, 1)
                Case " ", ",", "(", ")", "|", "+", "-", "*", "/", "<", ">", "=", ";", ".", vbCr, vbLf
                    lngStart = lngWork + 1
                    Exit For
                Case Else
            End Select
        Next

        '単語の終了位置を探す
        lngEnd = Len(rtbQuery.Text)
        For lngWork = rtbQuery.SelectionStart + 1 To Len(rtbQuery.Text) Step 1
            Select Case Mid(rtbQuery.Text, lngWork, 1)
                Case " ", ",", "(", ")", "|", "+", "-", "*", "/", "<", ">", "=", ";", ".", vbCr, vbLf
                    lngEnd = lngWork - 1
                    Exit For
                Case Else
            End Select
        Next
        If lngStart >= lngEnd Then
            GetCursorWord = ""
            Exit Function
        End If

        rtbQuery.SelectionStart = lngStart - 1
        rtbQuery.SelectionLength = lngEnd - lngStart + 1
        strWord = Mid(rtbQuery.Text, lngStart, lngEnd - lngStart + 1)
        GetCursorWord = strWord
    End Function


    Private Sub tvQuery_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles tvQuery.KeyDown
        If e.KeyCode = Keys.Enter Then
            rtbQuery.Focus()
            If lstCRUD.SelectedItems.Count = 0 Then
                If lstCRUD.Items.Count > 0 Then lstCRUD.Items(0).Selected = True
            End If
            e.Handled = True
        End If
    End Sub

    Private Sub frmAnalyzeQuery_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        tvQuery.Focus()
    End Sub

    Private Sub rtbQuery_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rtbQuery.DoubleClick
        On Error Resume Next
        Dim strTableName As String = "", strColumnName As String = ""
        GetCursorTableColumnName(strTableName, strColumnName)
        Dim objMatche As MatchCollection = Regex.Matches(strTableName, "^[%][^%]+[%]$", RegexOptions.IgnoreCase)
        Dim objNode As TreeNode

        If objMatche.Count = 1 Then 'サブクエリ部分をダブルクリックした場合
            objNode = FindSubquery(tvQuery.Nodes(0), strTableName)
            tvQuery.SelectedNode = objNode
            On Error Resume Next
            tvQuery_AfterSelect(Nothing, Nothing)
            Exit Sub
        End If

        Dim objItem As ListViewItem
        For Each objItem In lstCRUD.Items
            If strTableName = objItem.SubItems(2).Text Then strTableName = objItem.SubItems(0).Text
        Next

        If DictExists(dctTableDef, strTableName) Then
            Dim objForm As New frmTableDef
            objForm.ShowForm(strSourcePath, strTableName, strColumnName, rtbQuery)
        End If

    End Sub

    Private Function FindSubquery(ByVal objNode As TreeNode, ByVal strSubQueryName As String) As TreeNode
        FindSubquery = Nothing
        Dim objQuery As clsQuery = objNode.Tag

        If objQuery.SubQueryIndex = Replace(strSubQueryName, "%", "") Then
            FindSubquery = objNode
            Exit Function
        End If
        Dim objChild As TreeNode
        For Each objChild In objNode.Nodes
            FindSubquery = FindSubquery(objChild, strSubQueryName)
            If Not FindSubquery Is Nothing Then Exit For
        Next
    End Function


    Private Sub txtHighLight_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtHighLight1.TextChanged, txtHighLight3.TextChanged, txtHighLight2.TextChanged
        DecoText()
    End Sub


    Private Sub btnTableDef_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If lstCRUD.SelectedItems.Count > 0 Then
            Dim objForm As New frmTableDef
            objForm.ShowForm(strSourcePath, lstCRUD.SelectedItems(0).SubItems(0).Text)
        End If
    End Sub

    Private Sub lstColumnCRUD_ItemChecked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ItemCheckedEventArgs) Handles lstColumnCRUD.ItemChecked
        If bolInitialyze Or bolDisableDecoText Then Exit Sub

        DecoText()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim objItem As ListViewItem
        bolDisableDecoText = True
        For Each objItem In lstColumnCRUD.Items
            objItem.Checked = True
        Next
        bolDisableDecoText = False
        DecoText()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim objItem As ListViewItem
        bolDisableDecoText = True
        For Each objItem In lstColumnCRUD.Items
            objItem.Checked = False
        Next
        bolDisableDecoText = False
        DecoText()
    End Sub


    Private Sub btnQuickAnalyze_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuickAnalyze.Click
        If bolDemoFlag Then MsgBox("本機能は評価版ではご利用になれません。") : Exit Sub

        On Error Resume Next

        Dim strSQL As String = DeleteComment(rtbQuery.Text, False)

        strSQL = Replace(strSQL, ";", " ")
        strSQL = Replace(strSQL, vbCr, " ")
        strSQL = Replace(strSQL, vbLf, " ")
        strSQL = Replace(strSQL, "(", " ( ")
        strSQL = Replace(strSQL, ")", " ) ")
        strSQL = Regex.Replace(strSQL, "[ ]+", " ")
        AnalyzeQuery(strSQL, "クエリ解析")
    End Sub

    Private Sub btnDivideStrings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDivideStrings.Click
        If bolDemoFlag Then MsgBox("本機能は評価版ではご利用になれません。") : Exit Sub

        On Error Resume Next

        Dim strSQL As String = DeleteComment(rtbQuery.Text, False)
        Dim objMatches As MatchCollection = Regex.Matches(strSQL, "[']([^']*)[']")

        If objMatches.Count = 0 Then
            MsgBox("文字列が抽出できませんでした。")
            Exit Sub
        End If

        Dim strResult As String = ""
        Dim intCnt As Integer

        For intCnt = 0 To objMatches.Count - 1
            strResult &= objMatches(intCnt).Groups(1).Value & " "
        Next
        strResult = Regex.Replace(Trim(strResult), "[ ]+", " ")
        Dim objArrangeQuery As New clsArrangeQuery
        rtbQuery.Text = objArrangeQuery.CArrange(strResult)
    End Sub

    Private Sub tsmOtherCRUD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmOtherCRUD.Click
        Dim strTableName As String = "", strColumnName As String = ""
        Dim strEntityName As String = "", strAttributeName As String = ""
        Dim objItem As ListViewItem

        On Error Resume Next
        If bolInitialyze Or bolDisableDecoText Then Exit Sub
        ToolTip1.settooltip(rtbQuery, Nothing)

        GetCursorTableColumnName(strTableName, strColumnName)

        For Each objItem In lstCRUD.Items
            If objItem.SubItems(0).Text = strTableName Or objItem.SubItems(2).Text = strTableName Then
                objItem.BackColor = Color.Yellow
            End If
            If strTableName = objItem.SubItems(2).Text Then
                strTableName = objItem.SubItems(0).Text
            End If
        Next

        frmCRUDSearch.ShowForm(strSourcePath, strTableName, strColumnName)

    End Sub

    Private Sub frmAnalyzeQuery_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        rtbQuery.LanguageOption = RichTextBoxLanguageOptions.UIFonts
        objSettings.LoadFormSize(Me)

        If cmbQuery.Items.Count = 0 Then
            Dim strKey As String
            For Each strKey In dctQueryList.Keys
                cmbQuery.Items.Add(Replace(strKey, vbTab, ":"))
            Next
        End If

        If Not bolInitMenu Then
            InitCommonContextMenu(lstCRUD, strSourcePath, strFileName)
            InitCommonContextMenu(lstColumnCRUD, strSourcePath, strFileName)
        End If

        bolInitMenu = True

    End Sub

    Private Sub cmbQuery_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbQuery.SelectedIndexChanged
        If bolInitialyze Then Exit Sub

        bolDisableDecoText = True
        rtbQuery.Text = Split(dctQueryList(Replace(cmbQuery.Text, ":", vbTab)), vbTab)(0)
        Dim strCol As String() = Split(cmbQuery.Text, ":")
        txtFileName.Text = strCol(0)
        txtLineNo.Text = strCol(1)
        btnQuickAnalyze_Click(Nothing, Nothing)
        bolDisableDecoText = False
        DecoText()
    End Sub

    Private Sub GrepToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GrepToolStripMenuItem.Click
        GetCursorWord()
    End Sub

    Private Sub GrepFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GrepFileToolStripMenuItem.Click
        Dim strWord As String = GetCursorWord()
        If strWord = "" Then Exit Sub

        Dim objForm As New frmGrep
        objForm.ShowForm(strSourcePath, txtFileName.Text, strWord, frmGrep.enmGrepScope.ThisFile)
    End Sub

    Private Sub GrepProgramToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GrepProgramToolStripMenuItem.Click
        Dim strWord As String = GetCursorWord()
        If strWord = "" Then Exit Sub

        Dim objForm As New frmGrep
        objForm.ShowForm(strSourcePath, txtFileName.Text, strWord, frmGrep.enmGrepScope.ThisProgram)

    End Sub

    Private Sub GrepAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GrepAllToolStripMenuItem.Click
        Dim strWord As String = GetCursorWord()
        If strWord = "" Then Exit Sub

        Dim objForm As New frmGrep
        objForm.ShowForm(strSourcePath, txtFileName.Text, strWord, frmGrep.enmGrepScope.AllPrograms)

    End Sub

    Private Sub cmdExpandView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExpandView.Click
        If bolDemoFlag Then MsgBox("本機能は評価版ではご利用になれません。") : Exit Sub

        On Error Resume Next

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
        Dim strSQL As String = " " & DeleteComment(rtbQuery.Text, False) & " "
        rtbQuery.Visible = False

        strSQL = Replace(strSQL, vbCr, " ")
        strSQL = Replace(strSQL, vbLf, " ")
        strSQL = Replace(strSQL, "(", " ( ")
        strSQL = Replace(strSQL, ")", " ) ")
        strSQL = Regex.Replace(strSQL, "[ ]+", " ")

        Dim arrKu As String() = SplitKu(strSQL) '句の分解

        If arrKu.Length = 0 Then
            rtbQuery.Visible = True
            Exit Sub
        End If
        Dim intCnt As Integer
        Dim strFrom As String
        For intCnt = 0 To (UBound(arrKu) - 1) / 2
            If UCase(arrKu(intCnt * 2)) = "FROM" Then  'FROM句
                strFrom = arrKu(intCnt * 2 + 1)
                Dim strTables As String() = Split(strFrom, ",") 'テーブルに分解
                Dim intCnt2 As Integer
                For intCnt2 = 0 To UBound(strTables)
                    Dim strArr As String() = Split(Trim(strTables(intCnt2)), " ")   'テーブル名・別名に分解
                    Dim strViewName As String = strArr(0)


                    Dim objView As clsView
                    Dim strViewSql As String = ""
                    For Each objView In colViews
                        If objView.ViewName = strViewName Then
                            strViewSql = objView.Query  ' ViewのSQL
                            Exit For
                        End If
                    Next

                    If strViewSql <> "" Then
                        strSQL = Replace(strSQL, " " & Replace(strViewName, "/", "") & " ", " (" & strViewSql & ") ")
                    End If
                Next
            End If
        Next
        rtbQuery.Text = Trim(strSQL)
        btnQuickAnalyze_Click(Nothing, Nothing)
        rtbQuery.Visible = True
        System.Windows.Forms.Cursor.Current = Nothing
    End Sub

    Private Sub cmdExchangeLogicalName_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExchangeLogicalName.Click
        If bolDemoFlag Then MsgBox("本機能は評価版ではご利用になれません。") : Exit Sub

        On Error Resume Next

        Dim strSql As String = DeleteComment(rtbQuery.Text, False)
        If strSql = "" Then Exit Sub
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor

        strSql = Replace(strSql, vbCr, " ")
        strSql = Replace(strSql, vbLf, " ")
        strSql = Replace(strSql, "(", " ( ")
        strSql = Replace(strSql, ")", " ) ")
        strSql = Regex.Replace(strSql, "[ ]+", " ")

        Dim objQuery As New clsQuery
        Dim dctE As New Scripting.Dictionary
        AnalyzeCRUD(strSql, objQuery, dctE, "", Nothing, True, False)

        ExchangeLogicalName(objQuery)
        strSql = objQuery.Query
        ExpandSubQuery(strSql, objQuery)

        AnalyzeQuery(strSql, "クエリ解析")
        System.Windows.Forms.Cursor.Current = Nothing
    End Sub

    Private Sub ExpandSubQuery(ByRef strSql As String, ByRef objQuery As clsQuery)
        Dim strKey As String

        For Each strKey In objQuery.dctSubQuerys.Keys
            If InStr(objQuery.QueryKind, "UNION") > 0 OrElse InStr(objQuery.QueryKind, "MINUS") > 0 OrElse InStr(objQuery.QueryKind, "INTERSECT") > 0 Then
                strSql = Replace(strSql, strKey, objQuery.dctSubQuerys(strKey).Query)
            Else
                strSql = Replace(strSql, strKey, " ( " & objQuery.dctSubQuerys(strKey).Query & ") ")
            End If
            ExpandSubQuery(strSql, objQuery.dctSubQuerys(strKey))
        Next
    End Sub


    Private Sub ExchangeLogicalName(ByRef objQuery As clsQuery)
        Dim strKey As String
        Dim dctAllTables As Scripting.Dictionary = objQuery.AllTable(False) '下位は展開しない
        Dim dctAllColumns As Scripting.Dictionary = objQuery.AllColumn(False) '下位は展開しない

        '別名を正式名に変換
        For Each strKey In dctAllTables.Keys
            Dim strArr As String() = Split(dctAllTables(strKey), vbTab)
            If UBound(strArr) > 0 Then
                Dim strTableName As String = strArr(0)
                Dim strAlt As String = strArr(1)
                objQuery.Query = Regex.Replace(objQuery.Query, "( )" & EscapeRegular(strAlt) & "[.]", "$1" & strTableName & ".")
            End If
        Next

        'テーブル.カラムの論理名変換
        For Each strKey In dctAllColumns.Keys
            Dim strTableColumnName As String = dctAllColumns(strKey)
            Dim strEntityAttributeName As String = GetLogicalName(strTableColumnName)
            objQuery.Query = Regex.Replace(objQuery.Query, "( )" & EscapeRegular(strTableColumnName) & "( )", "$1" & strEntityAttributeName & "$2")
        Next

        'サブクエリの処理
        For Each strKey In objQuery.dctSubQuerys.Keys
            ExchangeLogicalName(objQuery.dctSubQuerys(strKey))
        Next

        'テーブルの論理名変換
        For Each strKey In dctAllTables.Keys
            Dim strArr As String() = Split(dctAllTables(strKey), vbTab)
            Dim strTableName As String = strArr(0)
            objQuery.Query = Regex.Replace(objQuery.Query, "( )" & EscapeRegular(strTableName) & "( )", "$1" & GetLogicalName(strTableName) & "$2")
            If UBound(strArr) > 0 AndAlso strArr(1) <> "" Then
                Dim strAlt As String = strArr(1)
                objQuery.Query = Regex.Replace(objQuery.Query, "( )" & EscapeRegular(strAlt) & "( )", "$1" & GetLogicalName(strTableName) & "$2")
            End If

            'カラム名のみの論理名変換
            If DictExists(dctTableDef, strTableName) Then
                Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, strTableName)
                Dim strColumnName As String
                For Each strColumnName In objTableDef.dctColumnts.Keys
                    Dim objColumnDef As clsColumnDef = GetDictObject(objTableDef.dctColumnts, strColumnName)
                    objQuery.Query = Regex.Replace(objQuery.Query, "([ .])" & EscapeRegular(objColumnDef.ColumnName) & "( )", "$1" & GetLogicalName(objColumnDef.AttributeName) & "$2")
                    '                   objQuery.Query = Regex.Replace(objQuery.Query, "(\s*|[ ])" & EscapeRegular(objColumnDef.ColumnName) & "(\s*)", "$1" & GetLogicalName(objColumnDef.AttributeName) & "$2")
                Next

            End If
        Next

    End Sub

    Private Sub HighLight1ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HighLight1ToolStripMenuItem.Click
        Dim strWord As String = GetCursorWord()
        If strWord = "" Then Exit Sub

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
        rtbQuery.Visible = False

        txtHighLight1.Text = strWord

        rtbQuery.Visible = True
        System.Windows.Forms.Cursor.Current = Nothing
    End Sub

    Private Sub HighLight2ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HighLight2ToolStripMenuItem.Click
        Dim strWord As String = GetCursorWord()
        If strWord = "" Then Exit Sub

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
        rtbQuery.Visible = False

        txtHighLight2.Text = strWord

        rtbQuery.Visible = True
        System.Windows.Forms.Cursor.Current = Nothing
    End Sub

    Private Sub HighLight3ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HighLight3ToolStripMenuItem.Click
        Dim strWord As String = GetCursorWord()
        If strWord = "" Then Exit Sub

        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
        rtbQuery.Visible = False

        txtHighLight3.Text = strWord

        rtbQuery.Visible = True
        System.Windows.Forms.Cursor.Current = Nothing
    End Sub

    Private Sub HighLightClearToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HighLightClearToolStripMenuItem.Click
        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
        rtbQuery.Visible = False

        txtHighLight1.Text = ""
        txtHighLight2.Text = ""
        txtHighLight3.Text = ""

        rtbQuery.Visible = True
        System.Windows.Forms.Cursor.Current = Nothing
    End Sub

    Private Sub 全選択ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 全選択ToolStripMenuItem.Click
        ListItemCheckAll(lstCRUD, True)
    End Sub

    Private Sub 全解除ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles 全解除ToolStripMenuItem.Click
        ListItemCheckAll(lstCRUD, False)
    End Sub

    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
        ListItemCheckAll(lstColumnCRUD, True)
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        ListItemCheckAll(lstColumnCRUD, False)
    End Sub

    Private Sub テーブル定義ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles テーブル定義ToolStripMenuItem.Click
        If lstCRUD.SelectedItems.Count > 0 Then
            Dim objForm As New frmTableDef
            objForm.ShowForm(strSourcePath, lstCRUD.SelectedItems(0).SubItems(0).Text, , rtbQuery)
        End If
    End Sub

    Private Sub tsmExpandSubQuery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmExpandSubQuery.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        ShowQuery(tvQuery.SelectedNode.Tag, True)
    End Sub

    Private Sub テーブル定義ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles テーブル定義ToolStripMenuItem1.Click
        If lstColumnCRUD.SelectedItems.Count > 0 Then
            Dim objForm As New frmTableDef
            objForm.ShowForm(strSourcePath, lstColumnCRUD.SelectedItems(0).SubItems(0).Text, lstColumnCRUD.SelectedItems(0).SubItems(2).Text, rtbQuery)
        End If
    End Sub

    Private Sub lstCRUD_ItemSelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ListViewItemSelectionChangedEventArgs) Handles lstCRUD.ItemSelectionChanged
        Dim intCnt As Integer
        If lstCRUD.SelectedItems.Count = 0 Then Exit Sub
        For intCnt = 0 To lstColumnCRUD.Items.Count - 1
            If lstCRUD.SelectedItems(0).Text = lstColumnCRUD.Items(intCnt).Text Then
                lstColumnCRUD.Items(intCnt).BackColor = Color.LightBlue
            Else
                lstColumnCRUD.Items(intCnt).BackColor = Color.White
            End If
        Next
    End Sub

    Private Sub ShowColumnInfo(ByVal colColumn As ColumnCollection)
        Dim strWork As String = ""
        Dim objColumn As Column
        Dim strLine As String
        Dim dctDuplicateCheck As New Scripting.Dictionary
        strWork = "エンティティ名" & vbTab & "属性名" & vbTab & "テーブル名" & vbTab & "#SEQ" & vbTab & "カラム名" & vbTab & "句" & vbCrLf
        For Each objColumn In colColumn
            Dim strEntityName As String = "", strAttributeName As String = ""
            GetLogicalName(objColumn.Table, objColumn.ColumnName, strEntityName, strAttributeName)
            Dim objColumnDef As clsColumnDef = GetColumnDef(objColumn.Table & "." & objColumn.ColumnName)
            Dim strSEQ As String
            If objColumnDef Is Nothing Then
                strSEQ = ""
            Else
                strSEQ = objColumnDef.SEQ
            End If
            strLine = strEntityName & vbTab & strAttributeName & vbTab & objColumn.Table & vbTab & strSEQ & vbTab & objColumn.ColumnName & vbTab & objColumn.KuName
            If Not dctDuplicateCheck.Exists(strLine) Then
                strWork &= strLine & vbCrLf
                dctDuplicateCheck.Add(strLine, "")
            End If
        Next
        Dim objForm As New frmList
        objForm.ShowForm("カラムアクセス一覧", strWork, strSourcePath, strFileName)
        '        Clipboard.SetText(strWork)
    End Sub

    Private Sub SelectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmSelect.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumnSelect, objQuery.ColumnSelect))
    End Sub

    Private Sub WhereToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmWhere.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumnWhere, objQuery.ColumnWhere))
    End Sub

    Private Sub GroupByToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmGroupBy.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumnGroupBy, objQuery.ColumnGroupBy))
    End Sub

    Private Sub OrderByToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmOrderBy.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumnOrderBy, objQuery.ColumnOrderBy))
    End Sub

    Private Sub HavingToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmHaving.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumnHaving, objQuery.ColumnHaving))
    End Sub

    Private Sub InsertToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmInsert.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumnInsert, objQuery.ColumnInsert))
    End Sub

    Private Sub UpdateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmUpdate.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumnUpdate, objQuery.ColumnUpdate))
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmDelete.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumnDelete, objQuery.ColumnDelete))
    End Sub

    Private Sub SetCondToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmSetCond.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumnSetCond, objQuery.ColumnSetCond))
    End Sub

    Private Sub tsmAllKu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmAllKu.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        ShowColumnInfo(IIf(objQuery.Parent Is Nothing, objQuery.AllColumn2(True), objQuery.AllColumn2(False)))
    End Sub

    Private Sub 句の情報ToolStripMenuItem_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles 句の情報ToolStripMenuItem.Paint
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        If objQuery.Parent Is Nothing Then
            tsmSelect.Enabled = IIf(objQuery.AllColumnSelect.Count > 0, True, False)
            tsmWhere.Enabled = IIf(objQuery.AllColumnWhere.Count > 0, True, False)
            tsmGroupBy.Enabled = IIf(objQuery.AllColumnGroupBy.Count > 0, True, False)
            tsmOrderBy.Enabled = IIf(objQuery.AllColumnOrderBy.Count > 0, True, False)
            tsmHaving.Enabled = IIf(objQuery.AllColumnHaving.Count > 0, True, False)
            tsmInsert.Enabled = IIf(objQuery.AllColumnInsert.Count > 0, True, False)
            tsmUpdate.Enabled = IIf(objQuery.AllColumnUpdate.Count > 0, True, False)
            tsmDelete.Enabled = IIf(objQuery.AllColumnDelete.Count > 0, True, False)
            tsmSetCond.Enabled = IIf(objQuery.AllColumnSetCond.Count > 0, True, False)
        Else
            tsmSelect.Enabled = IIf(objQuery.ColumnSelect.Count > 0, True, False)
            tsmWhere.Enabled = IIf(objQuery.ColumnWhere.Count > 0, True, False)
            tsmGroupBy.Enabled = IIf(objQuery.ColumnGroupBy.Count > 0, True, False)
            tsmOrderBy.Enabled = IIf(objQuery.ColumnOrderBy.Count > 0, True, False)
            tsmHaving.Enabled = IIf(objQuery.ColumnHaving.Count > 0, True, False)
            tsmInsert.Enabled = IIf(objQuery.ColumnInsert.Count > 0, True, False)
            tsmUpdate.Enabled = IIf(objQuery.ColumnUpdate.Count > 0, True, False)
            tsmDelete.Enabled = IIf(objQuery.ColumnDelete.Count > 0, True, False)
            tsmSetCond.Enabled = IIf(objQuery.ColumnSetCond.Count > 0, True, False)
        End If

        tsmIntoValues.Enabled = IIf(objQuery.Values.Count > 0, True, False)
        tsmSetValues.Enabled = IIf(objQuery.SetValues.Count > 0, True, False)
        tsmSelects.Enabled = IIf(objQuery.Selects.Count > 0, True, False)
    End Sub

    Private Sub tsmIntoValues_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmIntoValues.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        Dim strKey As String
        Dim strWork As String = ""
        Dim intIdx As Integer = 0

        On Error Resume Next

        strWork = "#" & vbTab & "エンティティ名" & vbTab & "属性名" & vbTab & "テーブル名" & vbTab & "カラム名" & vbTab & "VALUES句" & vbCrLf
        For Each strKey In objQuery.ColumnC.Keys
            intIdx += 1
            Dim strEntityName As String = "", strAttributeName As String = ""
            Dim strArr As String() = Split(strKey, ".")
            Dim strTable As String = strArr(0), strColumn As String = strArr(1)
            GetLogicalName(strTable, strColumn, strEntityName, strAttributeName)
            strWork &= intIdx & vbTab & strEntityName & vbTab & strAttributeName & vbTab & strTable & vbTab & strColumn & vbTab & GetDictValue(objQuery.Values, "K" & intIdx) & vbCrLf
        Next
        Dim objForm As New frmList
        objForm.ShowForm("INTO句とVALUES句の対応", strWork, strSourcePath, strFileName)
    End Sub

    Private Sub このテーブルにアクセスしている処理ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles このテーブルにアクセスしている処理ToolStripMenuItem.Click
        If lstCRUD.SelectedItems.Count = 0 Then Exit Sub

        Dim objForm As New frmCRUDSearch
        objForm.ShowForm(strSourcePath, lstCRUD.SelectedItems(0).SubItems(0).Text, "")
    End Sub

    Private Sub このテーブルにアクセスしている処理ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles このテーブルにアクセスしている処理ToolStripMenuItem1.Click
        If lstColumnCRUD.SelectedItems.Count = 0 Then Exit Sub

        Dim objForm As New frmCRUDSearch
        objForm.ShowForm(strSourcePath, lstColumnCRUD.SelectedItems(0).SubItems(0).Text, "")
    End Sub

    Private Sub このカラムにアクセスしている処理ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles このカラムにアクセスしている処理ToolStripMenuItem.Click
        If lstColumnCRUD.SelectedItems.Count = 0 Then Exit Sub

        Dim objForm As New frmCRUDSearch
        objForm.ShowForm(strSourcePath, lstColumnCRUD.SelectedItems(0).SubItems(0).Text, lstColumnCRUD.SelectedItems(0).SubItems(2).Text)
    End Sub

    Private Sub frmAnalyzeQuery_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs)
        objSettings.SaveFormSize(Me)
    End Sub

    Private Sub SET句のカラムと値の対応ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmSetValues.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        Dim strKey As String
        Dim strWork As String = ""
        Dim intIdx As Integer = 0

        On Error Resume Next

        strWork = "#" & vbTab & "エンティティ名" & vbTab & "属性名" & vbTab & "テーブル名" & vbTab & "カラム名" & vbTab & "SET句" & vbCrLf
        For Each strKey In objQuery.ColumnU.Keys
            intIdx += 1
            Dim strEntityName As String = "", strAttributeName As String = ""
            Dim strArr As String() = Split(strKey, ".")
            Dim strTable As String = strArr(0), strColumn As String = strArr(1)
            GetLogicalName(strTable, strColumn, strEntityName, strAttributeName)
            strWork &= intIdx & vbTab & strEntityName & vbTab & strAttributeName & vbTab & strTable & vbTab & strColumn & vbTab & GetDictValue(objQuery.SetValues, "K" & intIdx) & vbCrLf
        Next
        Dim objForm As New frmList
        objForm.ShowForm("SET句のカラムと値の対応", strWork, strSourcePath, strFileName)
    End Sub


    Private Sub SELECT句の別名と式対応ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmSelects.Click
        If tvQuery.SelectedNode Is Nothing Then Exit Sub
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        Dim strKey As String
        Dim strWork As String = ""
        Dim intIdx As Integer = 0

        On Error Resume Next

        strWork = "#" & vbTab & "別名" & vbTab & "式" & vbTab & "展開" & vbCrLf
        For Each strKey In objQuery.Selects.Keys
            intIdx += 1
            Dim strSelect As String = GetDictValue(objQuery.Selects, strKey)
            strWork &= intIdx & vbTab & strKey & vbTab & strSelect & vbTab & objQuery.ExpandSelect(strSelect) & vbCrLf
        Next
        Dim objForm As New frmList
        objForm.ShowForm("SELECT句の別名と式の対応", strWork, strSourcePath, strFileName)
    End Sub

    Private Sub rtbQuery_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles rtbQuery.KeyDown
        If (e.Control And e.KeyCode = Keys.F) Or (e.KeyCode = Keys.F3 And objSearchMatches Is Nothing) Then
            TextSearch()
        ElseIf e.KeyCode = Keys.F3 Then
            If Not e.Shift Then
                SearchNext()
            Else
                SearchPrev()
            End If
        End If

    End Sub

    Private Sub TextSearch()
        rtbQuery.Focus()
        objSearch.ShowDialog()
        If objSearch.bolAccept = False OrElse objSearch.txtSearch.Text = "" Then Exit Sub
        strSearchText = objSearch.txtSearch.Text
        Try
            objSearchMatches = Regex.Matches(rtbQuery.Text, IIf(objSearch.chkRegular.Checked, strSearchText, EscapeRegular(strSearchText)), RegexOptions.IgnoreCase)
            If objSearchMatches.Count = 0 Then
                MsgBox("検索文字列がみつかりません")
                objSearchMatches = Nothing
                Exit Sub
            End If
            intSearchIdx = objSearchMatches.Count - 1
            SearchNext()
        Catch
            MsgBox("正規表現が不正です")
        End Try
    End Sub

    Private Sub SearchNext()
        If strSearchText = "" Then
            TextSearch()
            Exit Sub
        End If
        If objSearchMatches Is Nothing Then Exit Sub
        rtbQuery.Focus()
        Dim intStartIdx As Integer = intSearchIdx
        intSearchIdx += 1
        If (objSearchMatches.Count - 1) < intSearchIdx Then intSearchIdx = 0

        rtbQuery.SelectionStart = objSearchMatches(intSearchIdx).Index
        rtbQuery.SelectionLength = objSearchMatches(intSearchIdx).Length
    End Sub

    Private Sub SearchPrev()
        If strSearchText = "" Then
            TextSearch()
            Exit Sub
        End If
        If objSearchMatches Is Nothing Then Exit Sub
        rtbQuery.Focus()
        Dim intStartIdx As Integer = intSearchIdx
        intSearchIdx -= 1
        If intSearchIdx < 0 Then intSearchIdx = objSearchMatches.Count - 1

        rtbQuery.SelectionStart = objSearchMatches(intSearchIdx).Index
        rtbQuery.SelectionLength = objSearchMatches(intSearchIdx).Length
    End Sub

    Private Sub btnTableDef_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTableDef.Click
        Dim objForm As New frmTableDef
        objForm.ShowForm(strSourcePath, strTableName, , rtbQuery)
    End Sub

    Private Sub rtbQuery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rtbQuery.Click
        TextDeco()
    End Sub

    Private Sub rtbQuery_KeyUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles rtbQuery.KeyUp
        TextDeco()
    End Sub

    Private Sub TextDeco()
        Dim strTableName As String = "", strColumnName As String = ""
        Dim strEntityName As String = "", strAttributeName As String = ""
        Dim objItem As ListViewItem

        On Error Resume Next
        If bolSelectionChanged Then Exit Sub
        If bolInitialyze Or bolDisableDecoText Then Exit Sub
        bolSelectionChanged = True
        ToolTip1.settooltip(rtbQuery, Nothing)

        '対応する括弧の修飾を解除
        If intKakkoDecoPos <> -1 Then
            Dim intSelStart As Integer = rtbQuery.SelectionStart
            Dim intSelLength As Integer = rtbQuery.SelectionLength
            rtbQuery.SelectionStart = intKakkoDecoPos
            rtbQuery.SelectionLength = 1
            rtbQuery.SelectionBackColor = Color.White
            rtbQuery.SelectionStart = intSelStart
            rtbQuery.SelectionLength = intSelLength
            intKakkoDecoPos = -1
        End If

        If Mid(rtbQuery.Text, rtbQuery.SelectionStart + 1, 1) = "(" Or Mid(rtbQuery.Text, rtbQuery.SelectionStart + 1, 1) = ")" Then
            DecoKakko(rtbQuery.SelectionStart)
        End If

        GetCursorTableColumnName(strTableName, strColumnName, False)

        For Each objItem In lstCRUD.Items
            If objItem.SubItems(0).Text = strTableName Or objItem.SubItems(2).Text = strTableName Then
                objItem.BackColor = Color.Yellow
            End If
            If strTableName = objItem.SubItems(2).Text Then
                strTableName = objItem.SubItems(0).Text
            End If
        Next
        Dim objMatche As MatchCollection = Regex.Matches(strTableName, "^[%][0-9]+[%]$", RegexOptions.IgnoreCase)
        If objMatche.Count > 0 Then
            ToolTip1.InitialDelay = 0
            'ToolTipが表示されている時に、別のToolTipを表示するまでの時間
            ToolTip1.ReshowDelay = 0
            'ToolTipを表示する時間
            ToolTip1.AutoPopDelay = 10000
            ToolTip1.ShowAlways = True
            ToolTip1.SetToolTip(rtbQuery, FindSubquery(tvQuery.Nodes(0), strTableName).Tag.Query)
        End If

        Dim strGuide As String = ""
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        Dim strWork As String = strTableName
        If strColumnName <> "" Then
            If strWork <> "" Then strWork &= "."
            strWork &= strColumnName
        End If
        strGuide = objQuery.ExpandSelect(strWork)

        bolSelectionChanged = False
        txtGuide.Text = strGuide

    End Sub

    Private Sub txtGuide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtGuide.Click
        txtGuide.SelectionStart = 0
        txtGuide.SelectionLength = txtGuide.Text.Length
    End Sub

    Private Sub tsmTableDef_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmTableDef.Click
        Dim strWork As String = GetCursorWord()
        If strWork = "" Then Exit Sub
        Dim strRealTableColumnName As String = GetRealName(strWork) & "."
        Dim strArr As String() = Split(strRealTableColumnName, ".")
        If strRealTableColumnName = "" Then Exit Sub
        Dim objForm As New frmTableDef
        objForm.ShowForm(strSourcePath, strArr(0), strArr(1), rtbQuery)
    End Sub

    Public Function GetRealName(ByVal strTableColumn As String) As String
        Dim objQuery As clsQuery = tvQuery.SelectedNode.Tag
        GetRealName = ""
        Dim strKey As String

        Dim strArr As String() = Split(strTableColumn, ".")

        Select Case strArr.Length
            Case 1
                'テーブルから探す
                If DictExists(dctTableName, strArr(0)) Then
                    GetRealName = strArr(0)
                    Exit Function
                End If

                '各テーブルのテーブル定義中からカラムを探す
                Dim dctAllTable As Scripting.Dictionary = objQuery.AllTable
                For Each strKey In dctAllTable
                    Dim strTableName As String = Split(dctAllTable(strKey), vbTab)(0)
                    If DictExists(dctTableDef, strTableName) Then
                        Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, strTableName)
                        If DictExists(objTableDef.dctColumnts, strArr(0)) Then
                            GetRealName = strTableName & "." & strArr(0)
                            Exit Function
                        End If
                    End If
                Next

            Case 2
                Dim strTableName As String = ""
                Dim strColumnName As String = ""

                ''テーブル名を探す
                If DictExists(dctTableName, strArr(0)) Then
                    strTableName = strArr(0)
                Else
                    '別名として探す
                    Dim dctAllTable As Scripting.Dictionary = objQuery.AllTable

                    For Each strKey In dctAllTable
                        Dim strArr2 As String() = Split(GetDictValue(dctAllTable, strKey), vbTab)
                        If strArr2(1) = strArr(0) Then  '別名と一致
                            strTableName = strArr2(0)
                            Exit For
                        End If
                    Next
                End If

                If strTableName <> "" Then          'テーブル名を解決できている場合
                    strColumnName = strArr(1)
                    GetRealName = strTableName & "." & strColumnName
                    Exit Function
                End If

        End Select

        GetRealName = ""

    End Function

    Private Sub btnTextSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTextSearch.Click
        TextSearch()
    End Sub

    Private Sub btnSearchNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearchNext.Click
        SearchNext()
    End Sub

    Private Sub btnSearchPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearchPrev.Click
        SearchPrev()
    End Sub

    Private Sub rtbQuery_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles rtbQuery.KeyPress
        If Not Char.IsControl(e.KeyChar) Then
            Dim fnt As New Font(rtbQuery.SelectionFont.FontFamily, rtbQuery.SelectionFont.Size)
            rtbQuery.SelectionColor = Color.Black : rtbQuery.SelectionBackColor = Color.White : rtbQuery.SelectionFont = fnt
        End If
    End Sub
End Class