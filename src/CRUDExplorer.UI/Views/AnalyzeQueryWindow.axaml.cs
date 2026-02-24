using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class AnalyzeQueryWindow : Window
{
    public AnalyzeQueryWindow()
    {
        InitializeComponent();
        DataContext = new AnalyzeQueryViewModel();
    }
}
