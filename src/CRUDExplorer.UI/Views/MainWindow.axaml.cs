using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnListBoxDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel && viewModel.SelectedCrudItem != null)
        {
            // TODO: Open query analysis window
            viewModel.StatusMessage = "クエリ解析画面を表示（未実装）";
        }
    }
}