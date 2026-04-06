using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.ViewModels;

/// <summary>
/// フィルタ設定ウィンドウの ViewModel。
/// オリジナル frmFilter.vb の「プログラムIDリスト × テーブル名リスト チェック選択」に相当。
/// 適用結果は GlobalState.Instance.FilterState に書き込む。
/// </summary>
public partial class FilterViewModel : ViewModelBase
{
    private readonly Action _closeWindow;

    // CRUD辞書: プログラム→テーブル一覧、テーブル→プログラム一覧を保持
    private readonly Dictionary<string, HashSet<string>> _programToTables = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, HashSet<string>> _tableToPrograms = new(StringComparer.OrdinalIgnoreCase);

    public FilterViewModel(Action? closeWindow = null)
    {
        _closeWindow = closeWindow ?? (() => { });

        // GlobalState の現在フィルタ状態を初期値として読み込む
        var f = GlobalState.Instance.FilterState;
        ProgramFilter = f.ProgramFilter;
        TableFilter   = f.TableFilter;
        ShowC = f.ShowC;
        ShowR = f.ShowR;
        ShowU = f.ShowU;
        ShowD = f.ShowD;

        // CRUDデータからプログラム⇔テーブルの関連を構築
        BuildCrudRelations();

        // プログラム/テーブル一覧を GlobalState から収集
        LoadProgramItems(f.ProgramFilter);
        LoadTableItems(f.TableFilter);

        // アクセス絞り込みComboBoxを初期化
        InitializeAccessItems();
    }

    // ── フィルタ文字列 ────────────────────────────────────────────────

    [ObservableProperty]
    private string _programFilter = string.Empty;

