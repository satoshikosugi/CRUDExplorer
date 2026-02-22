using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class FilterWindow : Window
{
    public FilterWindow()
    {
        InitializeComponent();
        DataContext = new FilterViewModel();
    }
}
