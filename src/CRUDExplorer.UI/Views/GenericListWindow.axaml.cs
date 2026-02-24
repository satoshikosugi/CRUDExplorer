using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class GenericListWindow : Window
{
    public GenericListWindow()
    {
        InitializeComponent();
        DataContext = new GenericListViewModel(closeWindow: () => Close());
    }
}
