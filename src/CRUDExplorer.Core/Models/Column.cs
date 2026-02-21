namespace CRUDExplorer.Core.Models;

/// <summary>
/// カラム情報を保持するクラス
/// </summary>
public class Column
{
    /// <summary>
    /// カラム名
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// テーブル名
    /// </summary>
    public string Table { get; set; } = string.Empty;

    /// <summary>
    /// 別名（エイリアス）
    /// </summary>
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// 句の種類
    /// </summary>
    public ClauseKind ClauseKind { get; set; }

    public Column(string columnName, string table, string alt, ClauseKind clauseKind)
    {
        ColumnName = columnName;
        Table = table;
        Alt = alt;
        ClauseKind = clauseKind;
    }

    public string GetClauseName()
    {
        return ClauseKind.ToClauseName();
    }
}

/// <summary>
/// カラムのコレクション
/// </summary>
public class ColumnCollection : List<Column>
{
}
