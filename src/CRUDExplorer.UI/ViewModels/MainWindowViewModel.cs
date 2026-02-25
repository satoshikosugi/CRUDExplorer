using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "フォルダを選択してCRUDマトリクスを表示するか、「CRUD解析を実行」で解析を行ってください";

    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private int _crudViewType = 0; // 0: TableCRUD, 1: ColumnCRUD

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
            CrudListData.Add(item);
        }
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