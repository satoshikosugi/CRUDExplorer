using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.ViewModels;

/// <summary>
/// テーブル定義画面の ViewModel。
/// オリジナル frmTableDef.vb の左右ペイン構成を踏襲。
/// 左: テーブル一覧（フィルタ付き）、右: 選択テーブルのカラム定義。
/// </summary>
public partial class TableDefinitionViewModel : ViewModelBase
{
    private readonly Action _closeWindow;
    private readonly Func<string, System.Threading.Tasks.Task>? _setClipboard;

    /// <summary>左ペイン: テーブル一覧</summary>
    public ObservableCollection<TableListItem> TableListItems { get; } = new();

    /// <summary>右ペイン: 選択テーブルのカラム定義</summary>
    [ObservableProperty]
    private ObservableCollection<ColumnDefinitionItem> _columns = new();

    [ObservableProperty]
    private string _tableFilter = string.Empty;

    [ObservableProperty]
    private TableListItem? _selectedTableItem;

    [ObservableProperty]
    private ColumnDefinitionItem? _selectedColumnItem;

    // 外部から設定するための互換プロパティ（MainWindowViewModel から vm.SelectedTable = "xxx" で呼ばれる）
    public string? SelectedTable
    {
        get => SelectedTableItem?.TableName;
        set
        {
            if (string.IsNullOrEmpty(value)) return;
            var item = TableListItems.FirstOrDefault(t =>
                string.Equals(t.TableName, value, StringComparison.OrdinalIgnoreCase));
            if (item != null)
                SelectedTableItem = item;
        }
    }

    public TableDefinitionViewModel(
        Action? closeWindow = null,
        Func<string, System.Threading.Tasks.Task>? setClipboard = null)
    {
        _closeWindow = closeWindow ?? (() => { });
        _setClipboard = setClipboard;
        LoadTableList();
    }

    partial void OnSelectedTableItemChanged(TableListItem? value)
    {
        if (value != null)
        {
            LoadTableDefinition(value.TableName);
            // タイトル更新
        }
        else
        {
            Columns.Clear();
        }
    }

    // ── コマンド ──────────────────────────────────────────────────────

    [RelayCommand]
    private void ApplyFilter()
    {
        LoadTableList();
    }

    [RelayCommand]
    private void ClearFilter()
    {
        TableFilter = string.Empty;
        LoadTableList();
    }

    [RelayCommand]
    private void SearchTableAccess()
    {
        // CRUDサーチ: このテーブルにアクセスしている処理
        // 将来的にCRUDサーチウィンドウを開く
        if (SelectedTableItem == null) return;
        // CRUDSearchWindow を開く処理は MainWindow 側で行うため、
        // ここではグローバルステートに情報をセットするのみ
    }

    [RelayCommand]
    private void SearchColumnAccess()
    {
        // CRUDサーチ: このカラムにアクセスしている処理
        if (SelectedTableItem == null || SelectedColumnItem == null) return;
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow();
    }

    // ── 内部処理 ──────────────────────────────────────────────────────

    private void LoadTableList()
    {
        var previousSelection = SelectedTableItem?.TableName;
        TableListItems.Clear();

        var tableDefinitions = GlobalState.Instance.TableDefinitions;
        var tableNames = GlobalState.Instance.TableNames;

        Regex? regex = null;
        if (!string.IsNullOrWhiteSpace(TableFilter))
        {
            try { regex = new Regex(TableFilter, RegexOptions.IgnoreCase); }
            catch (ArgumentException) { regex = null; }
        }

        foreach (var tableName in tableDefinitions.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase))
        {
            if (regex != null && !regex.IsMatch(tableName))
                continue;

            var logicalName = tableNames.TryGetValue(tableName, out var ln)
                ? ln
                : (tableNames.TryGetValue(tableName.ToUpperInvariant(), out ln) ? ln : string.Empty);

            TableListItems.Add(new TableListItem
            {
                TableName = tableName,
                LogicalName = logicalName
            });
        }

        // 選択復元 or 先頭選択
        if (!string.IsNullOrEmpty(previousSelection))
        {
            var prev = TableListItems.FirstOrDefault(t =>
                string.Equals(t.TableName, previousSelection, StringComparison.OrdinalIgnoreCase));
            SelectedTableItem = prev ?? TableListItems.FirstOrDefault();
        }
        else
        {
            SelectedTableItem = TableListItems.FirstOrDefault();
        }
    }

    private void LoadTableDefinition(string tableName)
    {
        Columns.Clear();

        var tableDefinitions = GlobalState.Instance.TableDefinitions;
        if (!tableDefinitions.TryGetValue(tableName, out var tableDef))
        {
            // 大文字でリトライ
            if (!tableDefinitions.TryGetValue(tableName.ToUpperInvariant(), out tableDef))
                return;
        }

        foreach (var col in tableDef.Columns.Values)
        {
            Columns.Add(new ColumnDefinitionItem
            {
                Sequence = col.Sequence,
                PhysicalName = col.ColumnName,
                ColumnName = col.AttributeName,
                PkDisplay = col.PrimaryKey == "Yes" ? "Yes" : string.Empty,
                FkDisplay = col.ForeignKey == "Yes" ? "Yes" : string.Empty,
                RequiredDisplay = col.Required == "Yes" ? "Yes" : string.Empty,
                DataType = col.DataType,
                Digits = col.Digits,
                Accuracy = col.Accuracy,
            });
        }
    }
}

/// <summary>左ペインのテーブル一覧アイテム</summary>
public class TableListItem
{
    public string TableName { get; set; } = string.Empty;
    public string LogicalName { get; set; } = string.Empty;
}

/// <summary>右ペインのカラム定義アイテム（オリジナルの lstTableDef に対応）</summary>
public class ColumnDefinitionItem
{
    public string Sequence { get; set; } = string.Empty;
    public string PhysicalName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string PkDisplay { get; set; } = string.Empty;
    public string FkDisplay { get; set; } = string.Empty;
    public string RequiredDisplay { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Digits { get; set; } = string.Empty;
    public string Accuracy { get; set; } = string.Empty;
}
