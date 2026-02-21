namespace CRUDExplorer.Core.Models;

/// <summary>
/// テーブル定義情報
/// </summary>
public class TableDefinition
{
    /// <summary>
    /// カラム定義辞書（キー: カラム名）
    /// </summary>
    public Dictionary<string, ColumnDefinition> Columns { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// カラム定義情報
/// </summary>
public class ColumnDefinition
{
    /// <summary>
    /// テーブル名
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// カラム名
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// 属性名（論理名）
    /// </summary>
    public string AttributeName { get; set; } = string.Empty;

    /// <summary>
    /// シーケンス（テーブル内のカラムの順序）
    /// </summary>
    public string Sequence { get; set; } = string.Empty;

    /// <summary>
    /// 主キー
    /// </summary>
    public string PrimaryKey { get; set; } = string.Empty;

    /// <summary>
    /// 外部キー
    /// </summary>
    public string ForeignKey { get; set; } = string.Empty;

    /// <summary>
    /// 必須（NOT NULL）
    /// </summary>
    public string Required { get; set; } = string.Empty;

    /// <summary>
    /// データ型
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// 桁数
    /// </summary>
    public string Digits { get; set; } = string.Empty;

    /// <summary>
    /// 精度
    /// </summary>
    public string Accuracy { get; set; } = string.Empty;
}
