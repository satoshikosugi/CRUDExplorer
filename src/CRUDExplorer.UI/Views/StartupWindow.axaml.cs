using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class StartupWindow : Window
{
    public StartupWindow()
    {
        InitializeComponent();
        DataContext = new StartupViewModel(closeWindow: () => Close());
    }
}
