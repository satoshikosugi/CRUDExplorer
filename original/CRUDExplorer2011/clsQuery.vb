Imports System.Text.RegularExpressions

Public Class clsQuery
    Public QueryKind As String
    Public Query As String
    Public SubQueryIndex As String = "0"
    Public AltName As String
    Public TableC As Scripting.Dictionary
    Public TableR As Scripting.Dictionary
    Public TableU As Scripting.Dictionary
    Public TableD As Scripting.Dictionary
    Public ColumnC As Scripting.Dictionary
    Public ColumnR As Scripting.Dictionary
    Public ColumnU As Scripting.Dictionary
    Public ColumnD As Scripting.Dictionary
    Public ColumnSelect As New ColumnCollection
    Public ColumnWhere As New ColumnCollection
    Public ColumnOrderBy As New ColumnCollection
    Public ColumnGroupBy As New ColumnCollection
    Public ColumnHaving As New ColumnCollection
    Public ColumnUpdate As New ColumnCollection
    Public ColumnSetCond As New ColumnCollection
    Public ColumnInsert As New ColumnCollection
    Public ColumnDelete As New ColumnCollection
    Public Withs As New Scripting.Dictionary
    Public Values As Scripting.Dictionary       'VALUES句をカンマで分解して管理
    Public SetValues As Scripting.Dictionary    'SET句の値をカンマで分解して管理
    Public Selects As Scripting.Dictionary      'SELECT句の値をカンマで分解して管理(キーは別名)

    Public dctSubQuerys As Scripting.Dictionary
    Public Parent As clsQuery

    Public FileName As String
    Public LineNo As Integer

    Public Sub New()
        TableC = New Scripting.Dictionary
        TableR = New Scripting.Dictionary
        TableU = New Scripting.Dictionary
        TableD = New Scripting.Dictionary
        ColumnC = New Scripting.Dictionary
        ColumnR = New Scripting.Dictionary
        ColumnU = New Scripting.Dictionary
        ColumnD = New Scripting.Dictionary
        dctSubQuerys = New Scripting.Dictionary
        Values = New Scripting.Dictionary
        SetValues = New Scripting.Dictionary
        Selects = New Scripting.Dictionary
    End Sub

    Public Function AllTable(Optional ByVal bolExpand As Boolean = False) As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "TableC", bolExpand)
        AllTableSub(Me, dct, "TableR", bolExpand)
        AllTableSub(Me, dct, "TableU", bolExpand)
        AllTableSub(Me, dct, "TableD", bolExpand)
        AllTable = dct
    End Function

    Public Function AllTableC() As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "TableC", True)
        AllTableC = dct
    End Function

    Public Function AllTableR() As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "TableR", True)
        AllTableR = dct
    End Function

    Public Function AllTableU() As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "TableU", True)
        AllTableU = dct
    End Function

    Public Function AllTableD() As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "TableD", True)
        AllTableD = dct
    End Function

    Public Function AllColumn(Optional ByVal bolExpand As Boolean = False) As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "ColumnC", bolExpand)
        AllTableSub(Me, dct, "ColumnR", bolExpand)
        AllTableSub(Me, dct, "ColumnU", bolExpand)
        AllTableSub(Me, dct, "ColumnD", bolExpand)
        AllColumn = dct
    End Function

    Public Function AllColumn2(Optional ByVal bolExpand As Boolean = False) As ColumnCollection
        Dim colColumn As New ColumnCollection
        AllColumnSub(Me, colColumn, "Select", bolExpand)
        AllColumnSub(Me, colColumn, "Where", bolExpand)
        AllColumnSub(Me, colColumn, "GroupBy", bolExpand)
        AllColumnSub(Me, colColumn, "OrderBy", bolExpand)
        AllColumnSub(Me, colColumn, "Having", bolExpand)
        AllColumnSub(Me, colColumn, "Insert", bolExpand)
        AllColumnSub(Me, colColumn, "Update", bolExpand)
        AllColumnSub(Me, colColumn, "SetCond", bolExpand)
        AllColumnSub(Me, colColumn, "Delete", bolExpand)
        AllColumn2 = colColumn
    End Function

    Public Function AllColumnC() As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "ColumnC", True)
        AllColumnC = dct
    End Function

    Public Function AllColumnR() As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "ColumnR", True)
        AllColumnR = dct
    End Function

    Public Function AllColumnU() As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "ColumnU", True)
        AllColumnU = dct
    End Function

    Public Function AllColumnD() As Scripting.Dictionary
        Dim dct As New Scripting.Dictionary
        AllTableSub(Me, dct, "ColumnD", True)
        AllColumnD = dct
    End Function

    Private Sub AllTableSub(ByVal objQuery As clsQuery, ByVal dct As Scripting.Dictionary, ByVal Kind As String, ByVal bolExpand As Boolean)
        Dim dctFrom As Scripting.Dictionary = Nothing
        Dim strKey As String

        Select Case Kind
            Case "TableC" : dctFrom = objQuery.TableC
            Case "TableR" : dctFrom = objQuery.TableR
            Case "TableU" : dctFrom = objQuery.TableU
            Case "TableD" : dctFrom = objQuery.TableD
            Case "ColumnC" : dctFrom = objQuery.ColumnC
            Case "ColumnR" : dctFrom = objQuery.ColumnR
            Case "ColumnU" : dctFrom = objQuery.ColumnU
            Case "ColumnD" : dctFrom = objQuery.ColumnD
        End Select

        For Each strKey In dctFrom.Keys
            dct.Add("K" & dct.Count + 1, dctFrom(strKey))
        Next

        'サブクエリ
        For Each strKey In objQuery.dctSubQuerys.Keys
            Dim objSubQuery As clsQuery = objQuery.dctSubQuerys(strKey)
            If bolExpand Then AllTableSub(objSubQuery, dct, Kind, bolExpand)
        Next
    End Sub

    Public Function AllColumnSelect() As ColumnCollection
        Dim colCollection As New ColumnCollection
        AllColumnSub(Me, colCollection, "Select", True)
        AllColumnSelect = colCollection
    End Function

    Public Function AllColumnWhere() As ColumnCollection
        Dim colCollection As New ColumnCollection
        AllColumnSub(Me, colCollection, "Where", True)
        AllColumnWhere = colCollection
    End Function

    Public Function AllColumnGroupBy() As ColumnCollection
        Dim colCollection As New ColumnCollection
        AllColumnSub(Me, colCollection, "GroupBy", True)
        AllColumnGroupBy = colCollection
    End Function

    Public Function AllColumnOrderBy() As ColumnCollection
        Dim colCollection As New ColumnCollection
        AllColumnSub(Me, colCollection, "OrderBy", True)
        AllColumnOrderBy = colCollection
    End Function

    Public Function AllColumnHaving() As ColumnCollection
        Dim colCollection As New ColumnCollection
        AllColumnSub(Me, colCollection, "Having", True)
        AllColumnHaving = colCollection
    End Function

    Public Function AllColumnInsert() As ColumnCollection
        Dim colCollection As New ColumnCollection
        AllColumnSub(Me, colCollection, "Insert", True)
        AllColumnInsert = colCollection
    End Function

    Public Function AllColumnUpdate() As ColumnCollection
        Dim colCollection As New ColumnCollection
        AllColumnSub(Me, colCollection, "Update", True)
        AllColumnUpdate = colCollection
    End Function

    Public Function AllColumnSetCond() As ColumnCollection
        Dim colCollection As New ColumnCollection
        AllColumnSub(Me, colCollection, "SetCond", True)
        AllColumnSetCond = colCollection
    End Function

    Public Function AllColumnDelete() As ColumnCollection
        Dim colCollection As New ColumnCollection
        AllColumnSub(Me, colCollection, "Delete", True)
        AllColumnDelete = colCollection
    End Function

    Public Function AllSubQuerys() As Scripting.Dictionary
        Dim dctAllSubQuerys As New Scripting.Dictionary
        AllSubQuerysCore(dctAllSubQuerys, Me)
        AllSubQuerys = dctAllSubQuerys
    End Function

    Private Sub AllSubQuerysCore(ByRef dctAllSubQuerys As Scripting.Dictionary, ByRef objQuery As clsQuery)
        Dim strKey As String
        For Each strKey In objQuery.dctSubQuerys
            DictAdd(dctAllSubQuerys, "K" & dctAllSubQuerys.Count + 1, objQuery.dctSubQuerys(strKey))
        Next
        For Each strKey In objQuery.dctSubQuerys
            AllSubQuerysCore(dctAllSubQuerys, objQuery.dctSubQuerys(strKey))
        Next
    End Sub


    Private Sub AllColumnSub(ByVal objQuery As clsQuery, ByVal colColumn As ColumnCollection, ByVal Kind As String, ByVal bolExpand As Boolean)
        Dim colFrom As ColumnCollection = Nothing
        Dim strKey As String

        Select Case Kind
            Case "Select" : colFrom = objQuery.ColumnSelect
            Case "Where" : colFrom = objQuery.ColumnWhere
            Case "GroupBy" : colFrom = objQuery.ColumnGroupBy
            Case "OrderBy" : colFrom = objQuery.ColumnOrderBy
            Case "Having" : colFrom = objQuery.ColumnHaving
            Case "Insert" : colFrom = objQuery.ColumnInsert
            Case "Update" : colFrom = objQuery.ColumnUpdate
            Case "SetCond" : colFrom = objQuery.ColumnSetCond
            Case "Delete" : colFrom = objQuery.ColumnDelete
        End Select

        Dim objColumn As Column
        For Each objColumn In colFrom
            colColumn.add(objColumn)
        Next

        'サブクエリ
        For Each strKey In objQuery.dctSubQuerys.Keys
            Dim objSubQuery As clsQuery = objQuery.dctSubQuerys(strKey)
            If bolExpand Then AllColumnSub(objSubQuery, colColumn, Kind, bolExpand)
        Next
    End Sub

    Public Sub DumpTableR(ByVal objQuery As clsQuery)
        Dim strKey As String

        Debug.Print("SQL: " & objQuery.Query)
        For Each strKey In objQuery.AllTableR.Keys
            Debug.Print(strKey & ": " & objQuery.AllTableR(True)(strKey))
        Next

        'サブクエリ
        For Each strKey In objQuery.dctSubQuerys.Keys
            Dim objSubQuery = objQuery.dctSubQuerys(strKey)
            DumpTableR(objSubQuery)
        Next
    End Sub

    Public Function Arrange(Optional ByVal bolExpand As Boolean = False) As String
        Dim objArrangeQuery As New clsArrangeQuery

        Dim strQuery As String = Split(Query, vbTab)(0)

        strQuery = objArrangeQuery.CArrange(strQuery)

        If bolExpand Then
            Dim strKey As String
            For Each strKey In dctSubQuerys.Keys
                Dim objSubQuery As clsQuery = dctSubQuerys(strKey)
                Dim strSubQuery As String
                strSubQuery = objSubQuery.Arrange(bolExpand)

                Dim intSubQueryPos As Integer = InStr(strQuery, strKey)
                If intSubQueryPos > 0 Then
                    Dim intLineTop As Integer = InStrRev(strQuery, vbLf, intSubQueryPos)
                    If intLineTop <= 1 Then intLineTop = 1
                    '空白以外の文字までの位置
                    Dim objMaches As MatchCollection = Regex.Matches(Mid(strQuery, intLineTop), "[^ ]")
                    Dim intIndent As Integer
                    If objMaches.Count = 0 Then
                        intIndent = 4
                    Else
                        intIndent = 4 + objMaches(0).Index
                    End If
                    '                    Dim intIndent As Integer = intSubQueryPos - intLineTop

                    'サブクエリにインデントを付加
                    Dim strLines As String() = Split(strSubQuery, vbCrLf)
                    If InStr(QueryKind, "UNION") = 0 And InStr(QueryKind, "MINUS") = 0 And InStr(QueryKind, "INTERSECT") = 0 Then
                        strSubQuery = "("
                    Else
                        strSubQuery = ""
                    End If
                    Dim intCnt As Integer
                    For intCnt = LBound(strLines) To UBound(strLines)
                        If intCnt <> LBound(strLines) Then
                            Dim strIndent As New String(" ", intIndent)
                            strLines(intCnt) = strIndent & strLines(intCnt)
                        End If
                        If intCnt <> LBound(strLines) Then strSubQuery &= vbCrLf
                        strSubQuery &= strLines(intCnt)
                    Next
                    If InStr(QueryKind, "UNION") = 0 And InStr(QueryKind, "MINUS") = 0 And InStr(QueryKind, "INTERSECT") = 0 Then
                        strSubQuery &= ")"
                    End If
                    'サブクエリを置換
                    strQuery = Replace(strQuery, strKey, strSubQuery)
                End If
            Next

        End If

        Arrange = strQuery
    End Function

    Public Function AllWiths() As Scripting.Dictionary
        Dim dctWiths As New Scripting.Dictionary
        Dim objNode As clsQuery

        objNode = Me
        Do Until objNode.Parent Is Nothing
            objNode = objNode.Parent
        Loop

        AllWithsCore(objNode, dctWiths)

        AllWiths = dctWiths
    End Function

    Private Sub AllWithsCore(ByRef objNode As clsQuery, ByRef dctWiths As Scripting.Dictionary)
        Dim strKey As String

        For Each strKey In objNode.Withs.Keys
            dctWiths.Add(strKey, objNode.Withs(strKey))
        Next

        For Each strKey In objNode.dctSubQuerys.Keys
            AllWithsCore(objNode.dctSubQuerys(strKey), dctWiths)
        Next

    End Sub

    Public Function ExpandSelect(ByVal strSelect As String) As String
        Dim strArr As String() = Split(strSelect, " ")
        Dim intCnt As Integer
        Dim strRet As String = ""

        For intCnt = 0 To strArr.Length - 1
            If strRet <> "" Then strRet &= " "
            Dim strFullName As String = FindFullName(strArr(intCnt))
            strRet &= strFullName
        Next
        ExpandSelect = strRet

    End Function

    '別名をテーブルやサブクエリを展開して探す
    Public Function FindFullName(ByVal strTableColumn As String) As String
        FindFullName = ""
        Dim strKey As String

        Dim strArr As String() = Split(strTableColumn, ".")

        Select Case strArr.Length
            Case 1
                'テーブルから探す
                If DictExists(dctTableName, strArr(0)) Then
                    FindFullName = GetDictValue(dctTableName, strArr(0))
                    Exit Function
                End If

                '各テーブルのテーブル定義中からカラムを探す
                Dim dctAllTable As Scripting.Dictionary = Me.AllTable
                For Each strKey In dctAllTable
                    Dim strTableName As String = Split(dctAllTable(strKey), vbTab)(0)
                    If DictExists(dctTableDef, strtablename) Then
                        Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, strTableName)
                        If DictExists(objTableDef.dctColumnts, strArr(0)) Then
                            Dim objColumnDef As clsColumnDef = GetDictObject(objTableDef.dctColumnts, strArr(0))
                            If DictExists(dctTableName, strTableName) Then
                                FindFullName = GetDictValue(dctTableName, strTableName) & "."
                            Else
                                FindFullName = strTableName & "."
                            End If
                            If objColumnDef.AttributeName <> "" Then
                                FindFullName &= objColumnDef.AttributeName
                            Else
                                FindFullName &= objColumnDef.ColumnName
                            End If
                            Exit Function
                        End If
                    End If
                Next

                'サブクエリから探す
                Dim dctAllSubQuerys As Scripting.Dictionary = Me.AllSubQuerys
                For Each strKey In dctAllSubQuerys
                    Dim objSubQuery As clsQuery = dctAllSubQuerys(strKey)
                    Dim strKey2 As String
                    For Each strKey2 In objSubQuery.Selects
                        If strKey2 = strArr(0) Then     'サブクエリ内のSELECT句に発見
                            Dim strRefer As String = GetDictValue(objSubQuery.Selects, strKey2)
                            Dim strReferLogical As String = objSubQuery.ExpandSelect(strRefer)
                            FindFullName = IIf(strReferLogical <> "", strReferLogical, strRefer)
                            Exit Function
                        End If
                    Next
                Next

            Case 2
                Dim strTableName As String = ""
                Dim strEntityName As String = ""
                Dim strColumnName As String = ""
                Dim strAttributeName As String = ""

                ''テーブル名を探す
                If DictExists(dctTableName, strArr(0)) Then
                    strTableName = strArr(0)
                    strEntityName = GetDictValue(dctTableName, strArr(0))
                Else
                    '別名として探す
                    Dim dctAllTable As Scripting.Dictionary = Me.AllTable

                    For Each strKey In dctAllTable
                        Dim strArr2 As String() = Split(GetDictValue(dctAllTable, strKey), vbTab)
                        If strArr2(1) = strArr(0) Then  '別名と一致
                            strTableName = strArr2(0)
                            strEntityName = strArr2(0)
                            If DictExists(dctTableName, strTableName) Then
                                strEntityName = GetDictValue(dctTableName, strTableName)
                            End If
                            Exit For
                        End If
                    Next
                End If

                If strTableName <> "" Then          'テーブル名を解決できている場合
                    strColumnName = strArr(1)
                    strAttributeName = strArr(1)
                    If DictExists(dctTableDef, strTableName) Then
                        Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, strTableName)
                        If DictExists(objTableDef.dctColumnts, strColumnName) Then
                            For Each strKey In objTableDef.dctColumnts
                                Dim objColumnDef As clsColumnDef = GetDictObject(objTableDef.dctColumnts, strKey)
                                If objColumnDef.ColumnName = strColumnName Then
                                    strAttributeName = objColumnDef.AttributeName
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                    FindFullName = strEntityName & "." & strAttributeName
                    Exit Function
                End If

                'サブクエリから探す
                Dim dctAllSubQuerys As Scripting.Dictionary = Me.AllSubQuerys
                For Each strKey In dctAllSubQuerys
                    Dim objSubQuery As clsQuery = dctAllSubQuerys(strKey)
                    If objSubQuery.AltName = strArr(0) Then             'サブクエリ名と一致
                        If DictExists(objSubQuery.Selects, strArr(1)) Then     ''★★★本当は再帰的に展開したい！！
                            Dim strRefer As String = GetDictValue(objSubQuery.Selects, strArr(1))
                            Dim strReferFullName As String = objSubQuery.ExpandSelect(strRefer)
                            If strReferFullName <> "" Then
                                FindFullName = strReferFullName
                            Else
                                FindFullName = strRefer
                            End If
                            Exit Function
                        End If
                    End If
                Next
        End Select

        FindFullName = strTableColumn

    End Function

End Class
