using System;
using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.ViewModels;

public partial class TableDefinitionViewModel : ViewModelBase
{
    private readonly Action _closeWindow;
    private readonly Func<string, System.Threading.Tasks.Task>? _setClipboard;

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

    public TableDefinitionViewModel(
        Action? closeWindow = null,
        Func<string, System.Threading.Tasks.Task>? setClipboard = null)
    {
        _closeWindow = closeWindow ?? (() => { });
        _setClipboard = setClipboard;
        LoadTables();
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
