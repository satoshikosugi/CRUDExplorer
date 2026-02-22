using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class VersionWindow : Window
{
    public VersionWindow()
    {
        InitializeComponent();
        DataContext = new VersionViewModel();
    }
}
