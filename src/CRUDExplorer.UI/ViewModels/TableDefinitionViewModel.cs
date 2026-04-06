using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.ViewModels;

public partial class TableDefinitionViewModel : ViewModelBase
{
    private readonly Action _closeWindow;
    private readonly Func<string, System.Threading.Tasks.Task>? _setClipboard;

    // コンテキストメニュー用コールバック: CrudSearchWindowを開く
    private Action<string, string>? _showCrudSearch;

    // SQL挿入用コールバック: 指定テキストを関連コントロールに挿入
    private Action<string>? _insertText;

    [ObservableProperty]
    private ObservableCollection<string> _tables = new();

    [ObservableProperty]
    private string? _selectedTable;

    [ObservableProperty]
    private ObservableCollection<ColumnDefinition> _columns = new();

    [ObservableProperty]
    private ObservableCollection<IndexDefinition> _indexes = new();

    [ObservableProperty]
    private ObservableCollection<ForeignKeyDefinition> _foreignKeys = new();

    [ObservableProperty]
    private string _ddlScript = string.Empty;

    [ObservableProperty]
    private ColumnDefinition? _selectedColumn;

    [ObservableProperty]
    private string _sqlBuildText = string.Empty;

    /// <summary>SQL挿入パネルを表示するか（関連コントロールが渡された場合のみ true）</summary>
    [ObservableProperty]
    private bool _showSqlInsertPanel = false;

    public TableDefinitionViewModel(
        Action? closeWindow = null,
        Func<string, System.Threading.Tasks.Task>? setClipboard = null,
        Action<string, string>? showCrudSearch = null,
        Action<string>? insertText = null)
    {
        _closeWindow = closeWindow ?? (() => { });
        _setClipboard = setClipboard;
        _showCrudSearch = showCrudSearch;
        _insertText = insertText;
        ShowSqlInsertPanel = insertText != null;
        LoadTables();
    }

    /// <summary>コールバックを後から設定する</summary>
    public void SetCallbacks(
        Action<string, string>? showCrudSearch = null,
        Action<string>? insertText = null)
    {
        _showCrudSearch = showCrudSearch;
        _insertText = insertText;
        ShowSqlInsertPanel = insertText != null;
    }

