namespace CRUDExplorer.Core.Models;

/// <summary>
/// VIEW情報
/// </summary>
public class View
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public string ViewName { get; set; } = string.Empty;

    /// <summary>
    /// ソースファイル名
    /// </summary>
    public string SourceFileName { get; set; } = string.Empty;

    /// <summary>
    /// 行番号
    /// </summary>
    public string LineNo { get; set; } = string.Empty;

    /// <summary>
    /// クエリ
    /// </summary>
    public string QueryText { get; set; } = string.Empty;

    public View(string viewName, string sourceFileName, string lineNo, string queryText)
    {
        ViewName = viewName;
        SourceFileName = sourceFileName;
        LineNo = lineNo;
        QueryText = queryText;
    }
}

/// <summary>
/// VIEWのコレクション
/// </summary>
public class ViewCollection : List<View>
{
}
