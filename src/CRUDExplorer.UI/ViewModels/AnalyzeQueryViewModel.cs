using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;
using CRUDExplorer.SqlParser.Analyzers;

namespace CRUDExplorer.UI.ViewModels;

public partial class AnalyzeQueryViewModel : ViewModelBase
{
    private Action<string>? _showGrepWindow;
    private Action<string>? _showTableDefinitionWindow;
    private Action<IEnumerable<CrudDisplayItem>, string>? _showCrudListWindow;
    private Func<string, string?>? _getSelectedText;
    private Action? _closeWindow;
    private Action<string, int>? _launchEditor;
    private Action? _openSearchWindow;
    private Func<string, Task>? _setClipboard;

    // テキスト検索状態
    private MatchCollection? _searchMatches;
    private int _searchIndex = -1;
    private string _lastSearchText = string.Empty;

    // 検索カーソル移動コールバック
    public Action<int, int>? SelectTextRange { get; set; }

    // チェック状態変更コールバック（コードビハインドでcolorizer更新用）
    public Action? OnCheckedItemsChanged { get; set; }

    [ObservableProperty]
    private ObservableCollection<Query> _queries = new();

    [ObservableProperty]
    private Query? _selectedQuery;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _lineNumber = string.Empty;

    [ObservableProperty]
    private string _tableName = string.Empty;

    [ObservableProperty]
    private string _altName = string.Empty;

    [ObservableProperty]
    private string _windowTitle = "クエリの分析";

    [ObservableProperty]
    private string _guideText = string.Empty;

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
    private string _subquerySqlText = string.Empty;

    [ObservableProperty]
    private bool _isAutoAnalyze = true;

    [ObservableProperty]
    private string _highlightText1 = string.Empty;

    [ObservableProperty]
    private string _highlightText2 = string.Empty;

    [ObservableProperty]
    private string _highlightText3 = string.Empty;

    [ObservableProperty]
    private string _searchStatusMessage = string.Empty;

    [ObservableProperty]
    private bool _isTreeNodeSelected;

    [ObservableProperty]
    private bool _hasSubQueries;

    [ObservableProperty]
    private bool _hasValues;

    [ObservableProperty]
    private bool _hasSetValues;

    [ObservableProperty]
    private bool _hasSelectAlias;

    [ObservableProperty]
    private bool _hasColumnSelect;

    [ObservableProperty]
    private bool _hasColumnWhere;

    [ObservableProperty]
    private bool _hasColumnGroupBy;

    [ObservableProperty]
    private bool _hasColumnOrderBy;

    [ObservableProperty]
    private bool _hasColumnHaving;

    [ObservableProperty]
    private bool _hasColumnInsert;

    [ObservableProperty]
    private bool _hasColumnUpdate;

    [ObservableProperty]
    private bool _hasColumnDelete;

    [ObservableProperty]
    private bool _hasColumnSetCond;

    [ObservableProperty]
    private CrudDisplayItem? _selectedTableCrudItem;

    [ObservableProperty]
    private CrudDisplayItem? _selectedColumnCrudItem;

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

    /// <summary>
    /// コールバックを後から設定する（WindowService経由でDataContextが設定された後に呼び出す）
    /// </summary>
    public void SetCallbacks(
        Action<string>? showGrepWindow = null,
        Action<string>? showTableDefinitionWindow = null,
        Action<IEnumerable<CrudDisplayItem>, string>? showCrudListWindow = null,
        Func<string, string?>? getSelectedText = null,
        Action? closeWindow = null,
        Action<string, int>? launchEditor = null,
        Action? openSearchWindow = null,
        Func<string, Task>? setClipboard = null)
    {
        _showGrepWindow = showGrepWindow;
        _showTableDefinitionWindow = showTableDefinitionWindow;
        _showCrudListWindow = showCrudListWindow;
        _getSelectedText = getSelectedText;
        _closeWindow = closeWindow;
        _launchEditor = launchEditor;
        _openSearchWindow = openSearchWindow;
        _setClipboard = setClipboard;
    }

