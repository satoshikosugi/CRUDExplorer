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

        // プログラム/テーブル一覧を GlobalState から収集
        LoadProgramItems(f.ProgramFilter);
        LoadTableItems(f.TableFilter);

        // アクセス連動 ComboBox の選択肢を構築
        LoadAccessComboBoxItems();
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

    // ── アクセス連動 ComboBox（オリジナル frmFilter.vb の cmbProgramAccess / cmbTableAccess 相当） ──

    [ObservableProperty]
    private ObservableCollection<string> _programAccessItems = new();

    [ObservableProperty]
    private string? _selectedProgramAccess;

    [ObservableProperty]
    private ObservableCollection<string> _tableAccessItems = new();

    [ObservableProperty]
    private string? _selectedTableAccess;

    /// <summary>
    /// プログラムアクセスComboBox選択時: テーブルチェックを全解除（オリジナル仕様）
    /// </summary>
    partial void OnSelectedProgramAccessChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            foreach (var i in TableItems) i.IsChecked = false;
            // 選択したプログラムがアクセスするテーブルのみチェック
            var programKey = ExtractKey(value);
            if (!string.IsNullOrEmpty(programKey))
            {
                var accessedTables = GetTablesAccessedByProgram(programKey);
                foreach (var t in TableItems)
                {
                    if (accessedTables.Contains(t.Name))
                        t.IsChecked = true;
                }
            }
        }
    }

    /// <summary>
    /// テーブルアクセスComboBox選択時: プログラムチェックを全解除（オリジナル仕様）
    /// </summary>
    partial void OnSelectedTableAccessChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            foreach (var i in ProgramItems) i.IsChecked = false;
            // 選択したテーブルにアクセスするプログラムのみチェック
            var tableKey = ExtractKey(value);
            if (!string.IsNullOrEmpty(tableKey))
            {
                var accessingPrograms = GetProgramsAccessingTable(tableKey);
                foreach (var p in ProgramItems)
                {
                    if (accessingPrograms.Contains(p.Name))
                        p.IsChecked = true;
                }
            }
        }
    }

    /// <summary>ComboBox表示名から先頭のキー部分を抽出: "KEY (名前)" → "KEY"</summary>
    private static string ExtractKey(string displayText)
    {
        var idx = displayText.IndexOf(" (", StringComparison.Ordinal);
        return idx > 0 ? displayText[..idx] : displayText;
    }

    /// <summary>指定プログラムがアクセスするテーブル一覧を返す</summary>
    private static HashSet<string> GetTablesAccessedByProgram(string programKey)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in GlobalState.Instance.QueryList)
        {
            var q = kvp.Value;
            var pid = System.IO.Path.GetFileNameWithoutExtension(q.FileName);
            if (!string.Equals(pid, programKey, StringComparison.OrdinalIgnoreCase)) continue;
            foreach (var t in q.GetAllTables().Values)
                result.Add(t.Split('\t')[0]);
        }
        return result;
    }

    /// <summary>指定テーブルにアクセスするプログラム一覧を返す</summary>
    private static HashSet<string> GetProgramsAccessingTable(string tableKey)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in GlobalState.Instance.QueryList)
        {
            var q = kvp.Value;
            foreach (var t in q.GetAllTables().Values)
            {
                if (string.Equals(t.Split('\t')[0], tableKey, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(System.IO.Path.GetFileNameWithoutExtension(q.FileName));
                    break;
                }
            }
        }
        return result;
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

    /// <summary>
    /// アクセス連動 ComboBox の選択肢を構築する（オリジナル frmFilter.vb の ShowForm 相当）。
    /// 表示形式: "KEY (論理名)"
    /// </summary>
    private void LoadAccessComboBoxItems()
    {
        ProgramAccessItems.Clear();
        ProgramAccessItems.Add(string.Empty); // 空=未選択
        foreach (var kv in GlobalState.Instance.ProgramNames)
        {
            ProgramAccessItems.Add($"{kv.Key} ({kv.Value})");
        }

        TableAccessItems.Clear();
        TableAccessItems.Add(string.Empty); // 空=未選択
        foreach (var kv in GlobalState.Instance.TableNames)
        {
            TableAccessItems.Add($"{kv.Key} ({kv.Value})");
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
