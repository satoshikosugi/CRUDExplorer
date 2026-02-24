using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.UI.ViewModels;

public partial class TableDefinitionViewModel : ViewModelBase
{
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

    public TableDefinitionViewModel()
    {
        // TODO: Load tables from GlobalState or database
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
        // TODO: Refresh table list and reload current table
        if (SelectedTable != null)
        {
            LoadTableDefinition(SelectedTable);
        }
    }

    [RelayCommand]
    private void CopyToClipboard()
    {
        // TODO: Copy DDL script to clipboard
    }

    [RelayCommand]
    private void Close()
    {
        // TODO: Close window
    }

    private void LoadTableDefinition(string tableName)
    {
        Columns.Clear();
        Indexes.Clear();
        ForeignKeys.Clear();
        DdlScript = string.Empty;

        // TODO: Load table definition from database or GlobalState
        // - Populate Columns from TableDefinition
        // - Populate Indexes
        // - Populate ForeignKeys
        // - Generate DDL script
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
