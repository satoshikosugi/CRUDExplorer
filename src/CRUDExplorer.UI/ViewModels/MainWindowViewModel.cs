using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
    private string _title = "CRUD Explorer";

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

    /// <summary>CRUD.tsv から読み込んだ全アイテム（フィルタ前）</summary>
    private List<CrudListItem> _allCrudListItems = new();

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
        Title = $"{Path.GetFileName(path)} - CRUD Explorer";
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
            Title = $"{Path.GetFileName(destPath)} - CRUD Explorer";
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
    private async Task ShowQueryAnalyzer()
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

    // ─── 追加メニューコマンド ─────────────────────────────────────────

    [RelayCommand]
    private void CopyMatrixToClipboard()
    {
        if (MatrixRows.Count == 0)
        {
            StatusMessage = "コピーするデータがありません";
            return;
        }

        var sb = new StringBuilder();

        // ヘッダ行1: プログラムID
        sb.Append('\t');
        foreach (var h in MatrixHeaders)
            sb.Append('\t').Append(h);
        sb.AppendLine();

        // ヘッダ行2: テーブル名, 論理名, プログラム名
        sb.Append("テーブル名\tエンティティ名");
        foreach (var h in MatrixHeaders)
        {
            GlobalState.Instance.ProgramNames.TryGetValue(h, out var pname);
            sb.Append('\t').Append(pname ?? h);
        }
        sb.AppendLine();

        // データ行
        foreach (var row in MatrixRows)
        {
            sb.Append(row.TableName).Append('\t').Append(row.LogicalName);
            foreach (var h in MatrixHeaders)
                sb.Append('\t').Append(row[h]);
            sb.AppendLine();
        }

        ClipboardText = sb.ToString();
        MatrixClipboardReady?.Invoke(this, EventArgs.Empty);
        StatusMessage = "マトリクスをクリップボードにコピーしました";
    }

    /// <summary>クリップボードにセットすべきテキスト（コードビハインドが実際にセット）</summary>
    public string ClipboardText { get; private set; } = string.Empty;

    /// <summary>クリップボードへのコピー準備完了イベント</summary>
    public event EventHandler? MatrixClipboardReady;

    [RelayCommand]
    private void OpenFolderWithoutCrud()
    {
        // CRUDは開かない: フォルダ選択のみでマトリクスはクリアする
        _ = OpenFolderWithoutCrudAsync();
    }

    private async Task OpenFolderWithoutCrudAsync()
    {
        var path = await _windowService.ShowFolderPickerAsync("ソースフォルダを選択");
        if (path == null) return;

        SourcePath = path;
        Title = $"{Path.GetFileName(path)} - CRUD Explorer";
        MatrixHeaders = Array.Empty<string>();
        MatrixRows.Clear();
        MatrixColumnsChanged?.Invoke(this, EventArgs.Empty);
        CrudListData.Clear();
        _allCrudListItems.Clear();
        StatusMessage = $"フォルダを選択しました: {path}（CRUD未読込）";
    }

    // ─── CRUD一覧アクションコマンド（下部ボタン用） ─────────────────

    [RelayCommand]
    private void RunTextEditor()
    {
        if (SelectedCrudItem is not CrudListItem item) return;

        var settings = Settings.Load();
        var launcher = new ExternalEditorLauncher(settings);
        var filePath = Path.Combine(SourcePath, item.FileName);
        launcher.RunTextEditor(filePath, item.LineNo, item.TableName);
    }

    [RelayCommand]
    private void AnalyzeSelectedQuery()
    {
        if (SelectedCrudItem is not CrudListItem item) return;
        OpenAnalyzeQueryWindow(item);
    }

    [RelayCommand]
    private void ShowSelectedTableDef()
    {
        if (SelectedCrudItem is not CrudListItem item) return;
        if (string.IsNullOrEmpty(item.TableName)) return;

        var win = new TableDefinitionWindow();
        if (win.DataContext is TableDefinitionViewModel vm)
            vm.SelectedTable = item.TableName;
        // NOTE: MainWindow参照はWindowServiceが持つ
        _windowService.ShowWindow<TableDefinitionWindow>();
    }

    /// <summary>
    /// CRUD一覧のダブルクリック処理（設定に応じてエディタ起動/クエリ解析を切り替え）
    /// </summary>
    public void HandleCrudListDoubleClick()
    {
        if (SelectedCrudItem is not CrudListItem item) return;

        var settings = Settings.Load();
        switch (settings.DoubleClickMode)
        {
            case Settings.ListDoubleClickMode.ExecTextEditor:
                var launcher = new ExternalEditorLauncher(settings);
                var filePath = Path.Combine(SourcePath, item.FileName);
                launcher.RunTextEditor(filePath, item.LineNo, item.TableName);
                StatusMessage = $"エディタ起動: {item.FileName}:{item.LineNo}";
                break;
            case Settings.ListDoubleClickMode.AnalyzeQuery:
                OpenAnalyzeQueryWindow(item);
                break;
            case Settings.ListDoubleClickMode.NoAction:
                break;
        }
    }

    private void OpenAnalyzeQueryWindow(CrudListItem item)
    {
        var vm = new AnalyzeQueryViewModel();
        // クエリファイルを読み込んで全クエリをドロップダウンに表示
        var queryFile = Path.Combine(SourcePath, "querys", item.FileName + ".query");
        if (File.Exists(queryFile))
        {
            try
            {
                var lines = File.ReadAllLines(queryFile);
                Query? targetQuery = null;
                var sqlAnalyzer = new SqlParser.Analyzers.SqlAnalyzer();
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    var cols = line.Split('\t');
                    if (cols.Length > 1 && int.TryParse(cols[0], out var lineNo))
                    {
                        var query = sqlAnalyzer.AnalyzeSql(cols[1], item.FileName, lineNo);
                        vm.Queries.Add(query);
                        if (lineNo == item.LineNo)
                            targetQuery = query;
                    }
                }
                if (targetQuery != null)
                    vm.SelectedQuery = targetQuery;
                else if (vm.Queries.Count > 0)
                    vm.SelectedQuery = vm.Queries[0];
            }
            catch { /* クエリファイル読み込み失敗 */ }
        }

        vm.FileName = item.FileName;
        vm.LineNumber = item.LineNo.ToString();
        vm.HighlightText1 = item.TableName;
        _windowService.ShowWindow<AnalyzeQueryWindow>(vm);
        StatusMessage = $"クエリ解析: {item.FileName}:{item.LineNo} {item.TableName}";
    }

    /// <summary>
    /// マトリクス上で選択されたテーブルのテーブル定義を表示
    /// </summary>
    public void ShowTableDefForSelectedRow(string? tableName)
    {
        if (string.IsNullOrEmpty(tableName)) return;

        var win = new TableDefinitionWindow();
        if (win.DataContext is TableDefinitionViewModel vm)
            vm.SelectedTable = tableName;
        _windowService.ShowWindow<TableDefinitionWindow>();
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
        _allCrudListItems.Clear();
        CrudListData.Clear();
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
            _allCrudListItems.Add(item);
        }
        // 初期状態: 全件表示（セル選択でフィルタされる）
    }

    // ─── マトリクスセル選択による CRUD 一覧フィルタ ───────────────────

    /// <summary>
    /// マトリクスのセル選択に応じて CRUD 一覧をフィルタする。
    /// originalの仕様:
    ///   - セルクリック: テーブル名＋プログラムIDの両方で絞り込み
    ///   - 行ヘッダクリック（programId=null）: テーブル名のみで絞り込み
    ///   - 列ヘッダクリック（tableName=null）: プログラムIDのみで絞り込み
    ///   - 両方null: 一覧クリア
    /// </summary>
    public void FilterCrudListBySelection(string? tableName, string? programId)
    {
        CrudListData.Clear();

        if (tableName == null && programId == null)
        {
            StatusMessage = $"選択: なし";
            return;
        }

        var filtered = _allCrudListItems.Where(item =>
        {
            if (tableName != null && !item.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase))
                return false;
            if (programId != null && !item.FileName.Equals(programId, StringComparison.OrdinalIgnoreCase))
                return false;
            return true;
        });

        foreach (var item in filtered)
            CrudListData.Add(item);

        StatusMessage = $"選択: {tableName ?? "(全テーブル)"} [{programId ?? "(全プログラム)"}] {CrudListData.Count} 件";
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


