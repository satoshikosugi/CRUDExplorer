using Avalonia.Controls;
using Avalonia.Input;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class CrudSearchWindow : Window
{
    public CrudSearchWindow()
    {
        InitializeComponent();
        DataContext = new CrudSearchViewModel(closeWindow: () => Close());
    }

    private void OnResultDoubleClicked(object? sender, TappedEventArgs e)
    {
        if (DataContext is CrudSearchViewModel vm)
        {
            vm.OpenInEditorCommand.Execute(null);
        }
    }
}
