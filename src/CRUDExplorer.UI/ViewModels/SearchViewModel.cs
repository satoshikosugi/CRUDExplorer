using System;
using System.Text.RegularExpressions;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRUDExplorer.UI.ViewModels;

public partial class SearchViewModel : ViewModelBase
{
    private readonly Action _closeWindow;
    private readonly Action<string, bool, bool, bool>? _onFindNext;
    private readonly Action<string, bool, bool, bool>? _onFindPrevious;

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

    public SearchViewModel(
        Action? closeWindow = null,
        Action<string, bool, bool, bool>? onFindNext = null,
        Action<string, bool, bool, bool>? onFindPrevious = null)
    {
        _closeWindow = closeWindow ?? (() => { });
        _onFindNext = onFindNext;
        _onFindPrevious = onFindPrevious;
    }

    [RelayCommand]
    private void FindNext()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            StatusMessage = "検索文字列を入力してください";
            StatusColor = Brushes.Red;
            return;
        }

        try
        {
            if (UseRegularExpression)
                _ = new Regex(SearchText); // パターン検証
        }
        catch
        {
            StatusMessage = "正規表現パターンが不正です";
            StatusColor = Brushes.Red;
            return;
        }

        _onFindNext?.Invoke(SearchText, MatchCase, MatchWholeWord, UseRegularExpression);
        StatusMessage = $"「{SearchText}」を次方向に検索";
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

        try
        {
            if (UseRegularExpression)
                _ = new Regex(SearchText); // パターン検証
        }
        catch
        {
            StatusMessage = "正規表現パターンが不正です";
            StatusColor = Brushes.Red;
            return;
        }

        _onFindPrevious?.Invoke(SearchText, MatchCase, MatchWholeWord, UseRegularExpression);
        StatusMessage = $"「{SearchText}」を前方向に検索";
        StatusColor = Brushes.Blue;
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow();
    }
}
