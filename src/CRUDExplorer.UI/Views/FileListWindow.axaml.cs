using Avalonia.Controls;
using Avalonia.Input;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class FileListWindow : Window
{
    public FileListWindow()
    {
        InitializeComponent();
        DataContext = new FileListViewModel(closeWindow: () => Close());
    }

    private void OnFileDoubleClicked(object? sender, TappedEventArgs e)
    {
        // ファイルダブルクリックでエディタを開く
        if (DataContext is FileListViewModel vm && vm.SelectedFile != null && vm.SelectedQuery == null)
        {
            vm.OpenCommand.Execute(null);
        }
    }

    private void OnQueryDoubleClicked(object? sender, TappedEventArgs e)
    {
        // クエリダブルクリックでエディタを開く
        if (DataContext is FileListViewModel vm && vm.SelectedQuery != null)
        {
            vm.OpenCommand.Execute(null);
        }
    }
}
