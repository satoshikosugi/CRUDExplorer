using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Threading;
using CRUDExplorer.Core.Utilities;
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

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_attachedVm != null)
        {
            _attachedVm.MatrixColumnsChanged -= OnMatrixColumnsChanged;
            _attachedVm.MatrixClipboardReady -= OnMatrixClipboardReady;
        }

        _attachedVm = DataContext as MainWindowViewModel;

        if (_attachedVm != null)
        {
            _attachedVm.MatrixColumnsChanged += OnMatrixColumnsChanged;
            _attachedVm.MatrixClipboardReady += OnMatrixClipboardReady;
        }
    }

    private void OnMatrixColumnsChanged(object? sender, EventArgs e)
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
    /// クリップボードにコピー準備完了時の処理
    /// </summary>
    private async void OnMatrixClipboardReady(object? sender, EventArgs e)
    {
        if (_attachedVm == null || string.IsNullOrEmpty(_attachedVm.ClipboardText)) return;

        try
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard != null)
                await clipboard.SetTextAsync(_attachedVm.ClipboardText);
        }
        catch
        {
            // クリップボード操作失敗は無視
        }
    }

    /// <summary>
    /// MatrixHeadersが変わるたびにDataGrid列を再構築する（UIスレッドのみ）。
    /// </summary>
    private void RebuildMatrixColumns(MainWindowViewModel vm)
    {
        var grid = this.FindControl<DataGrid>("CrudMatrixGrid");
        if (grid == null)
        {
            vm.StatusMessage = "内部エラー: CrudMatrixGridが見つかりません";
            return;
        }

        // イベントを一旦外す（列再構築中にSelectionChangedが発火するのを防ぐ）
        grid.SelectionChanged -= OnMatrixSelectionChanged;

        // ItemsSource を一旦外してから列を再構築
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

        // プログラムごとの動的列
        for (int idx = 0; idx < vm.MatrixHeaders.Length; idx++)
        {
            var programId = vm.MatrixHeaders[idx];
            
            // 論理名表示モードの場合、プログラム名（論理名）に切り替える
            string headerText = programId;
            if (vm.ShowLogicalName)
            {
                if (GlobalState.Instance.ProgramNames.TryGetValue(programId, out var logicalName)
                    && !string.IsNullOrEmpty(logicalName))
                {
                    headerText = logicalName;
                }
            }

            grid.Columns.Add(new DataGridTextColumn
            {
                Header  = headerText,
                Binding = new Binding(nameof(CrudMatrixRow.CellValues))
                {
                    Converter      = new CellValueConverter(),
                    ConverterParameter = idx,
                },
                Width   = new DataGridLength(80),
            });
        }

        // SelectionUnit を CellOrRowHeader にして、セル単位の選択を可能にする
        grid.SelectionMode = DataGridSelectionMode.Extended;

        grid.ItemsSource = vm.MatrixRows;

        // セル選択イベントを接続
        grid.SelectionChanged += OnMatrixSelectionChanged;

        var modeLabel = vm.ShowLogicalName ? "論理名" : "物理名";
        vm.StatusMessage += $"  [列:{grid.Columns.Count} / 行:{vm.MatrixRows.Count}] ({modeLabel}表示)";
    }

    /// <summary>
    /// DataGrid のセル選択が変わったら CRUD 一覧をフィルタする。
    /// originalの仕様:
    ///   - セルクリック → テーブル名＋プログラムIDで絞り込み
    ///   - 行選択 (固定列クリック) → テーブル名のみで絞り込み
    /// </summary>
    private void OnMatrixSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_attachedVm == null) return;
        var grid = sender as DataGrid;
        if (grid == null) return;

        // 選択された行を取得
        var row = grid.SelectedItem as CrudMatrixRow;
        if (row == null)
        {
            _attachedVm.FilterCrudListBySelection(null, null);
            return;
        }

        var tableName = row.TableName;

        // 現在フォーカスしている列を取得
        var colIndex = grid.CurrentColumn?.DisplayIndex ?? -1;

        // 固定列（テーブル名=0, 論理名=1, 合計=2）の場合はテーブル全体を絞り込み
        // プログラム列（3以降）の場合はテーブル＋プログラムで絞り込み
        if (colIndex >= 3 && _attachedVm.MatrixHeaders.Length > colIndex - 3)
        {
            var programId = _attachedVm.MatrixHeaders[colIndex - 3];
            _attachedVm.FilterCrudListBySelection(tableName, programId);
        }
        else
        {
            // 固定列や列不明の場合 → テーブル全体
            _attachedVm.FilterCrudListBySelection(tableName, null);
        }
    }

    /// <summary>
    /// CRUD一覧のダブルクリック → 設定に応じたアクション実行
    /// </summary>
    private void OnListBoxDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.HandleCrudListDoubleClick();
        }
    }

    /// <summary>
    /// CRUD一覧のキーボード操作: Enter → ダブルクリックと同じ動作
    /// </summary>
    private void OnListBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is MainWindowViewModel viewModel)
        {
            viewModel.HandleCrudListDoubleClick();
            e.Handled = true;
        }
    }

    /// <summary>
    /// マトリクスのキーボード操作:
    ///   Enter/Tab → CRUD一覧にフォーカス移動
    ///   T → 選択行のテーブル定義を表示
    /// </summary>
    private void OnMatrixKeyDown(object? sender, KeyEventArgs e)
    {
        if (_attachedVm == null) return;
        var grid = sender as DataGrid;

        if (e.Key == Key.Enter || e.Key == Key.Tab)
        {
            e.Handled = true;
            var listBox = this.FindControl<ListBox>("CrudListBox");
            if (listBox != null)
            {
                listBox.Focus();
                if (listBox.SelectedIndex < 0 && listBox.ItemCount > 0)
                    listBox.SelectedIndex = 0;
            }
        }
        else if (e.Key == Key.T)
        {
            var row = grid?.SelectedItem as CrudMatrixRow;
            if (row != null)
            {
                _attachedVm.ShowTableDefForSelectedRow(row.TableName);
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// コンテキストメニュー: テーブル定義表示
    /// </summary>
    private void OnContextMenuTableDef(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_attachedVm == null) return;

        var grid = this.FindControl<DataGrid>("CrudMatrixGrid");
        var row = grid?.SelectedItem as CrudMatrixRow;
        if (row != null)
        {
            _attachedVm.ShowTableDefForSelectedRow(row.TableName);
        }
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