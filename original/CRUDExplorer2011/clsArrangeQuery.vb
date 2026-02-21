Imports System.Text.RegularExpressions

Public Class clsArrangeQuery
    Private Structure Keyword
        Public Character As String
        Public IndentLevel As Integer
    End Structure

    Private Const INDENT_UNIT As Integer = 4

    Public Function CArrange(ByVal vstrQry As String) As String
        CArrange = ""

        Dim typKeyword() As Keyword = Nothing
        If ReadFile(typKeyword) = False Then
            Exit Function
        End If

        Dim strRet As String
        strRet = ""

        If EditQuery(vstrQry, typKeyword, strRet) = False Then
            Exit Function
        End If

        CArrange = ä÷êîÇÇPçsÇ…(Replace(Replace(strRet, vbCrLf & ",", ","), "AND" & vbCrLf & "    ", "AND "))
    End Function

    Function ä÷êîÇÇPçsÇ…(ByVal strSql As String) As String
        Dim strRet As String = ""
        Dim intCnt As Integer
        Dim intDepth As Integer = 0
        Dim intBaseDepth As Integer = 0
        Dim strChar As String
        Dim bSpc As Boolean = False

        'ä÷êîÇÇPçsÇ… = strSql
        'Exit Function

        If Mid(strSql, 1, 6) = "INSERT" Then intBaseDepth = 1
        For intCnt = 1 To Len(strSql)
            strChar = Mid(strSql, intCnt, 1)
            Select Case strChar
                Case "("
                    intDepth += 1
                    strRet &= strChar
                    bSpc = False
                Case ")"
                    intDepth -= 1
                    If intDepth < 0 Then intDepth = 0
                    strRet &= strChar
                    bSpc = False
                Case vbCr, vbLf
                    If intDepth = intBaseDepth Then
                        strRet &= strChar
                    Else
                        If Not bSpc Then strRet &= " "
                        bSpc = True
                    End If
                Case " "
                    If intDepth = intBaseDepth Then
                        strRet &= strChar
                    Else
                        If Not bSpc Then strRet &= strChar
                        bSpc = True
                    End If
                Case Else
                    strRet &= strChar
                    bSpc = False
            End Select
        Next
        'Else
        'strRet = strSql
        'End If
        ä÷êîÇÇPçsÇ… = strRet

    End Function

    Private Function ReadFile(ByRef rtypKeyword() As Keyword) As Boolean
        ReadFile = False

        Dim strCt As String
        strCt = ""

        If GetKeyword(strCt) = False Then
            Exit Function
        End If

        If SetKeyword(strCt, rtypKeyword) = False Then
            Exit Function
        End If

        ReadFile = True
    End Function

    Private Function GetKeyword(ByRef rstrCt As String) As Boolean
        rstrCt &= "Keyword:IndentLevel" & vbCrLf
        rstrCt &= "SELECT:1" & vbCrLf
        rstrCt &= "INSERT INTO:1" & vbCrLf
        rstrCt &= "INTO:1" & vbCrLf
        rstrCt &= "UPDATE:1" & vbCrLf
        rstrCt &= "DELETE:1" & vbCrLf
        rstrCt &= "FROM:1" & vbCrLf
        rstrCt &= "VALUES:1" & vbCrLf
        rstrCt &= "WHERE:1" & vbCrLf
        rstrCt &= "AND:1" & vbCrLf
        rstrCt &= "OR:1" & vbCrLf
        rstrCt &= "INNER JOIN:2" & vbCrLf
        rstrCt &= "LEFT JOIN:2" & vbCrLf
        rstrCt &= "RIGHT JOIN:2" & vbCrLf
        rstrCt &= "LEFT OUTER JOIN:2" & vbCrLf
        rstrCt &= "RIGHT OUTER JOIN:2" & vbCrLf
        rstrCt &= "ON:2" & vbCrLf
        rstrCt &= "ORDER BY:1" & vbCrLf
        rstrCt &= "GROUP BY:1" & vbCrLf
        rstrCt &= "UNION:0" & vbCrLf
        rstrCt &= "MINUS:0" & vbCrLf
        rstrCt &= "INTERSECT:0" & vbCrLf
        rstrCt &= "SET:1" & vbCrLf
        rstrCt &= ",:1" & vbCrLf

        GetKeyword = True
    End Function

    Private Function SetKeyword(ByVal vstrCt As String, ByRef rtypKeyword() As Keyword) As Boolean
        SetKeyword = False

        On Error GoTo Exception

        Dim arrLine As Object
        arrLine = Split(vstrCt, vbCrLf)

        Dim i As Integer
        For i = 1 To UBound(arrLine)
            Dim strLine As String
            strLine = Trim(arrLine(i))

            If strLine <> "" Then
                Dim arrData As Object
                arrData = Split(strLine, ":")

                ReDim Preserve rtypKeyword(i - 1)
                rtypKeyword(i - 1).Character = arrData(0)
                rtypKeyword(i - 1).IndentLevel = arrData(1)
            End If
        Next i

        SetKeyword = True
        Exit Function