    partial void OnProgramFilterChanged(string value)
    {
        // テキスト変更時、チェック状態を一括更新（簡易）
        foreach (var item in ProgramItems)
        {
            item.IsChecked = string.IsNullOrEmpty(value) ||
                System.Text.RegularExpressions.Regex.IsMatch(
                    item.Name, value,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }

    [ObservableProperty]
    private string _tableFilter = string.Empty;

    partial void OnTableFilterChanged(string value)
    {
        foreach (var item in TableItems)
        {
            item.IsChecked = string.IsNullOrEmpty(value) ||
                System.Text.RegularExpressions.Regex.IsMatch(
                    item.Name, value,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }

    // ── CRUD 種別 ─────────────────────────────────────────────────────

    [ObservableProperty] private bool _showC = true;
    [ObservableProperty] private bool _showR = true;
    [ObservableProperty] private bool _showU = true;
    [ObservableProperty] private bool _showD = true;

    // ── アクセス絞り込みComboBox ──────────────────────────────────────

    /// <summary>「このテーブルにアクセスしているプログラムに絞る」のComboBox選択肢</summary>
    public ObservableCollection<string> TableAccessItems { get; } = new();

    /// <summary>「このプログラムがアクセスしているテーブルに絞る」のComboBox選択肢</summary>
    public ObservableCollection<string> ProgramAccessItems { get; } = new();

    [ObservableProperty]
    private string? _selectedTableAccess;

    partial void OnSelectedTableAccessChanged(string? value)
    {
        if (string.IsNullOrEmpty(value) || value == "(テーブルアクセスで絞り込み)")
            return;

        // 選択したテーブルにアクセスしているプログラムのみチェック
        if (_tableToPrograms.TryGetValue(value, out var programs))
        {
            foreach (var item in ProgramItems)
            {
                item.IsChecked = programs.Contains(item.Name);
            }
        }
    }

    [ObservableProperty]
    private string? _selectedProgramAccess;

    partial void OnSelectedProgramAccessChanged(string? value)
    {
        if (string.IsNullOrEmpty(value) || value == "(プログラムアクセスで絞り込み)")
            return;

        // 選択したプログラムがアクセスしているテーブルのみチェック
        if (_programToTables.TryGetValue(value, out var tables))
        {
            foreach (var item in TableItems)
            {
                item.IsChecked = tables.Contains(item.Name);
            }
        }
    }

    private void InitializeAccessItems()
    {
        // 左パネル（プログラム側）: テーブルアクセスで絞り込む
        TableAccessItems.Clear();
        TableAccessItems.Add("(テーブルアクセスで絞り込み)");
        foreach (var table in _tableToPrograms.Keys.OrderBy(k => k))
        {
            TableAccessItems.Add(table);
        }
        SelectedTableAccess = "(テーブルアクセスで絞り込み)";

        // 右パネル（テーブル側）: プログラムアクセスで絞り込む
        ProgramAccessItems.Clear();
        ProgramAccessItems.Add("(プログラムアクセスで絞り込み)");
        foreach (var prog in _programToTables.Keys.OrderBy(k => k))
        {
            ProgramAccessItems.Add(prog);
        }
        SelectedProgramAccess = "(プログラムアクセスで絞り込み)";
    }

    private void BuildCrudRelations()
    {
        _programToTables.Clear();
        _tableToPrograms.Clear();

        // GlobalStateのQueryListからCRUD関連を抽出
        foreach (var kvp in GlobalState.Instance.QueryList)
        {
            var query = kvp.Value;
            var programId = query.FileName;
            if (string.IsNullOrEmpty(programId))
                continue;

            // 全テーブル辞書からテーブル名を収集
            var allTables = query.GetAllTables(true);
            foreach (var tableEntry in allTables)
            {
                var tableName = tableEntry.Key;
                if (string.IsNullOrEmpty(tableName))
                    continue;

                if (!_programToTables.TryGetValue(programId, out var tables))
                {
                    tables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _programToTables[programId] = tables;
                }
                tables.Add(tableName);

                if (!_tableToPrograms.TryGetValue(tableName, out var programs))
                {
                    programs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _tableToPrograms[tableName] = programs;
                }
                programs.Add(programId);
            }
        }
    }

    // ── チェックリスト ────────────────────────────────────────────────

    public ObservableCollection<FilterItem> ProgramItems { get; } = new();
    public ObservableCollection<FilterItem> TableItems   { get; } = new();

    private void LoadProgramItems(string currentFilter)
    {
        ProgramItems.Clear();
        // CRUD.tsv の ProgramId を GlobalState 経由で取得できないため、
        // FilterState に保存してある選択情報を参考にする（一時的: 次回改善）
        // ここでは CRUD 一覧や Global ProgramNames から収集
        foreach (var kv in GlobalState.Instance.ProgramNames)
        {
            var name = kv.Key;
            ProgramItems.Add(new FilterItem
            {
                Name      = name,
                IsChecked = string.IsNullOrEmpty(currentFilter) ||
                            System.Text.RegularExpressions.Regex.IsMatch(
                                name, currentFilter,
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            });
        }
    }

    private void LoadTableItems(string currentFilter)
    {
        TableItems.Clear();
        foreach (var kv in GlobalState.Instance.TableNames)
        {
            var name = kv.Key;
            TableItems.Add(new FilterItem
            {
                Name        = name,
                SubText     = kv.Value,
                IsChecked   = string.IsNullOrEmpty(currentFilter) ||
                              System.Text.RegularExpressions.Regex.IsMatch(
                                  name, currentFilter,
                                  System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            });
        }
    }

    // ── コマンド ──────────────────────────────────────────────────────

    [RelayCommand]
    private void SelectAllPrograms()
    {
        foreach (var i in ProgramItems) i.IsChecked = true;
    }

    [RelayCommand]
    private void DeselectAllPrograms()
    {
        foreach (var i in ProgramItems) i.IsChecked = false;
    }

    [RelayCommand]
    private void SelectAllTables()
    {
        foreach (var i in TableItems) i.IsChecked = true;
    }

    [RelayCommand]
    private void DeselectAllTables()
    {
        foreach (var i in TableItems) i.IsChecked = false;
    }

    [RelayCommand]
    private void Apply()
    {
        // チェック済みアイテムから正規表現を構築して GlobalState に保存
        var progFilter  = BuildRegexFilter(ProgramItems, ProgramFilter);
        var tableFilter = BuildRegexFilter(TableItems,   TableFilter);

        GlobalState.Instance.FilterState = new AppFilterState
        {
            ProgramFilter = progFilter,
            TableFilter   = tableFilter,
            ShowC         = ShowC,
            ShowR         = ShowR,
            ShowU         = ShowU,
            ShowD         = ShowD,
            WasApplied    = true,
        };
        _closeWindow();
    }

    /// <summary>
    /// チェックリストから正規表現フィルタ文字列を構築する。
    /// 全選択 / 全未選択の場合は手入力フィルタをそのまま使う。
    /// </summary>
    private static string BuildRegexFilter(
        ObservableCollection<FilterItem> items, string manualFilter)
    {
        var checked_ = items.Where(i => i.IsChecked).ToList();
        if (checked_.Count == 0 || checked_.Count == items.Count)
            return manualFilter;   // 手入力フィルタを優先
        return "^(" + string.Join("|", checked_.Select(i => System.Text.RegularExpressions.Regex.Escape(i.Name))) + ")$";
    }

    [RelayCommand]
    private void Clear()
    {
        ProgramFilter = string.Empty;
        TableFilter   = string.Empty;
        ShowC = ShowR = ShowU = ShowD = true;
        foreach (var i in ProgramItems) i.IsChecked = true;
        foreach (var i in TableItems)   i.IsChecked = true;
        SelectedTableAccess = "(テーブルアクセスで絞り込み)";
        SelectedProgramAccess = "(プログラムアクセスで絞り込み)";

        GlobalState.Instance.FilterState = new AppFilterState { WasApplied = false };
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow();
    }
}

/// <summary>フィルタチェックリストの1アイテム</summary>
public partial class FilterItem : ObservableObject
{
    public string Name    { get; set; } = string.Empty;
    public string SubText { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isChecked = true;
}
