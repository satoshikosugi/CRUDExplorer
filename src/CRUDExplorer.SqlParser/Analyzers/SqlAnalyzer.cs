using Antlr4.Runtime;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.SqlParser.Analyzers;

/// <summary>
/// SQLアナライザー（ANTLR4ベース）
/// VB.NET CommonAnalyze.vbのAnalyzeCRUD()を置き換え
/// </summary>
public class SqlAnalyzer
{
    /// <summary>
    /// SQLクエリを解析してQueryオブジェクトを構築
    /// </summary>
    /// <param name="sqlText">SQL文字列</param>
    /// <param name="fileName">ソースファイル名</param>
    /// <param name="lineNo">行番号</param>
    /// <returns>解析済みQueryオブジェクト</returns>
    public Query AnalyzeSql(string sqlText, string fileName = "", int lineNo = 0)
    {
        try
        {
            // ANTLR4パーサー初期化
            var inputStream = new AntlrInputStream(sqlText);
            var lexer = new Grammar.SqlLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Grammar.SqlParser(tokenStream);

            // エラーハンドリング設定
            parser.RemoveErrorListeners();
            var errorListener = new SqlErrorListener();
            parser.AddErrorListener(errorListener);

            // パース実行
            var sqlStatement = parser.sqlStatement();

            // Queryオブジェクト構築
            var query = new Query
            {
                QueryText = sqlText,
                FileName = fileName,
                LineNo = lineNo
            };

            // ビジターパターンでAST走査
            var visitor = new SqlVisitor(query);
            visitor.Visit(sqlStatement);

            return query;
        }
        catch (Exception ex)
        {
            // パースエラー時はエラー情報を含むQueryオブジェクトを返す
            return new Query
            {
                QueryText = sqlText,
                FileName = fileName,
                LineNo = lineNo,
                QueryKind = $"ERROR: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 複数のSQLクエリを一括解析
    /// </summary>
    public List<Query> AnalyzeMultipleSql(string sqlText, string fileName = "", int startLineNo = 0)
    {
        var queries = new List<Query>();

        // セミコロンでSQL文を分割（簡易実装、将来的には改善）
        var statements = SplitSqlStatements(sqlText);
        var currentLine = startLineNo;

        foreach (var statement in statements)
        {
            if (!string.IsNullOrWhiteSpace(statement))
            {
                var query = AnalyzeSql(statement.Trim(), fileName, currentLine);
                queries.Add(query);

                // 行番号を更新（改行数をカウント）
                currentLine += statement.Count(c => c == '\n');
            }
        }

        return queries;
    }

    /// <summary>
    /// SQL文をセミコロンで分割（文字列リテラル内のセミコロンは無視）
    /// </summary>
    private List<string> SplitSqlStatements(string sqlText)
    {
        var statements = new List<string>();
        var currentStatement = new System.Text.StringBuilder();
        var inString = false;
        var stringChar = '\0';

        for (int i = 0; i < sqlText.Length; i++)
        {
            char c = sqlText[i];

            if (inString)
            {
                currentStatement.Append(c);
                if (c == stringChar)
                {
                    // エスケープされた引用符をチェック
                    if (i + 1 < sqlText.Length && sqlText[i + 1] == stringChar)
                    {
                        currentStatement.Append(sqlText[i + 1]);
                        i++;
                    }
                    else
                    {
                        inString = false;
                    }
                }
            }
            else
            {
                if (c == '\'' || c == '"')
                {
                    inString = true;
                    stringChar = c;
                    currentStatement.Append(c);
                }
                else if (c == ';')
                {
                    statements.Add(currentStatement.ToString());
                    currentStatement.Clear();
                }
                else
                {
                    currentStatement.Append(c);
                }
            }
        }

        // 最後の文を追加
        if (currentStatement.Length > 0)
        {
            statements.Add(currentStatement.ToString());
        }

        return statements;
    }
}

/// <summary>
/// ANTLR4エラーリスナー
/// </summary>
internal class SqlErrorListener : BaseErrorListener
{
    public List<string> Errors { get; } = new();

    public override void SyntaxError(
        TextWriter output,
        IRecognizer recognizer,
        IToken offendingSymbol,
        int line,
        int charPositionInLine,
        string msg,
        RecognitionException e)
    {
        var error = $"Line {line}:{charPositionInLine} {msg}";
        Errors.Add(error);
    }
}
