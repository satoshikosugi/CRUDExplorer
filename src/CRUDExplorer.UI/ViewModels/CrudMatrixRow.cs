using System;
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
    /// プログラムIDをキーとしたCRUD値。DataGrid の Binding("[programId]") からアクセスされる。
    /// </summary>
    public Dictionary<string, string> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 文字列インデクサー。DataGridTextColumn の Binding("[key]") で利用される。
    /// Avalonia の [key] バインディングはブラケット内の文字列（"." 含む）を
    /// パス区切りではなくキー文字として扱うため、ファイル名キーも正しく解決される。
    /// </summary>
    public string this[string key] => Values.TryGetValue(key, out var v) ? v : string.Empty;

    /// <summary>
    /// 後方互換のためCellValues配列も保持（テスト等で利用）。
    /// </summary>
    public string[] CellValues { get; set; } = Array.Empty<string>();
}
