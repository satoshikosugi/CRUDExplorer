using System.Text.RegularExpressions;

namespace CRUDExplorer.Core.Utilities;

public enum LanguageKind
{
    Sql, Java, CSharp, Python, Go, Php, Ruby, TypeScript, JavaScript, Xml, Unknown
}

public record ExtractedSql(int LineNo, string Sql, string ContextName);

public static class LanguageSqlExtractor
{
    private static readonly Regex SqlKeywordRegex = new(
        @"\b(SELECT|INSERT|UPDATE|DELETE|WITH|TRUNCATE)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex SqlValidationRegex = new(
        @"(?i)(SELECT\s.+\sFROM\s|INSERT\s+INTO\s|UPDATE\s.+\sSET\s|DELETE\s+(FROM\s|WHERE\s|\w))",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static LanguageKind DetectLanguage(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".java" => LanguageKind.Java,
            ".cs" => LanguageKind.CSharp,
            ".py" => LanguageKind.Python,
            ".go" => LanguageKind.Go,
            ".php" => LanguageKind.Php,
            ".rb" => LanguageKind.Ruby,
            ".ts" => LanguageKind.TypeScript,
            ".js" => LanguageKind.JavaScript,
            ".xml" => LanguageKind.Xml,
            ".sql" or ".pls" or ".plsql" => LanguageKind.Sql,
            _ => LanguageKind.Unknown
        };
    }

    public static List<ExtractedSql> Extract(string source, string fileName)
    {
        var lang = DetectLanguage(fileName);
        return lang switch
        {
            LanguageKind.Java => ExtractFromJava(source),
            LanguageKind.CSharp => ExtractFromCSharp(source),
            LanguageKind.Python => ExtractFromPython(source),
            LanguageKind.Go => ExtractFromGo(source),
            LanguageKind.Php => ExtractFromPhp(source),
            LanguageKind.Ruby => ExtractFromRuby(source),
            LanguageKind.TypeScript or LanguageKind.JavaScript => ExtractFromTypeScript(source),
            LanguageKind.Xml => ExtractFromXml(source),
            LanguageKind.Sql => ExtractFromSql(source),
            _ => new List<ExtractedSql>()
        };
    }

    private static bool IsValidSql(string sql)
    {
        return SqlValidationRegex.IsMatch(sql);
    }

    private static string NormalizeSql(string sql)
    {
        sql = Regex.Replace(sql, @"\s+", " ").Trim();
        return sql;
    }

    private static int GetLineNumber(string source, int charIndex)
    {
        if (charIndex <= 0) return 1;
        int count = 1;
        for (int i = 0; i < charIndex && i < source.Length; i++)
            if (source[i] == '\n') count++;
        return count;
    }

    // ---- SQL ----
    private static List<ExtractedSql> ExtractFromSql(string source)
    {
        var results = new List<ExtractedSql>();
        var lines = source.Split('\n');
        var sqlKeywords = new[] { "SELECT", "INSERT", "UPDATE", "DELETE", "WITH", "TRUNCATE" };
        string context = "";
        var procRegex = new Regex(@"\b(PROCEDURE|FUNCTION|CURSOR)\s+(\w+)", RegexOptions.IgnoreCase);

        int i = 0;
        while (i < lines.Length)
        {
            var trimmed = lines[i].Trim();

            var procMatch = procRegex.Match(trimmed);
            if (procMatch.Success)
                context = procMatch.Groups[2].Value;

            bool isSqlLine = sqlKeywords.Any(k =>
                trimmed.StartsWith(k, StringComparison.OrdinalIgnoreCase));

            if (isSqlLine)
            {
                int startLine = i + 1;
                var sb = new System.Text.StringBuilder();
                sb.AppendLine(trimmed);
                i++;
                while (i < lines.Length)
                {
                    var l = lines[i].Trim();
                    if (l == "/" || l == ";")
                    {
                        i++;
                        break;
                    }
                    sb.AppendLine(l);
                    i++;
                }
                var sql = NormalizeSql(sb.ToString());
                if (IsValidSql(sql))
                    results.Add(new ExtractedSql(startLine, sql, context));
                continue;
            }
            i++;
        }
        return results;
    }

