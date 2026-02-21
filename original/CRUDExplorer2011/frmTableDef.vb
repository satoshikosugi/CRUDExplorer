Public Class frmTableDef
    Private bolFlag As Boolean = True
    Private strSourcePath As String = ""
    Private objRelateControl As RichTextBox

    Public Sub ShowForm(ByVal strPath As String, Optional ByVal strTableName As String = "", Optional ByVal strColumnName As String = "", Optional ByRef objRelate As RichTextBox = Nothing)
        Dim strKey As String

        If dctTableDef.Count = 0 Then MsgBox("テーブル定義を参照するには、テーブル定義辞書ファイルを作成しておく必要があります。") : Exit Sub

        strSourcePath = strPath
        objRelateControl = objRelate
        If objRelate Is Nothing Then pnlRelate.Visible = False
        If bolFlag Then
            lstTable.Visible = False
            lstTable.Items.Clear()
            For Each strKey In dctTableDef.Keys
                Dim objItem As ListViewItem
                objItem = lstTable.Items.Add(strKey)
                objItem.SubItems.Add(GetDictValue(dctTableName, strKey))
            Next
            lstTable.Visible = True
        End If

        Me.Show()
        ShowTableDef(strTableName, strColumnName)

        bolFlag = False
    End Sub

    Private Sub ShowTableDef(ByVal strTableName As String, Optional ByVal strColumnName As String = "")
        Dim intCnt As Integer

        For intCnt = 0 To lstTable.Items.Count - 1
            If lstTable.Items(intCnt).SubItems(0).Text = strTableName Then
                lstTable.Items(intCnt).Selected = True
                Me.Text = lstTable.Items(intCnt).SubItems(1).Text & " (" & strTableName & ") ～ テーブル定義"
                lstTable.EnsureVisible(intCnt)
                lstTable.Focus()
                Exit For
            End If
        Next

        lstTableDef.Visible = False
        lstTableDef.Items.Clear()

        If DictExists(dctTableDef, strTableName) Then
            Dim strKey As String
            Dim objTableDef As clsTableDef = GetDictObject(dctTableDef, strTableName)
            intCnt = 0
            For Each strKey In objTableDef.dctColumnts.Keys
                Dim objColumnDef As clsColumnDef = GetDictObject(objTableDef.dctColumnts, strKey)
                With objColumnDef
                    Dim objItem As ListViewItem = lstTableDef.Items.Add(.SEQ)
                    Dim fnt As New Font(objItem.Font.FontFamily, objItem.Font.Size, FontStyle.Bold)
                    Dim objSubItem As ListViewItem.ListViewSubItem
                    objSubItem = objItem.SubItems.Add(.ColumnName)
                    If .PK = "Yes" Then objItem.Font = fnt
                    objItem.SubItems.Add(.AttributeName)
                    objSubItem = objItem.SubItems.Add(.PK)
                    objSubItem = objItem.SubItems.Add(.FK)
                    objSubItem = objItem.SubItems.Add(.Required)
                    objItem.SubItems.Add(.DataType)
                    objItem.SubItems.Add(.Digits)
                    objItem.SubItems.Add(.Accuracy)
                    If strColumnName = objColumnDef.ColumnName Then
                        objItem.BackColor = Color.Yellow
                        lstTableDef.EnsureVisible(intCnt)
                    End If
                End With
                intCnt += 1
            Next
        End If
        If lstTableDef.Items.Count > 0 Then
            lstTableDef.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        End If
        lstTableDef.Visible = True

    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub

    Private Sub frmTableDef_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objSettings.LoadFormSize(Me)
        InitCommonContextMenu(lstTable, strSourcePath)
        InitCommonContextMenu(lstTableDef, strSourcePath)
    End Sub

    Private Sub このテーブルにアクセスしている処理ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmTableAccess.Click
        Dim objForm As New frmCRUDSearch
        objForm.ShowForm(strSourcePath, lstTable.SelectedItems(0).SubItems(0).Text, "")
    End Sub

    Private Sub このカラムにアクセスしている処理ToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsmColumnAccess.Click
        Dim objForm As New frmCRUDSearch
        objForm.ShowForm(strSourcePath, lstTable.SelectedItems(0).SubItems(0).Text, lstTableDef.SelectedItems(0).SubItems(1).Text)
    End Sub

    Private Sub テーブルにToolStripMenuItem_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles テーブルにToolStripMenuItem.Paint
        tsmTableAccess.Enabled = False
        tsmColumnAccess.Enabled = False
        If strSourcePath <> "" Then
            If lstTable.SelectedItems.Count > 0 Then tsmTableAccess.Enabled = True
            If lstTableDef.SelectedItems.Count <> 0 Then tsmColumnAccess.Enabled = True
        End If
    End Sub

    Private Sub lstTable_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstTable.SelectedIndexChanged
        If Me.bolFlag Then Exit Sub
        If lstTable.SelectedItems.Count = 0 Then Exit Sub

        lstTableDef.Visible = False
        Me.bolFlag = True

        ShowTableDef(lstTable.SelectedItems(0).SubItems(0).Text)

        Me.bolFlag = False
        lstTableDef.Visible = True
    End Sub

    Private Sub frmTableDef_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        objSettings.SaveFormSize(Me)
    End Sub

    Private Sub lstTableDef_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstTableDef.DoubleClick
        btnInsTableColumn_Click(Nothing, Nothing)
    End Sub

    Private Sub btnInsTableColumn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsTableColumn.Click
        If Not objRelateControl Is Nothing And lstTable.SelectedItems.Count > 0 And lstTableDef.SelectedItems.Count > 0 Then
            objRelateControl.SelectedText = lstTable.SelectedItems(0).SubItems(0).Text & "." & lstTableDef.SelectedItems(0).SubItems(1).Text
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub btnInsTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsTable.Click
        If Not objRelateControl Is Nothing And lstTable.SelectedItems.Count > 0 Then
            objRelateControl.SelectedText = lstTable.SelectedItems(0).SubItems(0).Text
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub bntInsEnter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bntInsEnter.Click
        If Not objRelateControl Is Nothing Then
            objRelateControl.SelectedText = vbCrLf
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub btnInsConma_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsConma.Click
        If Not objRelateControl Is Nothing Then
            objRelateControl.SelectedText = "," & vbCrLf
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub btnInsBeginKakko_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsBeginKakko.Click
        If Not objRelateControl Is Nothing Then
            objRelateControl.SelectedText = " ( "
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub bntInsEndKakko_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bntInsEndKakko.Click
        If Not objRelateControl Is Nothing Then
            objRelateControl.SelectedText = " ) "
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub btnInsAnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsAnd.Click
        If Not objRelateControl Is Nothing Then
            objRelateControl.SelectedText = vbCrLf & "AND "
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub btnInsOr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsOr.Click
        If Not objRelateControl Is Nothing Then
            objRelateControl.SelectedText = vbCrLf & "OR  "
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub btnInsEQ_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInsEQ.Click
        If Not objRelateControl Is Nothing Then
            objRelateControl.SelectedText = " = "
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub cmbInsText_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbInsText.SelectedIndexChanged
        If Not objRelateControl Is Nothing Then
            objRelateControl.SelectedText = cmbInsText.Text
            cmbInsText.Text = ""
            objRelateControl.Focus()
        End If
    End Sub

    Private Sub txtFilter_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtFilter.KeyPress
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Return) Then
            btnApplyFilter_Click(Nothing, Nothing)
            e.Handled = False
        End If
    End Sub

    Private Sub btnApplyFilter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApplyFilter.Click
        Dim strKey As String
        lstTable.Visible = False
        lstTable.Items.Clear()
        For Each strKey In dctTableDef.Keys
            If txtFilter.Text = "" OrElse RegMatch(strKey, txtFilter.Text, System.Text.RegularExpressions.RegexOptions.IgnoreCase) Then
                Dim objItem As ListViewItem = lstTable.Items.Add(strKey)
                objItem.SubItems.Add(GetDictValue(dctTableName, strKey))
            End If
        Next
        lstTable.Visible = True
    End Sub

    Private Sub btnClearFilter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearFilter.Click
        txtFilter.Clear()
        btnApplyFilter_Click(Nothing, Nothing)
    End Sub

 
End Class