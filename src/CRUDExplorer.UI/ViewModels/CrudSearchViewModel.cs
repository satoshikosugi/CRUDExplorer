using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRUDExplorer.UI.ViewModels;

public partial class CrudSearchViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _tableNamePattern = string.Empty;

    [ObservableProperty]
    private string _columnNamePattern = string.Empty;

    [ObservableProperty]
    private bool _searchCreate = true;

    [ObservableProperty]
    private bool _searchRead = true;

    [ObservableProperty]
    private bool _searchUpdate = true;

    [ObservableProperty]
    private bool _searchDelete = true;

    [ObservableProperty]
    private ObservableCollection<CrudSearchResult> _searchResults = new();

    [ObservableProperty]
    private int _resultCount = 0;

    [RelayCommand]
    private void Search()
    {
        SearchResults.Clear();

        // TODO: Implement actual CRUD search
        // - Search through GlobalState.Queries
        // - Filter by table name pattern
        // - Filter by column name pattern
        // - Filter by CRUD type (C/R/U/D)
        // - Populate SearchResults

        ResultCount = SearchResults.Count;
    }

    [RelayCommand]
    private void Close()
    {
        // TODO: Close window
    }
}

public class CrudSearchResult
{
    public string DisplayText { get; set; } = string.Empty;
    public string DetailText { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int LineNo { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string CrudType { get; set; } = string.Empty;
}
