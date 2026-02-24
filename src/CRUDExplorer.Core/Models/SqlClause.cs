namespace CRUDExplorer.Core.Models;

/// <summary>
/// SQL句を保持するクラス
/// </summary>
public class SqlClause
{
    public string SqlType { get; set; } = string.Empty;
    public string Select { get; set; } = string.Empty;
    public string Into { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string GroupBy { get; set; } = string.Empty;
    public string Having { get; set; } = string.Empty;
    public string OrderBy { get; set; } = string.Empty;
    public string Set { get; set; } = string.Empty;
    public string Values { get; set; } = string.Empty;
    public string Where { get; set; } = string.Empty;
    public string InsertTable { get; set; } = string.Empty;
    public string UpdateTable { get; set; } = string.Empty;
    public string DeleteTable { get; set; } = string.Empty;
}