    partial void OnSelectedQueryChanged(Query? value)
    {
        if (value != null)
        {
            FileName = value.FileName;
            LineNumber = value.LineNo.ToString();
            WindowTitle = $"{value.FileName}({value.LineNo}) ～ クエリの分析";
            LoadQueryAnalysis(value);
        }
    }

    partial void OnSelectedTreeNodeChanged(QueryTreeNode? value)
    {
        if (value?.Tag is Query query)
        {
            // VB.NET tvQuery_AfterSelect相当:
            // ラッパー（Parent==null && SubQueries>0）→ 最初のサブクエリを展開表示
            // それ以外 → ノード固有の%N%表示
            if (query.Parent == null && query.SubQueries.Count > 0)
            {
                var mainQuery = query.SubQueries.Values.First();
                SubquerySqlText = mainQuery.Arrange(expand: true);
                LoadCrudLists(mainQuery, isRoot: true);
            }
            else
            {
                SubquerySqlText = query.Arrange();
                LoadCrudLists(query, isRoot: false);
            }

            // メニュー有効/無効更新
            var targetQuery = (query.Parent == null && query.SubQueries.Count > 0)
                ? query.SubQueries.Values.First() : query;
            IsTreeNodeSelected = true;
            HasSubQueries = targetQuery.SubQueries.Count > 0;
            HasValues = targetQuery.Values.Count > 0;
            HasSetValues = targetQuery.SetValues.Count > 0;
            HasSelectAlias = targetQuery.ColumnSelect.Any(c => !string.IsNullOrEmpty(c.Alt));
            HasColumnSelect = targetQuery.ColumnSelect.Count > 0;
            HasColumnWhere = targetQuery.ColumnWhere.Count > 0;
            HasColumnGroupBy = targetQuery.ColumnGroupBy.Count > 0;
            HasColumnOrderBy = targetQuery.ColumnOrderBy.Count > 0;
            HasColumnHaving = targetQuery.ColumnHaving.Count > 0;
            HasColumnInsert = targetQuery.ColumnInsert.Count > 0;
            HasColumnUpdate = targetQuery.ColumnUpdate.Count > 0;
            HasColumnDelete = targetQuery.ColumnDelete.Count > 0;
            HasColumnSetCond = targetQuery.ColumnSetCond.Count > 0;
        }
        else
        {
            IsTreeNodeSelected = false;
            HasSubQueries = false;
            HasValues = false;
            HasSetValues = false;
            HasSelectAlias = false;
            HasColumnSelect = false;
            HasColumnWhere = false;
            HasColumnGroupBy = false;
            HasColumnOrderBy = false;
            HasColumnHaving = false;
            HasColumnInsert = false;
            HasColumnUpdate = false;
            HasColumnDelete = false;
            HasColumnSetCond = false;
        }
    }

    // ヘッダーバーコマンド
    [RelayCommand]
    private void LaunchEditor()
    {
        if (string.IsNullOrEmpty(FileName)) return;
        _launchEditor?.Invoke(FileName, int.TryParse(LineNumber, out var ln) ? ln : 0);
    }

    [RelayCommand]
    private void CloseWindow()
    {
        _closeWindow?.Invoke();
    }

    [RelayCommand]
    private void OpenSearch()
    {
        _openSearchWindow?.Invoke();
    }

    // Toolbar Commands
    [RelayCommand]
    private void QuickAnalyze()
    {
        if (string.IsNullOrEmpty(SqlText)) return;

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
        // VB.NET ViewExpand相当: 選択中のノードのクエリを展開表示
        if (SelectedTreeNode?.Tag is Query query)
        {
            SqlText = query.Arrange(expand: true);
        }
    }

    [RelayCommand]
    private void FormatSql()
    {
        if (string.IsNullOrEmpty(SqlText)) return;

        // SQL整形: フォーマッターで整形（%N%マーカーはそのまま保持）
        var formatter = new CRUDExplorer.Core.Formatting.QueryFormatter();
        SqlText = formatter.Format(SqlText);
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

    // 個別句コマンド
    [RelayCommand]
    private void ShowClauseSelect()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        var cols = query.Parent == null ? query.GetAllColumnSelect() : query.ColumnSelect;
        ShowColumnInfo(cols);
    }

