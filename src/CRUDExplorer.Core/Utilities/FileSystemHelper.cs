using CRUDExplorer.Core.Models;

namespace CRUDExplorer.Core.Utilities;

/// <summary>
/// ファイルシステム操作ヘルパー（VB.NET CommonModule.vbから移植）
/// </summary>
public static class FileSystemHelper
{
    /// <summary>
    /// TSVファイルを辞書として読み込む（VB.NET ReadDictionary相当）
    /// タブ区切り2カラムのファイルを読み込み、キー:値の辞書を構築
    /// </summary>
    /// <param name="filePath">ファイルパス</param>
    /// <returns>読み込まれた辞書（大文字小文字不問）</returns>
    public static Dictionary<string, string> ReadDictionary(string filePath)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(filePath))
            return dict;

        try
        {
            var content = File.ReadAllText(filePath);
            var lines = content.Replace("\r", "").Split('\n');

            foreach (var line in lines)
            {
                var cols = line.Split('\t');
                if (cols.Length == 2 && !string.IsNullOrEmpty(cols[0]))
                {
                    dict.TryAdd(cols[0].ToUpperInvariant(), cols[1]);
                }
            }
        }
        catch
        {
            // ファイル読み込みエラー時は空の辞書を返す
        }

        return dict;
    }

    /// <summary>
    /// テーブル定義TSVファイルを読み込む（VB.NET ReadTableDef相当）
    /// タブ区切り10カラムのファイルを読み込み、テーブル名:TableDefinitionの辞書を構築
    /// </summary>
    /// <param name="filePath">ファイルパス</param>
    /// <param name="dict">テーブル定義辞書（追記される）</param>
    public static void ReadTableDef(string filePath, Dictionary<string, TableDefinition> dict)
    {
        if (!File.Exists(filePath))
            return;

        try
        {
            var content = File.ReadAllText(filePath);
            var lines = content.Replace("\r", "").Split('\n');

            foreach (var line in lines)
            {
                var cols = line.Split('\t');
                if (cols.Length < 10)
                    continue;

                var colDef = new ColumnDefinition
                {
                    TableName = cols[0].ToUpperInvariant(),
                    ColumnName = cols[1].ToUpperInvariant(),
                    AttributeName = cols[2],
                    Sequence = cols[3],
                    PrimaryKey = cols[4],
                    ForeignKey = cols[5],
                    Required = cols[6],
                    DataType = cols[7],
                    Digits = cols[8],
                    Accuracy = cols[9]
                };

                if (!dict.TryGetValue(colDef.TableName, out var tableDef))
                {
                    tableDef = new TableDefinition();
                    dict[colDef.TableName] = tableDef;
                }

                tableDef.Columns.TryAdd(colDef.ColumnName, colDef);
            }
        }
        catch
        {
            // ファイル読み込みエラー時は何もしない
        }
    }

    /// <summary>
    /// SQL文からコメントを削除する（VB.NET DeleteComment相当）
    /// 単一行コメント（--）と複数行コメント（/* */）を削除
    /// </summary>
    /// <param name="source">SQL文</param>
    /// <param name="keepLineNo">行番号を保持するために改行を残すか</param>
    /// <returns>コメント削除済みのSQL文</returns>
    public static string DeleteComment(string source, bool keepLineNo)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;

        var src = source.Replace("\r", "");
        var result = new System.Text.StringBuilder();
        var pos = 0;

        while (pos < src.Length)
        {
            var hitPos = src.IndexOf("/*", pos, StringComparison.Ordinal);
            if (hitPos >= 0)
            {
                result.Append(src, pos, hitPos - pos);
                var endPos = src.IndexOf("*/", hitPos + 2, StringComparison.Ordinal);
                if (endPos < 0)
                {
                    // コメントの終わりが見つからない場合、残りをそのまま追加
                    result.Append(src, hitPos, src.Length - hitPos);
                    break;
                }
                else
                {
                    if (keepLineNo)
                    {
                        // 改行を保持するために、コメント内の改行数だけ改行を追加
                        var commentText = src.Substring(hitPos, endPos - hitPos + 2);
                        var lineCount = commentText.Count(c => c == '\n');
                        result.Append(new string('\n', lineCount));
                    }
                    pos = endPos + 2;
                }
            }
            else
            {
                result.Append(src, pos, src.Length - pos);
                break;
            }
        }

        // 単一行コメント（--）を削除
        var resultStr = System.Text.RegularExpressions.Regex.Replace(
            result.ToString(), "--.*\n", "\n", System.Text.RegularExpressions.RegexOptions.Multiline);

        return resultStr;
    }

    /// <summary>
    /// Forms Designerの属性情報を削除する（VB.NET DeleteFormsPropertyInfo相当）
    /// </summary>
    /// <param name="source">ソース文字列</param>
    /// <returns>属性情報削除済みの文字列</returns>
    public static string DeleteFormsPropertyInfo(string source)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;

        var result = source.Replace("* レコード・グループ合計", "");

        result = System.Text.RegularExpressions.Regex.Replace(result,
            @"^[ ]+[*\-o][ ][^ ]+[ ]+(SELECT.*)\n", "$1\n",
            System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        result = System.Text.RegularExpressions.Regex.Replace(result,
            @"^[ ]+[*\-o\^][ ]オブジェクト・グループの子オブジェクトを指定するオブジェクト\n", ";\n",
            System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        result = System.Text.RegularExpressions.Regex.Replace(result,
            @"^[ ]+[*\-o\^][ ][^ ]+.*\n", ";\n",
            System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        return result;
    }
}
