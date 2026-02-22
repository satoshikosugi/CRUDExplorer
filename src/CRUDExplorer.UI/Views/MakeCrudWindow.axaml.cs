using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class MakeCrudWindow : Window
{
    public MakeCrudWindow()
    {
        InitializeComponent();
        DataContext = new MakeCrudViewModel();
    }
}
