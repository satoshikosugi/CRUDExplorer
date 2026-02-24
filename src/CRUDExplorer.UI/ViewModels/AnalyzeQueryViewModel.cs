using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;
using CRUDExplorer.SqlParser.Analyzers;

namespace CRUDExplorer.UI.ViewModels;

public partial class AnalyzeQueryViewModel : ViewModelBase
{
    private readonly Action<string>? _showGrepWindow;
    private readonly Action<string>? _showTableDefinitionWindow;
    private readonly Action<IEnumerable<CrudDisplayItem>, string>? _showCrudListWindow;
    private readonly Func<string, string?>? _getSelectedText;

    // テキスト検索状態
    private MatchCollection? _searchMatches;
    private int _searchIndex = -1;
    private string _lastSearchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Query> _queries = new();

    [ObservableProperty]
    private Query? _selectedQuery;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _lineNumber = string.Empty;

    [ObservableProperty]
    private ObservableCollection<QueryTreeNode> _queryTreeData = new();

    [ObservableProperty]
    private QueryTreeNode? _selectedTreeNode;

    [ObservableProperty]
    private ObservableCollection<CrudDisplayItem> _tableCrudList = new();

    [ObservableProperty]
    private ObservableCollection<CrudDisplayItem> _columnCrudList = new();

    [ObservableProperty]
    private string _sqlText = string.Empty;

    [ObservableProperty]
    private string _highlightText1 = string.Empty;

    [ObservableProperty]
    private string _highlightText2 = string.Empty;

    [ObservableProperty]
    private string _highlightText3 = string.Empty;

    [ObservableProperty]
    private string _searchStatusMessage = string.Empty;

    public AnalyzeQueryViewModel(
        Action<string>? showGrepWindow = null,
        Action<string>? showTableDefinitionWindow = null,
        Action<IEnumerable<CrudDisplayItem>, string>? showCrudListWindow = null,
        Func<string, string?>? getSelectedText = null)
    {
        _showGrepWindow = showGrepWindow;
        _showTableDefinitionWindow = showTableDefinitionWindow;
        _showCrudListWindow = showCrudListWindow;
        _getSelectedText = getSelectedText;
    }

    partial void OnSelectedQueryChanged(Query? value)
    {
        if (value != null)
        {
            FileName = value.FileName;
            LineNumber = value.LineNo.ToString();
            SqlText = value.Arrange();
            LoadQueryAnalysis(value);
        }
    }

    // Toolbar Commands
    [RelayCommand]
    private void QuickAnalyze()
    {
        if (SelectedQuery == null) return;

        // 最新のSQLテキストを再解析してツリーとCRUDリストを更新
        var analyzer = new SqlAnalyzer();
        var query = analyzer.AnalyzeSql(SqlText, FileName,
            int.TryParse(LineNumber, out var ln) ? ln : 0);
        LoadQueryAnalysis(query);
    }

    [RelayCommand]
    private void ConvertLogicalName()
    {
        if (string.IsNullOrEmpty(SqlText)) return;

        var resolver = new LogicalNameResolver(
            GlobalState.Instance.TableNames,
            GlobalState.Instance.TableDefinitions);

        // SQLテキスト内のテーブル名・カラム名を論理名に変換
        var converted = ConvertSqlToLogicalNames(SqlText, resolver);
        SqlText = converted;
    }

    [RelayCommand]
    private void ExpandView()
    {
        if (string.IsNullOrEmpty(SqlText)) return;
        if (SelectedQuery == null) return;

        // VIEW展開: サブクエリを展開して表示
        SqlText = SelectedQuery.Arrange(expand: true);
    }

    [RelayCommand]
    private void ExtractStrings()
    {
        if (string.IsNullOrEmpty(SqlText)) return;

        // SQL内の文字列リテラルを抽出
        var matches = Regex.Matches(SqlText, @"'([^']*)'");
        var strings = matches
            .Select(m => m.Groups[1].Value)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .ToList();

        if (strings.Count > 0)
        {
            // 抽出された文字列をハイライト1に設定（表示用）
            HighlightText1 = string.Join(", ", strings.Take(5));
        }
    }

    [RelayCommand]
    private void ShowTableDefinition()
    {
        // 選択されたCRUDアイテムのテーブルを表示
        var selectedTable = TableCrudList.FirstOrDefault()?.TableName;
        _showTableDefinitionWindow?.Invoke(selectedTable ?? string.Empty);
    }

    [RelayCommand]
    private void SearchPrevious()
    {
        ExecuteSearch(forward: false);
    }

    [RelayCommand]
    private void SearchNext()
    {
        ExecuteSearch(forward: true);
    }

    // TreeView Context Menu Commands
    [RelayCommand]
    private void ExpandSubQuery()
    {
        if (SelectedTreeNode?.Tag is Query subQuery)
        {
            SqlText = subQuery.Arrange(expand: true);
            LoadQueryAnalysis(subQuery);
        }
    }

