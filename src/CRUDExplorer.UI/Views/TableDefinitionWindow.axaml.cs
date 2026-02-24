using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class TableDefinitionWindow : Window
{
    public TableDefinitionWindow()
    {
        InitializeComponent();
        DataContext = new TableDefinitionViewModel();
    }
}
