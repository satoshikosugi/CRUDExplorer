Public Class ViewCollection
    Inherits System.Collections.CollectionBase

    Public Sub add(ByVal Citem As clsView)
        List.Add(Citem) '’Ç‰Á‚·‚é
    End Sub

End Class

Public Class clsView
    Public ViewName As String
    Public SourceFileName As String
    Public LineNo As String
    Public Query As String

    Public Sub New(ByVal strViewName As String, ByVal strSourceFileName As String, ByVal strLineNo As String, ByVal strQuery As String)
        ViewName = strViewName
        SourceFileName = strSourceFileName
        LineNo = strLineNo
        Query = strQuery
    End Sub

End Class
