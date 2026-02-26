using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;
using CRUDExplorer.UI.Services;
using CRUDExplorer.UI.Views;

namespace CRUDExplorer.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IWindowService _windowService;

    // すべての CRUD 一覧（フィルタ前の全件）
    private readonly List<CrudListItem> _allCrudItems = new();

    // CRUD辞書: "PROGRAMID:TABLE" → CrudListItem のリスト（セル選択時の高速絞り込み用）
    private readonly Dictionary<string, List<CrudListItem>> _crudDict =
        new(StringComparer.OrdinalIgnoreCase);

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "フォルダを選択してCRUDマトリクスを表示するか、「CRUD解析を実行」で解析を行ってください";

    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private string _filterProgram = string.Empty;

    [ObservableProperty]
    private string _filterTable = string.Empty;

    [ObservableProperty]
    private bool _filterC = true;

    [ObservableProperty]
    private bool _filterR = true;

    [ObservableProperty]
    private bool _filterU = true;

    [ObservableProperty]
    private bool _filterD = true;

    [ObservableProperty]
    private int _crudViewType = 0; // 0: TableCRUD, 1: ColumnCRUD

    partial void OnCrudViewTypeChanged(int value)
    {
        if (!string.IsNullOrEmpty(SourcePath))
            _ = LoadCrudDataAsync();
    }

    [ObservableProperty]
    private ObservableCollection<CrudListItem> _crudListData = new();

    [ObservableProperty]
    private object? _selectedCrudItem;

    /// <summary>DataGridのヘッダ列（プログラムID一覧）</summary>
    public string[] MatrixHeaders { get; private set; } = Array.Empty<string>();

    /// <summary>DataGridの行データ</summary>
    public ObservableCollection<CrudMatrixRow> MatrixRows { get; } = new();

    /// <summary>MainWindow.axaml.cs がDataGrid列を再構築するために購読するイベント</summary>
    public event EventHandler? MatrixColumnsChanged;

    public MainWindowViewModel(IWindowService windowService)
    {
        _windowService = windowService;
    }

    // ─── ファイル メニュー ───────────────────────────────────────────

    [RelayCommand]
    private async Task OpenFolder()
    {
        var path = await _windowService.ShowFolderPickerAsync("CRUD解析結果フォルダを選択");
        if (path == null) return;

        SourcePath = path;
        GlobalState.Instance.LoadFromFolder(path);
        await LoadCrudDataAsync();
    }

    [RelayCommand]
    private void Exit() => Environment.Exit(0);

    // ─── 解析 メニュー ───────────────────────────────────────────────

    [RelayCommand]
    private async Task AnalyzeCrud()
    {
        await _windowService.ShowDialog<MakeCrudWindow>();

        var destPath = GlobalState.Instance.LastAnalysisDestPath;
        if (!string.IsNullOrEmpty(destPath) && Directory.Exists(destPath))
        {
            SourcePath = destPath;
            GlobalState.Instance.LoadFromFolder(destPath);
            await LoadCrudDataAsync();
        }
    }

    [RelayCommand]
    private async Task LoadTableDef()
    {
        if (string.IsNullOrEmpty(SourcePath))
        {
            StatusMessage = "先にフォルダを選択してください";
            return;
        }

        await _windowService.ShowDialog<TableDefinitionWindow>();
    }

    // ─── 表示 メニュー ───────────────────────────────────────────────

    [RelayCommand]
    private async Task ShowTableCrud()
    {
        CrudViewType = 0;
        if (!string.IsNullOrEmpty(SourcePath))
            await LoadCrudDataAsync();
    }

    [RelayCommand]
    private async Task ShowColumnCrud()
    {
        CrudViewType = 1;
        if (!string.IsNullOrEmpty(SourcePath))
            await LoadCrudDataAsync();
    }

    [RelayCommand]
    private async Task ShowFilter()
    {
        GlobalState.Instance.FilterState.WasApplied = false;
        await _windowService.ShowDialog<FilterWindow>();
        // FilterViewModel.Apply() が GlobalState.FilterState を書き込む
        var fs = GlobalState.Instance.FilterState;
        if (fs.WasApplied)
        {
            FilterProgram = fs.ProgramFilter;
            FilterTable   = fs.TableFilter;
            FilterC = fs.ShowC;
            FilterR = fs.ShowR;
            FilterU = fs.ShowU;
            FilterD = fs.ShowD;
            if (!string.IsNullOrEmpty(SourcePath))
                await LoadCrudDataAsync();
        }
    }

    [RelayCommand]
    private async Task ApplyFilter()
    {
        if (string.IsNullOrEmpty(SourcePath))
        {
            StatusMessage = "先にフォルダを選択してください";
            return;
        }
        await LoadCrudDataAsync();
    }

    [RelayCommand]
    private async Task ClearFilter()
    {
        FilterProgram = string.Empty;
        FilterTable   = string.Empty;
        FilterC = FilterR = FilterU = FilterD = true;
        if (!string.IsNullOrEmpty(SourcePath))
            await LoadCrudDataAsync();
    }

    [RelayCommand]
    private async Task ShowFileList()
    {
        if (string.IsNullOrEmpty(SourcePath))
        {
            StatusMessage = "先にフォルダを選択してください";
            return;
        }
        await _windowService.ShowDialog<FileListWindow>();
    }

    [RelayCommand]
    private async Task ShowQueryAnalyze()
    {
        await _windowService.ShowDialog<AnalyzeQueryWindow>();
    }

    [RelayCommand]
    private void OpenSupportSite()
    {
        try { Process.Start(new ProcessStartInfo("http://crudexplorer.ks-serv.com") { UseShellExecute = true }); }
        catch (Exception ex) { StatusMessage = $"ブラウザの起動に失敗しました: {ex.Message}"; }
    }

    // ─── 設定・ヘルプ メニュー ───────────────────────────────────────

    [RelayCommand]
    private async Task OpenSettings()
    {
        await _windowService.ShowDialog<SettingsWindow>();
    }

    [RelayCommand]
    private async Task ShowVersion()
    {
        await _windowService.ShowDialog<VersionWindow>();
    }

    // ─── ツールバー: エディタ起動 / クエリ解析 ───────────────────────

    /// <summary>CRUD一覧で選択中のアイテムをテキストエディタで開く</summary>
    [RelayCommand]
    private void OpenEditor()
    {
        OpenEditorForSelectedItem();
    }

    public void OpenEditorForSelectedItem()
    {
        if (SelectedCrudItem is not CrudListItem item) return;
        if (string.IsNullOrEmpty(SourcePath)) return;

        var filePath = Path.Combine(SourcePath, item.FileName);
        var launcher = new ExternalEditorLauncher(GlobalState.Instance.AppSettings);
        launcher.RunTextEditor(filePath, item.LineNo, item.FuncName);
    }

    /// <summary>CRUD一覧で選択中のアイテムをクエリ解析ウィンドウで開く</summary>
    [RelayCommand]
    private async Task AnalyzeQueryForSelected()
    {
        if (SelectedCrudItem is not CrudListItem item) return;

        GlobalState.Instance.AnalyzeQueryRequest = new AnalyzeQueryRequest
        {
            SourcePath = SourcePath,
            FileName   = item.FileName,
            LineNo     = item.LineNo,
            TableName  = item.TableName,
            FuncName   = item.FuncName,
        };
        await _windowService.ShowDialog<AnalyzeQueryWindow>();
    }

    // ─── マトリクス クリップボードコピー ───────────────────────────

    [RelayCommand]
    private void CopyMatrixToClipboard()
    {
        if (MatrixRows.Count == 0) return;
        var sb = new System.Text.StringBuilder();
        // ヘッダ行
        sb.Append("テーブル名\t論理名\t合計");
        foreach (var h in MatrixHeaders) sb.Append('\t').Append(h);
        sb.AppendLine();
        // データ行
        foreach (var row in MatrixRows)
        {
            sb.Append(row.TableName).Append('\t').Append(row.LogicalName).Append('\t').Append(row.Total);
            foreach (var v in row.CellValues) sb.Append('\t').Append(v);
            sb.AppendLine();
        }
        try
        {
            // クリップボードはウィンドウ側から設定する（IWindowServiceに委譲）
            _ = _windowService.SetClipboardTextAsync(sb.ToString());
            StatusMessage = "マトリクスをクリップボードにコピーしました";
        }
        catch { StatusMessage = "クリップボードへのコピーに失敗しました"; }
    }

    // ─── CRUD 一覧フィルタリング（DataGrid セル選択連動）────────────

    /// <summary>全件表示（フィルタなし）</summary>
    public void ClearCrudListFilter()
    {
        CrudListData.Clear();
        foreach (var item in _allCrudItems)
            CrudListData.Add(item);
    }

    /// <summary>セルクリック: テーブル × プログラムID の組み合わせを表示</summary>
    public void FilterCrudList(string tableName, string programId)
    {
        var key = $"{programId}:{tableName}";
        CrudListData.Clear();
        if (_crudDict.TryGetValue(key, out var items))
            foreach (var item in items) CrudListData.Add(item);

        StatusMessage = $"{tableName} × {programId}：{CrudListData.Count} 件";
    }

    /// <summary>行クリック: そのテーブルを参照している全プログラムを表示</summary>
    public void FilterCrudListByTable(string tableName)
    {
        CrudListData.Clear();
        foreach (var item in _allCrudItems)
            if (string.Equals(item.TableName, tableName, StringComparison.OrdinalIgnoreCase))
                CrudListData.Add(item);

        StatusMessage = $"テーブル: {tableName}：{CrudListData.Count} 件";
    }

    /// <summary>列クリック: そのプログラムIDが参照している全テーブルを表示</summary>
    public void FilterCrudListByProgram(string programId)
    {
        CrudListData.Clear();
        foreach (var item in _allCrudItems)
            if (string.Equals(item.ProgramId, programId, StringComparison.OrdinalIgnoreCase))
                CrudListData.Add(item);

        StatusMessage = $"プログラム: {programId}：{CrudListData.Count} 件";
    }

    // ─── CRUD データ読み込み ─────────────────────────────────────────

    private async Task LoadCrudDataAsync()
    {
        if (string.IsNullOrEmpty(SourcePath)) return;

        var querysDir = Path.Combine(SourcePath, "querys");
        if (!Directory.Exists(querysDir))
        {
            StatusMessage = $"querysフォルダが見つかりません: {querysDir}";
            return;
        }

        StatusMessage = "CRUD データを読み込み中...";

        string matrixFile, crudFile;
        if (CrudViewType == 0)
        {
            matrixFile = Path.Combine(querysDir, "CRUDMatrix.tsv");
            crudFile   = Path.Combine(querysDir, "CRUD.tsv");
        }
        else
        {
            matrixFile = Path.Combine(querysDir, "CRUDColumnsMatrix.tsv");
            crudFile   = Path.Combine(querysDir, "CRUDColumns.tsv");
        }

        if (File.Exists(matrixFile))
        {
            try { await LoadMatrixAsync(matrixFile); }
            catch (Exception ex) { StatusMessage = $"マトリクス読み込みエラー: {ex.Message}"; return; }
        }
        else
        {
            MatrixHeaders = Array.Empty<string>();
            MatrixRows.Clear();
            MatrixColumnsChanged?.Invoke(this, EventArgs.Empty);
        }

        if (File.Exists(crudFile))
        {
            try { LoadCrudList(crudFile); }
            catch (Exception ex) { StatusMessage = $"CRUD一覧読み込みエラー: {ex.Message}"; return; }
        }
        else
        {
            _allCrudItems.Clear();
            _crudDict.Clear();
            CrudListData.Clear();
        }

        var viewLabel = CrudViewType == 0 ? "テーブル" : "カラム";
        StatusMessage = $"{viewLabel}CRUDマトリクス読み込み完了：{MatrixRows.Count} 行 / {MatrixHeaders.Length} プログラム";
    }

    /// <summary>
    /// CRUDMatrix.tsv を解析してマトリクスを更新する。
    /// フィルタ（FilterProgram / FilterTable / FilterC~D）を適用する。
    /// 行0=ヘッダ(col3+=プログラムID), 行1=集計(スキップ), 行2+=データ行
    /// </summary>
    private Task LoadMatrixAsync(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        string[] programHeaders = Array.Empty<string>();
        bool[]   colShow        = Array.Empty<bool>();
        var newRows = new List<CrudMatrixRow>();

        // nonEmptyLineIndex は空行をスキップした後の行番号（0=ヘッダ、1=集計、2+=データ）
        int nonEmptyLineIndex = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line)) continue;
            var cols = line.Split('\t');

            if (nonEmptyLineIndex == 0)
            {
                // ヘッダ行: col3以降がプログラムID。FilterProgramでフィルタリング。
                var allHeaders = cols.Length > 3 ? cols[3..] : Array.Empty<string>();
                colShow = new bool[allHeaders.Length];
                var filteredHeaders = new List<string>();
                for (int j = 0; j < allHeaders.Length; j++)
                {
                    bool show = string.IsNullOrEmpty(FilterProgram) ||
                                System.Text.RegularExpressions.Regex.IsMatch(
                                    allHeaders[j], FilterProgram,
                                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    colShow[j] = show;
                    if (show) filteredHeaders.Add(allHeaders[j]);
                }
                programHeaders = filteredHeaders.ToArray();
            }
            else if (nonEmptyLineIndex == 1)
            {
                // 集計行スキップ
            }
            else
            {
                // データ行。FilterTableでフィルタリング。
                var tableKey = cols.Length > 0 ? cols[0] : string.Empty;
                if (!string.IsNullOrEmpty(FilterTable) &&
                    !System.Text.RegularExpressions.Regex.IsMatch(
                        tableKey, FilterTable,
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    nonEmptyLineIndex++;
                    continue;
                }

                var row = new CrudMatrixRow
                {
                    TableName   = tableKey,
                    LogicalName = cols.Length > 1 ? cols[1] : string.Empty,
                    Total       = cols.Length > 2 ? cols[2] : string.Empty
                };

                // FilterC/R/U/D を合計列に適用
                if (!string.IsNullOrEmpty(row.Total))
                {
                    var filteredTotal = FilterCrudString(row.Total);
                    if (string.IsNullOrEmpty(filteredTotal)) { nonEmptyLineIndex++; continue; }
                    row.Total = filteredTotal;
                }

                // 表示する列だけを取り出して CellValues に格納
                const int RawColBase = 3; // col0=テーブル名, col1=論理名, col2=合計, col3+=プログラム
                var cellValues = new List<string>(programHeaders.Length);
                int visibleIdx = 0;
                for (int j = 0; j < colShow.Length; j++)
                {
                    if (!colShow[j]) continue;
                    var rawIdx = RawColBase + j;
                    var value  = cols.Length > rawIdx ? cols[rawIdx] : string.Empty;
                    var fv     = FilterCrudString(value);
                    cellValues.Add(fv);
                    row.Values[programHeaders[visibleIdx]] = fv;
                    visibleIdx++;
                }
                row.CellValues = cellValues.ToArray();

                // 全セルが空なら行をスキップ（オリジナルと同じ「空行削除」）
                if (row.CellValues.All(string.IsNullOrEmpty)) { nonEmptyLineIndex++; continue; }

                newRows.Add(row);
            }
            nonEmptyLineIndex++;
        }

        MatrixHeaders = programHeaders;
        MatrixRows.Clear();
        foreach (var r in newRows) MatrixRows.Add(r);
        MatrixColumnsChanged?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    /// <summary>FilterC/R/U/D に基づいて CRUD 文字列をフィルタリングする</summary>
    private string FilterCrudString(string crud)
    {
        if (string.IsNullOrEmpty(crud)) return crud;
        var result = crud;
        if (!FilterC) result = result.Replace("C", "");
        if (!FilterR) result = result.Replace("R", "");
        if (!FilterU) result = result.Replace("U", "");
        if (!FilterD) result = result.Replace("D", "");
        return result;
    }

    /// <summary>
    /// CRUD.tsv を読み込んでリスト表示用データと検索辞書を更新する。
    /// 形式: col0=ファイル, col1=プログラムID, col2=行番号, col3=テーブル名, col4=CRUD, col5=関数名, col6=論理名
    /// </summary>
    private void LoadCrudList(string filePath)
    {
        _allCrudItems.Clear();
        _crudDict.Clear();

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrEmpty(line)) continue;
            var cols = line.Split('\t');
            if (cols.Length < 5) continue;

            var item = new CrudListItem
            {
                FileName    = cols.Length > 0 ? cols[0] : string.Empty,
                ProgramId   = cols.Length > 1 ? cols[1] : string.Empty,
                LineNo      = int.TryParse(cols.Length > 2 ? cols[2] : "0", out var ln) ? ln : 0,
                TableName   = cols.Length > 3 ? cols[3] : string.Empty,
                Crud        = cols.Length > 4 ? cols[4] : string.Empty,
                FuncName    = cols.Length > 5 ? cols[5] : string.Empty,
                LogicalName = cols.Length > 6 ? cols[6] : string.Empty,
            };
            item.DisplayText = $"{item.TableName}\t{item.Crud}\t{item.ProgramId}  ({item.FileName}:{item.LineNo})";

            _allCrudItems.Add(item);

            // 辞書に登録（"PROGRAMID:TABLENAME" → items list）
            var key = $"{item.ProgramId}:{item.TableName}";
            if (!_crudDict.TryGetValue(key, out var lst))
            {
                lst = new List<CrudListItem>();
                _crudDict[key] = lst;
            }
            lst.Add(item);
        }

        CrudListData.Clear();
        foreach (var item in _allCrudItems) CrudListData.Add(item);
    }
}

/// <summary>CRUD一覧の1アイテム</summary>
public class CrudListItem
{
    public string DisplayText  { get; set; } = string.Empty;
    public string FileName     { get; set; } = string.Empty;
    public string ProgramId    { get; set; } = string.Empty;
    public int    LineNo       { get; set; }
    public string TableName    { get; set; } = string.Empty;
    public string Crud         { get; set; } = string.Empty;
    public string FuncName     { get; set; } = string.Empty;
    public string LogicalName  { get; set; } = string.Empty;
    public Query? Query        { get; set; }
}

/// <summary>クエリ解析ウィンドウへ渡すリクエスト情報</summary>
public class AnalyzeQueryRequest
{
    public string SourcePath { get; set; } = string.Empty;
    public string FileName   { get; set; } = string.Empty;
    public int    LineNo     { get; set; }
    public string TableName  { get; set; } = string.Empty;
    public string FuncName   { get; set; } = string.Empty;
}


