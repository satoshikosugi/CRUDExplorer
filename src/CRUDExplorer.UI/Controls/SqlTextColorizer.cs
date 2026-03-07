using System.Text.RegularExpressions;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace CRUDExplorer.UI.Controls;

/// <summary>
/// SQL文のシンタックスハイライトとユーザー指定キーワードハイライトを行う
/// DocumentColorizingTransformer（AvaloniaEdit用）。
/// オリジナルVB.NETの DecoText/DecoReserve/DecoHighLight/DecoSubquery 相当。
/// </summary>
public class SqlTextColorizer : DocumentColorizingTransformer
{
    // SQL予約語パターン（オリジナルのDecoReserve相当）
    private static readonly Regex SqlKeywordRegex = new(
        @"\b(ALL|ALTER|AND|ANY|ARRAY|AS|ASC|AT|BEGIN|BETWEEN|BY|CASE|CHECK|CLUSTERS?|COLAUTH|COLUMNS?|COMPRESS|CONNECT|COUNT|CREATE|CROSS|CURRENT|CURSOR|DECODE|DECIMAL|DECLARE|DEFAULT|DELETE|DESC|DISTINCT|DROP|ELSE|END|EXCEPTION|EXCLUSIVE|EXISTS|FETCH|FOR|FORM|FROM|FULL|GOTO|GRANT|GROUP|HAVING|IDENTIFIED|IF|IN|INDEX|INDEXES|INNER|INSERT|INTERSECT|INTO|IS|JOIN|LEFT|LIKE|LOCK|MAX|MERGE|MIN|MINUS|MODE|NOCOMPRESS|NOT|NOWAIT|NULL|NVL|OF|ON|OPTION|OR|ORDER|OUTER|OVERLAPS|OVER|PARTITION|PERCENTILE_CONT|PRIOR|PROCEDURE|PUBLIC|RANGE|RANK|RECORD|RESOURCE|REVOKE|RIGHT|ROWNUM|ROW_NUMBER|SELECT|SET|SHARE|SIZE|SQL|START|SUBSTR|SUBTYPE|SUM|SYSDATE|TABAUTH|TABLE|THEN|TO|TO_CHAR|TO_DATE|TRUNCATE|TYPE|UNION|UNIQUE|UPDATE|USE|USING|VALUES|VIEW|VIEWS|WHEN|WHERE|WITH|WITHIN)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // サブクエリマーカーパターン（%1%, %2%, etc.）
    private static readonly Regex SubqueryMarkerRegex = new(
        @"%[0-9]+%",
        RegexOptions.Compiled);

    /// <summary>ハイライト1のキーワード</summary>
    public string HighlightText1 { get; set; } = string.Empty;

    /// <summary>ハイライト2のキーワード</summary>
    public string HighlightText2 { get; set; } = string.Empty;

    /// <summary>ハイライト3のキーワード</summary>
    public string HighlightText3 { get; set; } = string.Empty;

    // ハイライトカラー（オリジナルのtxtHighLight*.BackColor相当）
    // HighlightText1: Pink   (255, 192, 192)
    // HighlightText2: Cyan   (128, 255, 255)
    // HighlightText3: Yellow (255, 255, 128)
    private static readonly IBrush Highlight1Brush = new SolidColorBrush(Color.FromRgb(255, 192, 192));
    private static readonly IBrush Highlight2Brush = new SolidColorBrush(Color.FromRgb(128, 255, 255));
    private static readonly IBrush Highlight3Brush = new SolidColorBrush(Color.FromRgb(255, 255, 128));
    private static readonly IBrush SqlKeywordBrush = Brushes.Blue;
    private static readonly IBrush SubqueryBrush = Brushes.Blue;

    protected override void ColorizeLine(DocumentLine line)
    {
        var lineStartOffset = line.Offset;
        var lineText = CurrentContext.Document.GetText(line);

        // 1. SQL予約語ハイライト（青色）
        foreach (Match m in SqlKeywordRegex.Matches(lineText))
        {
            ChangeLinePart(
                lineStartOffset + m.Index,
                lineStartOffset + m.Index + m.Length,
                element => element.TextRunProperties.SetForegroundBrush(SqlKeywordBrush));
        }

        // 2. ユーザーハイライト1（ピンク背景）
        ApplyHighlight(lineText, lineStartOffset, HighlightText1, Highlight1Brush);

        // 3. ユーザーハイライト2（シアン背景）
        ApplyHighlight(lineText, lineStartOffset, HighlightText2, Highlight2Brush);

        // 4. ユーザーハイライト3（イエロー背景）
        ApplyHighlight(lineText, lineStartOffset, HighlightText3, Highlight3Brush);

        // 5. サブクエリマーカー（青色・下線）
        foreach (Match m in SubqueryMarkerRegex.Matches(lineText))
        {
            ChangeLinePart(
                lineStartOffset + m.Index,
                lineStartOffset + m.Index + m.Length,
                element =>
                {
                    element.TextRunProperties.SetForegroundBrush(SubqueryBrush);
                    element.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
                });
        }
    }

    private void ApplyHighlight(string lineText, int lineStartOffset, string pattern, IBrush backgroundBrush)
    {
        if (string.IsNullOrEmpty(pattern)) return;

        try
        {
            foreach (Match m in Regex.Matches(lineText, Regex.Escape(pattern), RegexOptions.IgnoreCase))
            {
                ChangeLinePart(
                    lineStartOffset + m.Index,
                    lineStartOffset + m.Index + m.Length,
                    element => element.TextRunProperties.SetBackgroundBrush(backgroundBrush));
            }
        }
        catch
        {
            // 不正な正規表現パターンは無視
        }
    }
}
