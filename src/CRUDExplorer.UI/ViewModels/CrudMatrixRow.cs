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
    /// プログラムIDをキーとしたCRUD値（内部用）。
    /// </summary>
    public Dictionary<string, string> Values { get; set; } = new();

    /// <summary>
    /// ヘッダー順に並べた配列。DataGridテキスト列を CellValues[i] でバインドする。
    /// Avalonia は整数インデクサーを確実にサポートするため、文字列インデクサーより安全。
    /// </summary>
    public string[] CellValues { get; set; } = Array.Empty<string>();
}
