using Avalonia.Controls;
using Avalonia.Data;
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
        // 旧VMのイベント解除（多重購読防止）
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
    /// DataGridTextColumn + Binding を使用（確実・シンプル）。
    /// "[key]" バインディングパスでは [] 内の "." はパス区切りとして扱われない。
    /// </summary>
    private void RebuildMatrixColumns(MainWindowViewModel vm)
    {
        var grid = this.FindControl<DataGrid>("CrudMatrixGrid");
        if (grid == null)
        {
            vm.StatusMessage = "内部エラー: CrudMatrixGridが見つかりません";
            return;
        }

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
        // CellValues[i] でバインド（整数インデクサーは Avalonia が確実にサポート）
        // ヘッダー名に "." が含まれるため文字列インデクサーは使用しない
        for (int idx = 0; idx < vm.MatrixHeaders.Length; idx++)
        {
            var header = vm.MatrixHeaders[idx];
            grid.Columns.Add(new DataGridTextColumn
            {
                Header  = header,
                Binding = new Binding($"CellValues[{idx}]"),
                Width   = new DataGridLength(80),
            });
        }

        grid.ItemsSource = vm.MatrixRows;

        // デバッグ確認用: 列数をステータスに追記（目視確認後に削除可）
        vm.StatusMessage += $"  [列:{grid.Columns.Count} / 行:{vm.MatrixRows.Count}]";
    }

    private void OnListBoxDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel && viewModel.SelectedCrudItem is CrudListItem item)
        {
            viewModel.StatusMessage = $"選択: {item.TableName} [{item.Crud}] {item.FileName}:{item.LineNo}";
        }
    }
}