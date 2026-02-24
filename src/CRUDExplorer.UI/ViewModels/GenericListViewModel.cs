using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRUDExplorer.UI.ViewModels;

public partial class GenericListViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _windowTitle = "リスト選択";

    [ObservableProperty]
    private ObservableCollection<ListItemModel> _items = new();

    [ObservableProperty]
    private ObservableCollection<ListItemModel> _filteredItems = new();

    [ObservableProperty]
    private ListItemModel? _selectedItem;

    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private string _itemCountText = "0 件";

    [ObservableProperty]
    private SelectionMode _selectionMode = SelectionMode.Single;

    public GenericListViewModel()
    {
        // Initialize with empty list
    }

    public void SetItems(ObservableCollection<ListItemModel> items, string title = "リスト選択")
    {
        Items = items;
        WindowTitle = title;
        ApplyFilter();
    }

    partial void OnFilterTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            FilteredItems = new ObservableCollection<ListItemModel>(Items);
        }
        else
        {
            var filtered = Items.Where(item =>
                item.DisplayText.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
            FilteredItems = new ObservableCollection<ListItemModel>(filtered);
        }

        ItemCountText = $"{FilteredItems.Count} 件";
    }

    [RelayCommand]
    private void ClearFilter()
    {
        FilterText = string.Empty;
    }

    [RelayCommand]
    private void Select()
    {
        // TODO: Return selected item to parent window
        // - Set DialogResult
        // - Close window
    }

    [RelayCommand]
    private void Close()
    {
        // TODO: Close window without selection
    }
}

public class ListItemModel
{
    public string DisplayText { get; set; } = string.Empty;
    public object? Tag { get; set; }
}
