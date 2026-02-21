Public Class MenuContents
    Public objListView As ListView
    Public strMenuKind As String
    Public strSourcePath As String
    Public strFileName As String

    Public Sub New(ByVal objL As ListView, ByVal strMK As String, ByVal strPath As String)
        objListView = objL
        strMenuKind = strMK
        strSourcePath = strPath
    End Sub

    Public Sub New(ByVal objL As ListView, ByVal strMK As String, ByVal strPath As String, ByVal strFile As String)
        objListView = objL
        strMenuKind = strMK
        strSourcePath = strPath
        strFileName = strFile
    End Sub
End Class