    [RelayCommand]
    private void ShowClauseWhere()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        var cols = query.Parent == null ? query.GetAllColumnWhere() : query.ColumnWhere;
        ShowColumnInfo(cols);
    }

    [RelayCommand]
    private void ShowClauseGroupBy()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        var cols = query.Parent == null ? query.GetAllColumnGroupBy() : query.ColumnGroupBy;
        ShowColumnInfo(cols);
    }

    [RelayCommand]
    private void ShowClauseOrderBy()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        var cols = query.Parent == null ? query.GetAllColumnOrderBy() : query.ColumnOrderBy;
        ShowColumnInfo(cols);
    }

    [RelayCommand]
    private void ShowClauseHaving()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        var cols = query.Parent == null ? query.GetAllColumnHaving() : query.ColumnHaving;
        ShowColumnInfo(cols);
    }

    [RelayCommand]
    private void ShowClauseInsert()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        ShowColumnInfo(query.ColumnInsert);
    }

    [RelayCommand]
    private void ShowClauseUpdate()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        ShowColumnInfo(query.ColumnUpdate);
    }

    [RelayCommand]
    private void ShowClauseDelete()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        ShowColumnInfo(query.ColumnDelete);
    }

    [RelayCommand]
    private void ShowClauseSetCond()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        ShowColumnInfo(query.ColumnSetCond);
    }

    [RelayCommand]
    private void ShowSelectAlias()
    {
        if (SelectedTreeNode?.Tag is not Query query) return;
        var items = new List<CrudDisplayItem>();
        foreach (var col in query.ColumnSelect)
        {
            if (!string.IsNullOrEmpty(col.Alt))
                items.Add(new CrudDisplayItem
                {
                    DisplayText = $"{col.Alt} = {col.ColumnName}",
                    ColumnName = col.ColumnName
                });
        }
        _showCrudListWindow?.Invoke(items, "SELECT句の別名と式の対応");
    }

    // CRUDリストコンテキストメニューコマンド
    [RelayCommand]
    private void CheckAllTables()
    {
        foreach (var item in TableCrudList) item.IsChecked = true;
        OnCheckedItemsChanged?.Invoke();
    }

    [RelayCommand]
    private void UncheckAllTables()
    {
        foreach (var item in TableCrudList) item.IsChecked = false;
        OnCheckedItemsChanged?.Invoke();
    }

    [RelayCommand]
    private void CheckAllColumns()
    {
        foreach (var item in ColumnCrudList) item.IsChecked = true;
        OnCheckedItemsChanged?.Invoke();
    }

    [RelayCommand]
    private void UncheckAllColumns()
    {
        foreach (var item in ColumnCrudList) item.IsChecked = false;
        OnCheckedItemsChanged?.Invoke();
    }

    [RelayCommand]
    private void ShowTableDefinitionFromCrud()
    {
        var selected = SelectedTableCrudItem;
        if (selected != null)
            _showTableDefinitionWindow?.Invoke(selected.TableName);
    }

    [RelayCommand]
    private void ShowTableDefinitionFromColumnCrud()
    {
        var selected = SelectedColumnCrudItem;
        if (selected != null)
            _showTableDefinitionWindow?.Invoke(selected.TableName);
    }

    [RelayCommand]
    private void CrudSearchTable()
    {
        var selected = SelectedTableCrudItem;
        if (selected != null)
            _showGrepWindow?.Invoke($"crud-table:{selected.TableName}");
    }

    [RelayCommand]
    private void CrudSearchColumn()
    {
        var selected = SelectedColumnCrudItem;
        if (selected != null)
            _showGrepWindow?.Invoke($"crud-column:{selected.TableName}.{selected.ColumnName}");
    }

    [RelayCommand]
    private void CrudSearchTableFromColumn()
    {
        var selected = SelectedColumnCrudItem;
        if (selected != null)
            _showGrepWindow?.Invoke($"crud-table:{selected.TableName}");
    }

    // ─── テーブルCRUD コピーコマンド ───────────────────────────────

    [RelayCommand]
    private async Task CopyTableSelectedRow()
    {
        var selected = SelectedTableCrudItem;
        if (selected == null || _setClipboard == null) return;
        var line = $"{selected.TableName}\t{selected.EntityName}\t{selected.AltName}\t{selected.CrudType}";
        await _setClipboard(line);
    }

    [RelayCommand]
    private async Task CopyTableCheckedRows()
    {
        if (_setClipboard == null) return;
        var sb = new StringBuilder();
        sb.AppendLine("テーブル名\tエンティティ名\t代替\tCRUD");
        foreach (var item in TableCrudList.Where(t => t.IsChecked))
            sb.AppendLine($"{item.TableName}\t{item.EntityName}\t{item.AltName}\t{item.CrudType}");
        await _setClipboard(sb.ToString().TrimEnd());
    }

    [RelayCommand]
    private async Task CopyTableAllRows()
    {
        if (_setClipboard == null) return;
        var sb = new StringBuilder();
        sb.AppendLine("テーブル名\tエンティティ名\t代替\tCRUD");
        foreach (var item in TableCrudList)
            sb.AppendLine($"{item.TableName}\t{item.EntityName}\t{item.AltName}\t{item.CrudType}");
        await _setClipboard(sb.ToString().TrimEnd());
    }

    [RelayCommand]
    private async Task CopyTableColumnValue(string columnName)
    {
        var selected = SelectedTableCrudItem;
        if (selected == null || _setClipboard == null) return;
        var value = columnName switch
        {
            "テーブル名" => selected.TableName,
            "エンティティ名" => selected.EntityName,
            "代替" => selected.AltName,
            "CRUD" => selected.CrudType,
            _ => string.Empty
        };
        await _setClipboard(value);
    }

    // ─── カラムCRUD コピーコマンド ───────────────────────────────

    [RelayCommand]
    private async Task CopyColumnSelectedRow()
    {
        var selected = SelectedColumnCrudItem;
        if (selected == null || _setClipboard == null) return;
        var line = $"{selected.TableName}\t{selected.EntityName}\t{selected.ColumnName}\t{selected.AttributeName}\t{selected.CrudType}";
        await _setClipboard(line);
    }

    [RelayCommand]
    private async Task CopyColumnCheckedRows()
    {
        if (_setClipboard == null) return;
        var sb = new StringBuilder();
        sb.AppendLine("テーブル名\tエンティティ名\tカラム名\t属性名\tCRUD");
        foreach (var item in ColumnCrudList.Where(c => c.IsChecked))
            sb.AppendLine($"{item.TableName}\t{item.EntityName}\t{item.ColumnName}\t{item.AttributeName}\t{item.CrudType}");
        await _setClipboard(sb.ToString().TrimEnd());
    }

    [RelayCommand]
    private async Task CopyColumnAllRows()
    {
        if (_setClipboard == null) return;
        var sb = new StringBuilder();
        sb.AppendLine("テーブル名\tエンティティ名\tカラム名\t属性名\tCRUD");
        foreach (var item in ColumnCrudList)
            sb.AppendLine($"{item.TableName}\t{item.EntityName}\t{item.ColumnName}\t{item.AttributeName}\t{item.CrudType}");
        await _setClipboard(sb.ToString().TrimEnd());
    }

    [RelayCommand]
    private async Task CopyColumnColumnValue(string columnName)
    {
        var selected = SelectedColumnCrudItem;
        if (selected == null || _setClipboard == null) return;
        var value = columnName switch
        {
            "テーブル名" => selected.TableName,
            "エンティティ名" => selected.EntityName,
            "カラム名" => selected.ColumnName,
            "属性名" => selected.AttributeName,
            "CRUD" => selected.CrudType,
            _ => string.Empty
        };
        await _setClipboard(value);
    }

    // ─── テーブルCRUD クエリGrepコマンド ───────────────────────────

    [RelayCommand]
    private void GrepTableFromFile(string columnName)
    {
        var selected = SelectedTableCrudItem;
        if (selected == null) return;
        var value = GetTableCrudColumnValue(selected, columnName);
        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FileName))
            _showGrepWindow?.Invoke($"file:{FileName}:{value}");
    }

    [RelayCommand]
    private void GrepTableFromProgram(string columnName)
    {
        var selected = SelectedTableCrudItem;
        if (selected == null) return;
        var value = GetTableCrudColumnValue(selected, columnName);
        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FileName))
            _showGrepWindow?.Invoke($"program:{FileName}:{value}");
    }

    [RelayCommand]
    private void GrepTableFromAll(string columnName)
    {
        var selected = SelectedTableCrudItem;
        if (selected == null) return;
        var value = GetTableCrudColumnValue(selected, columnName);
        if (!string.IsNullOrEmpty(value))
            _showGrepWindow?.Invoke($"all::{value}");
    }

    private static string GetTableCrudColumnValue(CrudDisplayItem item, string columnName)
    {
        return columnName switch
        {
            "テーブル名" => item.TableName,
            "エンティティ名" => item.EntityName,
            "代替" => item.AltName,
            "CRUD" => item.CrudType,
            _ => string.Empty
        };
    }

    // ─── カラムCRUD クエリGrepコマンド ───────────────────────────

    [RelayCommand]
    private void GrepColumnFromFile(string columnName)
    {
        var selected = SelectedColumnCrudItem;
        if (selected == null) return;
        var value = GetColumnCrudColumnValue(selected, columnName);
        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FileName))
            _showGrepWindow?.Invoke($"file:{FileName}:{value}");
    }

    [RelayCommand]
    private void GrepColumnFromProgram(string columnName)
    {
        var selected = SelectedColumnCrudItem;
        if (selected == null) return;
        var value = GetColumnCrudColumnValue(selected, columnName);
        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FileName))
            _showGrepWindow?.Invoke($"program:{FileName}:{value}");
    }

    [RelayCommand]
    private void GrepColumnFromAll(string columnName)
    {
        var selected = SelectedColumnCrudItem;
        if (selected == null) return;
        var value = GetColumnCrudColumnValue(selected, columnName);
        if (!string.IsNullOrEmpty(value))
            _showGrepWindow?.Invoke($"all::{value}");
    }

    private static string GetColumnCrudColumnValue(CrudDisplayItem item, string columnName)
    {
        return columnName switch
        {
            "テーブル名" => item.TableName,
            "エンティティ名" => item.EntityName,
            "カラム名" => item.ColumnName,
            "属性名" => item.AttributeName,
            "CRUD" => item.CrudType,
            _ => string.Empty
        };
    }

    private void ShowColumnInfo(ColumnCollection columns)
    {
        var items = new List<CrudDisplayItem>();
        AddColumnItems(columns, "", items);
        _showCrudListWindow?.Invoke(items, "句情報");
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

        // VB.NET AnalyzeQuery相当: ラッパーノードを作成
        // ラッパーはParent=null、dctSubQuerysに本体クエリを持つ
        var wrapperQuery = new Query
        {
            QueryKind = query.QueryKind,
            FileName = query.FileName,
            LineNo = query.LineNo
        };
        wrapperQuery.SubQueries["%0%"] = query;
        query.Parent = wrapperQuery;

        // ルートラッパーノード（VB: 【全体@関数名】）
        var funcName = !string.IsNullOrEmpty(query.FileName)
            ? System.IO.Path.GetFileNameWithoutExtension(query.FileName)
            : string.Empty;
        var rootWrapper = new QueryTreeNode
        {
            DisplayText = $"【全体@{funcName}】",
            Tag = wrapperQuery
        };

        // 本体ノード以下を構築
        var rootNode = BuildTreeNode(query, isRoot: true);
        rootWrapper.Children.Add(rootNode);
        QueryTreeData.Add(rootWrapper);

        // 左エディタに全体SQLを表示（ツリーノード変更では書き換えない）
        SqlText = query.Arrange(expand: true);

        // 初期表示: ルートを選択 → 展開表示（VB: tvQuery.SelectedNode = objRoot）
        SelectedTreeNode = rootWrapper;
    }

    private void LoadCrudLists(Query query, bool isRoot)
    {
        TableCrudList.Clear();
        ColumnCrudList.Clear();

        if (isRoot)
        {
            // ルート: GetAll系を使用
            AddTableCrudItems(query.GetAllTableC(), "C");
            AddTableCrudItems(query.GetAllTableR(), "R");
            AddTableCrudItems(query.GetAllTableU(), "U");
            AddTableCrudItems(query.GetAllTableD(), "D");
        }
        else
        {
            // 子ノード: ノード固有のテーブルのみ
            AddTableCrudItems(query.TableC, "C");
            AddTableCrudItems(query.TableR, "R");
            AddTableCrudItems(query.TableU, "U");
            AddTableCrudItems(query.TableD, "D");
        }

        // カラムCRUD: 全ColumnCollectionからカラムを収集し、エイリアス→実テーブル名に解決
        BuildColumnCrudFromCollections(query, isRoot);
    }

    /// <summary>
    /// 全ColumnCollectionからカラムを収集してColumnCrudListを構築する。
    /// Column.Tableがエイリアスの場合、TableR等から実テーブル名に解決する。
    /// </summary>
    private void BuildColumnCrudFromCollections(Query query, bool isRoot)
    {
        // エイリアス→実テーブル名の逆引きマップ構築
        var aliasMap = BuildAliasToTableMap(query, isRoot);

        // CRUD種別の決定: QueryKindに基づく
        var crudType = query.QueryKind?.ToUpperInvariant() switch
        {
            "INSERT" => "C",
            "UPDATE" => "U",
            "DELETE" => "D",
            _ => "R"
        };

        // 全カラムを収集（isRoot=true→サブクエリ含む）
        var allColumns = isRoot
            ? query.GetAllColumns2(expand: true)
            : query.GetAllColumns2(expand: false);

        // 重複排除用 (TABLE.COLUMN.CRUD)
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var col in allColumns)
        {
            // テーブル名解決: エイリアスなら実テーブル名に変換
            var rawTable = col.Table;
            var resolvedTable = ResolveTableName(rawTable, aliasMap);

            // カラムCRUD種別: INSERT/UPDATEカラムはC/U、それ以外はR
            var colCrud = col.ClauseKind switch
            {
                ClauseKind.Insert => "C",
                ClauseKind.Update => "U",
                _ => crudType == "R" ? "R" : "R" // SELECT/DELETEのカラムは全てR
            };
            // UPDATE文のSET句カラムはU
            if (crudType == "U" && (col.ClauseKind == ClauseKind.Update || col.ClauseKind == ClauseKind.SetCondition))
                colCrud = "U";

            var colName = col.ColumnName;
            if (string.IsNullOrEmpty(colName)) continue;

            // 重複チェック
            var dedupeKey = $"{resolvedTable}.{colName}.{colCrud}";
            if (!seen.Add(dedupeKey)) continue;

            var resolver = new LogicalNameResolver(
                GlobalState.Instance.TableNames,
                GlobalState.Instance.TableDefinitions);
            var entityName = !string.IsNullOrEmpty(resolvedTable) ? resolver.GetLogicalName(resolvedTable) : string.Empty;

            // 属性名
            string attrName = colName;
            var tableDefs = GlobalState.Instance.TableDefinitions;
            if (tableDefs != null && !string.IsNullOrEmpty(resolvedTable)
                && tableDefs.TryGetValue(resolvedTable, out var tableDef)
                && tableDef.Columns.TryGetValue(colName, out var colDef)
                && !string.IsNullOrEmpty(colDef.AttributeName))
            {
                attrName = colDef.AttributeName;
            }

            ColumnCrudList.Add(new CrudDisplayItem
            {
                DisplayText = $"[{colCrud}] {resolvedTable}.{colName}",
                TableName = resolvedTable,
                EntityName = entityName,
                ColumnName = colName,
                AttributeName = attrName,
                CrudType = colCrud
            });
        }
    }

    /// <summary>
    /// エイリアス→実テーブル名の逆引きマップを構築
    /// TableC/R/U/D の値が "TABLENAME\tALIAS" 形式
    /// </summary>
    private static Dictionary<string, string> BuildAliasToTableMap(Query query, bool isRoot)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        void AddFromDict(Dictionary<string, string> tables)
        {
            foreach (var kvp in tables)
            {
                var parts = kvp.Value.Split('\t');
                var realName = parts[0];
                var alias = parts.Length > 1 ? parts[1] : string.Empty;
                // 実テーブル名自体もマップに追加（大文字小文字の正規化用）
                if (!map.ContainsKey(realName))
                    map[realName] = realName;
                if (!string.IsNullOrEmpty(alias) && !map.ContainsKey(alias))
                    map[alias] = realName;
            }
        }

        void AddFromQuery(Query q)
        {
            AddFromDict(q.TableC);
            AddFromDict(q.TableR);
            AddFromDict(q.TableU);
            AddFromDict(q.TableD);
        }

        AddFromQuery(query);

        if (isRoot)
        {
            // サブクエリのテーブルも再帰的に収集
            void AddSubQueries(Query q)
            {
                foreach (var sub in q.SubQueries.Values)
                {
                    AddFromQuery(sub);
                    AddSubQueries(sub);
                }
            }
            AddSubQueries(query);
        }

        return map;
    }

    /// <summary>
    /// エイリアスまたはテーブル名を実テーブル名に解決
    /// </summary>
    private static string ResolveTableName(string nameOrAlias, Dictionary<string, string> aliasMap)
    {
        if (string.IsNullOrEmpty(nameOrAlias)) return string.Empty;
        return aliasMap.TryGetValue(nameOrAlias, out var realName) ? realName : nameOrAlias;
    }

    private QueryTreeNode BuildTreeNode(Query query, bool isRoot = false)
    {
        var resolver = new LogicalNameResolver(
            GlobalState.Instance.TableNames,
            GlobalState.Instance.TableDefinitions);

        // ノードタイトルを構築（オリジナルのAddSubQueryNode相当）
        string title;
        if (!string.IsNullOrEmpty(query.AltName))
            title = $"【{query.AltName}】{query.QueryKind} ";
        else if (query.SubQueryIndex == "0" || isRoot)
            title = $"【本体】{query.QueryKind} ";
        else
            title = $"【%{query.SubQueryIndex}%】{query.QueryKind} ";

        // VB.NET準拠: ノード固有のテーブルのみ表示（GetAllTablesではなくAllTable相当）
        var ownTables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in query.TableC) if (!ownTables.ContainsKey(kvp.Key)) ownTables[kvp.Key] = kvp.Value;
        foreach (var kvp in query.TableR) if (!ownTables.ContainsKey(kvp.Key)) ownTables[kvp.Key] = kvp.Value;
        foreach (var kvp in query.TableU) if (!ownTables.ContainsKey(kvp.Key)) ownTables[kvp.Key] = kvp.Value;
        foreach (var kvp in query.TableD) if (!ownTables.ContainsKey(kvp.Key)) ownTables[kvp.Key] = kvp.Value;

        foreach (var tableEntry in ownTables.Values)
        {
            var parts = tableEntry.Split('\t');
            var tableName = parts[0];
            var logicalName = resolver.GetLogicalName(tableName);
            title += $"【{logicalName}】";
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
            var altName = parts.Length > 1 ? parts[1] : string.Empty;
            var logicalName = resolver.GetLogicalName(tableName);

            TableCrudList.Add(new CrudDisplayItem
            {
                DisplayText = $"[{crudType}] {tableName} ({logicalName})",
                TableName = tableName,
                EntityName = logicalName,
                AltName = altName,
                CrudType = crudType
            });
        }
    }

    private void AddColumnCrudItems(Dictionary<string, object> columns, string crudType)
    {
        var resolver = new LogicalNameResolver(
            GlobalState.Instance.TableNames,
            GlobalState.Instance.TableDefinitions);

        // VB.NET準拠: KEYが"TABLE.COLUMN"形式
        foreach (var key in columns.Keys)
        {
            // key形式: "TableName.ColumnName" or "ColumnName"
            var dotIndex = key.IndexOf('.');
            var tblName = dotIndex >= 0 ? key[..dotIndex] : string.Empty;
            var colName = dotIndex >= 0 ? key[(dotIndex + 1)..] : key;
            var entityName = !string.IsNullOrEmpty(tblName) ? resolver.GetLogicalName(tblName) : string.Empty;

            // 属性名: テーブル定義がある場合はカラムの属性名を取得、なければカラム名
            string attrName = colName;
            var tableDefs = GlobalState.Instance.TableDefinitions;
            if (tableDefs != null && tableDefs.TryGetValue(tblName, out var tableDef)
                && tableDef.Columns.TryGetValue(colName, out var colDef)
                && !string.IsNullOrEmpty(colDef.AttributeName))
            {
                attrName = colDef.AttributeName;
            }

            ColumnCrudList.Add(new CrudDisplayItem
            {
                DisplayText = $"[{crudType}] {key} ({attrName})",
                TableName = tblName,
                EntityName = entityName,
                ColumnName = colName,
                AttributeName = attrName,
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

    /// <summary>
    /// SearchWindow からのコールバック、または前/次ボタンで呼ばれる検索実行
    /// </summary>
    public void ExecuteSearch(string searchText, bool matchCase, bool matchWholeWord, bool useRegex, bool forward)
    {
        if (string.IsNullOrEmpty(SqlText) || string.IsNullOrEmpty(searchText))
        {
            SearchStatusMessage = string.IsNullOrEmpty(SqlText) ? "テキストがありません" : "検索文字列を入力してください";
            return;
        }

        // 検索キーを構築（条件が変わったらリセット）
        var searchKey = $"{searchText}|{matchCase}|{matchWholeWord}|{useRegex}";
        if (searchKey != _lastSearchText)
        {
            var options = matchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
            var escaped = useRegex ? searchText : Regex.Escape(searchText);
            if (matchWholeWord)
                escaped = $@"\b{escaped}\b";
            _searchMatches = Regex.Matches(SqlText, escaped, options);
            _searchIndex = -1;
            _lastSearchText = searchKey;
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
        SearchStatusMessage = $"「{searchText}」: {_searchIndex + 1}/{_searchMatches.Count} 件目";

        // ハイライト1にも設定（エディタ上で色付き表示）
        HighlightText1 = searchText;

        // カーソルを移動してテキストを選択
        SelectTextRange?.Invoke(match.Index, match.Length);
    }

    private void ExecuteSearch(bool forward)
    {
        // ツールバーの前/次ボタン用 - HighlightText1で簡易検索
        var searchText = !string.IsNullOrEmpty(HighlightText1) ? HighlightText1
            : !string.IsNullOrEmpty(HighlightText2) ? HighlightText2
            : !string.IsNullOrEmpty(HighlightText3) ? HighlightText3
            : string.Empty;

        if (!string.IsNullOrEmpty(searchText))
            ExecuteSearch(searchText, matchCase: false, matchWholeWord: false, useRegex: false, forward: forward);
        else
            SearchStatusMessage = "ハイライトテキストを設定してから検索してください";
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
/// CRUD display item for DataGrid lists
/// </summary>
public partial class CrudDisplayItem : ObservableObject
{
    public string DisplayText { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string AltName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string CrudType { get; set; } = string.Empty; // C, R, U, D

    [ObservableProperty]
    private bool _isChecked;
}
