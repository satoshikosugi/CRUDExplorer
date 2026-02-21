Public Class frmSettings

    Private Sub frmSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Select Case objSettings.TextEditor
            Case "hidemaru"
                rdoEditorHidemaru.Checked = True
            Case "sakura"
                rdoEditorSakura.Checked = True
            Case Else
                rdoEditorNotepad.Checked = True
        End Select
        txtNotepadPath.Text = objSettings.NotepadPath
        txtSakuraPath.Text = objSettings.SakuraPath
        txtHidemaruPath.Text = objSettings.HidemaruPath
        Select Case objSettings.ListDblClickMode
            Case clsSettings.enmListDblClickMode.ExecTextEditor
                rdoListDblClickMode0.Checked = True
            Case clsSettings.enmListDblClickMode.AnalyzeQuery
                rdoListDblClickMode1.Checked = True
            Case clsSettings.enmListDblClickMode.NoAction
                rdoListDblClickMode2.Checked = True
        End Select
        txtProgramIdPattern.Text = objSettings.ProgramIdPattern
        chkDebugMode.Checked = objSettings.DebugMode
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.Close()
    End Sub

    Private Sub cmdSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click
        Dim strTextEditor As String
        If rdoEditorHidemaru.Checked Then
            strTextEditor = "hidemaru"
        ElseIf rdoEditorSakura.Checked Then
            strTextEditor = "sakura"
        Else
            strTextEditor = "notepad"
        End If

        Select Case objSettings.TextEditor
            Case "hidemaru"
                If txtHidemaruPath.Text = "" Then
                    MsgBox("秀丸のパスを指定してください")
                    txtHidemaruPath.Focus()
                    Exit Sub
                End If
            Case "sakura"
                If txtSakuraPath.Text = "" Then
                    MsgBox("サクラエディタのパスを指定してください")
                    txtSakuraPath.Focus()
                    Exit Sub
                End If
            Case Else
                If txtNotepadPath.Text = "" Then
                    MsgBox("メモ帳(Notepad)のパスを指定してください")
                    txtNotepadPath.Focus()
                    Exit Sub
                End If
        End Select

        objSettings.TextEditor = strTextEditor
        objSettings.NotepadPath = txtNotepadPath.Text
        objSettings.SakuraPath = txtSakuraPath.Text
        objSettings.HidemaruPath = txtHidemaruPath.Text
        If rdoListDblClickMode0.Checked Then
            objSettings.ListDblClickMode = clsSettings.enmListDblClickMode.ExecTextEditor
        ElseIf rdoListDblClickMode1.Checked Then
            objSettings.ListDblClickMode = clsSettings.enmListDblClickMode.AnalyzeQuery
        Else
            objSettings.ListDblClickMode = clsSettings.enmListDblClickMode.NoAction
        End If
        objSettings.ProgramIdPattern = txtProgramIdPattern.Text
        objSettings.DebugMode = chkDebugMode.Checked

        objSettings.SaveSettings()
        Me.Close()
    End Sub

    Private Sub btnSelectHidemaruPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectHidemaruPath.Click
        OpenFileDialog1.FileName = txtHidemaruPath.Text
        OpenFileDialog1.Filter = "exe files (*.exe)|*.exe"
        OpenFileDialog1.Title = "秀丸(Hidemaru.exe)のパスを選択"
        On Error Resume Next
        OpenFileDialog1.OpenFile()
        On Error GoTo 0
        If OpenFileDialog1.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub

        txtHidemaruPath.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub btnSelectNotepadPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectNotepadPath.Click
        OpenFileDialog1.FileName = txtNotepadPath.Text
        OpenFileDialog1.Filter = "exe files (*.exe)|*.exe"
        OpenFileDialog1.Title = "メモ帳(notepad.exe)のパスを選択"
        On Error Resume Next
        OpenFileDialog1.OpenFile()
        On Error GoTo 0
        If OpenFileDialog1.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub

        txtNotepadPath.Text = OpenFileDialog1.FileName
    End Sub

    Private Sub btnSelectSakuraPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectSakuraPath.Click
        OpenFileDialog1.FileName = txtSakuraPath.Text
        OpenFileDialog1.Filter = "exe files (*.exe)|*.exe"
        OpenFileDialog1.Title = "サクラエディタ(sakura.exe)のパスを選択"
        On Error Resume Next
        OpenFileDialog1.OpenFile()
        On Error GoTo 0
        If OpenFileDialog1.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub

        txtSakuraPath.Text = OpenFileDialog1.FileName
    End Sub
End Class