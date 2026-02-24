using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class CrudSearchWindow : Window
{
    public CrudSearchWindow()
    {
        InitializeComponent();
        DataContext = new CrudSearchViewModel(closeWindow: () => Close());
    }
}
