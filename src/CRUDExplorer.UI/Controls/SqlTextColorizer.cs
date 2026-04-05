using System.Collections.Generic;
using System.Text.RegularExpressions;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.UI.Controls;

/// <summary>
/// SQL文のシンタックスハイライトとユーザー指定キーワードハイライトを行う
/// DocumentColorizingTransformer（AvaloniaEdit用）。
/// オリジナルVB.NETの DecoText/DecoReserve/DecoHighLight/DecoKakko/DecoSelectTable/DecoSelectColumn 相当。
/// </summary>
public class SqlTextColorizer : DocumentColorizingTransformer
{
    /// <summary>
    /// 設定からカラーを適用する
    /// </summary>
    public void ApplySettingsColors(Settings settings)
    {
        if (Color.TryParse(settings.SqlKeywordColor, out var kw))
            KeywordBrush = new SolidColorBrush(kw);
        if (Color.TryParse(settings.SqlStringLiteralColor, out var sl))
            StringLiteralBrush = new SolidColorBrush(sl);
        if (Color.TryParse(settings.SqlCommentColor, out var cm))
            CommentBrush = new SolidColorBrush(cm);
        if (Color.TryParse(settings.SqlForegroundColor, out var fg))
            DefaultForegroundBrush = new SolidColorBrush(fg);
        SubqueryBrush = KeywordBrush;
    }

