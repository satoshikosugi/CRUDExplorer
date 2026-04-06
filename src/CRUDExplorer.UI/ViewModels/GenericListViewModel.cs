using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRUDExplorer.UI.ViewModels;

public partial class GenericListViewModel : ViewModelBase
{
    private readonly Action _closeWindow;
    private readonly Action<ListItemModel?>? _onSelect;

    // ソート状態
    private string? _sortColumn;
    private bool _sortAscending = true;

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

    /// <summary>列定義（複数列表示対応用）</summary>
    public ObservableCollection<string> ColumnHeaders { get; } = new();

    /// <summary>クリップボード設定用コールバック</summary>
    public Func<string, System.Threading.Tasks.Task>? SetClipboard { get; set; }

    public GenericListViewModel(
        Action? closeWindow = null,
        Action<ListItemModel?>? onSelect = null)
    {
        _closeWindow = closeWindow ?? (() => { });
        _onSelect = onSelect;
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
        IEnumerable<ListItemModel> source = Items;

        if (!string.IsNullOrWhiteSpace(FilterText))
        {
            source = source.Where(item =>
                item.DisplayText.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
        }

        // ソート適用
        if (!string.IsNullOrEmpty(_sortColumn))
        {
            var colIdx = ColumnHeaders.IndexOf(_sortColumn);
            if (colIdx >= 0)
            {
                source = _sortAscending
                    ? source.OrderBy(i => GetColumnValue(i, colIdx))
                    : source.OrderByDescending(i => GetColumnValue(i, colIdx));
            }
        }

        FilteredItems = new ObservableCollection<ListItemModel>(source);
        ItemCountText = $"{FilteredItems.Count} 件";
    }

    /// <summary>
    /// 列クリック時のソートトグル処理（オリジナル frmList.vb の lstList_ColumnClick 相当）
    /// </summary>
    public void SortByColumn(string columnName)
    {
        if (_sortColumn == columnName)
        {
            _sortAscending = !_sortAscending;
        }
        else
        {
            _sortColumn = columnName;
            _sortAscending = true;
        }
        ApplyFilter();
    }

    private static string GetColumnValue(ListItemModel item, int colIdx)
    {
        if (item.ColumnValues != null && colIdx < item.ColumnValues.Length)
            return item.ColumnValues[colIdx];
        return item.DisplayText;
    }

    [RelayCommand]
    private void ClearFilter()
    {
        FilterText = string.Empty;
    }

    [RelayCommand]
    private void Select()
    {
        _onSelect?.Invoke(SelectedItem);
        _closeWindow();
    }

    [RelayCommand]
    private void Close()
    {
        _onSelect?.Invoke(null);
        _closeWindow();
    }

    // ── コンテキストメニュー ──────────────────────────────────────────

    [RelayCommand]
    private async System.Threading.Tasks.Task CopySelected()
    {
        if (SelectedItem == null || SetClipboard == null) return;
        await SetClipboard(SelectedItem.DisplayText);
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task CopyAll()
    {
        if (FilteredItems.Count == 0 || SetClipboard == null) return;
        var text = string.Join(Environment.NewLine, FilteredItems.Select(i => i.DisplayText));
        await SetClipboard(text);
    }
}

public class ListItemModel
{
    public string DisplayText { get; set; } = string.Empty;
    public string[]? ColumnValues { get; set; }
    public object? Tag { get; set; }
}