Exception:
        '        Call ShowError(Err)
    End Function

    Private Function EditQuery( _
        ByVal vstrBefore As String, _
        ByRef rtypKeyword() As Keyword, _
        ByRef rstrAfter As String _
    ) As Boolean
        EditQuery = False

        On Error GoTo Exception

        rstrAfter = ReplaceSpecialChar(vstrBefore)

        Dim i As Integer
        For i = 0 To UBound(rtypKeyword)

            Dim lngPos As Long
            lngPos = 1

            Do
                If lngPos >= Len(rstrAfter) Then
                    Exit Do
                End If

                lngPos = InStr(CInt(lngPos), LCase(rstrAfter), LCase(rtypKeyword(i).Character))
                If lngPos < 1 Then
                    Exit Do
                End If

                Dim lngPosPriCh As Long
                lngPosPriCh = lngPos - 1
                Dim strPriCh As String
                If lngPosPriCh > 0 Then
                    strPriCh = Mid(rstrAfter, lngPosPriCh, 1)
                Else
                    strPriCh = ""
                End If

                Dim lngPosNxtCh As Long
                lngPosNxtCh = lngPos + Len(rtypKeyword(i).Character)
                Dim strNxtCh As String
                strNxtCh = Mid(rstrAfter, lngPosNxtCh, 1)

                If IsSeparate(strPriCh) And IsSeparate(strNxtCh) Then
                    rstrAfter = CQueryKeyword(rstrAfter, rtypKeyword(i), lngPos, lngPosNxtCh)
                End If

                lngPos = lngPosNxtCh + 1
            Loop
        Next i

        rstrAfter = ReplaceSpecialCharUndo(rstrAfter)

        EditQuery = True
        Exit Function

Exception:
        '        Call ShowError(Err)
    End Function

    Private Function ReplaceSpecialChar(ByVal vstrBefore As String) As String
        Dim strAfter As String
        strAfter = vstrBefore
        strAfter = Replace(strAfter, vbCrLf, vbCr)
        strAfter = Replace(strAfter, vbTab, AddBlank(1))

        ReplaceSpecialChar = strAfter
    End Function

    Private Function ReplaceSpecialCharUndo(ByVal vstrBefore As String) As String
        Dim strAfter As String
        strAfter = vstrBefore
        strAfter = Replace(strAfter, vbCr, vbCrLf)

        ReplaceSpecialCharUndo = strAfter
    End Function

    Private Function IsSeparate(ByVal vstrChar As String) As Boolean
        If _
            (vstrChar = "") Or _
            (vstrChar = " ") Or _
            (vstrChar = vbTab) Or _
            (vstrChar = vbCr) Then
            IsSeparate = True
        Else
            IsSeparate = False
        End If
    End Function

    Private Function CQueryKeyword( _
        ByVal vstrBefore As String, _
        ByRef rtypKeyword As Keyword, _
        ByVal vlngPos As Long, _
        ByVal vlngPosNxtCh As Long _
    ) As String

        Dim strLeft As String
        strLeft = Trim(Left(vstrBefore, vlngPos - 1))
        If strLeft <> "" Then
            If Right(strLeft, 1) <> vbCr Then
                strLeft = strLeft & vbCr
            End If
        End If

        Dim strTarget As String
        strTarget = Trim(Mid(vstrBefore, vlngPos, Len(rtypKeyword.Character)))
        strTarget = AddBlank(rtypKeyword.IndentLevel - 1, strTarget)
        strTarget = strTarget & vbCr

        Dim strRight As String
        If Len(vstrBefore) >= vlngPosNxtCh Then
            strRight = Trim(Right(vstrBefore, Len(vstrBefore) - vlngPosNxtCh))
            strRight = Trim(ELeftN(strRight))
            strRight = AddBlank(rtypKeyword.IndentLevel, strRight)
        Else
            strRight = ""
        End If

        CQueryKeyword = strLeft & strTarget & strRight
    End Function

    Private Function ELeftN(ByVal vstrChar As String) As String
        Dim strRet As String
        strRet = vstrChar

        Do
            Dim strLeft As String
            strLeft = Left(strRet, 1)
            If (strLeft = vbCr) Then
                strRet = Right(strRet, Len(strRet) - 1)
            Else
                Exit Do
            End If
        Loop

        ELeftN = strRet
    End Function

    Private Function AddBlank( _
        ByVal vintLevel As Integer, _
        Optional ByVal vstrTarget As String = "" _
    ) As String
        Dim strBlank As String
        strBlank = ""

        Dim i As Integer
        For i = 1 To INDENT_UNIT * vintLevel
            strBlank = strBlank & " "
        Next i

        AddBlank = strBlank & vstrTarget
    End Function

End Class