    [RelayCommand]
    private void ShowClauseInfo()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;

        var items = new List<CrudDisplayItem>();
        AddColumnItems(query.ColumnSelect, "SELECT", items);
        AddColumnItems(query.ColumnWhere, "WHERE", items);
        AddColumnItems(query.ColumnOrderBy, "ORDER BY", items);
        AddColumnItems(query.ColumnGroupBy, "GROUP BY", items);
        AddColumnItems(query.ColumnHaving, "HAVING", items);
        _showCrudListWindow?.Invoke(items, "句情報");
    }

    [RelayCommand]
    private void ShowIntoValues()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;

        var items = new List<CrudDisplayItem>();
        AddColumnItems(query.ColumnInsert, "INSERT", items);
        foreach (var kvp in query.Values)
            items.Add(new CrudDisplayItem { DisplayText = $"VALUES: {kvp.Value}", ColumnName = kvp.Key });
        _showCrudListWindow?.Invoke(items, "INTO/VALUES情報");
    }

    [RelayCommand]
    private void ShowSetClause()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;

        var items = new List<CrudDisplayItem>();
        AddColumnItems(query.ColumnUpdate, "UPDATE SET", items);
        foreach (var kvp in query.SetValues)
            items.Add(new CrudDisplayItem { DisplayText = $"SET: {kvp.Key} = {kvp.Value}", ColumnName = kvp.Key });
        _showCrudListWindow?.Invoke(items, "SET句情報");
    }

    // SQL Text Context Menu Commands
    [RelayCommand]
    private void ShowCrudList()
    {
        var allItems = TableCrudList.Concat(ColumnCrudList).ToList();
        _showCrudListWindow?.Invoke(allItems, "CRUDリスト");
    }

    [RelayCommand]
    private void GrepInFile()
    {
        var word = _getSelectedText?.Invoke("search") ?? HighlightText1;
        if (!string.IsNullOrEmpty(word) && !string.IsNullOrEmpty(FileName))
            _showGrepWindow?.Invoke($"file:{FileName}:{word}");
    }

    [RelayCommand]
    private void GrepInProgram()
    {
        var word = _getSelectedText?.Invoke("search") ?? HighlightText1;
        if (!string.IsNullOrEmpty(word) && !string.IsNullOrEmpty(FileName))
            _showGrepWindow?.Invoke($"program:{FileName}:{word}");
    }

    [RelayCommand]
    private void GrepAll()
    {
        var word = _getSelectedText?.Invoke("search") ?? HighlightText1;
        if (!string.IsNullOrEmpty(word))
            _showGrepWindow?.Invoke($"all::{word}");
    }

    [RelayCommand]
    private void HighlightColor1()
    {
        var selected = _getSelectedText?.Invoke("highlight");
        if (!string.IsNullOrEmpty(selected))
            HighlightText1 = selected;
    }

    [RelayCommand]
    private void HighlightColor2()
    {
        var selected = _getSelectedText?.Invoke("highlight");
        if (!string.IsNullOrEmpty(selected))
            HighlightText2 = selected;
    }

    [RelayCommand]
    private void HighlightColor3()
    {
        var selected = _getSelectedText?.Invoke("highlight");
        if (!string.IsNullOrEmpty(selected))
            HighlightText3 = selected;
    }

    [RelayCommand]
    private void ClearHighlight()
    {
        HighlightText1 = string.Empty;
        HighlightText2 = string.Empty;
        HighlightText3 = string.Empty;
    }

    private void LoadQueryAnalysis(Query query)
    {
        QueryTreeData.Clear();
        TableCrudList.Clear();
        ColumnCrudList.Clear();

        // クエリツリーを構築
        var rootNode = BuildTreeNode(query, isRoot: true);
        QueryTreeData.Add(rootNode);

        // テーブルCRUDリストを構築
        AddTableCrudItems(query.GetAllTableC(), "C");
        AddTableCrudItems(query.GetAllTableR(), "R");
        AddTableCrudItems(query.GetAllTableU(), "U");
        AddTableCrudItems(query.GetAllTableD(), "D");

        // カラムCRUDリストを構築
        AddColumnCrudItems(query.GetAllColumnC(), "C");
        AddColumnCrudItems(query.GetAllColumnR(), "R");
        AddColumnCrudItems(query.GetAllColumnU(), "U");
        AddColumnCrudItems(query.GetAllColumnD(), "D");
    }

    private QueryTreeNode BuildTreeNode(Query query, bool isRoot = false)
    {
        var resolver = new LogicalNameResolver(
            GlobalState.Instance.TableNames,
            GlobalState.Instance.TableDefinitions);

        // ノードタイトルを構築（オリジナルのAddSubQueryNode相当）
        string title;
        if (!string.IsNullOrEmpty(query.AltName))
            title = $"[{query.AltName}] {query.QueryKind} ";
        else if (query.SubQueryIndex == "0" || isRoot)
            title = $"[本体] {query.QueryKind} ";
        else
            title = $"[%{query.SubQueryIndex}%] {query.QueryKind} ";

        var allTables = query.GetAllTables();
        foreach (var tableEntry in allTables.Values)
        {
            var parts = tableEntry.Split('\t');
            var tableName = parts[0];
            var logicalName = resolver.GetLogicalName(tableName);
            title += $"[{logicalName}]";
        }

        var node = new QueryTreeNode
        {
            DisplayText = title,
            Tag = query
        };

        // サブクエリのノードを追加
        foreach (var subQuery in query.SubQueries.Values)
        {
            node.Children.Add(BuildTreeNode(subQuery));
        }

        return node;
    }

    private void AddTableCrudItems(Dictionary<string, string> tables, string crudType)
    {
        var resolver = new LogicalNameResolver(
            GlobalState.Instance.TableNames,
            GlobalState.Instance.TableDefinitions);

        foreach (var tableEntry in tables.Values)
        {
            var parts = tableEntry.Split('\t');
            var tableName = parts[0];
            var logicalName = resolver.GetLogicalName(tableName);

            TableCrudList.Add(new CrudDisplayItem
            {
                DisplayText = $"[{crudType}] {tableName} ({logicalName})",
                TableName = tableName,
                CrudType = crudType
            });
        }
    }

    private void AddColumnCrudItems(Dictionary<string, object> columns, string crudType)
    {
        var resolver = new LogicalNameResolver(
            GlobalState.Instance.TableNames,
            GlobalState.Instance.TableDefinitions);

        foreach (var key in columns.Keys)
        {
            var logicalName = resolver.GetLogicalName(key);
            ColumnCrudList.Add(new CrudDisplayItem
            {
                DisplayText = $"[{crudType}] {key} ({logicalName})",
                ColumnName = key,
                CrudType = crudType
            });
        }
    }

    private void AddColumnItems(ColumnCollection columns, string clauseName, List<CrudDisplayItem> items)
    {
        foreach (var col in columns)
        {
            items.Add(new CrudDisplayItem
            {
                DisplayText = $"{clauseName}: {col.ColumnName}",
                ColumnName = col.ColumnName
            });
        }
    }

    private void ExecuteSearch(bool forward)
    {
        if (string.IsNullOrEmpty(SqlText))
        {
            SearchStatusMessage = "テキストがありません";
            return;
        }

        // 検索対象文字列を取得（HighlightText1 or HighlightText2 or HighlightText3から）
        var searchText = !string.IsNullOrEmpty(HighlightText1) ? HighlightText1
            : !string.IsNullOrEmpty(HighlightText2) ? HighlightText2
            : !string.IsNullOrEmpty(HighlightText3) ? HighlightText3
            : string.Empty;

        if (string.IsNullOrEmpty(searchText))
        {
            SearchStatusMessage = "ハイライトテキストを設定してから検索してください";
            return;
        }

        // 検索対象が変わった場合はリセット
        if (searchText != _lastSearchText)
        {
            _searchMatches = Regex.Matches(SqlText, Regex.Escape(searchText), RegexOptions.IgnoreCase);
            _searchIndex = -1;
            _lastSearchText = searchText;
        }

        if (_searchMatches == null || _searchMatches.Count == 0)
        {
            SearchStatusMessage = $"「{searchText}」は見つかりませんでした";
            return;
        }

        if (forward)
            _searchIndex = (_searchIndex + 1) % _searchMatches.Count;
        else
            _searchIndex = (_searchIndex - 1 + _searchMatches.Count) % _searchMatches.Count;

        var match = _searchMatches[_searchIndex];
        SearchStatusMessage = $"「{searchText}」: {_searchIndex + 1}/{_searchMatches.Count} 件目 (位置: {match.Index})";
    }

    private static string ConvertSqlToLogicalNames(string sqlText, LogicalNameResolver resolver)
    {
        // テーブル名.カラム名のパターンを論理名に変換
        return Regex.Replace(sqlText, @"\b(\w+)\.(\w+)\b", m =>
        {
            var logical = resolver.GetLogicalName(m.Value);
            return logical != m.Value ? logical : m.Value;
        });
    }
}

/// <summary>
/// Tree node for query structure display
/// </summary>
public class QueryTreeNode
{
    public string DisplayText { get; set; } = string.Empty;
    public ObservableCollection<QueryTreeNode> Children { get; set; } = new();
    public object? Tag { get; set; }
}

/// <summary>
/// CRUD display item for lists
/// </summary>
public class CrudDisplayItem
{
    public string DisplayText { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string CrudType { get; set; } = string.Empty; // C, R, U, D
}
