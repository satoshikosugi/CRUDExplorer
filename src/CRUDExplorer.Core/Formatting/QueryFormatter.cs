using System.Text;

namespace CRUDExplorer.Core.Formatting;

/// <summary>
/// SQL整形（VB.NET clsArrangeQuery.vbから移植）
/// SQLクエリをキーワード単位でインデントして見やすく整形
/// </summary>
public class QueryFormatter
{
    private const int IndentUnit = 4;

    /// <summary>
    /// SQLキーワード定義
    /// </summary>
    private struct Keyword
    {
        public string Character;
        public int IndentLevel;

        public Keyword(string character, int indentLevel)
        {
            Character = character;
            IndentLevel = indentLevel;
        }
    }

    /// <summary>
    /// デフォルトキーワードリスト
    /// </summary>
    private static readonly Keyword[] DefaultKeywords =
    {
        new("SELECT", 1),
        new("INSERT INTO", 1),
        new("INTO", 1),
        new("UPDATE", 1),
        new("DELETE", 1),
        new("FROM", 1),
        new("VALUES", 1),
        new("WHERE", 1),
        new("AND", 1),
        new("OR", 1),
        new("INNER JOIN", 2),
        new("LEFT JOIN", 2),
        new("RIGHT JOIN", 2),
        new("LEFT OUTER JOIN", 2),
        new("RIGHT OUTER JOIN", 2),
        new("ON", 2),
        new("ORDER BY", 1),
        new("GROUP BY", 1),
        new("UNION", 0),
        new("MINUS", 0),
        new("INTERSECT", 0),
        new("SET", 1),
        new(",", 1),
    };

    /// <summary>
    /// SQL文を整形する（VB.NET CArrange相当）
    /// </summary>
    /// <param name="sql">整形対象のSQL文</param>
    /// <returns>整形後のSQL文</returns>
    public string Format(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return string.Empty;

        var result = EditQuery(sql, DefaultKeywords);
        if (string.IsNullOrEmpty(result))
            return string.Empty;

        result = result
            .Replace("\r\n,", ",")
            .Replace("AND\r\n    ", "AND ");

        return CollapseFunctionToSingleLine(result);
    }

    /// <summary>
    /// 関数呼び出しを1行に折り畳む（VB.NET 関数を1行に相当）
    /// 括弧内の改行を空白に変換し、関数呼び出しを1行にまとめる
    /// </summary>
    private static string CollapseFunctionToSingleLine(string sql)
    {
        var sb = new StringBuilder();
        var depth = 0;
        var baseDepth = 0;
        var hadSpace = false;

        if (sql.Length >= 6 && sql.Substring(0, 6).Equals("INSERT", StringComparison.OrdinalIgnoreCase))
        {
            baseDepth = 1;
        }

        for (int i = 0; i < sql.Length; i++)
        {
            var c = sql[i];
            switch (c)
            {
                case '(':
                    depth++;
                    sb.Append(c);
                    hadSpace = false;
                    break;
                case ')':
                    depth--;
                    if (depth < 0) depth = 0;
                    sb.Append(c);
                    hadSpace = false;
                    break;
                case '\r':
                case '\n':
                    if (depth == baseDepth)
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        if (!hadSpace) sb.Append(' ');
                        hadSpace = true;
                    }
                    break;
                case ' ':
                    if (depth == baseDepth)
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        if (!hadSpace) sb.Append(c);
                        hadSpace = true;
                    }
                    break;
                default:
                    sb.Append(c);
                    hadSpace = false;
                    break;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// キーワードを使ってSQL文を整形
    /// </summary>
    private static string EditQuery(string sql, Keyword[] keywords)
    {
        var result = ReplaceSpecialChars(sql);

        foreach (var keyword in keywords)
        {
            long pos = 0;
            while (pos < result.Length)
            {
                var lowerResult = result.ToLowerInvariant();
                var lowerKeyword = keyword.Character.ToLowerInvariant();
                var hitPos = lowerResult.IndexOf(lowerKeyword, (int)pos, StringComparison.Ordinal);

                if (hitPos < 0)
                    break;

                // 前後の文字が区切り文字か確認
                var prevChar = hitPos > 0 ? result[hitPos - 1].ToString() : "";
                var nextCharPos = hitPos + keyword.Character.Length;
                var nextChar = nextCharPos < result.Length ? result[nextCharPos].ToString() : "";

                if (IsSeparator(prevChar) && IsSeparator(nextChar))
                {
                    result = FormatQueryKeyword(result, keyword, hitPos, nextCharPos);
                }

                pos = hitPos + keyword.Character.Length + 1;
            }
        }

        return ReplaceSpecialCharsUndo(result);
    }

    /// <summary>
    /// 特殊文字を変換
    /// </summary>
    private static string ReplaceSpecialChars(string text)
    {
        return text
            .Replace("\r\n", "\r")
            .Replace("\t", new string(' ', IndentUnit));
    }

    /// <summary>
    /// 特殊文字を元に戻す
    /// </summary>
    private static string ReplaceSpecialCharsUndo(string text)
    {
        return text.Replace("\r", "\r\n");
    }

    /// <summary>
    /// 区切り文字判定
    /// </summary>
    private static bool IsSeparator(string c)
    {
        return c == "" || c == " " || c == "\t" || c == "\r";
    }

    /// <summary>
    /// キーワード前後のインデントを設定
    /// </summary>
    private static string FormatQueryKeyword(string text, Keyword keyword, int pos, int nextCharPos)
    {
        // 左側: キーワード前のテキスト（末尾改行追加）
        var left = text.Substring(0, pos).TrimEnd();
        if (!string.IsNullOrEmpty(left) && !left.EndsWith('\r'))
        {
            left += "\r";
        }

        // キーワード部分（インデント + キーワード + 改行）
        var target = text.Substring(pos, keyword.Character.Length).Trim();
        target = AddIndent(keyword.IndentLevel - 1, target);
        target += "\r";

        // 右側: キーワード後のテキスト（インデント追加）
        var right = string.Empty;
        if (text.Length > nextCharPos)
        {
            right = text.Substring(nextCharPos).TrimStart();
            right = StripLeadingNewlines(right);
            right = AddIndent(keyword.IndentLevel, right);
        }

        return left + target + right;
    }

    /// <summary>
    /// 先頭の改行を削除
    /// </summary>
    private static string StripLeadingNewlines(string text)
    {
        while (text.Length > 0 && text[0] == '\r')
        {
            text = text.Substring(1);
        }
        return text;
    }

    /// <summary>
    /// インデントを追加
    /// </summary>
    private static string AddIndent(int level, string text = "")
    {
        return new string(' ', IndentUnit * level) + text;
    }
}
