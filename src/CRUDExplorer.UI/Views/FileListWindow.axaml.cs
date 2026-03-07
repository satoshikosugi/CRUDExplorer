using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class FileListWindow : Window
{
    public FileListWindow()
    {
        InitializeComponent();
        DataContext = new FileListViewModel(closeWindow: () => Close());
    }
}
