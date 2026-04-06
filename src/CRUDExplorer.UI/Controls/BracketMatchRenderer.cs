using System;
using Avalonia;
using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace CRUDExplorer.UI.Controls;

/// <summary>
/// SQLエディタ上でカーソル位置の括弧に対応するもう一方の括弧をハイライトする。
/// オリジナル VB.NET の DecoKakko / TextDeco 相当。
/// </summary>
public class BracketMatchRenderer : IBackgroundRenderer
{
    private static readonly IBrush MatchBrush = new SolidColorBrush(Color.FromRgb(0, 255, 255)); // Cyan
    private static readonly IPen MatchPen = new Pen(Brushes.DarkCyan, 1);

    private int _matchOffset = -1;
    private int _caretBracketOffset = -1;

    public KnownLayer Layer => KnownLayer.Selection;

    /// <summary>
    /// カーソル位置に基づいて対応する括弧のオフセットを更新する。
    /// </summary>
    public void UpdateBracketMatch(TextDocument? document, int caretOffset)
    {
        _matchOffset = -1;
        _caretBracketOffset = -1;

        if (document == null || caretOffset < 0 || caretOffset >= document.TextLength)
            return;

        var text = document.Text;
        var ch = text[caretOffset];

        if (ch == '(' || ch == ')')
        {
            _caretBracketOffset = caretOffset;
            _matchOffset = FindMatchingBracket(text, caretOffset, ch);
        }
    }

    /// <summary>
    /// ネスト対応の括弧マッチング。depth カウンタでペアを検出する。
    /// </summary>
    private static int FindMatchingBracket(string text, int position, char bracket)
    {
        int direction = bracket == '(' ? 1 : -1;
        char open = '(';
        char close = ')';
        int depth = 0;

        for (int i = position; i >= 0 && i < text.Length; i += direction)
        {
            var c = text[i];
            if (c == open) depth++;
            else if (c == close) depth--;

            if (depth == 0)
                return i;
        }

        return -1; // マッチなし
    }

    public void Draw(TextView textView, DrawingContext drawingContext)
    {
        if (_matchOffset < 0 && _caretBracketOffset < 0)
            return;

        var visualLines = textView.VisualLines;
        if (visualLines.Count == 0) return;

        DrawBracketHighlight(textView, drawingContext, _matchOffset);
        DrawBracketHighlight(textView, drawingContext, _caretBracketOffset);
    }

    private static void DrawBracketHighlight(TextView textView, DrawingContext drawingContext, int offset)
    {
        if (offset < 0) return;

        var segment = new TextSegment { StartOffset = offset, Length = 1 };
        foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, segment))
        {
            drawingContext.DrawRectangle(MatchBrush, MatchPen,
                new Rect(rect.X, rect.Y, rect.Width, rect.Height));
        }
    }
}