    // ---- Java ----
    private static List<ExtractedSql> ExtractFromJava(string source)
    {
        var results = new List<ExtractedSql>();
        var lines = source.Split('\n');
        string context = "";
        var classRegex = new Regex(@"\bclass\s+(\w+)");
        var methodRegex = new Regex(@"\b(?:void|String|List|int|long|boolean|User|Order|Object)\s+(\w+)\s*\(");

        void UpdateContext(string line)
        {
            var cm = classRegex.Match(line);
            if (cm.Success) context = cm.Groups[1].Value;
            var mm = methodRegex.Match(line);
            if (mm.Success) context = (context.Length > 0 ? context + "." : "") + mm.Groups[1].Value;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            UpdateContext(lines[i]);
            var combined = CollectJavaConcatenatedString(lines, i, out int consumed);
            if (consumed > 0 && IsValidSql(combined))
            {
                results.Add(new ExtractedSql(i + 1, NormalizeSql(combined), context));
                i += consumed - 1;
                continue;
            }

            var matches = Regex.Matches(lines[i], @"""((?:[^""\\]|\\.)*)""");
            foreach (Match m in matches)
            {
                var candidate = m.Groups[1].Value.Replace("\\\"", "\"").Replace("\\n", " ").Replace("\\t", " ");
                if (SqlKeywordRegex.IsMatch(candidate) && IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }
        }
        return results;
    }

    private static string CollectJavaConcatenatedString(string[] lines, int startIdx, out int consumed)
    {
        consumed = 0;
        var line = lines[startIdx].Trim();
        if (!Regex.IsMatch(line, @"""(?:SELECT|INSERT|UPDATE|DELETE|WITH|TRUNCATE)", RegexOptions.IgnoreCase))
            return "";

        var sb = new System.Text.StringBuilder();
        int i = startIdx;
        while (i < lines.Length)
        {
            var l = lines[i].Trim();
            var parts = Regex.Matches(l, @"""((?:[^""\\]|\\.)*)""");
            foreach (Match p in parts)
                sb.Append(p.Groups[1].Value.Replace("\\\"", "\"").Replace("\\n", " ").Replace("\\t", " ") + " ");

            consumed++;
            if (!l.TrimEnd().EndsWith("+"))
                break;
            i++;
        }
        return sb.ToString().Trim();
    }

    // ---- C# ----
    private static List<ExtractedSql> ExtractFromCSharp(string source)
    {
        var results = new List<ExtractedSql>();
        var lines = source.Split('\n');
        string context = "";
        var classRegex = new Regex(@"\bclass\s+(\w+)");
        var methodRegex = new Regex(@"\b(?:public|private|protected|internal|async)\s+(?:\w+\s+)+(\w+)\s*\(");

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.Trim();

            var cm = classRegex.Match(trimmed);
            if (cm.Success) context = cm.Groups[1].Value;
            var mm = methodRegex.Match(trimmed);
            if (mm.Success) context = (context.Length > 0 ? context + "." : "") + mm.Groups[1].Value;

            // Verbatim strings @"..."
            var verbatimMatches = Regex.Matches(line, @"@""((?:[^""]|"""")*)""");
            foreach (Match m in verbatimMatches)
            {
                var candidate = m.Groups[1].Value.Replace("\"\"", "\"");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }

            // Regular double-quoted strings
            var strMatches = Regex.Matches(line, @"(?<!@)""((?:[^""\\]|\\.)*)""");
            foreach (Match m in strMatches)
            {
                var candidate = m.Groups[1].Value.Replace("\\\"", "\"").Replace("\\n", " ").Replace("\\t", " ");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }
        }
        return results;
    }

    // ---- Python ----
    private static List<ExtractedSql> ExtractFromPython(string source)
    {
        var results = new List<ExtractedSql>();

        // Triple-quoted strings (double quotes): matches """..."""
        var tripleDoubleMatches = Regex.Matches(source, @"""""""([\s\S]*?)""""""");
        foreach (Match m in tripleDoubleMatches)
        {
            var candidate = m.Groups[1].Value;
            if (IsValidSql(candidate))
            {
                int lineNo = GetLineNumber(source, m.Index);
                string ctx = GetPythonContext(source, m.Index);
                results.Add(new ExtractedSql(lineNo, NormalizeSql(candidate), ctx));
            }
        }

        // Triple-quoted strings (single quotes)
        var tripleSingleMatches = Regex.Matches(source, @"'''([\s\S]*?)'''");
        foreach (Match m in tripleSingleMatches)
        {
            var candidate = m.Groups[1].Value;
            if (IsValidSql(candidate))
            {
                int lineNo = GetLineNumber(source, m.Index);
                string ctx = GetPythonContext(source, m.Index);
                results.Add(new ExtractedSql(lineNo, NormalizeSql(candidate), ctx));
            }
        }

        // Single/double quoted strings per line
        var lines = source.Split('\n');
        string context = "";
        var defRegex = new Regex(@"\bdef\s+(\w+)\s*\(");
        for (int i = 0; i < lines.Length; i++)
        {
            var defMatch = defRegex.Match(lines[i]);
            if (defMatch.Success) context = defMatch.Groups[1].Value;

            var singles = Regex.Matches(lines[i], @"'((?:[^'\\]|\\.)*)'");
            foreach (Match m in singles)
            {
                var candidate = m.Groups[1].Value.Replace("\\'", "'");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }

            var doubles = Regex.Matches(lines[i], @"""((?:[^""\\]|\\.)*)""");
            foreach (Match m in doubles)
            {
                var candidate = m.Groups[1].Value.Replace("\\\"", "\"");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }
        }

        // Deduplicate
        return results.GroupBy(r => r.Sql).Select(g => g.First()).ToList();
    }

    private static string GetPythonContext(string source, int index)
    {
        var before = source.Substring(0, index);
        var defMatches = Regex.Matches(before, @"\bdef\s+(\w+)\s*\(");
        return defMatches.Count > 0 ? defMatches[defMatches.Count - 1].Groups[1].Value : "";
    }

    // ---- Go ----
    private static List<ExtractedSql> ExtractFromGo(string source)
    {
        var results = new List<ExtractedSql>();

        // Backtick raw strings
        var backtickMatches = Regex.Matches(source, @"`([\s\S]*?)`");
        foreach (Match m in backtickMatches)
        {
            var candidate = m.Groups[1].Value;
            if (IsValidSql(candidate))
            {
                int lineNo = GetLineNumber(source, m.Index);
                string ctx = GetGoContext(source, m.Index);
                results.Add(new ExtractedSql(lineNo, NormalizeSql(candidate), ctx));
            }
        }

        // Double-quoted strings
        var lines = source.Split('\n');
        string context = "";
        var funcRegex = new Regex(@"\bfunc\s+(?:\(\w+\s+\*?\w+\)\s+)?(\w+)\s*\(");
        for (int i = 0; i < lines.Length; i++)
        {
            var fm = funcRegex.Match(lines[i]);
            if (fm.Success) context = fm.Groups[1].Value;

            var strMatches = Regex.Matches(lines[i], @"""((?:[^""\\]|\\.)*)""");
            foreach (Match m in strMatches)
            {
                var candidate = m.Groups[1].Value.Replace("\\\"", "\"");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }
        }

        return results.GroupBy(r => r.Sql).Select(g => g.First()).ToList();
    }

    private static string GetGoContext(string source, int index)
    {
        var before = source.Substring(0, index);
        var funcMatches = Regex.Matches(before, @"\bfunc\s+(?:\(\w+\s+\*?\w+\)\s+)?(\w+)\s*\(");
        return funcMatches.Count > 0 ? funcMatches[funcMatches.Count - 1].Groups[1].Value : "";
    }

    // ---- PHP ----
    private static List<ExtractedSql> ExtractFromPhp(string source)
    {
        var results = new List<ExtractedSql>();
        var lines = source.Split('\n');
        string context = "";
        var funcRegex = new Regex(@"\bfunction\s+(\w+)\s*\(");

        for (int i = 0; i < lines.Length; i++)
        {
            var fm = funcRegex.Match(lines[i]);
            if (fm.Success) context = fm.Groups[1].Value;

            var singles = Regex.Matches(lines[i], @"'((?:[^'\\]|\\.)*)'");
            foreach (Match m in singles)
            {
                var candidate = m.Groups[1].Value.Replace("\\'", "'");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }

            var doubles = Regex.Matches(lines[i], @"""((?:[^""\\]|\\.)*)""");
            foreach (Match m in doubles)
            {
                var candidate = m.Groups[1].Value.Replace("\\\"", "\"");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }
        }
        return results;
    }

    // ---- Ruby ----
    private static List<ExtractedSql> ExtractFromRuby(string source)
    {
        var results = new List<ExtractedSql>();

        // Heredoc <<~SQL...SQL or <<-SQL...SQL
        var heredocMatches = Regex.Matches(source, @"<<[~-]?SQL\s*\n([\s\S]*?)\nSQL", RegexOptions.IgnoreCase);
        foreach (Match m in heredocMatches)
        {
            var candidate = m.Groups[1].Value;
            if (IsValidSql(candidate))
            {
                int lineNo = GetLineNumber(source, m.Index);
                string ctx = GetRubyContext(source, m.Index);
                results.Add(new ExtractedSql(lineNo, NormalizeSql(candidate), ctx));
            }
        }

        var lines = source.Split('\n');
        string context = "";
        var defRegex = new Regex(@"\bdef\s+(\w+)");

        for (int i = 0; i < lines.Length; i++)
        {
            var dm = defRegex.Match(lines[i]);
            if (dm.Success) context = dm.Groups[1].Value;

            var singles = Regex.Matches(lines[i], @"'((?:[^'\\]|\\.)*)'");
            foreach (Match m in singles)
            {
                var candidate = m.Groups[1].Value.Replace("\\'", "'");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }

            var doubles = Regex.Matches(lines[i], @"""((?:[^""\\]|\\.)*)""");
            foreach (Match m in doubles)
            {
                var candidate = m.Groups[1].Value.Replace("\\\"", "\"");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }
        }

        return results.GroupBy(r => r.Sql).Select(g => g.First()).ToList();
    }

    private static string GetRubyContext(string source, int index)
    {
        var before = source.Substring(0, index);
        var defMatches = Regex.Matches(before, @"\bdef\s+(\w+)");
        return defMatches.Count > 0 ? defMatches[defMatches.Count - 1].Groups[1].Value : "";
    }

    // ---- TypeScript/JavaScript ----
    private static List<ExtractedSql> ExtractFromTypeScript(string source)
    {
        var results = new List<ExtractedSql>();

        // Template literals
        var templateMatches = Regex.Matches(source, @"`([\s\S]*?)`");
        foreach (Match m in templateMatches)
        {
            var candidate = m.Groups[1].Value;
            if (IsValidSql(candidate))
            {
                int lineNo = GetLineNumber(source, m.Index);
                string ctx = GetTsContext(source, m.Index);
                results.Add(new ExtractedSql(lineNo, NormalizeSql(candidate), ctx));
            }
        }

        // Regular strings
        var lines = source.Split('\n');
        string context = "";
        var funcRegex = new Regex(@"\b(?:async\s+)?function\s+(\w+)\s*\(|(?:const|let|var)\s+(\w+)\s*=\s*(?:async\s+)?\(");

        for (int i = 0; i < lines.Length; i++)
        {
            var fm = funcRegex.Match(lines[i]);
            if (fm.Success)
                context = fm.Groups[1].Success ? fm.Groups[1].Value : fm.Groups[2].Value;

            var doubles = Regex.Matches(lines[i], @"""((?:[^""\\]|\\.)*)""");
            foreach (Match m in doubles)
            {
                var candidate = m.Groups[1].Value.Replace("\\\"", "\"");
                if (IsValidSql(candidate))
                    results.Add(new ExtractedSql(i + 1, NormalizeSql(candidate), context));
            }
        }

        return results.GroupBy(r => r.Sql).Select(g => g.First()).ToList();
    }

    private static string GetTsContext(string source, int index)
    {
        var before = source.Substring(0, index);
        var funcMatches = Regex.Matches(before, @"\b(?:async\s+)?function\s+(\w+)\s*\(|(?:const|let|var)\s+(\w+)\s*=\s*(?:async\s+)?\(");
        if (funcMatches.Count > 0)
        {
            var last = funcMatches[funcMatches.Count - 1];
            return last.Groups[1].Success ? last.Groups[1].Value : last.Groups[2].Value;
        }
        return "";
    }

    // ---- XML (MyBatis) ----
    private static List<ExtractedSql> ExtractFromXml(string source)
    {
        var results = new List<ExtractedSql>();
        var tagPattern = new Regex(
            @"<(select|insert|update|delete)\s+[^>]*id=""(\w+)""[^>]*>([\s\S]*?)</\1>",
            RegexOptions.IgnoreCase);

        foreach (Match m in tagPattern.Matches(source))
        {
            var context = m.Groups[2].Value;
            var candidate = Regex.Replace(m.Groups[3].Value, @"<[^>]+>", " ");
            candidate = NormalizeSql(candidate);
            if (IsValidSql(candidate))
            {
                int lineNo = GetLineNumber(source, m.Index);
                results.Add(new ExtractedSql(lineNo, candidate, context));
            }
        }
        return results;
    }
}
