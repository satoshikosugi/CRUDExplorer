Imports System.Text.RegularExpressions

Public Class frmMakeCRUD

    Private fso As New Scripting.FileSystemObject
    Dim regkey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\CRUDExplorer")
    Private strPath As String

    Private Sub chkProcAll_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkProcAll.CheckedChanged
        If chkProcAll.Checked Then
            chkStep0.Checked = True : chkStep0.Enabled = False
            chkStep1.Checked = True : chkStep1.Enabled = False
            chkStep2.Checked = True : chkStep2.Enabled = False
            chkStep3.Checked = True : chkStep3.Enabled = False
            chkStep4.Checked = True : chkStep4.Enabled = False
        Else
            chkStep0.Checked = False : chkStep0.Enabled = True
            chkStep1.Checked = False : chkStep1.Enabled = True
            chkStep2.Checked = False : chkStep2.Enabled = True
            chkStep3.Checked = False : chkStep3.Enabled = True
            chkStep4.Checked = False : chkStep4.Enabled = True
        End If
    End Sub

    Private Sub btnAnalyzeCRUD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAnalyzeCRUD.Click
        If txtSourcePath.Text = "" Then
            MsgBox("ソースフォルダを指定してください")
            btnSelectSourceFolder.Focus()
            Exit Sub
        End If

        If Not fso.FolderExists(txtSourcePath.Text) Then
            MsgBox("ソースフォルダが存在しません")
            btnSelectSourceFolder.Focus()
            Exit Sub
        End If

        If txtDestPath.Text = "" Then
            MsgBox("解析結果フォルダを指定してください")
            btnSelectDestFolder.Focus()
            Exit Sub
        End If

        If txtSourcePath.Text = txtDestPath.Text Then
            MsgBox("ソースフォルダと同じフォルダが指定されています")
            txtDestPath.Focus()
            Exit Sub
        End If

        Dim strWork As String = txtSourcePath.Text & "\"
        If UCase(GetRight(txtDestPath.Text, strWork.Length)) = UCase(strWork) Then
            MsgBox("ソースフォルダのサブフォルダは指定できません")
            txtDestPath.Focus()
            Exit Sub
        End If

        If chkStep0.Checked = False And chkStep1.Checked = False And chkStep2.Checked = False And chkStep3.Checked = False And chkStep4.Checked = False Then
            MsgBox("処理を選択してください")
            Exit Sub
        End If

        If MsgBox("前回の解析結果がある場合、上書きされます。" & vbCrLf & "処理を実行してもよろしいですか？", MsgBoxStyle.YesNo) = MsgBoxResult.No Then
            Exit Sub
        End If

        If Not fso.FolderExists(txtDestPath.Text) Then
            Try
                fso.CreateFolder(txtDestPath.Text)
            Catch
                MsgBox("解析結果フォルダが作成できません")
                Exit Sub
            End Try
        End If

        strPath = txtDestPath.Text
        txtLog.Clear()

        If chkStep0.Checked Then
            lblPhase.Text = chkStep0.Text : My.Application.DoEvents()
            AddLog(chkStep0.Text & " " & "開始")
            '解析結果フォルダをクリア
            DeleteFolder(txtDestPath.Text, True)

            '解析結果フォルダにソース、定義ファイルをコピー
            Dim objSourceFolder As Scripting.Folder = fso.GetFolder(txtSourcePath.Text)
            Dim objFile As Scripting.File
            For Each objFile In objSourceFolder.Files
                If fso.FileExists(txtDestPath.Text & "\" & objFile.Name) Then
                    Dim objDestFile As Scripting.File = fso.GetFile(txtDestPath.Text & "\" & objFile.Name)
                    objDestFile.Delete(True)
                End If
                objFile.Copy(txtDestPath.Text & "\" & objFile.Name)
            Next
            AddLog(chkStep0.Text & " " & "終了")
        End If

        pnlControl.Enabled = False
        If chkStep1.Checked Then Step1()
        If chkStep2.Checked Then Step2()
        If chkStep3.Checked Then Step3()
        If chkStep4.Checked Then
            Step4()
            Step4Column()
        End If

        AddLog("解析完了")
        lblPhase.Text = "解析完了"
        pnlControl.Enabled = True
        regkey.SetValue("SourceFolder", txtSourcePath.Text)
        regkey.SetValue("DestFolder", txtDestPath.Text)
        btnClose.Focus()


    End Sub

    Private Sub btnSelectSourceFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectSourceFolder.Click
        FolderBrowserDialog1.SelectedPath = txtSourcePath.Text
        FolderBrowserDialog1.Description = "ソースフォルダを指定"
        If FolderBrowserDialog1.ShowDialog() <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        txtSourcePath.Text = FolderBrowserDialog1.SelectedPath
        btnSelectDestFolder.Focus()
    End Sub

    Private Sub btnSelectDestFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectDestFolder.Click
        FolderBrowserDialog1.SelectedPath = txtDestPath.Text
        FolderBrowserDialog1.Description = "解析結果フォルダを指定"
        If FolderBrowserDialog1.ShowDialog() <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If
        txtDestPath.Text = FolderBrowserDialog1.SelectedPath
        btnAnalyzeCRUD.Focus()
    End Sub

    Private Sub AddLog(ByVal strLog As String)
        txtLog.Text &= strLog & vbCrLf
        txtLog.SelectionStart = Len(txtLog.Text) - 1
        txtLog.ScrollToCaret()
        My.Application.DoEvents()
    End Sub

    Sub Step1()
        Dim t As Scripting.TextStream
        Dim lngFileCnt As Long
        Dim objFolder As Scripting.Folder
        Dim objFile As Scripting.File
        Dim strSrc As String, strWork As String
        Dim objMatches As MatchCollection
        Dim strLines As String()
        Dim lngLineNo As Long
        Dim lngCnt As Long
        Dim strDynamicSQL As String
        Dim lngBeforeIndex As Long, lngMergin As Long
        Dim bolSQL As Boolean, bolSQLWork As Boolean

        lblPhase.Text = chkStep1.Text : My.Application.DoEvents()
        AddLog(chkStep1.Text & " " & "開始")

        lngFileCnt = 0
        objFolder = fso.GetFolder(strPath)
        For Each objFile In objFolder.Files
            If LCase(GetRight(objFile.Name, 5)) = ".dsql" Then objFile.Delete()
        Next

        For Each objFile In objFolder.Files
            lngFileCnt = lngFileCnt + 1
            t = fso.OpenTextFile(strPath & "\" & objFile.Name)
            strSrc = t.ReadAll
            t.Close()

            'Formsのプロパティ部分を削除
            If GetRight(objFile.Name, 4) = ".txt" Then
                strSrc = DeleteFormsPropertyInfo(strSrc)
            End If

            '            AddLog(lblPhase.Text & " " & lngFileCnt & "/" & objFolder.Files.Count & " " & objFile.Name & " 処理中...")
            lblPhase.Text = chkStep1.Text & " " & lngFileCnt & "/" & objFolder.Files.Count & " " & objFile.Name : My.Application.DoEvents()
            '動的SQLをなんとなく分析
            strDynamicSQL = ""
            bolSQL = False
            lngBeforeIndex = 0
            lngMergin = 0
            strWork = Replace(Replace(DeleteComment(strSrc, True), "''", ""), vbCr, "")
            strWork = Replace(strWork, vbTab, " ")
            strWork = Replace(strWork, "(", " ( ")
            strWork = Replace(strWork, ")", " ) ")

            strLines = Split(strWork, vbLf)
            For lngLineNo = LBound(strLines) To UBound(strLines)
                If InStr(strLines(lngLineNo), "W_MSG") < 1 Then
                    objMatches = Regex.Matches(strLines(lngLineNo), "['][^']*[']")
                    If objMatches.Count > 0 Then
                        If bolSQL = True Then
                            '有る程度前回の文字列を含む行か離れてれている場合、SQL編集処理が終わっているものと判断する
                            If ((lngBeforeIndex + 10) < lngLineNo) Or InStr(strLines(lngLineNo), "EXCEPTION") > 0 Or InStr(strLines(lngLineNo), "PROCEDURE") > 0 Or InStr(strLines(lngLineNo), "FUNCTION") > 0 Then
                                bolSQL = False
                                strDynamicSQL = strDynamicSQL & " ;" & vbLf
                            End If
                        End If

                        For lngCnt = 0 To objMatches.Count - 1
                            If objMatches(lngCnt).Length > 2 Then
                                strWork = Mid(objMatches(lngCnt).Value, 2, Len(objMatches(lngCnt).Value) - 2)
                                bolSQLWork = bolSQL
                                If RegMatchI(strWork, "SELECT") Or RegMatchI(strWork, "INSERT") Or RegMatchI(strWork, "UPDATE") Or RegMatchI(strWork, "DELETE") Or RegMatchI(strWork, "TRUNCATE") Then
                                    If bolSQLWork = False Then  'SQLに入ったところにタグジャンプ用のコードを追加
                                        strDynamicSQL = strDynamicSQL & vbCrLf & objFile.Name & "(" & lngLineNo + 1 & "):----------------------------------------------------;" & vbCrLf
                                    End If
                                    bolSQL = True
                                End If
                                If bolSQL Then strDynamicSQL = strDynamicSQL & vbCrLf & strWork
                            End If
                        Next
                        lngBeforeIndex = lngLineNo
                    End If
                End If
            Next
            If strDynamicSQL <> "" Then
                strDynamicSQL = Regex.Replace(strDynamicSQL, "(\s*SELECT\s*)", "$1", RegexOptions.IgnoreCase)
                strDynamicSQL = Regex.Replace(strDynamicSQL, "(\s*INSERT\s*)", "$1", RegexOptions.IgnoreCase)
                strDynamicSQL = Regex.Replace(strDynamicSQL, "(\s*UPDATE\s*)", "$1", RegexOptions.IgnoreCase)
                strDynamicSQL = Regex.Replace(strDynamicSQL, "(\s*DELETE\s*)", "$1", RegexOptions.IgnoreCase)
                strDynamicSQL = Regex.Replace(strDynamicSQL, "(\s*TRUNCATE\s*)", "$1", RegexOptions.IgnoreCase)

                strSrc = strSrc & vbLf & strDynamicSQL & ";"
                CreateFile(strPath, objFile.Name & ".dsql", strDynamicSQL)
            End If
        Next
        AddLog(chkStep1.Text & " " & "終了")

    End Sub

    Private Sub Step2()
        Dim t As Scripting.TextStream, w As Scripting.TextStream, tv As Scripting.TextStream
        Dim strSrc As String, strWork As String
        Dim objMatches As MatchCollection, objMatches2 As MatchCollection
        Dim objFolder As Scripting.Folder, objFile As Scripting.File
        Dim strLines As String()
        Dim lngCnt As Long, lngLineNo As Long, lngWorkCnt As Long
        Dim lngPos As Long
        Dim lngStart As Long, lngWork As Long
        Dim strChar As String, strSql As String
        Dim strWriteFileName As String
        Dim strDynamicSQL As String
        Dim lngBeforeIndex As Long, lngMergin As Long
        Dim lngFileCnt As Long
        Dim bolSQL As Boolean, bolSQLWork As Boolean
        Dim strPackageName As String = ""
        Dim strFuncProcName As String = ""
        Dim strCursorName As String = ""
        Dim bolIsView As Boolean = False
        Dim strViewQuery As String = ""

        lblPhase.Text = chkStep2.Text : My.Application.DoEvents()
        AddLog(chkStep2.Text & " " & "開始")

        DeleteFolder(strPath & "\querys")

        If Not fso.FolderExists(strPath & "\querys") Then
            fso.CreateFolder(strPath & "\querys")
        End If
        objFolder = fso.GetFolder(strPath)
        tv = fso.OpenTextFile(strPath & "\querys\" & "views.txt", 2, True)

        lngFileCnt = 0
        For Each objFile In objFolder.Files
            If bolDemoFlag And lngFileCnt = 3 Then AddLog(lblPhase.Text & " " & lngFileCnt & "/" & objFolder.Files.Count & " " & objFile.Name & " 評価版の制限につき処理を中断") : Exit For
            strPackageName = ""
            strFuncProcName = ""
            strCursorName = ""
            lngFileCnt = lngFileCnt + 1
            '            AddLog(lblPhase.Text & " " & lngFileCnt & "/" & objFolder.Files.Count & " " & objFile.Name & " 処理中...")
            lblPhase.Text = chkStep2.Text & " " & lngFileCnt & "/" & objFolder.Files.Count & " " & objFile.Name : My.Application.DoEvents()
            t = fso.OpenTextFile(strPath & "\" & objFile.Name)
            strSrc = t.ReadAll
            t.Close()

            strWriteFileName = objFile.Name & ".query"
            On Error Resume Next
            fso.DeleteFile(strPath & "\querys\" & strWriteFileName)
            On Error GoTo 0
            w = fso.OpenTextFile(strPath & "\querys\" & strWriteFileName, 2, True)

            strSrc = DeleteComment(strSrc, True)                                 'コメントを削除する
            'Formsのプロパティ部分を削除
            If GetRight(objFile.Name, 4) = ".txt" Then
                strSrc = DeleteFormsPropertyInfo(strSrc)
                CreateFile(strPath & "\querys\formsconv\", objFile.Name, strSrc)
            End If

            strSrc = Regex.Replace(strSrc, "\\""", "", RegexOptions.IgnoreCase)         'エスケープされたダブルクォートを削除
            '            strSrc = Regex.Replace(strSrc, "'[^']*'", "'@'", RegexOptions.IgnoreCase)   '文字列部分を削除
            strSrc = Regex.Replace(strSrc, "[(]", " ( ", RegexOptions.IgnoreCase)
            strSrc = Regex.Replace(strSrc, "[)]", " ) ", RegexOptions.IgnoreCase)
            strSrc = Regex.Replace(strSrc, "\t", " ", RegexOptions.IgnoreCase)
            strSrc = Regex.Replace(strSrc, "[ ]+", " ", RegexOptions.IgnoreCase)
            '            strSrc = Replace(strSrc, vbCr, "")
            ''''           CreateFile(objFolder.Path & "\work\", objFile.Name, strSrc)
            strLines = Split(strSrc, vbLf)
            Dim intQueryCnt As Integer = 0

            For lngCnt = LBound(strLines) To UBound(strLines)
                If bolDemoFlag And intQueryCnt = 10 Then AddLog(lblPhase.Text & " " & lngFileCnt & "/" & objFolder.Files.Count & " " & objFile.Name & " 評価版の制限につき処理を中断") : Exit For
                'パッケージ名を取得
                If strPackageName <> "" Then
                    If RegMatch(strLines(lngCnt), "\b*END\s+" & EscapeRegular(strPackageName) & "\b*", RegexOptions.IgnoreCase) Then
                        strPackageName = ""     'パッケージの終端
                    End If
                End If
                Dim strWork2 As String = Regex.Replace(strLines(lngCnt), "'[^']*'", "")
                objMatches = Regex.Matches(strWork2, "\b*CREATE\s+OR\s+REPLACE\s+PACKAGE\s+BODY\s+([^\s]+)\s+IS$", RegexOptions.IgnoreCase)
                If objMatches.Count > 0 Then
                    strPackageName = objMatches(0).Groups(1).Value
                Else
                    objMatches = Regex.Matches(strWork2, "\b*CREATE\s+OR\s+REPLACE\s+PACKAGE\s+([^\s]+)\s+IS$", RegexOptions.IgnoreCase)
                    If objMatches.Count > 0 Then
                        strPackageName = objMatches(0).Groups(1).Value
                    Else
                        objMatches = Regex.Matches(strWork2, "\b*CREATE\s+PACKAGE\s+BODY\s+([^\s]+)\s+IS$", RegexOptions.IgnoreCase)
                        If objMatches.Count > 0 Then
                            strPackageName = objMatches(0).Groups(1).Value
                        Else
                            objMatches = Regex.Matches(strWork2, "\b*CREATE\s+PACKAGE\s+([^\s]+)\s+IS$", RegexOptions.IgnoreCase)
                            If objMatches.Count > 0 Then strPackageName = objMatches(0).Groups(1).Value
                        End If
                    End If
                End If

                'FUNCTION/PROCEDURE名を取得
                objMatches = Regex.Matches(strWork2, "(FUNCTION|PROCEDURE|CURSOR)\s+([^\b]+)[\b]*$", RegexOptions.IgnoreCase)
                If objMatches.Count > 0 Then
                    Dim strArr As String() = Split(objMatches(0).Groups(2).Value, " ")
                    '                    lngPos = InStr(objMatches(0).Groups(2).Value, "(")
                    If UCase(objMatches(0).Groups(1).Value) = "CURSOR" Then
                        strCursorName = strArr(0)
                    Else
                        strFuncProcName = strArr(0)
                    End If
                End If

                lngPos = 0
                objMatches = Regex.Matches(strLines(lngCnt), "(\s*CREATE\s+VIEW\s*|\s*CREATE\s+OR\s+REPLACE\s+VIEW\s*)", RegexOptions.IgnoreCase)
                If objMatches.Count > 0 Then
                    bolIsView = True
                End If
                If bolIsView Then
                    strViewQuery &= strLines(lngCnt) & " "
                End If

                objMatches = Regex.Matches(strLines(lngCnt), "(?:[(])?(\s*SELECT\s*|\s*INSERT\s*|\s*UPDATE\s*|\s*DELETE\s*|\s*TRUNCATE\s*|\s*WITH\s*)", RegexOptions.IgnoreCase)
                If objMatches.Count > 0 Then
                    If objMatches(0).Index = 0 Then
                        lngPos = objMatches(0).Index + 1
                    ElseIf Mid(strLines(lngCnt), objMatches(0).Index, 1) = " " Then
                        lngPos = objMatches(0).Index + 1
                    End If
                    If lngPos <> 0 Then
                        If Replace(GetRight(objMatches(0).Groups(0).Value, 1), vbTab, " ") <> " " Then
                            Dim strC As String = Mid(strLines(lngCnt), lngPos + objMatches(0).Groups(0).Length, 1)
                            If strC <> "" And strC <> " " Then lngPos = 0 '次の文字が~文字かスペースでない場合はSQLの開始とみなさない
                        End If
                    End If

                Else
                    objMatches = Regex.Matches(strLines(lngCnt), "(IS SELECT\s|AS SELECT\s)", RegexOptions.IgnoreCase)
                    If objMatches.Count > 0 Then
                        lngPos = objMatches(0).Index + 4
                    End If
                End If

                'SQLを発見
                If lngPos <> 0 Then
                    intQueryCnt += 1
                    lngLineNo = lngCnt - LBound(strLines) + 1
                    lngStart = lngPos
                    lngWorkCnt = lngCnt
                    strSql = ""
                    Do
                        If lngCnt > UBound(strLines) Then Exit Do
                        lngWork = InStr(CInt(lngStart), strLines(lngCnt), ";")
                        If lngWork < 1 AndAlso bolIsView Then    ';が見つからない且つビューの場合
                            lngWork = InStr(CInt(lngStart), Regex.Replace(strLines(lngCnt), "['][^']*[']", ""), "/")      '文字列を除去した状態で/を探す
                        End If
                        '                        lngWork = InStr(CInt(lngStart), strLines(lngCnt), ";")
                        If lngWork > 0 Then
                            strSql = strSql & " " & Mid(strLines(lngCnt), lngStart, lngWork - lngStart)
                            Exit Do
                        Else
                            If lngWorkCnt <> lngCnt Then
                                objMatches2 = Regex.Matches(strLines(lngCnt), "^\s*-[*]", RegexOptions.IgnoreCase)
                                If objMatches2.Count > 0 Then   'FormsのレコードグループのSQLの終端
                                    Exit Do
                                End If
                            End If
                            strSql = strSql & " " & Mid(strLines(lngCnt), lngStart)
                        End If
                        lngCnt = lngCnt + 1
                        lngStart = 1
                    Loop
                    strSql = Regex.Replace(strSql, "[ ]+", " ", RegexOptions.IgnoreCase)
                    strSql = Trim(strSql)
                    strSql = DeleteYobunKakko(strSql) '余分な括弧を削除
                    strSql = FixCursorForLoop(strSql) 'カーソルForループの場合の後ろの余計な部分を削除する


                    If UCase(Mid(strSql, 1, 4)) = "WITH" Then
                        If RegMatch(strSql, " SELECT ", RegexOptions.IgnoreCase) Then w.WriteLine(lngLineNo & vbTab & strSql & vbTab & PackFuncProcCurName(strPackageName, strFuncProcName, strCursorName)) 'SELECT句が含まれる場合にファイル出力(WITH誤検出排除)
                    End If
                    If UCase(Mid(strSql, 1, 6)) = "SELECT" Then
                        If RegMatch(strSql, " FROM ", RegexOptions.IgnoreCase) Then w.WriteLine(lngLineNo & vbTab & strSql & vbTab & PackFuncProcCurName(strPackageName, strFuncProcName, strCursorName)) 'FROM句が含まれる場合にファイル出力(SELECT誤検出排除)
                    End If
                    If UCase(Mid(strSql, 1, 6)) = "INSERT" Then
                        If RegMatch(strSql, " INTO ", RegexOptions.IgnoreCase) Then w.WriteLine(lngLineNo & vbTab & strSql & vbTab & PackFuncProcCurName(strPackageName, strFuncProcName, strCursorName)) 'INTO句が含まれる場合にファイル出力(INSERT誤検出排除)
                    End If
                    If UCase(Mid(strSql, 1, 6)) = "UPDATE" Then
                        If RegMatch(strSql, " SET ", RegexOptions.IgnoreCase) Then w.WriteLine(lngLineNo & vbTab & strSql & vbTab & PackFuncProcCurName(strPackageName, strFuncProcName, strCursorName)) 'SET句が含まれる場合にファイル出力(UPDATE誤検出排除)
                    End If
                    If UCase(Mid(strSql, 1, 6)) = "DELETE" Then   'DELETE    ※本来はFROMが付いているはずだが、FROMをつけていない行儀の悪いSQLが多いための措置
                        If RegMatch(strSql, " WHERE ", RegexOptions.IgnoreCase) Then
                            If UCase(Mid(strSql, 8, 4)) <> "FROM" Then
                                strSql = "DELETE FROM " & Mid(strSql, 8)
                            End If
                            w.WriteLine(lngLineNo & vbTab & strSql & vbTab & PackFuncProcCurName(strPackageName, strFuncProcName, strCursorName))
                        End If
                    End If
                    If UCase(Mid(strSql, 1, 8)) = "TRUNCATE" Then
                        If RegMatch(strSql, " TABLE ", RegexOptions.IgnoreCase) Then w.WriteLine(lngLineNo & vbTab & strSql & vbTab & PackFuncProcCurName(strPackageName, strFuncProcName, strCursorName)) 'TABLE句が含まれる場合にファイル出力(TRUNCATE誤検出排除)
                    End If

                    If bolIsView Then   'Viewの場合
                        objMatches = Regex.Matches(strViewQuery, "\s*CREATE\s+(?:OR\s+REPLACE\s+)?VIEW\s*([^\s]*)\s*", RegexOptions.IgnoreCase)
                        If objMatches.Count > 0 Then
                            tv.WriteLine(objMatches(0).Groups(1).Value & vbTab & objFile.Name & vbTab & lngLineNo & vbTab & strSql)
                        End If

                        bolIsView = False
                        strViewQuery = ""
                    End If
                    If strCursorName <> "" Then
                        strCursorName = ""
                    End If
                End If
            Next

            w.Close()

        Next
        tv.Close()
        '	MsgBox "SQLを以下のフォルダに抽出しました" & vbCrLf & strPath & "\querys\"
        AddLog(chkStep2.Text & " " & "終了")

    End Sub

    Private Function PackFuncProcCurName(ByVal strPackageName As String, ByVal strFuncProcName As String, ByVal strCursorName As String) As String
        Dim strRet As String = ""

        If strPackageName <> "" Then strRet = strPackageName
        If strFuncProcName <> "" Then
            If strRet <> "" Then strRet &= "."
            strRet &= strFuncProcName
        End If
        If strCursorName <> "" Then
            If strRet <> "" Then strRet &= "."
            strRet &= strCursorName
        End If

        PackFuncProcCurName = strRet
    End Function

    Private Sub Step3()
        Dim t As Scripting.TextStream, w As Scripting.TextStream, w2 As Scripting.TextStream, wc As Scripting.TextStream = Nothing
        Dim sql As String
        Dim dctE As New Scripting.Dictionary
        Dim intCnt As Integer
        Dim objFolder As Scripting.Folder, objFile As Scripting.File
        Dim strLine As String
        Dim strLineNo As String
        Dim lngPos As Long
        Dim strModuleName As String, strProgramID As String
        Dim lngFileCnt As Long
        Dim strFolderName As String
        Dim arrWork As String()
        Dim strFuncProName As String, strTableName As String, strColumnName As String, strArr As String
        Dim strKey As String
        Dim intQueryFileCnt As Integer

        lblPhase.Text = chkStep3.Text : My.Application.DoEvents()
        AddLog(chkStep3.Text & " " & "開始")

        objFolder = fso.GetFolder(strPath & "\querys")
        On Error Resume Next
        fso.DeleteFile(strPath & "\querys\CRUD.tsv")
        fso.DeleteFile(strPath & "\querys\CRUDColumns.tsv")
        On Error GoTo 0
        w = fso.OpenTextFile(strPath & "\querys\CRUD.tsv", 2, True)
        wc = fso.OpenTextFile(strPath & "\querys\CRUDColumns.tsv", 2, True)

        DeleteFolder(strPath & "\querys\subquerys")
        If Not fso.FolderExists(strPath & "\querys\subquerys") Then
            fso.CreateFolder(strPath & "\querys\subquerys")
        End If

        intQueryFileCnt = 0
        For Each objFile In objFolder.Files
            If LCase(GetRight(objFile.Name, 6)) = ".query" Then intQueryFileCnt += 1
        Next

        lngFileCnt = 0
        For Each objFile In objFolder.Files
            If bolDemoFlag And lngFileCnt = 3 Then AddLog(lblPhase.Text & " " & lngFileCnt & "/" & intQueryFileCnt & " " & objFile.Name & " 評価版の制限につき処理を中断") : Exit For
            If LCase(GetRight(objFile.Name, 6)) = ".query" Then
                lngFileCnt = lngFileCnt + 1
                w2 = fso.OpenTextFile(strPath & "\querys\subquerys\" & objFile.Name, 2, True)
                '                AddLog(lblPhase.Text & " " & lngFileCnt & "/" & intQueryFileCnt & " " & objFile.Name & " 処理中...")
                lblPhase.Text = chkStep3.Text & " " & lngFileCnt & "/" & intQueryFileCnt & " " & objFile.Name : My.Application.DoEvents()
                t = fso.OpenTextFile(objFile.Path)
                strModuleName = Mid(objFile.Name, 1, Len(objFile.Name) - 6)  '拡張子を除いた部分がモジュール名
                strProgramID = GetProgramId(strModuleName)              'プログラムIDらしき部分を抽出
                Do While t.AtEndOfStream = False
                    strLine = Trim(t.ReadLine)
                    If strLine <> "" Then
                        lngPos = InStr(strLine, vbTab) - 1
                        strLineNo = Mid(strLine, 1, lngPos)                '行番号
                        arrWork = Split(Mid(strLine, lngPos + 2), vbTab)
                        sql = arrWork(0)                        'SQL
                        If UBound(arrWork) >= 1 Then
                            strFuncProName = arrWork(1)
                        Else
                            strFuncProName = ""
                        End If
                        Dim objQuery As New clsQuery
                        objQuery.FileName = Mid(objFile.Name, 1, objFile.Name.Length - 6)
                        objQuery.LineNo = strLineNo
                        w2.WriteLine(strLine)
                        Call AnalyzeCRUD(sql, objQuery, dctE, objFile.Name, w2, , chkReferenceCond.Checked)
                        w2.WriteLine("")

                        Dim dctC As Scripting.Dictionary = objQuery.AllTableC
                        Dim dctR As Scripting.Dictionary = objQuery.AllTableR
                        Dim dctU As Scripting.Dictionary = objQuery.AllTableU
                        Dim dctD As Scripting.Dictionary = objQuery.AllTableD

                        For Each strKey In dctC.Keys
                            arrWork = Split(dctC.Item(strKey), vbTab)
                            strTableName = arrWork(0)
                            If UBound(arrWork) >= 1 Then
                                strArr = arrWork(1)
                            Else
                                strArr = ""
                            End If
                            w.WriteLine(strModuleName & vbTab & strProgramID & vbTab & strLineNo & vbTab & strTableName & vbTab & "C" & vbTab & strFuncProName & vbTab & strArr)
                        Next

                        For Each strKey In dctR.Keys
                            arrWork = Split(dctR.Item(strKey), vbTab)
                            strTableName = arrWork(0)
                            If UBound(arrWork) >= 1 Then
                                strArr = arrWork(1)
                            Else
                                strArr = ""
                            End If
                            w.WriteLine(strModuleName & vbTab & strProgramID & vbTab & strLineNo & vbTab & strTableName & vbTab & "R" & vbTab & strFuncProName & vbTab & strArr)
                        Next
                        For Each strKey In dctU.Keys
                            arrWork = Split(dctU.Item(strKey), vbTab)
                            strTableName = arrWork(0)
                            If UBound(arrWork) >= 1 Then
                                strArr = arrWork(1)
                            Else
                                strArr = ""
                            End If
                            w.WriteLine(strModuleName & vbTab & strProgramID & vbTab & strLineNo & vbTab & strTableName & vbTab & "U" & vbTab & strFuncProName & vbTab & strArr)
                        Next
                        For Each strKey In dctD.Keys
                            arrWork = Split(dctD.Item(strKey), vbTab)
                            strTableName = arrWork(0)
                            If UBound(arrWork) >= 1 Then
                                strArr = arrWork(1)
                            Else
                                strArr = ""
                            End If
                            w.WriteLine(strModuleName & vbTab & strProgramID & vbTab & strLineNo & vbTab & strTableName & vbTab & "D" & vbTab & strFuncProName & vbTab & strArr)
                        Next

                        'テーブル・カラムCRUD
                        strTableName = ""
                        strColumnName = ""
                        Dim dct As Scripting.Dictionary
                        dct = objQuery.AllColumnC
                        For Each strKey In dct
                            wc.WriteLine(strModuleName & vbTab & strProgramID & vbTab & strLineNo & vbTab & dct(strKey) & vbTab & "C" & vbTab & strFuncProName & vbTab)
                        Next
                        dct = objQuery.AllColumnR
                        For Each strKey In dct
                            wc.WriteLine(strModuleName & vbTab & strProgramID & vbTab & strLineNo & vbTab & dct(strKey) & vbTab & "R" & vbTab & strFuncProName & vbTab)
                        Next
                        dct = objQuery.AllColumnU
                        For Each strKey In dct
                            wc.WriteLine(strModuleName & vbTab & strProgramID & vbTab & strLineNo & vbTab & dct(strKey) & vbTab & "U" & vbTab & strFuncProName & vbTab)
                        Next
                        'Dはテーブルのカラムを全部出力するでよし
                        For Each strKey In dctD.Keys
                            Dim strTable As String = Split(dctD(strKey), vbTab)(0)
                            If DictExists(dctTableDef, strTable) Then        'テーブル定義に存在
                                Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, strTable)
                                Dim objColumnDef As clsColumnDef
                                Dim strKey2 As String
                                For Each strKey2 In objTableDef.dctColumnts.Keys
                                    objColumnDef = GetDictObject(objTableDef.dctColumnts, strKey2)
                                    wc.WriteLine(strModuleName & vbTab & strProgramID & vbTab & strLineNo & vbTab & strTable & "." & objColumnDef.ColumnName & vbTab & "D" & vbTab & strFuncProName & vbTab)
                                Next
                            End If
                        Next
                    End If
                Loop
                w2.Close()
            End If
        Next
        w.Close()
        wc.Close()
        '	MsgBox "解析結果を" & vbCrLf & strPath & "\CRUD.tsv" & vbCrLf & "に出力しました。"
        On Error Resume Next
        Call fso.DeleteFile(strPath & "\querys\ERR.tsv")
        On Error GoTo 0
        If dctE.Count > 0 Then
            w = fso.OpenTextFile(strPath & "\querys\ERR.tsv", 2, True)
            w.WriteLine("エラー内容" & vbTab & "ファイル名" & vbTab & "SQL")
            AddLog(lblPhase.Text & " ★エラーがありました。解析結果が不完全である可能性があります★エラーが多い場合、表示までに時間がかかります★")
            Dim strErr As String = ""
            For Each strKey In dctE.Keys
                w.WriteLine(dctE.Item(strKey))
                strErr &= lblPhase.Text & " " & dctE.Item(strKey) & vbCrLf
                If intCnt Mod 1000 = 0 Then
                    My.Application.DoEvents()
                End If
            Next
            txtLog.Text &= strErr
            w.Close()
        End If
        AddLog(chkStep3.Text & " " & "終了")

    End Sub

    Private Sub Step4()
        Dim t As Scripting.TextStream, w As Scripting.TextStream, wc As Scripting.TextStream
        Dim lngCnt As Long, lngPIdx As Long, lngTIdx As Long, lngProgramCnt As Long, lngTableCnt As Long, lngProgramIdx As Long, lngTableIdx As Long
        Dim dctPrograms As New Scripting.Dictionary, dctTables As New Scripting.Dictionary, dctMatrix As New Scripting.Dictionary
        Dim arrLine As String(), arrCols As String(), arrMatrix As String(,)
        Dim strProgramName As String, strTableName As String, strViewName As String, strCRUD As String
        Dim objCRUD As clsCRUD, objCRUD2 As clsCRUD
        Dim strLine As String, strFolderName As String

        AddLog(chkStep4.Text & " " & "開始")
        If Not fso.FileExists(strPath & "\querys\CRUD.tsv") Then
            AddLog(lblPhase.Text & " " & "\querys\CRUD.tsv ファイルが見つからないため、CRUD分析を行えません")
            Exit Sub
        End If

        lblPhase.Text = chkStep4.Text : My.Application.DoEvents()
        '        AddLog(lblPhase.Text & " テーブルCRUD 生成中...")

        ReDim arrMatrix(0, 0)

        strFolderName = Mid(strPath, 1, InStrRev(strPath, "\"))

        t = fso.OpenTextFile(strPath & "\querys\CRUD.tsv")
        On Error Resume Next
        fso.DeleteFile(strPath & "\querys\CRUDMatrix.tsv")
        On Error GoTo 0
        w = fso.OpenTextFile(strPath & "\querys\CRUDMatrix.tsv", Scripting.IOMode.ForWriting, True)

        lngProgramCnt = 0
        lngTableCnt = 0
        Do
            If t.AtEndOfStream Then Exit Do

            arrLine = Split(t.ReadLine, vbTab)

            strProgramName = arrLine(1)
            If strProgramName = "" Then strProgramName = arrLine(0)
            strTableName = arrLine(3)
            strCRUD = arrLine(4)
            If Not DictExists(dctPrograms, strProgramName) Then
                lngProgramCnt = lngProgramCnt + 1
                dctPrograms(strProgramName) = lngProgramCnt
            End If
            If Not DictExists(dctTables, strTableName) Then
                lngTableCnt = lngTableCnt + 1
                dctTables(strTableName) = lngTableCnt
            End If
            If DictExists(dctMatrix, strProgramName & ":" & strTableName) Then
                objCRUD = GetDictObject(dctMatrix, strProgramName & ":" & strTableName)
            Else
                objCRUD = New clsCRUD
                Call DictAdd(dctMatrix, strProgramName & ":" & strTableName, objCRUD)
            End If

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
        Loop

        On Error Resume Next
        If chkReference.Checked Then
            '間接的に参照(VIEW経由)しているテーブルの情報の構築

            colViews.Clear()
            If fso.FileExists(strPath & "\querys\views.txt") Then
                Dim t2 As Scripting.TextStream
                t2 = fso.OpenTextFile(strPath & "\querys\views.txt")
                Do Until t2.AtEndOfStream
                    strLine = t2.ReadLine
                    Dim strCols As String() = Split(strLine, vbTab)
                    colViews.add(New clsView(strCols(0), strCols(1), strCols(2), strCols(3)))
                Loop
            End If

            Dim objView As clsView
            Dim objQuery As New clsQuery
            For Each objView In colViews
                AnalyzeCRUD(objView.Query, objQuery, Nothing, "", Nothing, True)

                Dim strKey As String
                For Each strKey In dctMatrix.Keys
                    Debug.Print(strKey)
                Next

                For Each strKey In dctMatrix.Keys
                    Dim strArr As String() = Split(strKey, ":")
                    If UCase(strArr(1)) = UCase(objView.ViewName) Then  'Viewにアクセスしているプログラム
                        Dim strViewAccessProgram As String = UCase(strArr(0))
                        Dim strKey2 As String
                        For Each strKey2 In objQuery.TableR.Keys
                            Dim strViewAccessTable As String = UCase(Split(objQuery.TableR(strKey2), vbTab)(0))
                            If DictExists(dctMatrix, strViewAccessProgram & ":" & strViewAccessTable) Then
                                objCRUD = GetDictObject(dctMatrix, strViewAccessProgram & ":" & strViewAccessTable)
                            Else
                                objCRUD = New clsCRUD
                                Call DictAdd(dctMatrix, strViewAccessProgram & ":" & strViewAccessTable, objCRUD)
                            End If
                            objCRUD.RefR = True
                        Next
                    End If
                Next
            Next




            '    For Each strViewName In dctPrograms.Keys
            '        If DictExists(dctTables, Replace(strViewName, ".sql", "")) Then       'テーブル名がプログラム群に存在する場合はVIEWと判断
            '            For Each strTableName In dctTables.Keys
            '                If GetCRUD(dctMatrix, strViewName & ":" & strTableName) <> "" Then       'VIEWが参照しているテーブル
            '                    objCRUD = GetDictObject(dctMatrix, strViewName & ":" & strTableName)

            '                    For Each strProgramName In dctPrograms.Keys                     'VIEWを参照しているプログラム
            '                        If strProgramName <> strViewName And GetCRUD(dctMatrix, strProgramName & ":" & Replace(strViewName, ".sql", "")) <> "" Then   'VIEWを参照しているプログラム
            '                            If DictExists(dctMatrix, strProgramName & ":" & strTableName) Then
            '                                objCRUD2 = GetDictObject(dctMatrix, strProgramName & ":" & strTableName)
            '                            Else
            '                                objCRUD2 = New clsCRUD
            '                                Call DictAdd(dctMatrix, strProgramName & ":" & strTableName, objCRUD2)
            '                            End If
            '                            objCRUD2.RefC = objCRUD.C
            '                            objCRUD2.RefR = objCRUD.R
            '                            objCRUD2.RefU = objCRUD.U
            '                            objCRUD2.RefD = objCRUD.D
            '                        End If
            '                    Next

            '                End If
            '            Next

            '        End If
            '    Next
        End If
        On Error GoTo 0

        dctPrograms = SortDictionary(dctPrograms)
        dctTables = SortDictionary(dctTables)

        strLine = "テーブル名" & vbTab & "エンティティ名" & vbTab
        For Each strProgramName In dctPrograms.Keys
            strLine = strLine & vbTab & strProgramName
        Next
        '数式
        w.WriteLine(strLine)
        strLine = vbTab & vbTab
        lngCnt = 4
        For Each strProgramName In dctPrograms.Keys
            strLine = strLine & vbTab & "=COUNTA(" & ColToRange(lngCnt) & "3:" & ColToRange(lngCnt) & dctTables.Count + 3 & ")"
            lngCnt = lngCnt + 1
        Next
        w.WriteLine(strLine)

        lngCnt = 3
        For Each strTableName In dctTables.Keys
            strLine = strTableName & vbTab
            If DictExists(dctTableName, strTableName) Then strLine = strLine & GetDictValue(dctTableName, strTableName)
            strLine = strLine & vbTab & "=COUNTA(" & ColToRange(4) & lngCnt & ":" & ColToRange(dctPrograms.Count + 4) & lngCnt & ")"
            For Each strProgramName In dctPrograms.Keys
                strLine = strLine & vbTab
                If DictExists(dctMatrix, strProgramName & ":" & strTableName) Then
                    objCRUD = GetDictObject(dctMatrix, strProgramName & ":" & strTableName)
                    strLine = strLine & objCRUD.GetCRUD
                    If objCRUD.GetRefCRUD <> "" Then        '間接参照
                        strLine = strLine & "(" & objCRUD.GetRefCRUD & ")"
                    End If
                End If
            Next
            w.WriteLine(strLine)
            lngCnt = lngCnt + 1
        Next

        w.Close()
        AddLog(chkStep4.Text & " " & "終了")

    End Sub

    Private Sub Step4Column()
        Dim t As Scripting.TextStream, w As Scripting.TextStream, wc As Scripting.TextStream
        Dim lngCnt As Long, lngPIdx As Long, lngTIdx As Long, lngProgramCnt As Long, lngTableCnt As Long, lngProgramIdx As Long, lngTableIdx As Long
        Dim dctPrograms As New Scripting.Dictionary, dctTables As New Scripting.Dictionary, dctMatrix As New Scripting.Dictionary
        Dim arrLine As String(), arrCols As String(), arrMatrix As String(,)
        Dim strProgramName As String, strTableName As String, strViewName As String, strCRUD As String
        Dim objCRUD As clsCRUD, objCRUD2 As clsCRUD
        Dim strLine As String, strFolderName As String

        If Not fso.FileExists(strPath & "\querys\CRUDColumns.tsv") Then
            AddLog(lblPhase.Text & " " & "\querys\CRUDColumns.tsv ファイルが見つからないため、CRUD分析を行えません")
            Exit Sub
        End If

        lblPhase.Text = chkStep4.Text : My.Application.DoEvents()
        '        AddLog(lblPhase.Text & " カラムCRUD 生成中...")
        ReDim arrMatrix(0, 0)

        strFolderName = Mid(strPath, 1, InStrRev(strPath, "\"))

        t = fso.OpenTextFile(strPath & "\querys\CRUDColumns.tsv")
        On Error Resume Next
        fso.DeleteFile(strPath & "\querys\CRUDColumnsMatrix.tsv")
        On Error GoTo 0
        w = fso.OpenTextFile(strPath & "\querys\CRUDColumnsMatrix.tsv", Scripting.IOMode.ForWriting, True)

        lngProgramCnt = 0
        lngTableCnt = 0
        Do
            If t.AtEndOfStream Then Exit Do

            arrLine = Split(t.ReadLine, vbTab)

            strProgramName = arrLine(1)
            If strProgramName = "" Then strProgramName = arrLine(0)
            strTableName = arrLine(3)
            strCRUD = arrLine(4)
            If Not DictExists(dctPrograms, strProgramName) Then
                lngProgramCnt = lngProgramCnt + 1
                dctPrograms(strProgramName) = lngProgramCnt
            End If
            If Not DictExists(dctTables, strTableName) Then
                lngTableCnt = lngTableCnt + 1
                dctTables(strTableName) = lngTableCnt
            End If
            If DictExists(dctMatrix, strProgramName & ":" & strTableName) Then
                objCRUD = GetDictObject(dctMatrix, strProgramName & ":" & strTableName)
            Else
                objCRUD = New clsCRUD
                Call DictAdd(dctMatrix, strProgramName & ":" & strTableName, objCRUD)
            End If

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
        Loop

        On Error Resume Next
        'If chkReference.Checked Then
        '    '間接的に参照(VIEW経由)しているテーブルの情報の構築
        '    For Each strViewName In dctPrograms.Keys
        '        Dim strKey As String
        '        For Each strKey In dctTables.Keys
        '            Dim strTableColumn As String() = Split(strKey, ".")
        '            If strTableColumn(0) = Replace(strViewName, ".sql", "") Then    'VIEWと判明
        '                Dim strViewRefTable As String
        '                For Each strViewRefTable In dctMatrix.Keys
        '                    If strViewName = Split(strViewRefTable, ":")(0) Then 'VIEWが参照しているテーブル
        '                        objCRUD = dctMatrix(strViewName & ":" & Split(strViewRefTable, ":")(1))
        '                        For Each strProgramName In dctPrograms.Keys                     'VIEWを参照しているプログラム
        '                            If strProgramName <> strViewName And GetCRUD(dctMatrix, strProgramName & ":" & Replace(strViewName, ".sql", "")) <> "" Then   'VIEWを参照しているプログラム
        '                                If dctMatrix.Exists(strProgramName & ":" & Split(strViewRefTable, ":")(1)) Then
        '                                    objCRUD2 = dctMatrix(strProgramName & ":" & Split(strViewRefTable, ":")(1))
        '                                Else
        '                                    objCRUD2 = New clsCRUD
        '                                    Call dctMatrix.Add(strProgramName & ":" & Split(strViewRefTable, ":")(1), objCRUD2)
        '                                End If
        '                                objCRUD2.RefC = objCRUD.C
        '                                objCRUD2.RefR = objCRUD.R
        '                                objCRUD2.RefU = objCRUD.U
        '                                objCRUD2.RefD = objCRUD.D
        '                            End If
        '                        Next

        '                    End If
        '                Next
        '            End If

        '            '    If GetCRUD(dctMatrix, strKey & ":" & strKey) <> "" Then       'VIEWが参照しているテーブル
        '            '        objCRUD = dctMatrix(strKey & ":" & strKey)

        '            '        For Each strProgramName In dctPrograms.Keys                     'VIEWを参照しているプログラム
        '            '            If strProgramName <> strViewName And GetCRUD(dctMatrix, strProgramName & ":" & Replace(strViewName, ".sql", "")) <> "" Then   'VIEWを参照しているプログラム
        '            '                If dctMatrix.Exists(strProgramName & ":" & strKey) Then
        '            '                    objCRUD2 = dctMatrix(strProgramName & ":" & strKey)
        '            '                Else
        '            '                    objCRUD2 = New clsCRUD
        '            '                    Call dctMatrix.Add(strProgramName & ":" & strKey, objCRUD2)
        '            '                End If
        '            '                objCRUD2.RefC = objCRUD.C
        '            '                objCRUD2.RefR = objCRUD.R
        '            '                objCRUD2.RefU = objCRUD.U
        '            '                objCRUD2.RefD = objCRUD.D
        '            '            End If
        '            '        Next

        '            '    End If
        '        Next
        '    Next
        'End If
        On Error GoTo 0

        dctPrograms = SortDictionary(dctPrograms)
        dctTables = SortDictionary(dctTables)

        strLine = "テーブル名.カラム名" & vbTab & "エンティティ名.属性名" & vbTab
        For Each strProgramName In dctPrograms.Keys
            strLine = strLine & vbTab & strProgramName
        Next
        '数式
        w.WriteLine(strLine)
        strLine = vbTab & vbTab
        lngCnt = 4
        For Each strProgramName In dctPrograms.Keys
            strLine = strLine & vbTab & "=COUNTA(" & ColToRange(lngCnt) & "3:" & ColToRange(lngCnt) & dctTables.Count + 3 & ")"
            lngCnt = lngCnt + 1
        Next
        w.WriteLine(strLine)

        lngCnt = 3
        For Each strTableName In dctTables.Keys
            strLine = strTableName & vbTab
            Dim arrTableName As String() = Split(strTableName, ".")
            If DictExists(dctTableName, arrTableName(0)) Then strLine &= GetDictValue(dctTableName, arrTableName(0))
            strLine &= "."
            If DictExists(dctTableDef, arrTableName(0)) Then
                Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, arrTableName(0))
                If DictExists(objTableDef.dctColumnts, arrTableName(1)) Then
                    Dim objColumnDef As clsColumnDef = GetDictObject(objTableDef.dctColumnts, arrTableName(1))
                    strLine &= objColumnDef.AttributeName
                End If
            End If
            strLine = strLine & vbTab & "=COUNTA(" & ColToRange(4) & lngCnt & ":" & ColToRange(dctPrograms.Count + 4) & lngCnt & ")"
            For Each strProgramName In dctPrograms.Keys
                strLine = strLine & vbTab
                If DictExists(dctMatrix, strProgramName & ":" & strTableName) Then
                    objCRUD = GetDictObject(dctMatrix, strProgramName & ":" & strTableName)
                    strLine = strLine & objCRUD.GetCRUD
                    If objCRUD.GetRefCRUD <> "" Then        '間接参照
                        strLine = strLine & "(" & objCRUD.GetRefCRUD & ")"
                    End If
                End If
            Next
            w.WriteLine(strLine)
            lngCnt = lngCnt + 1
        Next

        w.Close()

    End Sub

    Private Sub CreateFile(ByVal strFolder As String, ByVal strFileName As String, ByVal strDesc As String)
        Dim t As Scripting.TextStream

        If Not fso.FolderExists(strFolder) Then
            fso.CreateFolder(strFolder)
        End If
        t = fso.OpenTextFile(strFolder & "\" & strFileName, 2, True)
        t.Write(strDesc)
        t.Close()
    End Sub


    Private Sub DeleteFolder(ByVal strFolder As String, Optional ByVal bolOnlyFiles As Boolean = False)
        Dim objFolder As Scripting.Folder, objFile As Scripting.File

        On Error Resume Next
        objFolder = fso.GetFolder(strFolder)
        For Each objFile In objFolder.Files
            objFile.Delete()
        Next
        If Not bolOnlyFiles Then objFolder.Delete()

    End Sub



    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub


    Private Sub frmMakeCRUD_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        btnSelectSourceFolder.Focus()
    End Sub

    Private Sub txtLog_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtLog.DoubleClick
        Select Case objSettings.ListDblClickMode
            Case clsSettings.enmListDblClickMode.ExecTextEditor
                cmdRunHidemaru_Click(Nothing, Nothing)
            Case clsSettings.enmListDblClickMode.AnalyzeQuery
                cmdAnalyzeQuery_Click(Nothing, Nothing)
        End Select
    End Sub

    Private Sub CurrentFileAndLineAndWord(ByRef strFileName As String, ByRef lngLineNo As Long, ByRef strWord As String)
        Dim intSPos As Integer
        Dim intEPos As Integer
        Dim m As MatchCollection

        strFileName = ""
        lngLineNo = 0
        strWord = ""
        intSPos = InStrRev(txtLog.Text, vbLf, txtLog.SelectionStart + 1)
        If intSPos < 1 Then
            intSPos = 1
        Else
            intSPos += 1
        End If

        intEPos = InStr(intSPos, txtLog.Text, vbLf)
        If intEPos < 1 Then Exit Sub

        m = Regex.Matches(Mid(txtLog.Text, intSPos, intEPos - intSPos), "\s*([^\s]*)[(]([0-9]+)[)]:.*テーブル[[]([^]]*)[]]")
        If m.Count > 0 Then
            strFileName = m(0).Groups(1).Value
            lngLineNo = CInt(m(0).Groups(2).Value)
            strWord = m(0).Groups(3).Value
        End If
    End Sub

    Private Sub cmdRunHidemaru_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRunHidemaru.Click
        Dim strFileName As String = ""
        Dim intLineNo As Integer = 0
        Dim strWord As String = ""

        CurrentFileAndLineAndWord(strFileName, intLineNo, strWord)
        If strFileName <> "" Then
            On Error Resume Next
            Dim objform As New frmAnalyzeQuery
            txtLog.Focus()
            RunTextEditor(txtSourcePath.Text & "\" & strFileName, intLineNo, strWord)
        End If
    End Sub

    Private Sub cmdAnalyzeQuery_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAnalyzeQuery.Click
        Dim strFileName As String = ""
        Dim intLineNo As Integer = 0
        Dim strWord As String = ""

        CurrentFileAndLineAndWord(strFileName, intLineNo, strWord)
        If strFileName <> "" Then
            On Error Resume Next
            Dim objform As New frmAnalyzeQuery
            txtLog.Focus()
            objform.ShowForm(txtSourcePath.Text, strFileName, intLineNo, strWord, "")
        End If

    End Sub

    Private Sub frmMakeCRUD_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not objSettings.DebugMode Then
            cmdRunHidemaru.Visible = False
            cmdAnalyzeQuery.Visible = False
        End If
        txtSourcePath.Text = regkey.GetValue("SourceFolder")
        txtDestPath.Text = regkey.GetValue("DestFolder")
    End Sub

End Class