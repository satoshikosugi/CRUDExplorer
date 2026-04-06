using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class GenericListWindow : Window
{
    public GenericListWindow()
    {
        InitializeComponent();
        var vm = new GenericListViewModel(closeWindow: () => Close());
        vm.SetClipboard = async (text) =>
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard != null)
                await clipboard.SetTextAsync(text);
        };
        DataContext = vm;
    }
}