    /// <summary>デフォルト文字色（設定から反映、colorizer内では未使用だがエディタ側で参照可能）</summary>
    public IBrush? DefaultForegroundBrush { get; set; }
    // SQL予約語パターン（オリジナルのDecoReserve相当）
    private static readonly Regex SqlKeywordRegex = new(
        @"\b(ALL|ALTER|AND|ANY|ARRAY|AS|ASC|AT|BEGIN|BETWEEN|BY|CASE|CHECK|CLUSTERS?|COLAUTH|COLUMNS?|COMPRESS|CONNECT|COUNT|CREATE|CROSS|CURRENT|CURSOR|DECODE|DECIMAL|DECLARE|DEFAULT|DELETE|DESC|DISTINCT|DROP|ELSE|END|EXCEPTION|EXCLUSIVE|EXISTS|FETCH|FOR|FORM|FROM|FULL|GOTO|GRANT|GROUP|HAVING|IDENTIFIED|IF|IN|INDEX|INDEXES|INNER|INSERT|INTERSECT|INTO|IS|JOIN|LEFT|LIKE|LOCK|MAX|MERGE|MIN|MINUS|MODE|NOCOMPRESS|NOT|NOWAIT|NULL|NVL|OF|ON|OPTION|OR|ORDER|OUTER|OVERLAPS|OVER|PARTITION|PERCENTILE_CONT|PRIOR|PROCEDURE|PUBLIC|RANGE|RANK|RECORD|RESOURCE|REVOKE|RIGHT|ROWNUM|ROW_NUMBER|SELECT|SET|SHARE|SIZE|SQL|START|SUBSTR|SUBTYPE|SUM|SYSDATE|TABAUTH|TABLE|THEN|TO|TO_CHAR|TO_DATE|TRUNCATE|TYPE|UNION|UNIQUE|UPDATE|USE|USING|VALUES|VIEW|VIEWS|WHEN|WHERE|WITH|WITHIN)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // サブクエリマーカーパターン（%1%, %2%, etc.）
    private static readonly Regex SubqueryMarkerRegex = new(
        @"%[0-9]+%",
        RegexOptions.Compiled);

    // 文字列リテラルパターン（'...'）
    private static readonly Regex StringLiteralRegex = new(
        @"'[^']*'",
        RegexOptions.Compiled);

    // コメントパターン（-- 行コメント）
    private static readonly Regex CommentRegex = new(
        @"--.*$",
        RegexOptions.Compiled);

    /// <summary>ハイライト1のキーワード</summary>
    public string HighlightText1 { get; set; } = string.Empty;

    /// <summary>ハイライト2のキーワード</summary>
    public string HighlightText2 { get; set; } = string.Empty;

    /// <summary>ハイライト3のキーワード</summary>
    public string HighlightText3 { get; set; } = string.Empty;

    /// <summary>チェック済みテーブル名リスト（色インデックス付き）</summary>
    public List<CheckedTableEntry> CheckedTables { get; set; } = new();

    /// <summary>チェック済みカラム名リスト</summary>
    public List<string> CheckedColumns { get; set; } = new();

    /// <summary>括弧対応ハイライトの位置ペア（開括弧offset, 閉括弧offset）</summary>
    public int BracketOpenOffset { get; set; } = -1;
    public int BracketCloseOffset { get; set; } = -1;

    // ハイライトカラー（オリジナルのtxtHighLight*.BackColor相当）
    private static readonly IBrush Highlight1Brush = new SolidColorBrush(Color.FromRgb(255, 192, 192));
    private static readonly IBrush Highlight2Brush = new SolidColorBrush(Color.FromRgb(128, 255, 255));
    private static readonly IBrush Highlight3Brush = new SolidColorBrush(Color.FromRgb(255, 255, 128));
    private static readonly IBrush BracketBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255)); // シアン
    private static readonly IBrush ColumnFgBrush = Brushes.White;
    private static readonly IBrush ColumnBgBrush = Brushes.Black;

    /// <summary>SQL予約語の色（設定から反映）</summary>
    public IBrush KeywordBrush { get; set; } = Brushes.Blue;

    /// <summary>文字列リテラルの色（設定から反映）</summary>
    public IBrush StringLiteralBrush { get; set; } = Brushes.Red;

    /// <summary>サブクエリマーカーの色</summary>
    public IBrush SubqueryBrush { get; set; } = Brushes.Blue;

    /// <summary>コメントの色（設定から反映）</summary>
    public IBrush CommentBrush { get; set; } = Brushes.Green;

    // テーブル名20色（オリジナルのGetColor相当）
    private static readonly IBrush[] TableColorBrushes = new IBrush[]
    {
        Brushes.Red,                                          // 0
        Brushes.Green,                                        // 1
        Brushes.Blue,                                         // 2
        Brushes.Brown,                                        // 3
        Brushes.Magenta,                                      // 4
        new SolidColorBrush(Color.FromRgb(138, 43, 226)),     // 5 BlueViolet
        new SolidColorBrush(Color.FromRgb(95, 158, 160)),     // 6 CadetBlue
        new SolidColorBrush(Color.FromRgb(255, 140, 0)),      // 7 DarkOrange
        new SolidColorBrush(Color.FromRgb(0, 206, 209)),      // 8 DarkTurquoise
        new SolidColorBrush(Color.FromRgb(72, 61, 139)),      // 9 DarkSlateBlue
        new SolidColorBrush(Color.FromRgb(107, 142, 35)),     // 10 OliveDrab
        new SolidColorBrush(Color.FromRgb(0, 139, 139)),      // 11 DarkCyan
        new SolidColorBrush(Color.FromRgb(75, 0, 130)),       // 12 Indigo
        new SolidColorBrush(Color.FromRgb(139, 0, 139)),      // 13 DarkMagenta
        new SolidColorBrush(Color.FromRgb(255, 69, 0)),       // 14 OrangeRed
        new SolidColorBrush(Color.FromRgb(105, 105, 105)),    // 15 DimGray
        new SolidColorBrush(Color.FromRgb(210, 105, 30)),     // 16 Chocolate
        new SolidColorBrush(Color.FromRgb(65, 105, 225)),     // 17 RoyalBlue
        new SolidColorBrush(Color.FromRgb(255, 20, 147)),     // 18 DeepPink
        new SolidColorBrush(Color.FromRgb(0, 255, 127)),      // 19 SpringGreen
    };

    protected override void ColorizeLine(DocumentLine line)
    {
        var lineStartOffset = line.Offset;
        var lineText = CurrentContext.Document.GetText(line);

        // 1. SQL予約語ハイライト
        foreach (Match m in SqlKeywordRegex.Matches(lineText))
        {
            ChangeLinePart(
                lineStartOffset + m.Index,
                lineStartOffset + m.Index + m.Length,
                element => element.TextRunProperties.SetForegroundBrush(KeywordBrush));
        }

        // 1.5. 文字列リテラルハイライト
        foreach (Match m in StringLiteralRegex.Matches(lineText))
        {
            ChangeLinePart(
                lineStartOffset + m.Index,
                lineStartOffset + m.Index + m.Length,
                element => element.TextRunProperties.SetForegroundBrush(StringLiteralBrush));
        }

        // 1.6. コメントハイライト（-- 行コメント）
        foreach (Match m in CommentRegex.Matches(lineText))
        {
            ChangeLinePart(
                lineStartOffset + m.Index,
                lineStartOffset + m.Index + m.Length,
                element => element.TextRunProperties.SetForegroundBrush(CommentBrush));
        }

        // 2. チェック済みテーブル名ハイライト（20色ローテ＋太字）
        foreach (var entry in CheckedTables)
        {
            var brush = TableColorBrushes[entry.ColorIndex % 20];
            ApplyTableHighlight(lineText, lineStartOffset, entry.TableName, brush);
            if (!string.IsNullOrEmpty(entry.AltName))
                ApplyTableHighlight(lineText, lineStartOffset, entry.AltName, brush);
        }

        // 3. ユーザーハイライト1（ピンク背景）
        ApplyHighlight(lineText, lineStartOffset, HighlightText1, Highlight1Brush);

        // 4. ユーザーハイライト2（シアン背景）
        ApplyHighlight(lineText, lineStartOffset, HighlightText2, Highlight2Brush);

        // 5. ユーザーハイライト3（イエロー背景）
        ApplyHighlight(lineText, lineStartOffset, HighlightText3, Highlight3Brush);

        // 6. チェック済みカラム名ハイライト（白文字・黒背景）
        foreach (var colName in CheckedColumns)
        {
            if (string.IsNullOrEmpty(colName)) continue;
            try
            {
                foreach (Match m in Regex.Matches(lineText, @"\b" + Regex.Escape(colName) + @"\b", RegexOptions.IgnoreCase))
                {
                    ChangeLinePart(
                        lineStartOffset + m.Index,
                        lineStartOffset + m.Index + m.Length,
                        element =>
                        {
                            element.TextRunProperties.SetForegroundBrush(ColumnFgBrush);
                            element.TextRunProperties.SetBackgroundBrush(ColumnBgBrush);
                        });
                }
            }
            catch { }
        }

        // 7. サブクエリマーカー（青色・下線）
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

        // 8. 括弧対応ハイライト（シアン背景）
        if (BracketOpenOffset >= 0 && BracketOpenOffset >= lineStartOffset && BracketOpenOffset < lineStartOffset + line.Length)
        {
            ChangeLinePart(BracketOpenOffset, BracketOpenOffset + 1,
                element => element.TextRunProperties.SetBackgroundBrush(BracketBrush));
        }
        if (BracketCloseOffset >= 0 && BracketCloseOffset >= lineStartOffset && BracketCloseOffset < lineStartOffset + line.Length)
        {
            ChangeLinePart(BracketCloseOffset, BracketCloseOffset + 1,
                element => element.TextRunProperties.SetBackgroundBrush(BracketBrush));
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
        catch { }
    }

    private void ApplyTableHighlight(string lineText, int lineStartOffset, string tableName, IBrush foregroundBrush)
    {
        if (string.IsNullOrEmpty(tableName)) return;

        try
        {
            foreach (Match m in Regex.Matches(lineText, @"\b" + Regex.Escape(tableName) + @"\b", RegexOptions.IgnoreCase))
            {
                var fgBrush = foregroundBrush; // capture for closure
                ChangeLinePart(
                    lineStartOffset + m.Index,
                    lineStartOffset + m.Index + m.Length,
                    element =>
                    {
                        element.TextRunProperties.SetForegroundBrush(fgBrush);
                    });
            }
        }
        catch { }
    }

    /// <summary>
    /// カーソル位置から対応する括弧のオフセットを検出する
    /// </summary>
    public static (int openOffset, int closeOffset) FindMatchingBrackets(string text, int cursorOffset)
    {
        if (string.IsNullOrEmpty(text) || cursorOffset < 0 || cursorOffset >= text.Length)
            return (-1, -1);

        char ch = text[cursorOffset];

        if (ch == '(')
        {
            // 前方スキャン → 対応する閉括弧を探す
            int depth = 0;
            for (int i = cursorOffset; i < text.Length; i++)
            {
                if (text[i] == '(') depth++;
                else if (text[i] == ')')
                {
                    depth--;
                    if (depth == 0) return (cursorOffset, i);
                }
            }
        }
        else if (ch == ')')
        {
            // 後方スキャン → 対応する開括弧を探す
            int depth = 0;
            for (int i = cursorOffset; i >= 0; i--)
            {
                if (text[i] == ')') depth++;
                else if (text[i] == '(')
                {
                    depth--;
                    if (depth == 0) return (i, cursorOffset);
                }
            }
        }

        return (-1, -1);
    }
}

/// <summary>
/// チェック済みテーブルエントリ（色インデックス付き）
/// </summary>
public class CheckedTableEntry
{
    public string TableName { get; set; } = string.Empty;
    public string AltName { get; set; } = string.Empty;
    public int ColorIndex { get; set; }
}
