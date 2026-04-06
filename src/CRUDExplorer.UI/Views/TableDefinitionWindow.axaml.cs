using System.Threading.Tasks;
using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class TableDefinitionWindow : Window
{
    public TableDefinitionWindow()
    {
        InitializeComponent();
        DataContext = new TableDefinitionViewModel(
            closeWindow: () => Close(),
            setClipboard: async (text) =>
            {
                var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                if (clipboard != null)
                    await clipboard.SetTextAsync(text);
            },
            showCrudSearch: (tableName, columnName) =>
            {
                var vm = new CrudSearchViewModel(closeWindow: null);
                vm.TableNamePattern = tableName;
                vm.ColumnNamePattern = columnName;
                vm.SearchCommand.Execute(null);
                var win = new CrudSearchWindow { DataContext = vm };
                win.Show();
            });
    }
}
