Imports System.Collections

Public Enum enmKuKind
    KuSelect = 1
    KuWhere = 2
    KuGroupBy = 3
    KuOrderBy = 4
    KuHaving = 5
    KuInsert = 6
    KuUpdate = 7
    KuSetCond = 8
    KuDelete = 9
End Enum

Public Class ColumnCollection
    Inherits System.Collections.CollectionBase

    Public Sub add(ByVal Citem As Column)
        List.Add(Citem) 'í«â¡Ç∑ÇÈ
    End Sub

End Class

Public Class Column
    Public ColumnName As String
    Public Table As String
    Public Alt As String
    Public KuKind As enmKuKind

    Public Sub New(ByVal strColumnName As String, ByVal strTable As String, ByVal strAlt As String, ByVal Kind As enmKuKind)
        ColumnName = strColumnName
        Table = strTable
        Alt = strAlt
        KuKind = Kind
    End Sub

    Public Function KuName() As String
        KuName = ""
        Select Case KuKind
            Case enmKuKind.KuSelect : KuName = "SELECT"
            Case enmKuKind.KuWhere : KuName = "WHERE"
            Case enmKuKind.KuGroupBy : KuName = "GROUP BY"
            Case enmKuKind.KuOrderBy : KuName = "ORDER BY"
            Case enmKuKind.KuHaving : KuName = "HAVIND"
            Case enmKuKind.KuInsert : KuName = "INSERT"
            Case enmKuKind.KuUpdate : KuName = "SET(UPDATE)"
            Case enmKuKind.KuSetCond : KuName = "SETãÂì‡ÇÃéQè∆ÉJÉâÉÄ"
            Case enmKuKind.KuDelete : KuName = "DELETE"
        End Select
    End Function
End Class
