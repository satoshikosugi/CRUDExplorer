using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.ViewModels;

public partial class FileListViewModel : ViewModelBase
{
    private readonly Action _closeWindow;

    [ObservableProperty]
    private ObservableCollection<FileItem> _files = new();

    [ObservableProperty]
    private FileItem? _selectedFile;

    [ObservableProperty]
    private ObservableCollection<QueryItem> _queries = new();

    [ObservableProperty]
    private QueryItem? _selectedQuery;

    [ObservableProperty]
    private string _filterPattern = string.Empty;

    [ObservableProperty]
    private string _grepPattern = string.Empty;

    [ObservableProperty]
    private int _fileCount = 0;

    [ObservableProperty]
    private int _queryCount = 0;

    /// <summary>ソースパス（エディタ起動等で使用）</summary>
    public string SourcePath { get; set; } = string.Empty;

    public FileListViewModel(Action? closeWindow = null)
    {
        _closeWindow = closeWindow ?? (() => { });
        SourcePath = GlobalState.Instance.LastAnalysisDestPath;
        LoadFiles();
    }

    partial void OnSelectedFileChanged(FileItem? value)
    {
        LoadQueries();
    }

    partial void OnFilterPatternChanged(string value)
    {
        LoadFiles();
    }

    [RelayCommand]
    private void Grep()
    {
        LoadQueries();
    }

    [RelayCommand]
    private void Open()
    {
        if (SelectedQuery != null)
        {
            OpenQueryInEditor(SelectedQuery);
        }
        else if (SelectedFile != null)
        {
            var settings = Settings.Load();
            var launcher = new ExternalEditorLauncher(settings);
            var filePath = !string.IsNullOrEmpty(SourcePath)
                ? Path.Combine(SourcePath, SelectedFile.FilePath)
                : SelectedFile.FilePath;
            launcher.RunTextEditor(filePath);
        }
    }

    [RelayCommand]
    private void AnalyzeQuery()
    {
        if (SelectedQuery == null) return;

        // GlobalState に解析リクエストを設定
        GlobalState.Instance.AnalyzeQueryRequest = new AnalyzeQueryRequest
        {
            SourcePath = SourcePath,
            FileName = SelectedFile?.FilePath ?? string.Empty,
            LineNo = SelectedQuery.LineNo,
            TableName = string.Empty
        };
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow();
    }

    private void OpenQueryInEditor(QueryItem query)
    {
        var settings = Settings.Load();
        var launcher = new ExternalEditorLauncher(settings);
        var filePath = !string.IsNullOrEmpty(SourcePath) && SelectedFile != null
            ? Path.Combine(SourcePath, SelectedFile.FilePath)
            : SelectedFile?.FilePath ?? string.Empty;
        launcher.RunTextEditor(filePath, query.LineNo);
    }

    private void LoadFiles()
    {
        Files.Clear();

        var state = GlobalState.Instance;
        var programNames = state.ProgramNames;

        foreach (var kvp in state.Files)
        {
            var fileName = kvp.Value?.ToString() ?? kvp.Key;

            // フィルタ適用
            if (!string.IsNullOrWhiteSpace(FilterPattern)
                && !fileName.Contains(FilterPattern, StringComparison.OrdinalIgnoreCase))
                continue;

            var programId = Path.GetFileNameWithoutExtension(fileName);
            if (programNames.TryGetValue(programId, out var programName))
                programId = $"{programName}({programId})";

            Files.Add(new FileItem
            {
                FileName = programId,
                FilePath = fileName
            });
        }

        FileCount = Files.Count;

        // 最初のファイルを選択
        if (Files.Count > 0 && SelectedFile == null)
        {
            SelectedFile = Files[0];
        }
    }

    private void LoadQueries()
    {
        Queries.Clear();

        if (SelectedFile == null)
        {
            QueryCount = 0;
            return;
        }

        var state = GlobalState.Instance;
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(SelectedFile.FilePath);

        // QueryList から該当ファイルのクエリを抽出
        foreach (var kvp in state.QueryList)
        {
            var query = kvp.Value;
            var queryFileName = Path.GetFileNameWithoutExtension(query.FileName);

            if (!string.Equals(queryFileName, fileNameWithoutExt, StringComparison.OrdinalIgnoreCase))
                continue;

            // Grepパターンフィルタ
            if (!string.IsNullOrWhiteSpace(GrepPattern)
                && !query.QueryText.Contains(GrepPattern, StringComparison.OrdinalIgnoreCase))
                continue;

            var preview = query.QueryText.Length > 80
                ? query.QueryText.Substring(0, 80).Replace('\n', ' ').Replace('\r', ' ') + "..."
                : query.QueryText.Replace('\n', ' ').Replace('\r', ' ');

            // クエリの種類（SELECT/INSERT/UPDATE/DELETE）を取得
            var queryKind = query.QueryKind;
            if (string.IsNullOrEmpty(queryKind))
            {
                queryKind = query.QueryText.TrimStart().Split(' ').FirstOrDefault()?.ToUpper() ?? "SQL";
            }

            Queries.Add(new QueryItem
            {
                LineNo = query.LineNo,
                FuncName = queryKind,
                Preview = preview,
                Query = query
            });
        }

        // 行番号でソート
        var sorted = Queries.OrderBy(q => q.LineNo).ToList();
        Queries.Clear();
        foreach (var q in sorted)
            Queries.Add(q);

        QueryCount = Queries.Count;
    }
}

public class FileItem
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}

public class QueryItem
{
    public int LineNo { get; set; }
    public string FuncName { get; set; } = string.Empty;
    public string Preview { get; set; } = string.Empty;
    public Query? Query { get; set; }
}
