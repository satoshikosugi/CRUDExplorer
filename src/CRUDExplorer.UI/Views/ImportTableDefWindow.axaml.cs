using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class ImportTableDefWindow : Window
{
    public ImportTableDefWindow()
    {
        InitializeComponent();
        var vm = new ImportTableDefViewModel();
        vm.SetCloseAction(() => Close());
        vm.LoadSavedSettings();
        DataContext = vm;
    }
}
