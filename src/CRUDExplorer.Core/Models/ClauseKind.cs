namespace CRUDExplorer.Core.Models;

/// <summary>
/// SQL句の種類を表す列挙型
/// </summary>
public enum ClauseKind
{
    Select = 1,
    Where = 2,
    GroupBy = 3,
    OrderBy = 4,
    Having = 5,
    Insert = 6,
    Update = 7,
    SetCondition = 8,
    Delete = 9
}

/// <summary>
/// ClauseKind拡張メソッド
/// </summary>
public static class ClauseKindExtensions
{
    public static string ToClauseName(this ClauseKind kind)
    {
        return kind switch
        {
            ClauseKind.Select => "SELECT",
            ClauseKind.Where => "WHERE",
            ClauseKind.GroupBy => "GROUP BY",
            ClauseKind.OrderBy => "ORDER BY",
            ClauseKind.Having => "HAVING",
            ClauseKind.Insert => "INSERT",
            ClauseKind.Update => "SET(UPDATE)",
            ClauseKind.SetCondition => "SET句内の参照カラム",
            ClauseKind.Delete => "DELETE",
            _ => string.Empty
        };
    }
}
