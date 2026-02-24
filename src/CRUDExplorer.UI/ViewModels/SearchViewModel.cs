using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRUDExplorer.UI.ViewModels;

public partial class SearchViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _matchCase = false;

    [ObservableProperty]
    private bool _matchWholeWord = false;

    [ObservableProperty]
    private bool _useRegularExpression = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private IBrush _statusColor = Brushes.Black;

    [RelayCommand]
    private void FindNext()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            StatusMessage = "検索文字列を入力してください";
            StatusColor = Brushes.Red;
            return;
        }

        // TODO: Implement find next
        // - Search forward from current position
        // - Highlight match
        // - Update status

        StatusMessage = "検索中...";
        StatusColor = Brushes.Blue;
    }

    [RelayCommand]
    private void FindPrevious()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            StatusMessage = "検索文字列を入力してください";
            StatusColor = Brushes.Red;
            return;
        }

        // TODO: Implement find previous
        // - Search backward from current position
        // - Highlight match
        // - Update status

        StatusMessage = "検索中...";
        StatusColor = Brushes.Blue;
    }

    [RelayCommand]
    private void Close()
    {
        // TODO: Close window
    }
}
