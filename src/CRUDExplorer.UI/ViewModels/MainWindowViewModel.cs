using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _title = "CRUD Explorer";

    [ObservableProperty]
    private string _statusMessage = "フォルダを選択してCRUDマトリクスを表示するか、「CRUD解析を実行」で解析を行ってください";

    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private int _crudViewType = 0; // 0: TableCRUD, 1: ColumnCRUD

    partial void OnCrudViewTypeChanged(int value)
    {
        // ComboBox でビュー種別が変更されたら自動的にデータを再読み込み
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
        // CRUD解析実行ウィンドウを開く
        await _windowService.ShowDialog<MakeCrudWindow>();

        // 解析完了後、出力フォルダを自動的に読み込む
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

        // querys/tabledef.tsv を自動検索、なければファイルピッカー
        var defaultPath = Path.Combine(SourcePath, "querys", "tabledef.tsv");
        string? filePath;

        if (File.Exists(defaultPath))
        {
            filePath = defaultPath;
        }
        else
        {
            filePath = await _windowService.ShowFilePickerAsync(
                "テーブル定義TSVファイルを選択", new[] { "tsv", "txt" });
        }

        if (filePath == null) return;

        StatusMessage = "テーブル定義を読み込み中...";
        try
        {
            var state = GlobalState.Instance;
            state.TableDefinitions.Clear();
            FileSystemHelper.ReadTableDef(filePath, state.TableDefinitions);

            var tableNamePath = Path.Combine(Path.GetDirectoryName(filePath)!, "tablename.tsv");
            if (File.Exists(tableNamePath))
                state.TableNames = FileSystemHelper.ReadDictionary(tableNamePath);

            StatusMessage = $"テーブル定義読み込み完了：{state.TableDefinitions.Count} テーブル";
        }
        catch (Exception ex)
        {
            StatusMessage = $"テーブル定義の読み込みに失敗しました: {ex.Message}";
        }
    }

    // ─── 表示 メニュー ───────────────────────────────────────────────

    [RelayCommand]
    private async Task ShowTableCrud()
    {
        CrudViewType = 0;
        if (!string.IsNullOrEmpty(SourcePath))
            await LoadCrudDataAsync();
        else
            StatusMessage = "テーブルCRUDビューを選択しました";
    }

    [RelayCommand]
    private async Task ShowColumnCrud()
    {
        CrudViewType = 1;
        if (!string.IsNullOrEmpty(SourcePath))
            await LoadCrudDataAsync();
        else
            StatusMessage = "カラムCRUDビューを選択しました";
    }

    [RelayCommand]
    private async Task ShowFilter()
    {
        await _windowService.ShowDialog<FilterWindow>();
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
    private void ShowFileList()
    {
        if (string.IsNullOrEmpty(SourcePath))
        {
            StatusMessage = "先にフォルダを選択してください";
            return;
        }
        _windowService.ShowWindow<FileListWindow>();
    }

    [RelayCommand]
    private void ShowQueryAnalyzer()
    {
        _windowService.ShowWindow<AnalyzeQueryWindow>();
    }

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

    [RelayCommand]
    private void OpenSupportSite()
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/your-repo/CRUDExplorer",
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(psi);
        }
        catch
        {
            StatusMessage = "サポートサイトを開けませんでした";
        }
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

    /// <summary>
    /// 選択フォルダから CRUDMatrix.tsv / CRUD.tsv を読み込んでマトリクスを更新する
    /// </summary>
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
            CrudListData.Clear();
        }

        var viewLabel = CrudViewType == 0 ? "テーブル" : "カラム";
        StatusMessage = $"{viewLabel}CRUDマトリクス読み込み完了：{MatrixRows.Count} 行 / {MatrixHeaders.Length} プログラム";
    }

    /// <summary>
    /// CRUDMatrix.tsv を解析。
    /// 行0=ヘッダ(col3+=プログラムID), 行1=集計(スキップ), 行2+=データ行
    /// </summary>
    private Task LoadMatrixAsync(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        string[] programHeaders = Array.Empty<string>();
        var newRows = new List<CrudMatrixRow>();

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;
            var cols = lines[i].Split('\t');

            if (i == 0)
            {
                programHeaders = cols.Length > 3 ? cols[3..] : Array.Empty<string>();
            }
            else if (i == 1)
            {
                // 集計行スキップ
            }
            else
            {
                var row = new CrudMatrixRow
                {
                    TableName   = cols.Length > 0 ? cols[0] : string.Empty,
                    LogicalName = cols.Length > 1 ? cols[1] : string.Empty,
                    Total       = cols.Length > 2 ? cols[2] : string.Empty
                };
                var cellValues = new string[programHeaders.Length];
                for (int j = 0; j < programHeaders.Length; j++)
                {
                    var header = programHeaders[j];
                    var value  = cols.Length > 3 + j ? cols[3 + j] : string.Empty;
                    cellValues[j] = value;
                    if (!string.IsNullOrEmpty(header))
                        row.Values[header] = value;
                }
                row.CellValues = cellValues;
                if (!string.IsNullOrEmpty(row.TableName) || row.Values.Values.Any(v => !string.IsNullOrEmpty(v)))
                    newRows.Add(row);
            }
        }

        MatrixHeaders = programHeaders;
        MatrixRows.Clear();
        foreach (var r in newRows) MatrixRows.Add(r);
        MatrixColumnsChanged?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    /// <summary>
    /// CRUD.tsv を読み込んでリスト表示用データを更新する。
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