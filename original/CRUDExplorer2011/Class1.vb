Class clsCRUD
    Public TableName As String
    Public AltName As String
    Public FuncProcName As String
    Public C As Boolean
    Public R As Boolean
    Public U As Boolean
    Public D As Boolean
    Public RefC As Boolean
    Public RefR As Boolean
    Public RefU As Boolean
    Public RefD As Boolean

    Function GetCRUD() As String
        Dim strRet As String = ""

        If C Then strRet = strRet & "C"
        If R Then strRet = strRet & "R"
        If U Then strRet = strRet & "U"
        If D Then strRet = strRet & "D"

        GetCRUD = strRet
    End Function

    Function GetRefCRUD() As String
        Dim strRet As String = ""

        If RefC Then strRet = strRet & "C"
        If RefR Then strRet = strRet & "R"
        If RefU Then strRet = strRet & "U"
        If RefD Then strRet = strRet & "D"

        GetRefCRUD = strRet
    End Function

End Class

Public Class clsSQLKu
    Public SQLType As String
    Public KuSelect As String
    Public KuInto As String
    Public KuFrom As String
    Public KuGroupBy As String
    Public KuHaving As String
    Public KuOderBy As String
    Public KuSet As String
    Public KuValues As String
    Public KuWhere As String
    Public KuInsertTable As String
    Public KuUpdateTable As String
    Public KuDeleteTable As String
End Class


Public Class ListViewItemComparer
    Implements IComparer
    Private _column As Integer
    Private _order As Integer

    ''' <summary>
    ''' ListViewItemComparerクラスのコンストラクタ
    ''' </summary>
    ''' <param name="col">並び替える列番号</param>
    Public Sub New(ByVal col As Integer, ByVal order As Long)
        _column = col
        _order = order
    End Sub

    'xがyより小さいときはマイナスの数、大きいときはプラスの数、
    '同じときは0を返す
    Public Function Compare(ByVal x As Object, ByVal y As Object) _
            As Integer Implements System.Collections.IComparer.Compare
        'ListViewItemの取得
        Dim itemx As ListViewItem = CType(x, ListViewItem)
        Dim itemy As ListViewItem = CType(y, ListViewItem)

        On Error Resume Next
        'xとyを文字列として比較する
        If _column <> 0 And _column <> 1 Then
            Return String.Compare(itemx.SubItems(_column).Text, itemy.SubItems(_column).Text) * _order
        Else
            Dim strX As String = itemx.SubItems(0).Text & New String(" ", 10 - Len(itemx.SubItems(1).Text)) & itemx.SubItems(1).Text
            Dim strY As String = itemy.SubItems(0).Text & New String(" ", 10 - Len(itemy.SubItems(1).Text)) & itemy.SubItems(1).Text
            Return String.Compare(strX, strY) * _order
        End If
    End Function
End Class

Public Class ListViewItemComparerQueryList
    Implements IComparer
    Private _column As Integer
    Private _order As Integer

    ''' <summary>
    ''' ListViewItemComparerクラスのコンストラクタ
    ''' </summary>
    ''' <param name="col">並び替える列番号</param>
    Public Sub New(ByVal col As Integer, ByVal order As Long)
        _column = col
        _order = order
    End Sub

    'xがyより小さいときはマイナスの数、大きいときはプラスの数、
    '同じときは0を返す
    Public Function Compare(ByVal x As Object, ByVal y As Object) _
            As Integer Implements System.Collections.IComparer.Compare
        'ListViewItemの取得
        Dim itemx As ListViewItem = CType(x, ListViewItem)
        Dim itemy As ListViewItem = CType(y, ListViewItem)

        On Error Resume Next
        'xとyを文字列として比較する
        If _column = 0 OrElse _column = 1 Then
            Dim strX As String = itemx.SubItems(0).Text & New String(" ", 10 - Len(itemx.SubItems(1).Text)) & itemx.SubItems(1).Text
            Dim strY As String = itemy.SubItems(0).Text & New String(" ", 10 - Len(itemy.SubItems(1).Text)) & itemy.SubItems(1).Text
            Return String.Compare(strX, strY) * _order
        Else
            Return String.Compare(itemx.SubItems(_column).Text, itemy.SubItems(_column).Text) * _order
        End If
    End Function
End Class

Public Class ListViewItemComparerOtherList
    Implements IComparer
    Private _column As Integer
    Private _order As Integer

    ''' <summary>
    ''' ListViewItemComparerクラスのコンストラクタ
    ''' </summary>
    ''' <param name="col">並び替える列番号</param>
    Public Sub New(ByVal col As Integer, ByVal order As Long)
        _column = col
        _order = order
    End Sub

    'xがyより小さいときはマイナスの数、大きいときはプラスの数、
    '同じときは0を返す
    Public Function Compare(ByVal x As Object, ByVal y As Object) _
            As Integer Implements System.Collections.IComparer.Compare
        'ListViewItemの取得
        Dim itemx As ListViewItem = CType(x, ListViewItem)
        Dim itemy As ListViewItem = CType(y, ListViewItem)

        On Error Resume Next
        'xとyを文字列として比較する
        If _column <> 2 Then
            Return String.Compare(itemx.SubItems(_column).Text, itemy.SubItems(_column).Text) * _order
        Else
            Dim strX As String = itemx.SubItems(1).Text & New String(" ", 10 - Len(itemx.SubItems(_column).Text)) & itemx.SubItems(_column).Text
            Dim strY As String = itemy.SubItems(1).Text & New String(" ", 10 - Len(itemy.SubItems(_column).Text)) & itemy.SubItems(_column).Text
            Return String.Compare(strX, strY) * _order
        End If
    End Function

End Class

Public Class ListViewItemComparerCommonList
    Implements IComparer
    Private _column As Integer
    Private _order As Integer

    ''' <summary>
    ''' ListViewItemComparerクラスのコンストラクタ
    ''' </summary>
    ''' <param name="col">並び替える列番号</param>
    Public Sub New(ByVal col As Integer, ByVal order As Long)
        _column = col
        _order = order
    End Sub

    'xがyより小さいときはマイナスの数、大きいときはプラスの数、
    '同じときは0を返す
    Public Function Compare(ByVal x As Object, ByVal y As Object) _
            As Integer Implements System.Collections.IComparer.Compare
        'ListViewItemの取得
        Dim itemx As ListViewItem = CType(x, ListViewItem)
        Dim itemy As ListViewItem = CType(y, ListViewItem)

        On Error Resume Next
        'xとyを文字列として比較する
        If Mid(itemx.ListView.Columns(_column).Text, 1, 1) <> "#" Then
            Return String.Compare(itemx.SubItems(_column).Text, itemy.SubItems(_column).Text) * _order
        Else
            If _column > 0 Then
                Dim strX As String = itemx.SubItems(_column - 1).Text & New String(" ", 10 - Len(itemx.SubItems(_column).Text)) & itemx.SubItems(_column).Text
                Dim strY As String = itemy.SubItems(_column - 1).Text & New String(" ", 10 - Len(itemy.SubItems(_column).Text)) & itemy.SubItems(_column).Text
                Return String.Compare(strX, strY) * _order
            Else
                Dim strX As String = New String(" ", 10 - Len(itemx.SubItems(_column).Text)) & itemx.SubItems(_column).Text
                Dim strY As String = New String(" ", 10 - Len(itemy.SubItems(_column).Text)) & itemy.SubItems(_column).Text
                Return String.Compare(strX, strY) * _order
            End If
        End If
    End Function
End Class

