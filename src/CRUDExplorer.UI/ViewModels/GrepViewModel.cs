using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.ViewModels;

public partial class GrepViewModel : ViewModelBase
{
    private readonly Action _closeWindow;

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

    /// <summary>
    /// Grepの対象ファイル（AnalyzeQueryWindowから設定）
    /// </summary>
    public string CurrentFile { get; set; } = string.Empty;

    public GrepViewModel(Action? closeWindow = null)
    {
        _closeWindow = closeWindow ?? (() => { });
    }

    [RelayCommand]
    private void Search()
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(SearchPattern))
        {
            ResultSummary = "検索パターンを入力してください";
            return;
        }

        var queryList = GlobalState.Instance.QueryList;
        var programNames = GlobalState.Instance.ProgramNames;

        // 検索パターンを構築
        string regexPattern;
        try
        {
            if (UseRegex)
            {
                regexPattern = SearchPattern;
            }
            else
            {
                regexPattern = Regex.Escape(SearchPattern);
                if (WholeWord)
                    regexPattern = $@"\b{regexPattern}\b";
            }
        }
        catch
        {
            ResultSummary = "正規表現パターンが不正です";
            return;
        }

        var regexOptions = CaseSensitive
            ? RegexOptions.None
            : RegexOptions.IgnoreCase;

        // ファイルグループ別にクエリを検索
        var matchFileNames = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (SearchCurrentFile && !string.IsNullOrEmpty(CurrentFile))
            matchFileNames.Add(CurrentFile);

        foreach (var kvp in queryList)
        {
            var query = kvp.Value;

            // スコープフィルタ
            if (SearchCurrentFile && !matchFileNames.Contains(query.FileName))
                continue;
            if (SearchCurrentProgram && !string.IsNullOrEmpty(CurrentFile))
            {
                var currentProgramId = Path.GetFileNameWithoutExtension(CurrentFile);
                var queryProgramId = Path.GetFileNameWithoutExtension(query.FileName);
                if (!string.Equals(currentProgramId, queryProgramId, StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            // ファイルフィルタ
            if (!string.IsNullOrWhiteSpace(FileFilter)
                && !MatchesFileFilter(query.FileName, FileFilter.Split(';', StringSplitOptions.RemoveEmptyEntries)))
                continue;

            // クエリテキストを検索
            var text = query.QueryText;
            try
            {
                var matches = Regex.Matches(text, regexPattern, regexOptions);
                if (matches.Count > 0)
                {
                    // マッチ周辺のプレビューテキストを生成
                    var firstMatch = matches[0];
                    var start = Math.Max(0, firstMatch.Index - 20);
                    var length = Math.Min(text.Length - start, firstMatch.Length + 40);
                    var preview = text.Substring(start, length).Replace('\n', ' ').Replace('\r', ' ');

                    SearchResults.Add(new GrepResult
                    {
                        FileName = query.FileName,
                        FilePath = query.FileName,
                        LineNumber = query.LineNo,
                        PreviewText = preview,
                        MatchCount = matches.Count
                    });
                }
            }
            catch
            {
                // 検索エラーはスキップ
            }
        }

        var totalMatches = SearchResults.Sum(r => r.MatchCount);
        ResultSummary = $"検索結果: {SearchResults.Count}クエリ, {totalMatches}件のマッチ";
    }

    [RelayCommand]
    private void OpenInEditor()
    {
        if (SelectedResult == null) return;

        var settings = Settings.Load();
        var launcher = new ExternalEditorLauncher(settings);
        launcher.RunTextEditor(SelectedResult.FilePath, SelectedResult.LineNumber, SearchPattern);
    }

    [RelayCommand]
    private void AnalyzeQuery()
    {
        if (SelectedResult == null) return;
        OpenAnalyzeQueryWindow(SelectedResult, false);
    }

    [RelayCommand]
    private void AnalyzeQueryNew()
    {
        if (SelectedResult == null) return;
        OpenAnalyzeQueryWindow(SelectedResult, true);
    }

    private void OpenAnalyzeQueryWindow(GrepResult result, bool newWindow)
    {
        // クエリ解析ウィンドウを開く
        // 新規ウィンドウの場合は新しいViewModelで開く
        var vm = new AnalyzeQueryViewModel();
        vm.FileName = result.FileName;
        vm.LineNumber = result.LineNumber.ToString();
        vm.HighlightText1 = SearchPattern;

        // クエリをロード
        var queryFile = Path.Combine(GlobalState.Instance.LastAnalysisDestPath ?? string.Empty, "querys", result.FileName + ".query");
        if (File.Exists(queryFile))
        {
            try
            {
                var sqlAnalyzer = new CRUDExplorer.SqlParser.Analyzers.SqlAnalyzer();
                var lines = File.ReadAllLines(queryFile);
                Query? targetQuery = null;
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    var cols = line.Split('\t');
                    if (cols.Length > 1 && int.TryParse(cols[0], out var lineNo))
                    {
                        var query = sqlAnalyzer.AnalyzeSql(cols[1], result.FileName, lineNo);
                        vm.Queries.Add(query);
                        if (lineNo == result.LineNumber)
                            targetQuery = query;
                    }
                }
                if (targetQuery != null)
                    vm.SelectedQuery = targetQuery;
                else if (vm.Queries.Count > 0)
                    vm.SelectedQuery = vm.Queries[0];
            }
            catch
            {
                // クエリファイル読み込み失敗は無視し、空のViewModelで続行
                // ユーザーには結果サマリーで通知される
            }
        }

        // TODO: ウィンドウを開くにはWindowServiceをコンストラクタで注入する必要がある
        // 現時点ではViewModelの準備のみ行い、ステータスを更新する
        ResultSummary = $"クエリ解析を開始: {result.FileName}:{result.LineNumber}";
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow();
    }

    private static bool MatchesFileFilter(string fileName, string[] filters)
    {
        return filters.Any(f =>
        {
            var pattern = "^" + Regex.Escape(f.Trim('*', ' ')).Replace("\\*", ".*") + "$";
            return Regex.IsMatch(fileName, pattern, RegexOptions.IgnoreCase);
        });
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
