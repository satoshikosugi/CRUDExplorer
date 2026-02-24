using System.Text.RegularExpressions;
using CRUDExplorer.Core.Formatting;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.Core.Models;

/// <summary>
/// SQLクエリ情報を保持するクラス（VB.NET clsQuery.vbから移植）
/// </summary>
public class Query
{
    public string QueryKind { get; set; } = string.Empty;
    public string QueryText { get; set; } = string.Empty;
    public string SubQueryIndex { get; set; } = "0";
    public string AltName { get; set; } = string.Empty;

    // CRUD用テーブル辞書（テーブル名 → テーブル名\tエイリアス）
    public Dictionary<string, string> TableC { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> TableR { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> TableU { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> TableD { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    // CRUD用カラム辞書
    public Dictionary<string, object> ColumnC { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, object> ColumnR { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, object> ColumnU { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, object> ColumnD { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    // 句ごとのカラムコレクション
    public ColumnCollection ColumnSelect { get; set; } = new();
    public ColumnCollection ColumnWhere { get; set; } = new();
    public ColumnCollection ColumnOrderBy { get; set; } = new();
    public ColumnCollection ColumnGroupBy { get; set; } = new();
    public ColumnCollection ColumnHaving { get; set; } = new();
    public ColumnCollection ColumnUpdate { get; set; } = new();
    public ColumnCollection ColumnSetCond { get; set; } = new();
    public ColumnCollection ColumnInsert { get; set; } = new();
    public ColumnCollection ColumnDelete { get; set; } = new();

    // WITH句（CTE）
    public Dictionary<string, Query> Withs { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    // VALUES、SET、SELECT句の値（カンマで分割して管理）
    public Dictionary<string, string> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> SetValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> Selects { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    // サブクエリ
    public Dictionary<string, Query> SubQueries { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    // 親クエリ
    public Query? Parent { get; set; }

    // ソースファイル情報
    public string FileName { get; set; } = string.Empty;
    public int LineNo { get; set; }

    /// <summary>
    /// 全テーブルを取得
    /// </summary>
    public Dictionary<string, string> GetAllTables(bool expand = false)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "TableC", expand);
        AddAllTablesInternal(this, result, "TableR", expand);
        AddAllTablesInternal(this, result, "TableU", expand);
        AddAllTablesInternal(this, result, "TableD", expand);
        return result;
    }

    public Dictionary<string, string> GetAllTableC()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "TableC", true);
        return result;
    }

    public Dictionary<string, string> GetAllTableR()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "TableR", true);
        return result;
    }

    public Dictionary<string, string> GetAllTableU()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "TableU", true);
        return result;
    }

    public Dictionary<string, string> GetAllTableD()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "TableD", true);
        return result;
    }

    /// <summary>
    /// 全カラムを取得
    /// </summary>
    public Dictionary<string, object> GetAllColumns(bool expand = false)
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "ColumnC", expand);
        AddAllTablesInternal(this, result, "ColumnR", expand);
        AddAllTablesInternal(this, result, "ColumnU", expand);
        AddAllTablesInternal(this, result, "ColumnD", expand);
        return result;
    }

    /// <summary>
    /// 全カラムをColumnCollectionとして取得
    /// </summary>
    public ColumnCollection GetAllColumns2(bool expand = false)
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "Select", expand);
        AddAllColumnsInternal(this, result, "Where", expand);
        AddAllColumnsInternal(this, result, "GroupBy", expand);
        AddAllColumnsInternal(this, result, "OrderBy", expand);
        AddAllColumnsInternal(this, result, "Having", expand);
        AddAllColumnsInternal(this, result, "Insert", expand);
        AddAllColumnsInternal(this, result, "Update", expand);
        AddAllColumnsInternal(this, result, "SetCond", expand);
        AddAllColumnsInternal(this, result, "Delete", expand);
        return result;
    }

    public Dictionary<string, object> GetAllColumnC()
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "ColumnC", true);
        return result;
    }

    public Dictionary<string, object> GetAllColumnR()
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "ColumnR", true);
        return result;
    }

    public Dictionary<string, object> GetAllColumnU()
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "ColumnU", true);
        return result;
    }

    public Dictionary<string, object> GetAllColumnD()
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        AddAllTablesInternal(this, result, "ColumnD", true);
        return result;
    }

    public ColumnCollection GetAllColumnSelect()
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "Select", true);
        return result;
    }

    public ColumnCollection GetAllColumnWhere()
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "Where", true);
        return result;
    }

    public ColumnCollection GetAllColumnGroupBy()
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "GroupBy", true);
        return result;
    }

    public ColumnCollection GetAllColumnOrderBy()
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "OrderBy", true);
        return result;
    }

    public ColumnCollection GetAllColumnHaving()
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "Having", true);
        return result;
    }

    public ColumnCollection GetAllColumnInsert()
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "Insert", true);
        return result;
    }

    public ColumnCollection GetAllColumnUpdate()
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "Update", true);
        return result;
    }

    public ColumnCollection GetAllColumnSetCond()
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "SetCond", true);
        return result;
    }

    public ColumnCollection GetAllColumnDelete()
    {
        var result = new ColumnCollection();
        AddAllColumnsInternal(this, result, "Delete", true);
        return result;
    }

    /// <summary>
    /// 全サブクエリを取得
    /// </summary>
    public Dictionary<string, Query> GetAllSubQueries()
    {
        var result = new Dictionary<string, Query>(StringComparer.OrdinalIgnoreCase);
        GetAllSubQueriesCore(result, this);
        return result;
    }

    private void GetAllSubQueriesCore(Dictionary<string, Query> allSubQueries, Query query)
    {
        foreach (var key in query.SubQueries.Keys)
        {
            allSubQueries[$"K{allSubQueries.Count + 1}"] = query.SubQueries[key];
        }
        foreach (var key in query.SubQueries.Keys)
        {
            GetAllSubQueriesCore(allSubQueries, query.SubQueries[key]);
        }
    }

    /// <summary>
    /// 全WITH句を取得
    /// </summary>
    public Dictionary<string, Query> GetAllWiths()
    {
        var result = new Dictionary<string, Query>(StringComparer.OrdinalIgnoreCase);

        // ルートノードまで遡る
        var node = this;
        while (node.Parent != null)
        {
            node = node.Parent;
        }

        GetAllWithsCore(node, result);
        return result;
    }

    private void GetAllWithsCore(Query node, Dictionary<string, Query> allWiths)
    {
        foreach (var key in node.Withs.Keys)
        {
            allWiths[key] = node.Withs[key];
        }

        foreach (var key in node.SubQueries.Keys)
        {
            GetAllWithsCore(node.SubQueries[key], allWiths);
        }
    }

    private void AddAllTablesInternal<T>(Query query, Dictionary<string, T> target, string kind, bool expand)
    {
        Dictionary<string, T>? sourceDict = kind switch
        {
            "TableC" => query.TableC as Dictionary<string, T>,
            "TableR" => query.TableR as Dictionary<string, T>,
            "TableU" => query.TableU as Dictionary<string, T>,
            "TableD" => query.TableD as Dictionary<string, T>,
            "ColumnC" => query.ColumnC as Dictionary<string, T>,
            "ColumnR" => query.ColumnR as Dictionary<string, T>,
            "ColumnU" => query.ColumnU as Dictionary<string, T>,
            "ColumnD" => query.ColumnD as Dictionary<string, T>,
            _ => null
        };

        if (sourceDict != null)
        {
            foreach (var kvp in sourceDict)
            {
                target[$"K{target.Count + 1}"] = kvp.Value;
            }
        }

        // サブクエリ
        if (expand)
        {
            foreach (var subQuery in query.SubQueries.Values)
            {
                AddAllTablesInternal(subQuery, target, kind, expand);
            }
        }
    }

    private void AddAllColumnsInternal(Query query, ColumnCollection target, string kind, bool expand)
    {
        ColumnCollection? source = kind switch
        {
            "Select" => query.ColumnSelect,
            "Where" => query.ColumnWhere,
            "GroupBy" => query.ColumnGroupBy,
            "OrderBy" => query.ColumnOrderBy,
            "Having" => query.ColumnHaving,
            "Insert" => query.ColumnInsert,
            "Update" => query.ColumnUpdate,
            "SetCond" => query.ColumnSetCond,
            "Delete" => query.ColumnDelete,
            _ => null
        };

        if (source != null)
        {
            target.AddRange(source);
        }

        // サブクエリ
        if (expand)
        {
            foreach (var subQuery in query.SubQueries.Values)
            {
                AddAllColumnsInternal(subQuery, target, kind, expand);
            }
        }
    }

    /// <summary>
    /// クエリを整形して返す
    /// </summary>
    public string Arrange(bool expand = false)
    {
        var formatter = new QueryFormatter();
        var strQuery = formatter.Format(QueryText);

        if (expand)
        {
            foreach (var key in SubQueries.Keys)
            {
                var subQuery = SubQueries[key];
                var strSubQuery = subQuery.Arrange(expand);

                var intSubQueryPos = strQuery.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (intSubQueryPos >= 0)
                {
                    // インデント量を計算
                    var intLineTop = strQuery.LastIndexOf('\n', intSubQueryPos);
                    if (intLineTop < 0) intLineTop = 0;
                    var linePrefix = strQuery.Substring(intLineTop);
                    var firstNonSpace = Regex.Match(linePrefix, "[^ ]");
                    var intIndent = 4 + (firstNonSpace.Success ? firstNonSpace.Index : 0);

                    // サブクエリにインデントを付与
                    var lines = strSubQuery.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                    var isUnionLike = QueryKind.Contains("UNION", StringComparison.OrdinalIgnoreCase)
                        || QueryKind.Contains("MINUS", StringComparison.OrdinalIgnoreCase)
                        || QueryKind.Contains("INTERSECT", StringComparison.OrdinalIgnoreCase);

                    var sb = new System.Text.StringBuilder();
                    if (!isUnionLike) sb.Append('(');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append('\n');
                            sb.Append(new string(' ', intIndent));
                        }
                        sb.Append(lines[i]);
                    }
                    if (!isUnionLike) sb.Append(')');

                    strQuery = strQuery.Replace(key, sb.ToString());
                }
            }
        }

        return strQuery;
    }

    /// <summary>
    /// SELECT句を展開する
    /// </summary>
    public string ExpandSelect(string selectClause)
    {
        var words = selectClause.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();

        foreach (var word in words)
        {
            var fullName = FindFullName(word);
            result.Add(fullName);
        }

        return string.Join(" ", result);
    }

    /// <summary>
    /// 別名からフルネーム（テーブル名.カラム名）を検索
    /// </summary>
    public string FindFullName(string tableColumn)
    {
        if (string.IsNullOrEmpty(tableColumn))
            return tableColumn;

        var state = GlobalState.Instance;
        var tableNames = state.TableNames;
        var tableDefinitions = state.TableDefinitions;

        var parts = tableColumn.Split('.');

        if (parts.Length == 1)
        {
            // テーブル名のみ: 論理名を返す
            var physicalName = parts[0];
            if (tableNames.TryGetValue(physicalName, out var logicalName))
                return logicalName;
            return tableColumn;
        }
        else
        {
            // テーブル名.カラム名形式
            var tableName = parts[0];
            var columnName = parts[1];

            var logicalTableName = tableNames.TryGetValue(tableName, out var lt) ? lt : tableName;

            if (tableDefinitions.TryGetValue(tableName, out var tableDef)
                && tableDef.Columns.TryGetValue(columnName, out var colDef)
                && !string.IsNullOrEmpty(colDef.AttributeName))
            {
                return logicalTableName + "." + colDef.AttributeName;
            }

            return logicalTableName + "." + columnName;
        }
    }
}
