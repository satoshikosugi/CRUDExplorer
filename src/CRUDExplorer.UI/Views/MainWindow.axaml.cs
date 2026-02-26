using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Threading;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? _attachedVm;

    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (_attachedVm != null)
            _attachedVm.MatrixColumnsChanged -= OnMatrixColumnsChanged;

        _attachedVm = DataContext as MainWindowViewModel;

        if (_attachedVm != null)
            _attachedVm.MatrixColumnsChanged += OnMatrixColumnsChanged;
    }

    private void OnMatrixColumnsChanged(object? sender, System.EventArgs e)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.InvokeAsync(() => OnMatrixColumnsChanged(sender, e));
            return;
        }

        if (_attachedVm != null)
            RebuildMatrixColumns(_attachedVm);
    }

    /// <summary>
    /// MatrixHeadersが変わるたびにDataGrid列を再構築する（UIスレッドのみ）。
    /// 動的列は CellValues[idx] を IValueConverter 経由で参照する。
    /// これは Avalonia 11 でカスタムインデクサーバインディングが動作しない問題を回避する。
    /// </summary>
    private void RebuildMatrixColumns(MainWindowViewModel vm)
    {
        var grid = this.FindControl<DataGrid>("CrudMatrixGrid");
        if (grid == null)
        {
            vm.StatusMessage = "内部エラー: CrudMatrixGridが見つかりません";
            return;
        }

        grid.ItemsSource = null;
        grid.Columns.Clear();

        // 固定列: テーブル名 / 論理名 / 合計
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "テーブル名",
            Binding = new Binding(nameof(CrudMatrixRow.TableName)),
            Width   = new DataGridLength(150),
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "論理名",
            Binding = new Binding(nameof(CrudMatrixRow.LogicalName)),
            Width   = new DataGridLength(150),
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "合計",
            Binding = new Binding(nameof(CrudMatrixRow.Total)),
            Width   = new DataGridLength(60),
        });

        // プログラムごとの動的列。
        // CrudMatrixRow.CellValues (string[]) を IValueConverter でインデックス参照する。
        // この方式は単純な named-property バインディングを使うため Avalonia の
        // カスタムインデクサー非対応の問題を完全に回避できる。
        for (int idx = 0; idx < vm.MatrixHeaders.Length; idx++)
        {
            grid.Columns.Add(new DataGridTextColumn
            {
                Header  = vm.MatrixHeaders[idx],
                Binding = new Binding(nameof(CrudMatrixRow.CellValues))
                {
                    Converter      = new CellValueConverter(),
                    ConverterParameter = idx,
                },
                Width   = new DataGridLength(80),
            });
        }

        grid.ItemsSource = vm.MatrixRows;
    }

    // ── DataGrid セルクリック → CRUD一覧フィルタリング ─────────────

    /// <summary>
    /// DataGrid のセルがクリックされたとき、右ペインの CRUD 一覧を更新する。
    /// 行クリック(row≥0, col=-1)、列ヘッダクリック(row=-1, col≥0)、
    /// セルクリック(row≥0, col≥0) を区別して処理する。
    /// </summary>
    internal void OnMatrixCellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if (_attachedVm == null) return;

        // DataGrid の固定列数（テーブル名・論理名・合計）
        const int FixedCols = 3;

        var row    = e.Row;
        var column = e.Column;

        if (row == null && column == null) return;

        var rowItem   = row?.DataContext as CrudMatrixRow;
        var colHeader = column?.Header as string;

        // 固定列（テーブル名・論理名・合計）クリックは行選択として扱う
        int colDisplayIdx = column != null ? (sender as DataGrid)?.Columns.IndexOf(column) ?? -1 : -1;
        bool isFixedCol = colDisplayIdx >= 0 && colDisplayIdx < FixedCols;

        if (rowItem != null && (column == null || isFixedCol))
        {
            // 行ヘッダ or 固定列クリック: そのテーブルの全プログラムを表示
            _attachedVm.FilterCrudListByTable(rowItem.TableName);
        }
        else if (rowItem == null && colHeader != null && !isFixedCol)
        {
            // 列ヘッダクリック: そのプログラムの全テーブルを表示
            _attachedVm.FilterCrudListByProgram(colHeader);
        }
        else if (rowItem != null && colHeader != null && !isFixedCol)
        {
            // セルクリック: テーブル×プログラムの組み合わせを表示
            _attachedVm.FilterCrudList(rowItem.TableName, colHeader);
        }
    }

    // ── CRUD一覧 ダブルクリック ────────────────────────────────────

    internal void OnListBoxDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (_attachedVm?.SelectedCrudItem is CrudListItem item)
            _attachedVm.OpenEditorForSelectedItem();
    }

    // ── IValueConverter（動的列用）──────────────────────────────────

    /// <summary>
    /// CrudMatrixRow.CellValues (string[]) から指定インデックスの値を取り出すコンバーター。
    /// ConverterParameter に整数インデックスを渡す。
    /// </summary>
    private sealed class CellValueConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string[] arr && parameter is int idx && idx < arr.Length)
                return arr[idx];
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}