    partial void OnSelectedTableChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            LoadTableDefinition(value);
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadTables();
        if (SelectedTable != null)
        {
            LoadTableDefinition(SelectedTable);
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task CopyToClipboard()
    {
        if (_setClipboard != null)
            await _setClipboard(DdlScript);
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow();
    }

    // ── コンテキストメニュー（オリジナル frmTableDef.vb 相当） ─────────

    /// <summary>このテーブルにアクセスしている処理を検索</summary>
    [RelayCommand]
    private void SearchTableAccess()
    {
        if (string.IsNullOrEmpty(SelectedTable)) return;
        _showCrudSearch?.Invoke(SelectedTable, string.Empty);
    }

    /// <summary>このカラムにアクセスしている処理を検索</summary>
    [RelayCommand]
    private void SearchColumnAccess()
    {
        if (string.IsNullOrEmpty(SelectedTable) || SelectedColumn == null) return;
        _showCrudSearch?.Invoke(SelectedTable, SelectedColumn.PhysicalName);
    }

    // ── SQL挿入ボタン（オリジナル frmTableDef.vb の SQL文構築補助） ──

    /// <summary>テーブル名.カラム名 を挿入</summary>
    [RelayCommand]
    private void InsertTableColumn()
    {
        if (string.IsNullOrEmpty(SelectedTable) || SelectedColumn == null) return;
        AppendSqlText($"{SelectedTable}.{SelectedColumn.PhysicalName}");
    }

    /// <summary>テーブル名 を挿入</summary>
    [RelayCommand]
    private void InsertTable()
    {
        if (string.IsNullOrEmpty(SelectedTable)) return;
        AppendSqlText(SelectedTable);
    }

    /// <summary>改行を挿入</summary>
    [RelayCommand]
    private void InsertNewLine() => AppendSqlText(Environment.NewLine);

    /// <summary>カンマ + 改行 を挿入</summary>
    [RelayCommand]
    private void InsertComma() => AppendSqlText("," + Environment.NewLine);

    /// <summary>( を挿入</summary>
    [RelayCommand]
    private void InsertOpenParen() => AppendSqlText(" ( ");

    /// <summary>) を挿入</summary>
    [RelayCommand]
    private void InsertCloseParen() => AppendSqlText(" ) ");

    /// <summary>AND を挿入</summary>
    [RelayCommand]
    private void InsertAnd() => AppendSqlText(Environment.NewLine + "AND ");

    /// <summary>OR を挿入</summary>
    [RelayCommand]
    private void InsertOr() => AppendSqlText(Environment.NewLine + "OR  ");

    /// <summary>= を挿入</summary>
    [RelayCommand]
    private void InsertEquals() => AppendSqlText(" = ");

    /// <summary>SQL構築テキストに追加、または関連コントロールに挿入</summary>
    private void AppendSqlText(string text)
    {
        if (_insertText != null)
        {
            _insertText(text);
        }
        else
        {
            SqlBuildText += text;
        }
    }

    /// <summary>SQL構築テキストをクリア</summary>
    [RelayCommand]
    private void ClearSqlBuildText()
    {
        SqlBuildText = string.Empty;
    }

    /// <summary>SQL構築テキストをクリップボードにコピー</summary>
    [RelayCommand]
    private async System.Threading.Tasks.Task CopySqlBuildText()
    {
        if (_setClipboard != null && !string.IsNullOrEmpty(SqlBuildText))
            await _setClipboard(SqlBuildText);
    }

    private void LoadTables()
    {
        Tables.Clear();
        var tableDefinitions = GlobalState.Instance.TableDefinitions;
        foreach (var tableName in tableDefinitions.Keys)
        {
            Tables.Add(tableName);
        }
    }

    private void LoadTableDefinition(string tableName)
    {
        Columns.Clear();
        Indexes.Clear();
        ForeignKeys.Clear();
        DdlScript = string.Empty;

        var tableDefinitions = GlobalState.Instance.TableDefinitions;
        var tableNames = GlobalState.Instance.TableNames;

        if (!tableDefinitions.TryGetValue(tableName, out var tableDef))
            return;

        // カラム情報を設定
        foreach (var col in tableDef.Columns.Values)
        {
            Columns.Add(new ColumnDefinition
            {
                ColumnName = col.AttributeName,
                PhysicalName = col.ColumnName,
                DataType = col.DataType,
                Size = string.IsNullOrEmpty(col.Accuracy)
                    ? col.Digits
                    : $"{col.Digits},{col.Accuracy}",
                IsNullable = col.Required != "Yes",
                IsPrimaryKey = col.PrimaryKey == "Yes",
                DefaultValue = string.Empty,
                Description = col.AttributeName
            });
        }

        // DDLスクリプトを生成
        var logicalName = tableNames.TryGetValue(tableName, out var ln) ? ln : tableName;
        DdlScript = GenerateDdlScript(tableName, logicalName, tableDef);
    }

    private static string GenerateDdlScript(string tableName, string logicalName, CRUDExplorer.Core.Models.TableDefinition tableDef)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"-- {logicalName} ({tableName})");
        sb.AppendLine($"CREATE TABLE {tableName} (");

        var cols = new System.Collections.Generic.List<string>();
        var pkCols = new System.Collections.Generic.List<string>();

        foreach (var col in tableDef.Columns.Values)
        {
            var notNull = col.Required == "Yes" ? " NOT NULL" : string.Empty;
            string sizeStr;
            if (string.IsNullOrEmpty(col.Digits))
                sizeStr = string.Empty;
            else if (string.IsNullOrEmpty(col.Accuracy))
                sizeStr = $"({col.Digits})";
            else
                sizeStr = $"({col.Digits},{col.Accuracy})";
            cols.Add($"    {col.ColumnName} {col.DataType}{sizeStr}{notNull}");
            if (col.PrimaryKey == "Yes")
                pkCols.Add(col.ColumnName);
        }

        sb.AppendLine(string.Join(",\n", cols));

        if (pkCols.Count > 0)
        {
            sb.AppendLine($"    , PRIMARY KEY ({string.Join(", ", pkCols)})");
        }

        sb.AppendLine(");");
        return sb.ToString();
    }
}

public class ColumnDefinition
{
    public string ColumnName { get; set; } = string.Empty;
    public string PhysicalName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string DefaultValue { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class IndexDefinition
{
    public string IndexName { get; set; } = string.Empty;
    public string Columns { get; set; } = string.Empty;
    public bool IsUnique { get; set; }
    public bool IsClustered { get; set; }
}

public class ForeignKeyDefinition
{
    public string ForeignKeyName { get; set; } = string.Empty;
    public string Columns { get; set; } = string.Empty;
    public string ReferencedTable { get; set; } = string.Empty;
    public string ReferencedColumns { get; set; } = string.Empty;
}
