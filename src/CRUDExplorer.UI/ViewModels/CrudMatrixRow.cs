using System.Collections.Generic;

namespace CRUDExplorer.UI.ViewModels;

/// <summary>
/// CRUDマトリクスの1行を表すクラス（DataGrid表示用）
/// </summary>
public class CrudMatrixRow
{
    public string TableName { get; set; } = string.Empty;
    public string LogicalName { get; set; } = string.Empty;
    public string Total { get; set; } = string.Empty;

    /// <summary>
    /// プログラムIDをキーとしたCRUD値。DataGridの動的列バインディングに使用。
    /// </summary>
    public Dictionary<string, string> Values { get; set; } = new();

    /// <summary>
    /// インデクサー — DataGridTextColumnのBinding("[key]")から参照される
    /// </summary>
    public string this[string key] => Values.TryGetValue(key, out var v) ? v : string.Empty;
}
