Public Class clsTableDef
    Public dctColumnts As New Scripting.Dictionary
End Class

Public Class clsColumnDef
    Public TableName As String      'テーブル名
    Public ColumnName As String     'カラム名
    Public AttributeName As String  '属性名
    Public SEQ As String            'シーケンス(テーブル内のカラムの順番)
    Public PK As String             '主キー
    Public FK As String             '外部キー   
    Public Required As String       '必須(NOT NULL)
    Public DataType As String       '型
    Public Digits As String         '桁
    Public Accuracy As String       '精度

    'Public Sub New(ByVal pTableName As String, ByVal pColumnName As String, ByVal pAttributeName As String, ByVal pSEQ As String, ByVal pPK As Boolean, ByVal pFK As Boolean, ByVal pRequired As Boolean, ByVal pDataType As String, ByVal pDigits As String, ByVal pAccuracy As String)
    '    TableName = pTableName
    '    ColumnName = pColumnName
    '    AttributeName = pAttributeName
    '    SEQ = pSEQ
    '    PK = pPK
    '    FK = pFK
    '    Required = pRequired
    '    DataType = pDataType
    '    Digits = pDigits
    '    Accuracy = pAccuracy
    'End Sub

End Class
