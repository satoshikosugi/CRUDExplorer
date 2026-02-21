Imports System.Text.RegularExpressions

Module CommonAnalyze

    Public Sub AnalyzeCRUD(ByVal sql As String, ByRef objQuery As clsQuery, ByRef dctE As Scripting.Dictionary, ByVal strFileName As String, ByVal tsub As Scripting.TextStream, Optional ByVal bolExpandView As Boolean = False, Optional ByVal bolCheckReferenceCond As Boolean = False)
        Dim dctSubQuerys As New Scripting.Dictionary
        sql = Regex.Replace(sql, "[(]", " ( ", RegexOptions.IgnoreCase)
        sql = Regex.Replace(sql, "[)]", " ) ", RegexOptions.IgnoreCase)
        sql = Regex.Replace(sql, "[,]", " , ", RegexOptions.IgnoreCase)
        sql = Regex.Replace(sql, "[|][|]", " || ", RegexOptions.IgnoreCase)
        sql = Regex.Replace(sql, "[\t]", " ", RegexOptions.IgnoreCase)
        sql = Regex.Replace(sql, "　", " ", RegexOptions.IgnoreCase)    '全角のスペースを半角に置換
        sql = Regex.Replace(sql, "[ ]+", " ", RegexOptions.IgnoreCase)

        sql = Trim(sql)
        '        sql = UCase(sql)    ''''''''''

        Try
            'サブクエリの分解
            Dim intQueryIndex As Integer = 0
            objQuery.Query = sql
            objQuery.QueryKind = UCase(Mid(sql, 1, InStr(sql, " ") - 1))


            DivideWith(objQuery, intQueryIndex)


            If Not tsub Is Nothing Then
                tsub.WriteLine(vbTab & objQuery.Query)
                WriteSubQuery(objQuery, tsub)
            End If

            'テーブルCRUD解析
            '★★★★            AnalyzeTableCRUD(objQuery, dctE, strFileName, bolExpandView)

            'カラムCRUD解析
            '            If bolColumnCRUD Then
            AnalyzeColumnCRUD(objQuery, dctE, strFileName, bolCheckReferenceCond)
            '           End If
        Catch
            '解析失敗
        End Try
    End Sub

    Public Sub DivideWith(ByRef objQuery As clsQuery, ByRef intQueryIndex As Integer)
        Dim sql As String = objQuery.Query

        If UCase(Trim(Mid(objQuery.Query, 1, 5))) = "WITH" Then
            objQuery.QueryKind = "SELECT" '★★★★SELECTに限るのか？？？
            'Withの分解
            Dim strQuery As String = ""
            Dim strSplitWords(1) As String
            strSplitWords(0) = " , "
            strSplitWords(1) = " SELECT "
            Dim strWiths As String() = SplitWords(Mid(sql, 5), strSplitWords, True)
            If UBound(strWiths) > 0 Then
                Dim intCnt As Integer
                For intCnt = LBound(strWiths) To UBound(strWiths)
                    Dim objMatches As MatchCollection = Regex.Matches(strWiths(intCnt), "^([^ ]+) AS (.*)$", RegexOptions.IgnoreCase)
                    If objMatches.Count > 0 Then
                        Dim strWithName As String = objMatches(0).Groups(1).Value
                        Dim strWithSql As String = Trim(objMatches(0).Groups(2).Value)
                        strWithSql = DeleteYobunKakko(strWithSql) '余分な括弧を削除
                        DictAdd(objQuery.Withs, strWithName, strWithSql)

                        ''暫定でサブクエリとして追加
                        Dim objSubQuery As New clsQuery
                        objSubQuery.Query = strWithSql
                        objSubQuery.Parent = objQuery
                        objSubQuery.SubQueryIndex = strWithName
                        DictAdd(objQuery.dctSubQuerys, "%" & strWithName & "%", objSubQuery)
                        DivideUnion(objSubQuery, intQueryIndex)
                    End If
                    intCnt += 1
                    If Trim(strWiths(intCnt)) <> "," Then
                        Do Until intCnt > UBound(strWiths)
                            strQuery &= strWiths(intCnt)
                            intCnt += 1
                        Loop
                        Exit For
                    End If
                Next
                objQuery.Query = strQuery
            Else
                '★ありえるのか
            End If
        End If
        DivideUnion(objQuery, intQueryIndex)

    End Sub

    Public Sub DivideUnion(ByRef objQuery As clsQuery, ByRef intQueryIndex As Integer)
        Dim sql As String = objQuery.Query

        'UNIONの分解
        Dim strSplitWords(3) As String
        strSplitWords(0) = " UNION ALL "
        strSplitWords(1) = " UNION "
        strSplitWords(2) = " MINUS "
        strSplitWords(3) = " INTERSECT "
        Dim strUnions As String() = SplitWords(sql, strSplitWords, True)
        If UBound(strUnions) > 0 Then
            objQuery.QueryKind = ""
            If InStr(UCase(sql), strSplitWords(1)) > 0 Then
                If objQuery.QueryKind <> "" Then objQuery.QueryKind &= " / "
                objQuery.QueryKind &= "UNION"
            End If
            If InStr(UCase(sql), strSplitWords(2)) > 0 Then
                If objQuery.QueryKind <> "" Then objQuery.QueryKind &= " / "
                objQuery.QueryKind &= "MINUS"
            End If
            If InStr(UCase(sql), strSplitWords(3)) > 0 Then
                If objQuery.QueryKind <> "" Then objQuery.QueryKind &= " / "
                objQuery.QueryKind &= "INTERSECT"
            End If
            Dim strMainSql As String = ""
            Dim intCnt As Integer
            For intCnt = LBound(strUnions) To UBound(strUnions)
                intQueryIndex += 1
                Dim objSubQuery As New clsQuery
                objSubQuery.Query = Trim(strUnions(intCnt))
                objSubQuery.Query = DeleteYobunKakko(objSubQuery.Query) '余分な括弧を削除
                objSubQuery.SubQueryIndex = intQueryIndex
                objSubQuery.Parent = objQuery
                strMainSql &= "%" & intQueryIndex & "% "
                objQuery.dctSubQuerys.Add("%" & intQueryIndex & "%", objSubQuery)
                DivideUnion(objSubQuery, intQueryIndex)
                If intCnt <> UBound(strUnions) Then
                    intCnt += 1
                    strMainSql &= strUnions(intCnt) & " "
                End If

            Next
            objQuery.Query = strMainSql
        Else
            DivideSubQuery(objQuery, intQueryIndex)
        End If

    End Sub

    'サブクエリ分解
    Public Function DivideSubQuery(ByRef objQuery As clsQuery, ByRef intSubQueryIndex As Integer) As String
        Dim lngPos As Long, lngSelectPos As Long, lngWorkPos As Long
        Dim strSubQuery As String
        Dim lngDepth As Long
        Dim strSql As String

        objQuery.Query = Trim(objQuery.Query)
        strSql = objQuery.Query
        lngPos = 1

        Do
            lngSelectPos = InStr(CInt(lngPos), UCase(strSql), " SELECT ")
            If lngSelectPos = 0 Then
                Exit Do
            Else
                'サブクエリの終端を検索
                lngDepth = 1
                lngWorkPos = lngSelectPos + Len(" SELECT ")
                Do
                    Select Case Mid(strSql, lngWorkPos, 1)
                        Case "("
                            lngDepth = lngDepth + 1
                        Case ")"
                            lngDepth = lngDepth - 1
                    End Select

                    If lngDepth = 0 Then
                        Exit Do
                    End If

                    If lngWorkPos = Len(strSql) Then
                        Exit Do
                    End If

                    lngWorkPos = lngWorkPos + 1
                Loop
                strSubQuery = Trim(Mid(strSql, lngSelectPos + 1, lngWorkPos - lngSelectPos - 1))
                Dim objSubQuery As New clsQuery
                intSubQueryIndex += 1
                objSubQuery.FileName = objQuery.FileName
                objSubQuery.LineNo = objQuery.LineNo
                objSubQuery.SubQueryIndex = intSubQueryIndex
                objSubQuery.Query = strSubQuery
                objSubQuery.QueryKind = "SELECT"
                objSubQuery.Parent = objQuery
                objQuery.dctSubQuerys.Add("%" & intSubQueryIndex & "%", objSubQuery)
                Dim intAltEndPos As Integer = InStr(CInt(lngWorkPos) + 2, strSql, " ")
                If intAltEndPos <> 0 Then
                    objSubQuery.AltName = Trim(Mid(strSql, lngWorkPos + 2, intAltEndPos - (lngWorkPos + 2)))
                    If RegMatch(objSubQuery.AltName, "DELETE|FROM|GROUP|HAVING|INTO|INSERT|ORDER|SELECT|SET|TABLE|UNION|MINUS|INTERSECT|UPDATE|VALUES|WITH|WHERE", RegexOptions.IgnoreCase) Then
                        objSubQuery.AltName = ""
                    End If
                End If

                strSql = Mid(strSql, 1, lngSelectPos - 2) & "%" & intSubQueryIndex & "%" & Mid(strSql, lngWorkPos + 1)
                If GetRight(strSql, 1) = ")" Then strSql = Trim(Mid(strSql, 1, Len(strSql) - 1))
                lngPos = lngSelectPos
                End If
        Loop

        'サブクエリ置換部分の補正 TABLE%1%→TABLE %1%
        objQuery.Query = Regex.Replace(strSql, "(%[0-9]+%)", " $1", RegexOptions.IgnoreCase)

        'サブクエリ内のサブクエリを分解
        Dim strKey As String
        For Each strKey In objQuery.dctSubQuerys.Keys
            Dim objSubQuery As clsQuery
            objSubQuery = objQuery.dctSubQuerys.Item(strKey)
            DivideUnion(objSubQuery, intSubQueryIndex)
            '            objSubQuery.Query = DivideSubQuery(objSubQuery, intSubQueryIndex)
        Next

        DivideSubQuery = objQuery.Query
    End Function

    Public Function SplitKu(ByVal strSql As String) As String()
        Dim objMatches As MatchCollection
        Dim intPos As Integer = 1
        Dim intCnt As Integer
        Dim arrRet As String()

        strSql = " " & Replace(strSql, " AS ", " ") & " "
        strSql = Regex.Replace(strSql, EscapeRegular(" SELECT* "), " SELECT * ", RegexOptions.IgnoreCase)
        strSql = Regex.Replace(strSql, EscapeRegular(" INSERT "), " INSERT DUMMY ", RegexOptions.IgnoreCase)
        strSql = Regex.Replace(strSql, EscapeRegular(" DELETE "), " DELETE DUMMY ", RegexOptions.IgnoreCase)
        'strSql = Replace(strSql, " SELECT* ", " SELECT * ")
        'strSql = Replace(strSql, " INSERT ", " INSERT DUMMY ")      'INSERTの後にダミーを挿入
        'strSql = Replace(strSql, " DELETE ", " DELETE DUMMY ")      'DELETEの後にダミーを挿入

        objMatches = Regex.Matches(Mid(strSql, intPos), " DELETE | FROM | GROUP BY | HAVING | INTO | INSERT | ORDER BY | SELECT | SET | TABLE | UNION ALL | UNION | MINUS | INTERSECT | UPDATE | VALUES | WITH | WHERE | MODEL | PIVOT | UNPIVOT | START WITH | CONNECT WITH ", RegexOptions.IgnoreCase)
        ReDim arrRet(0 To objMatches.Count * 2 - 1)
        For intCnt = 0 To objMatches.Count - 1
            arrRet(intCnt * 2) = Trim(objMatches(intCnt).Value)
            If intCnt <> (objMatches.Count - 1) Then    '最後の句以外
                arrRet(intCnt * 2 + 1) = Trim(Mid(strSql, objMatches(intCnt).Index + objMatches(intCnt).Length, (objMatches(intCnt + 1).Index + 1) - (objMatches(intCnt).Index + objMatches(intCnt).Length)))
            Else                                        '最後の句
                arrRet(intCnt * 2 + 1) = Trim(Mid(strSql, objMatches(intCnt).Index + objMatches(intCnt).Length))
            End If
        Next
        SplitKu = arrRet

    End Function

    Public Sub AnalyzeColumnCRUD(ByRef objQuery As clsQuery, ByRef dctE As Scripting.Dictionary, ByVal strFileName As String, ByVal bolCheckReferenceCond As Boolean)
        Dim intCnt As Integer
        Dim arrKu As String(), arrWords As String()
        Dim dctWhereColumn As New Scripting.Dictionary

        Dim objKu As New clsSQLKu
        arrKu = SplitKu(objQuery.Query) '句の分解
        If arrKu.Length = 0 Then Exit Sub
        objKu.SQLType = UCase(arrKu(0))
        Dim intWord As Integer
        For intCnt = 0 To (UBound(arrKu) - 1) / 2
            If arrKu(intCnt * 2 + 1) <> "" Then
                Select Case UCase(arrKu(intCnt * 2))
                    Case "INSERT"
                        Debug.Print("INSERT")
                    Case "DELETE"
                        objKu.KuDeleteTable = arrKu(intCnt * 2 + 1)
                    Case "FROM"
                        Select Case objKu.SQLType
                            Case "SELECT"
                                objKu.KuFrom = arrKu(intCnt * 2 + 1)
                            Case "DELETE"
                                objKu.KuDeleteTable = arrKu(intCnt * 2 + 1)
                        End Select
                    Case "GROUP BY"
                        objKu.KuGroupBy = arrKu(intCnt * 2 + 1)
                    Case "HAVING"
                        objKu.KuHaving = arrKu(intCnt * 2 + 1)
                    Case "INTO" 'INSERTの場合はCだがそれ以外は無視
                        objKu.KuInto = arrKu(intCnt * 2 + 1)
                    Case "ORDER BY"
                        objKu.KuOderBy = arrKu(intCnt * 2 + 1)
                    Case "SELECT"
                        objKu.KuSelect = arrKu(intCnt * 2 + 1)
                    Case "SET"
                        objKu.KuSet = arrKu(intCnt * 2 + 1)
                        'Dim arrSetValues As String() = SplitKanma(Mid(objKu.KuSet, 2, objKu.KuSet.Length - 2))
                        'Dim intX As Integer
                        'For intX = LBound(arrSetValues) To UBound(arrSetValues)
                        '    Dim intPos As Integer = InStr(arrSetValues(intX), "=")
                        '    Dim strSetValue As String = Trim(Mid(arrSetValues(intX), intPos + 1))
                        '    objQuery.SetValues.Add("K" & objQuery.SetValues.Count + 1, strSetValue)
                        'Next
                    Case "TABLE"
                        objKu.KuDeleteTable = arrKu(intCnt * 2 + 1)
                    Case "UNION ALL"
                    Case "UNION"
                    Case "MINUS"
                    Case "INTERSECT"
                    Case "UPDATE"
                        objKu.KuUpdateTable = arrKu(intCnt * 2 + 1)
                    Case "VALUES"
                        objKu.KuValues = arrKu(intCnt * 2 + 1)
                        Dim arrValues As String() = SplitKanma(Mid(objKu.KuValues, 2, objKu.KuValues.Length - 2))
                        Dim intX As Integer
                        For intX = LBound(arrValues) To UBound(arrValues)
                            objQuery.Values.Add("K" & objQuery.Values.Count + 1, arrValues(intX))
                        Next
                    Case "WITH"
                    Case "WHERE"
                        objKu.KuWhere = arrKu(intCnt * 2 + 1)
                    Case "MODEL", "PIVOT", "UNPIVOT", "START WITH", "CONNECT BY" '★未対応
                        dctE.Add("K" & dctE.Count + 1, vbTab & objQuery.FileName & "(" & objQuery.LineNo & "): " & UCase(arrKu(intCnt * 2)) & "句は対応していません。句の内容は無視されます。")
                End Select
            End If
        Next
        Select Case objKu.SQLType
            Case "SELECT"
                If objKu.KuFrom <> "" Then AnalyzeFrom(objQuery, objKu.KuFrom, dctE)
                If objKu.KuSelect <> "" Then arrWords = SplitWords(objKu.KuSelect) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD(objQuery, objQuery.ColumnR, arrWords(intWord), dctE) : Next
                If objKu.KuGroupBy <> "" Then arrWords = SplitWords(objKu.KuGroupBy) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD(objQuery, objQuery.ColumnR, arrWords(intWord), dctE) : Next
                If objKu.KuOderBy <> "" Then arrWords = SplitWords(objKu.KuOderBy) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD(objQuery, objQuery.ColumnR, arrWords(intWord), dctE) : Next
                If objKu.KuHaving <> "" Then arrWords = SplitWords(objKu.KuHaving) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD(objQuery, objQuery.ColumnR, arrWords(intWord), dctE) : Next
                If objKu.KuWhere <> "" Then arrWords = SplitWords(objKu.KuWhere) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD(objQuery, objQuery.ColumnR, arrWords(intWord), dctE) : AddColumnCRUD(objQuery, dctWhereColumn, arrWords(intWord), dctE) : Next

                If objKu.KuSelect <> "" Then
                    arrWords = SplitWords(objKu.KuSelect) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnSelect, enmKuKind.KuSelect, arrWords(intWord), dctE) : Next
                    AnalyzeSelectKu(objQuery, objKu.KuSelect, dctE)
                End If
                If objKu.KuGroupBy <> "" Then arrWords = SplitWords(objKu.KuGroupBy) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnGroupBy, enmKuKind.KuGroupBy, arrWords(intWord), dctE) : Next
                If objKu.KuOderBy <> "" Then arrWords = SplitWords(objKu.KuOderBy) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnOrderBy, enmKuKind.KuOrderBy, arrWords(intWord), dctE) : Next
                If objKu.KuHaving <> "" Then arrWords = SplitWords(objKu.KuHaving) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnHaving, enmKuKind.KuHaving, arrWords(intWord), dctE) : Next
                If objKu.KuWhere <> "" Then arrWords = SplitWords(objKu.KuWhere) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnWhere, enmKuKind.KuWhere, arrWords(intWord), dctE) : Next
                '                If objKu.KuWhere <> "" Then arrWords = SplitWords(objKu.KuWhere) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnWhere, enmKuKind.KuWhere, arrWords(intWord), dctE) : AddColumnCRUD(objQuery, dctWhereColumn, arrWords(intWord), dctE) : Next

            Case "INSERT"
                If objKu.KuInto <> "" Then
                    Dim strInsertTable As String = Split(objKu.KuInto, " ")(0)
                    AddTableCRUD(objQuery.TableC, strInsertTable, dctE)
                    Dim intPos As Integer = InStr(objKu.KuInto, "(")
                    If intpos > 0 Then
                        arrWords = SplitWords(Mid(objKu.KuInto, intPos))
                        For intWord = 0 To UBound(arrWords)
                            Dim strColumnName As String = arrWords(intWord)
                            If InStr(strColumnName, ".") < 1 Then   'テーブル名.カラム名でない場合
                                strColumnName = strInsertTable & "." & strColumnName    'テーブル名を修飾する
                            End If
                            AddColumnCRUD(objQuery, objQuery.ColumnC, strColumnName, dctE)
                            AddColumnCRUD2(objQuery, objQuery.ColumnInsert, enmKuKind.KuInsert, strColumnName, dctE)
                        Next
                    End If
                End If
            Case "UPDATE"
                    If objKu.KuUpdateTable <> "" Then AddTableCRUD(objQuery.TableU, objKu.KuUpdateTable, dctE)
                    If objKu.KuSet <> "" Then arrWords = SplitWords(objKu.KuSet) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD(objQuery, objQuery.ColumnU, arrWords(intWord), dctE) : Next
                    If objKu.KuWhere <> "" Then arrWords = SplitWords(objKu.KuWhere) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD(objQuery, objQuery.ColumnR, arrWords(intWord), dctE) : AddColumnCRUD(objQuery, dctWhereColumn, arrWords(intWord), dctE) : Next

                    '                If objKu.KuSet <> "" Then arrWords = SplitWords(objKu.KuSet) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnUpdate, enmKuKind.KuUpdate, arrWords(intWord), dctE) : Next
                    If objKu.KuSet <> "" Then AnalyzeSetKu(objQuery, objKu.KuSet, dctE)
                    If objKu.KuWhere <> "" Then arrWords = SplitWords(objKu.KuWhere) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnWhere, enmKuKind.KuWhere, arrWords(intWord), dctE) : Next
                    '                If objKu.KuWhere <> "" Then arrWords = SplitWords(objKu.KuWhere) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnWhere, enmKuKind.KuWhere, arrWords(intWord), dctE) : AddColumnCRUD(objQuery, dctWhereColumn, arrWords(intWord), dctE) : Next
            Case "DELETE"   'ここではなにもしない
                    If objKu.KuDeleteTable <> "" Then AddTableCRUD(objQuery.TableD, objKu.KuDeleteTable, dctE)
                    If objKu.KuWhere <> "" Then arrWords = SplitWords(objKu.KuWhere) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD(objQuery, objQuery.ColumnR, arrWords(intWord), dctE) : AddColumnCRUD(objQuery, dctWhereColumn, arrWords(intWord), dctE) : Next

                    If objKu.KuWhere <> "" Then arrWords = SplitWords(objKu.KuWhere) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnWhere, enmKuKind.KuDelete, arrWords(intWord), dctE) : Next
                    '                If objKu.KuWhere <> "" Then arrWords = SplitWords(objKu.KuWhere) : For intWord = 0 To UBound(arrWords) : AddColumnCRUD2(objQuery, objQuery.ColumnWhere, enmKuKind.KuDelete, arrWords(intWord), dctE) : AddColumnCRUD(objQuery, dctWhereColumn, arrWords(intWord), dctE) : Next
            Case "TRUNCATE" 'ここではなにもしない
                    If objKu.KuDeleteTable <> "" Then AddTableCRUD(objQuery.TableD, objKu.KuDeleteTable, dctE)
        End Select

        'サブクエリ
        Dim strKey As String
        For Each strKey In objQuery.dctSubQuerys.Keys
            AnalyzeColumnCRUD(objQuery.dctSubQuerys(strKey), dctE, strFileName, bolCheckReferenceCond)
        Next

        '★★★有効期間を持つテーブル参照処理のテスト
        If bolCheckReferenceCond Then
            Dim strTableName As String
            For Each strTableName In dctReferenceCond.Keys
                For Each strKey In objQuery.TableR
                    If strTableName = (Split(objQuery.TableR(strKey), vbTab)(0)) Then 'チェック対象のテーブル
                        Dim strErrColumn As String = ""
                        Dim strCheckColumns As String() = Split(dctReferenceCond(strTableName), ":")
                        For intCnt = LBound(strCheckColumns) To UBound(strCheckColumns)
                            If Not DictExists(dctWhereColumn, strTableName & "." & strCheckColumns(intCnt)) Then
                                If strErrColumn <> "" Then strErrColumn &= ","
                                strErrColumn &= strCheckColumns(intCnt)
                            End If
                        Next
                        If strErrColumn <> "" Then
                            dctE.Add("K" & dctE.Count + 1, vbTab & objQuery.FileName & "(" & objQuery.LineNo & "): 参照必須条件エラー テーブル[" & strTableName & "] カラム[" & strErrColumn & "]")
                        Else
                            dctE.Add("K" & dctE.Count + 1, vbTab & vbTab & objQuery.FileName & "(" & objQuery.LineNo & "): 参照必須条件ＯＫ テーブル[" & strTableName & "]")
                        End If
                    End If
                Next
            Next
        End If
    End Sub

    Public Sub AnalyzeSetKu(ByRef objQuery As clsQuery, ByVal strSetKu As String, ByRef dctE As Scripting.Dictionary)
        Dim intPos As Integer
        Dim intIdx As Integer = 0
        Dim strSet As String = ""
        Dim strSetValue As String = ""
        Dim strArr As String()

        strArr = SplitKanma(strSetKu)

        For intIdx = 0 To UBound(strArr)
            intPos = InStr(strArr(intIdx), "=")
            If intPos > 0 Then
                strSet = Trim(Mid(strArr(intIdx), 1, intPos - 1))
                strSetValue = Trim(Mid(strArr(intIdx), intPos + 1))
                objQuery.SetValues.Add("K" & objQuery.SetValues.Count + 1, strSetValue)
                AddColumnCRUD(objQuery, objQuery.ColumnU, strSet, dctE)
                AddColumnCRUD2(objQuery, objQuery.ColumnUpdate, enmKuKind.KuUpdate, strSet, dctE)
                Dim arrWords As String() = SplitWords(Mid(strArr(intIdx), intPos + 1))
                Dim intWord As Integer
                For intWord = 0 To UBound(arrWords)
                    AddColumnCRUD(objQuery, objQuery.ColumnR, arrWords(intWord), dctE)
                    AddColumnCRUD2(objQuery, objQuery.ColumnSetCond, enmKuKind.KuSetCond, arrWords(intWord), dctE)
                Next
            End If
        Next
    End Sub

    Private Sub AnalyzeSelectKu(ByRef objQuery As clsQuery, ByVal strSelectKu As String, ByRef dctE As Scripting.Dictionary)
        Dim intIdx As Integer = 0
        Dim strSet As String = ""
        Dim strSetValue As String = ""
        Dim strArr As String()

        strArr = SplitKanma(Regex.Replace(strSelectKu, "\bDISTINCT\s", "", RegexOptions.IgnoreCase))

        For intIdx = 0 To UBound(strArr)
            Dim strWords(0 To 0) As String
            Dim strAlt As String
            Dim strSelectValue As String
            strWords(0) = " "
            Dim strArr2 As String()
            strArr2 = SplitWords(strArr(intIdx), strWords)
            If strArr2.Length = 1 Then
                Dim strArrKanma As String()
                strArrKanma = Split(strArr(intIdx), ".")
                If DictExists(objQuery.Selects, strArrKanma(strArrKanma.Length - 1)) Then
                    strAlt = strArr(intIdx)
                    strSelectValue = strArr(intIdx)
                Else
                    strAlt = strArrKanma(strArrKanma.Length - 1)
                    strSelectValue = strArr(intIdx)
                End If
            ElseIf Mid(strArr2(strArr2.Length - 1), 1, 1) = "(" Then    '関数コールとみなす
                strAlt = "未定義" & objQuery.Selects.Count + 1
                strSelectValue = strArr(intIdx)
                '最後から１つ手前の単語が演算子か
            ElseIf RegMatch(strArr2(strArr2.Length - 2), WordEQ("=") & "|" & WordEQ("!=") & "|" & WordEQ("^=") & "|" & WordEQ("<>") & "|" & WordEQ(">") & "|" & WordEQ("<") & "|" & WordEQ(">=") & "|" & WordEQ("<=") & "|" & WordEQ("IN") & "|" & WordEQ("IS") & "|" & WordEQ("+") & "|" & WordEQ("-") & "|" & WordEQ("/") & "|" & WordEQ("*") & "|" & WordEQ("||"), RegexOptions.IgnoreCase) Then
                strAlt = "未定義" & objQuery.Selects.Count + 1
                strSelectValue = strArr(intIdx)
            ElseIf RegMatch(strArr2(strArr2.Length - 1), "^[0-9]+$|^['][^']*[']$", RegexOptions.IgnoreCase) Then    '数値、文字列
                strAlt = "未定義" & objQuery.Selects.Count + 1
                strSelectValue = strArr(intIdx)
            Else
                '最後が別名とみなす
                strAlt = strArr2(strArr2.Length - 1)
                strSelectValue = Trim(Mid(strArr(intIdx), 1, strArr(intIdx).Length - strArr2(strArr2.Length - 1).Length))
            End If
            If DictExists(objQuery.Selects, strAlt) AndAlso strAlt <> strSelectValue Then
                '                MsgBox("SELECT句の別名が重複しています★★★★ " & strAlt)
                DictAdd(dctE, "K" & dctE.Count + 1, "SELECT句の別名が重複しています 行番号[" & objQuery.LineNo & "] 別名[" & strAlt & "] SELECT句[" & strSelectKu & "]")
            Else
                objQuery.Selects.Add(strAlt, strSelectValue)
            End If
        Next
    End Sub

    Private Function WordEQ(ByVal strWord As String, Optional ByVal bolEscapeRegular As Boolean = True) As String
        Dim strWork As String = strWord
        If bolEscapeRegular Then strWork = EscapeRegular(strWork)
        strWork = "^" & strWork & "$"
        WordEQ = strWork
    End Function

    Public Function SplitWords(ByVal strSource As String, ByVal strWords As String(), Optional ByVal bolRetuenSplitWord As Boolean = False) As String()
        Dim strArr As String()
        Dim intPos As Integer
        Dim intIdx As Integer = 0
        Dim strSet As String = ""
        Dim intKakkoDepth As Integer = 0

        For intPos = 1 To Len(strSource)
            Dim strChar As String = Mid(strSource, intPos, 1)
            Select Case strChar
                Case "("
                    strSet &= strChar
                    intKakkoDepth += 1
                Case ")"
                    strSet &= strChar
                    intKakkoDepth -= 1
                    If intKakkoDepth < 0 Then intKakkoDepth = 0
                Case Else
                    If intKakkoDepth = 0 Then
                        Dim intWords As Integer
                        Dim bolMatch As Boolean = False
                        For intWords = LBound(strWords) To UBound(strWords)
                            If UCase(Mid(strSource, intPos, strWords(intWords).Length)) = UCase(strWords(intWords)) Then
                                ReDim Preserve strArr(0 To intIdx)
                                strArr(intIdx) = Trim(strSet)
                                intIdx += 1
                                strSet = ""
                                intPos += strWords(intWords).Length - 1
                                bolMatch = True
                                If bolRetuenSplitWord Then
                                    ReDim Preserve strArr(0 To intIdx)
                                    strArr(intIdx) = strWords(intWords)
                                    intIdx += 1
                                End If
                                Exit For
                            End If
                        Next
                        If Not bolMatch Then
                            strSet &= strChar
                        End If
                    Else
                        strSet &= strChar
                    End If
            End Select
        Next
        ReDim Preserve strArr(0 To intIdx)
        strArr(intIdx) = Trim(strSet)

        SplitWords = strArr
    End Function

    Public Function SplitKanma(ByVal strSource As String) As String()
        Dim strWords(0) As String
        strWords(0) = ","
        SplitKanma = SplitWords(strSource, strWords)

        'Dim strArr As String()
        'Dim intPos As Integer
        'Dim intIdx As Integer = 0
        'Dim strSet As String = ""
        'Dim intKakkoDepth As Integer = 0

        'For intPos = 1 To Len(strSource)
        '    Dim strChar As String = Mid(strSource, intPos, 1)
        '    Select Case strChar
        '        Case "("
        '            intKakkoDepth += 1
        '        Case ")"
        '            intKakkoDepth -= 1
        '            If intKakkoDepth < 0 Then intKakkoDepth = 0
        '        Case ","
        '            If intKakkoDepth = 0 Then
        '                ReDim Preserve strArr(0 To intIdx)
        '                strArr(intIdx) = Trim(strSet)
        '                intIdx += 1
        '                strSet = ""
        '            Else
        '                strSet &= strChar
        '            End If
        '        Case Else
        '            strSet &= strChar
        '    End Select
        'Next
        'ReDim Preserve strArr(0 To intIdx)
        'strArr(intIdx) = Trim(strSet)

        'SplitKanma = strArr
    End Function

    Public Sub AnalyzeFrom(ByRef objQuery As clsQuery, ByVal strFromKu As String, ByRef dctE As Scripting.Dictionary)
        Dim strSql As String = objQuery.Query
        Dim strArr As String()
        Dim strTable As String
        Dim strAlt As String

        If InStr(UCase(strSql), " JOIN ") > 0 Then         ' SQL92の結合を含むFROM句
            Dim arrJoin As String() = SplitJOIN(strFromKu)
            Dim intCnt As Integer

            strArr = Split(Trim(arrJoin(0)), " ")
            strTable = strArr(0)
            If UBound(strArr) >= 1 Then
                strAlt = strArr(1)
            Else
                strAlt = ""
            End If
            If Mid(strTable, 1, 1) <> "%" Then
                On Error Resume Next
                DictAdd(objQuery.TableR, "K" & objQuery.TableR.Count + 1, UCase(strTable & vbTab & strAlt))
                '                objQuery.TableR.Add("K" & objQuery.TableR.Count + 1, strTable & vbTab & strAlt)      '1つ目はテーブル
                On Error GoTo 0
            End If
            '            objQuery.TableR.Add("K" & objQuery.TableR.Count + 1, Trim(arrJoin(0)))      '1つ目はテーブル
            For intCnt = 1 To UBound(arrJoin)
                If UCase(GetRight(arrJoin(intCnt), 5)) = " JOIN" Then
                    intCnt += 1
                    strArr = Split(Trim(arrJoin(intCnt)), " ")
                    strTable = strArr(0)
                    If UBound(strArr) >= 1 Then
                        strAlt = strArr(1)
                    Else
                        strAlt = ""
                    End If
                    If Mid(strTable, 1, 1) <> "%" Then
                        On Error Resume Next
                        DictAdd(objQuery.TableR, "K" & objQuery.TableR.Count + 1, UCase(strTable & vbTab & strAlt))
                        '                        objQuery.TableR.Add("K" & objQuery.TableR.Count + 1, strTable & vbTab & strAlt)      '1つ目はテーブル
                        On Error GoTo 0
                    End If
                ElseIf UCase(arrJoin(intCnt)) = "ON" Then
                    intCnt += 1
                    Dim strWords As String() = SplitWords(arrJoin(intCnt))
                    Dim intWord As Integer
                    For intWord = 0 To UBound(strWords)
                        AddColumnCRUD(objQuery, objQuery.ColumnR, strWords(intWord), dctE)
                    Next
                Else
                    dctE.Add(dctE.Count + 1, "FROM句解析エラー From句=" & strFromKu & " エラー箇所=" & arrJoin(intCnt))
                End If

            Next
        Else                '旧形式のFROM句
            AddTableCRUD(objQuery.TableR, strFromKu, dctE)
        End If

    End Sub

    Public Sub AddTableCRUD(ByRef dctTables As Scripting.Dictionary, ByVal strWord As String, ByRef dctE As Scripting.Dictionary)
        Dim strTables As String() = Split(strWord, ",")
        Dim strWork As String
        Dim lngTableCnt As Long
        Dim strArr As String()
        Dim strTable As String
        Dim strAlt As String
        For lngTableCnt = LBound(strTables) To UBound(strTables)
            strWork = strTables(lngTableCnt)
            strWork = Replace(strWork, "(", "")
            strWork = Replace(strWork, ")", "")
            strWork = Trim(strWork)
            If strWork <> "" Then
                strArr = Split(strWork, " ")
                strTable = strArr(0)
                If UBound(strArr) >= 1 Then
                    strAlt = strArr(1)
                Else
                    strAlt = ""
                End If
                If Mid(strTable, 1, 1) <> "%" Then
                    On Error Resume Next
                    dctTables.Add("K" & dctTables.Count + 1, UCase(strTable & vbTab & strAlt))
                    On Error GoTo 0
                End If
            End If
        Next
    End Sub

    Public Function SplitJOIN(ByVal strSql As String) As String()
        Dim objMatches As MatchCollection
        Dim intCnt As Integer
        Dim arrRet As String()

        strSql = Regex.Replace(strSql, " AS ", " ", RegexOptions.IgnoreCase)
        strSql = Replace(strSql, "(", " ")
        strSql = Replace(strSql, ")", " ")
        strSql = Regex.Replace(strSql, "[ ]+", " ")

        objMatches = Regex.Matches(strSql, " INNER JOIN | LEFT OUTER JOIN | LEFT JOIN | RIGHT OUTER JOIN | RIGHT JOIN | FULL OUTER JOIN | FULL JOIN | CROSS JOIN | NATURAL JOIN | ON ", RegexOptions.IgnoreCase)
        ReDim arrRet(0 To objMatches.Count * 2)
        arrRet(0) = Mid(strSql, 1, objMatches(0).Index)
        For intCnt = 0 To objMatches.Count - 1
            arrRet(intCnt * 2 + 1) = Trim(objMatches(intCnt).Value)
            If intCnt <> (objMatches.Count - 1) Then    '最後の句以外
                arrRet(intCnt * 2 + 2) = Mid(strSql, objMatches(intCnt).Index + objMatches(intCnt).Length, (objMatches(intCnt + 1).Index + 1) - (objMatches(intCnt).Index + objMatches(intCnt).Length))
            Else                                        '最後の句
                arrRet(intCnt * 2 + 2) = Mid(strSql, objMatches(intCnt).Index + objMatches(intCnt).Length)
            End If
        Next
        SplitJOIN = arrRet

    End Function


    Public Sub AddColumnCRUD(ByRef objQuery As clsQuery, ByRef dctColumns As Scripting.Dictionary, ByVal strWord As String, ByRef dctE As Scripting.Dictionary)
        Dim arrWord As String() = Split(strWord, ".")
        Dim strTempTableName As String = "", strTempColumnName As String = ""
        Dim strTableName As String = "", strColumnName As String = ""
        If UBound(arrWord) > 0 Then
            strTempTableName = arrWord(0)
            strTempColumnName = arrWord(1)
        Else
            strTempColumnName = arrWord(0)
        End If

        ConvertRealName(objQuery, strWord, strTempTableName, strTempColumnName, strTableName, strColumnName)
        If strTableName <> "" AndAlso strColumnName <> "" Then
            On Error Resume Next
            dctColumns.Add(strTableName & "." & strColumnName, strTableName & "." & strColumnName)
            On Error GoTo 0
            ''''            dctColumns.Add("K" & dctColumns.Count + 1, strTableName & "." & strColumnName)
        Else
            If objSettings.DebugMode Then
                dctE.Add(dctE.Count + 1, "テーブル名を解決できないワード[" & strWord & "]")
            End If
        End If
    End Sub

    Public Sub AddColumnCRUD2(ByRef objQuery As clsQuery, ByRef colColumns As ColumnCollection, ByVal KuKind As enmKuKind, ByVal strWord As String, ByRef dctE As Scripting.Dictionary)
        Dim arrWord As String() = Split(strWord, ".")
        Dim strTempTableName As String = "", strTempColumnName As String = ""
        Dim strTableName As String = "", strColumnName As String = ""
        If UBound(arrWord) > 0 Then
            strTempTableName = arrWord(0)
            strTempColumnName = arrWord(1)
        Else
            strTempColumnName = arrWord(0)
        End If

        ConvertRealName(objQuery, strWord, strTempTableName, strTempColumnName, strTableName, strColumnName)
        If strTableName <> "" AndAlso strColumnName <> "" Then
            On Error Resume Next
            colColumns.add(New Column(strColumnName, strTableName, strTempTableName, KuKind))
            On Error GoTo 0
            ''''            dctColumns.Add("K" & dctColumns.Count + 1, strTableName & "." & strColumnName)
        Else
            If objSettings.DebugMode Then
                dctE.Add(dctE.Count + 1, "テーブル名を解決できないワード[" & strWord & "]")
            End If
        End If
    End Sub

    Public Sub ConvertRealName(ByRef objQuery As clsQuery, ByVal strWord As String, ByVal strTempTableName As String, ByVal strTempColumnName As String, ByRef strTableName As String, ByRef strColumnName As String)
        Dim objTableDef As clsTableDef
        Dim strKey As String
        Dim dctAllTable As Scripting.Dictionary = objQuery.AllTable

        If strTempTableName <> "" Then 'テーブル名か別名が付いている場合
            For Each strKey In dctAllTable.Keys
                Dim arrTable As String() = Split(GetDictValue(dctAllTable, strKey), vbTab)
                Dim intT As Integer
                For intT = LBound(arrTable) To UBound(arrTable)
                    If UCase(strTempTableName) = UCase(arrTable(intT)) Then
                        strTableName = UCase(arrTable(0))
                        strColumnName = UCase(strTempColumnName)
                        Exit For
                    End If
                Next
                If strTableName <> "" Then Exit For
            Next
        Else
            'テーブル定義からカラムを探す
            For Each strKey In dctAllTable.Keys
                Dim arrTable As String() = Split(dctAllTable(strKey), vbTab)
                If DictExists(dctTableDef, arrTable(0)) Then        'テーブル定義に存在
                    objTableDef = GetDictObject(dctTableDef, arrTable(0))
                    Dim strKey2 As String
                    For Each strKey2 In objTableDef.dctColumnts.Keys
                        If UCase(strTempColumnName) = UCase(strKey2) Then
                            strTableName = UCase(arrTable(0))
                            strColumnName = UCase(strTempColumnName)
                        End If
                    Next
                End If
                If strTableName <> "" Then Exit For
            Next
        End If

        '解決したら上位を辿らない
        If strTableName = "" AndAlso Not objQuery.Parent Is Nothing Then
            ConvertRealName(objQuery.Parent, strWord, strTempTableName, strTempColumnName, strTableName, strColumnName)
        End If

    End Sub

    '文字列を検索し、みかった場合は探した文字列の次の位置を返す
    Public Function InStrEx(ByVal lngStart, ByVal strSrc, ByVal strSearch)
        Dim lngPos
        lngPos = InStr(lngStart, strSrc, strSearch)
        If lngPos > 0 Then
            InStrEx = lngPos + Len(strSearch)
        Else
            InStrEx = 0
        End If
    End Function



    Public Function GetCRUD(ByRef dctMatrix As Scripting.Dictionary, ByVal strKey As String) As String
        Dim objCRUD As clsCRUD

        If DictExists(dctMatrix, strKey) Then
            objCRUD = GetDictObject(dctMatrix, strKey)
            GetCRUD = objCRUD.GetCRUD()
        Else
            GetCRUD = ""
        End If
    End Function

    Public Function SortDictionary(ByVal dctSrc As Scripting.Dictionary) As Scripting.Dictionary
        Dim dctDesc As New Scripting.Dictionary
        Dim arrKeys As String(), strKey As String, strWork As String
        Dim lngCnt1 As Long, lngCnt2 As Long

        'キーを配列に格納
        ReDim arrKeys(dctSrc.Count)
        lngCnt1 = 0
        For Each strKey In dctSrc.Keys
            arrKeys(lngCnt1) = strKey
            lngCnt1 = lngCnt1 + 1
        Next

        'キー順にソート
        For lngCnt2 = UBound(arrKeys) - 2 To 0 Step -1
            For lngCnt1 = 0 To lngCnt2
                If arrKeys(lngCnt1) > arrKeys(lngCnt1 + 1) Then
                    strWork = arrKeys(lngCnt1)
                    arrKeys(lngCnt1) = arrKeys(lngCnt1 + 1)
                    arrKeys(lngCnt1 + 1) = strWork
                End If
            Next
        Next

        dctDesc.RemoveAll()

        'ディクショナリの再構築
        For lngCnt1 = LBound(arrKeys) To UBound(arrKeys) - 1
            dctDesc.Add(arrKeys(lngCnt1), dctSrc(arrKeys(lngCnt1)))
        Next
        SortDictionary = dctDesc
    End Function

    '文字列を検索し、みかった場合は探した文字列の次の位置を返す
    Public Function InStrEx(ByVal lngStart As Long, ByVal strSrc As String, ByVal strSearch As String) As Long
        Dim lngPos As Long
        lngPos = InStr(CInt(lngStart), strSrc, strSearch)
        If lngPos > 0 Then
            InStrEx = lngPos + Len(strSearch)
        Else
            InStrEx = 0
        End If
    End Function

    Public Function ColToRange(ByVal lngCol As Long) As String
        ColToRange = ""
        If Fix((lngCol - 1) / 26) > 0 Then
            ColToRange = Chr(Fix((lngCol + 1) / 26) + Asc("A") - 1)
        End If
        ColToRange &= Chr(((lngCol - 1) Mod 26) + Asc("A"))
    End Function

    Function SplitWords(ByVal strSql As String) As String()
        Dim strReserveWords = "ALL,ALTER,AND,ANY,ARRAY,ARROW,AS,ASC,AT,BEGIN,BETWEEN,BY,CASE,CHECK,CLUSTERS,CLUSTER,COLAUTH,COLUMNS,COMPRESS,CONNECT,CRASH,CREATE,CURRENT,DECIMAL,DECLARE,DEFAULT,DELETE,DESC,DISTINCT,DROP,ELSE,END,EXCEPTION,EXCLUSIVE,EXISTS,FETCH,FORM,FOR,FROM,GOTO,GRANT,GROUP,HAVING,IDENTIFIED,IF,IN,INDEXES,INDEX,INSERT,INTERSECT,INTO,IS,LIKE,LOCK,MINUS,MODE,NOCOMPRESS,NOT,NOWAIT,NULL,OF,ON,OPTION,OR,ORDER,OVERLAPS,PRIOR,PROCEDURE,PUBLIC,RANGE,RECORD,RESOURCE,REVOKE,SELECT,SHARE,SIZE,SQL,START,SUBTYPE,TABAUTH,TABLE,THEN,TO,TYPE,UNION,UNIQUE,UPDATE,USE,VALUES,VIEW,VIEWS,WHEN,WHERE,WITH,ROWNUM"
        Dim arrReserve As String() = Split(strReserveWords, ",")
        Dim intCnt As Integer

        strSql = Replace(strSql, vbLf, " ")
        strSql = Replace(strSql, "<", " < ")
        strSql = Replace(strSql, ">", " > ")
        strSql = Replace(strSql, "=", " = ")
        strSql = Replace(strSql, "+", " + ")
        strSql = Replace(strSql, "-", " - ")
        strSql = Replace(strSql, "*", " * ")
        strSql = Replace(strSql, "/", " / ")
        strSql = Replace(strSql, "[|][|]", " || ")
        strSql = Regex.Replace(strSql, "'[^']*'", "")
        '        strSql = Regex.Replace(strSql, "[ ][\w.]+[ ][(]", " ", RegexOptions.IgnoreCase)    '関数の除去
        strSql = " " & ReplaceDelimToSpace(strSql) '演算子・区切り文字の除去
        strSql = Replace(strSql, "(", " ( ")
        strSql = Regex.Replace(strSql, "[ ]+", " ")
        strSql = Replace(strSql, "(", " ")
        strSql = Regex.Replace(strSql, "[ ]\d+[ ]", " ", RegexOptions.IgnoreCase)

        '予約語の除去
        For intCnt = LBound(arrReserve) To UBound(arrReserve)
            strSql = Regex.Replace(strSql, " " & arrReserve(intCnt) & " ", " ", RegexOptions.IgnoreCase)
            '            strSql = Replace(strSql, " " & arrReserve(intCnt) & " ", " ")
        Next

        strSql = Regex.Replace(strSql, "[ ]+", " ") '重複するスペースの除去
        SplitWords = Split(Trim(strSql), " ")
    End Function

    Public Function ReplaceDelimToSpace(ByVal str As String) As String
        Dim strPattern = "!""#$%&)=-~^|\`@[{]}:;+?/><,"
        Dim intCnt As Integer
        Dim strChar As String

        For intCnt = 1 To Len(strPattern)
            strChar = Mid(strPattern, intCnt, 1)
            str = Replace(str, strChar, " ")
        Next
        ReplaceDelimToSpace = str
    End Function

    Public Sub WriteSubQuery(ByVal objQuery As clsQuery, ByVal t As Scripting.TextStream)
        Dim strKey As String

        For Each strKey In objQuery.dctSubQuerys.Keys
            Dim objSubQuery As clsQuery = objQuery.dctSubQuerys(strKey)
            t.WriteLine("%" & objSubQuery.SubQueryIndex & "%" & vbTab & objSubQuery.Query)
        Next
        For Each strKey In objQuery.dctSubQuerys.Keys
            Dim objSubQuery As clsQuery = objQuery.dctSubQuerys(strKey)
            If objSubQuery.dctSubQuerys.Count > 0 Then
                WriteSubQuery(objSubQuery, t)
            End If
        Next
    End Sub

    Public Function DeleteYobunKakko(ByVal strSql As String) As String
        Dim strResult As String = ""
        Dim intDepth As Integer = 0
        Dim intPos As Integer
        Dim strChar As String
        strSql = Trim(strSql)
        If Mid(strSql, 1, 1) <> "(" Then    '最初の文字が(以外の場合は、そのまま文字列を返す
            DeleteYobunKakko = strSql
            Exit Function
        End If

        For intPos = 1 To strSql.Length
            strChar = Mid(strSql, intPos, 1)
            Select Case strChar
                Case "("
                    If intDepth <> 0 Then strResult &= strChar
                    intDepth += 1
                Case ")"
                    intDepth -= 1
                    If intDepth <> 0 Then strResult &= strChar
                Case Else
                    strResult &= strChar
            End Select
        Next
        DeleteYobunKakko = Trim(strResult)
    End Function

    Public Function FixCursorForLoop(ByVal strSql As String) As String
        Dim strResult As String = ""
        Dim intDepth As Integer = 0
        Dim intPos As Integer
        Dim strChar As String

        For intPos = 1 To strSql.Length
            strChar = Mid(strSql, intPos, 1)
            Select Case strChar
                Case "("
                    intDepth += 1
                Case ")"
                    intDepth -= 1
            End Select
            If intDepth < 0 Then Exit For '括弧閉じの方が多くなったらループを抜ける
            strResult &= strChar
        Next
        FixCursorForLoop = Trim(strResult)
    End Function

End Module
