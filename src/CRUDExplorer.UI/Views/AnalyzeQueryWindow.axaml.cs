using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class AnalyzeQueryWindow : Window
{
    public AnalyzeQueryWindow()
    {
        InitializeComponent();
        DataContext = new AnalyzeQueryViewModel(
            showGrepWindow: (param) =>
            {
                // param形式: "scope:file:word"
                var parts = param.Split(':', 3);
                var scope = parts.Length > 0 ? parts[0] : "all";
                var file = parts.Length > 1 ? parts[1] : string.Empty;
                var word = parts.Length > 2 ? parts[2] : string.Empty;

                var grepVm = new GrepViewModel(closeWindow: () => { });
                grepVm.SearchPattern = word;
                grepVm.CurrentFile = file;
                grepVm.SearchCurrentFile = scope == "file";
                grepVm.SearchCurrentProgram = scope == "program";
                grepVm.SearchAllFiles = scope == "all";

                var grepWin = new GrepWindow();
                grepWin.DataContext = grepVm;
                grepWin.Show(this);
            },
            showTableDefinitionWindow: (tableName) =>
            {
                var win = new TableDefinitionWindow();
                if (win.DataContext is TableDefinitionViewModel vm && !string.IsNullOrEmpty(tableName))
                    vm.SelectedTable = tableName;
                win.Show(this);
            },
            showCrudListWindow: (items, title) =>
            {
                var listVm = new GenericListViewModel(closeWindow: () => { });
                var listItems = items
                    .Select(i => new ListItemModel { DisplayText = i.DisplayText, Tag = i })
                    .ToList();
                listVm.SetItems(
                    new System.Collections.ObjectModel.ObservableCollection<ListItemModel>(listItems),
                    title);
                var win = new GenericListWindow();
                win.DataContext = listVm;
                win.Show(this);
            });
    }
}
