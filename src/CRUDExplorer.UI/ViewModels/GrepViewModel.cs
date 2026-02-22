using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRUDExplorer.UI.ViewModels;

public partial class GrepViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _searchPattern = string.Empty;

    [ObservableProperty]
    private bool _useRegex = false;

    [ObservableProperty]
    private bool _caseSensitive = false;

    [ObservableProperty]
    private bool _wholeWord = false;

    [ObservableProperty]
    private bool _searchCurrentFile = true;

    [ObservableProperty]
    private bool _searchCurrentProgram = false;

    [ObservableProperty]
    private bool _searchAllFiles = false;

    [ObservableProperty]
    private string _fileFilter = "*.cs;*.vb;*.sql";

    [ObservableProperty]
    private ObservableCollection<GrepResult> _searchResults = new();

    [ObservableProperty]
    private GrepResult? _selectedResult;

    [ObservableProperty]
    private string _resultSummary = "検索結果: 0ファイル, 0件のマッチ";

    [RelayCommand]
    private void Search()
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(SearchPattern))
        {
            ResultSummary = "検索パターンを入力してください";
            return;
        }

        // TODO: Implement actual grep search
        // - Determine search scope (current file/program/all files)
        // - Apply file filter
        // - Search using regex or plain text
        // - Populate SearchResults with matches

        var totalMatches = SearchResults.Sum(r => r.MatchCount);
        ResultSummary = $"検索結果: {SearchResults.Count}ファイル, {totalMatches}件のマッチ";
    }

    [RelayCommand]
    private void OpenInEditor()
    {
        if (SelectedResult == null) return;

        // TODO: Open file in external editor
        // - Use ExternalEditorLauncher
        // - Jump to line number if supported
    }

    [RelayCommand]
    private void Close()
    {
        // TODO: Close window
    }
}

public class GrepResult
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string PreviewText { get; set; } = string.Empty;
    public int MatchCount { get; set; }
}
