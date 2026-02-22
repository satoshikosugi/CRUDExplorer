using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRUDExplorer.UI.ViewModels;

public partial class FilterViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _tableFilter = string.Empty;

    [ObservableProperty]
    private bool _tableCaseSensitive = false;

    [ObservableProperty]
    private string _columnFilter = string.Empty;

    [ObservableProperty]
    private bool _columnCaseSensitive = false;

    [ObservableProperty]
    private bool _showCreate = true;

    [ObservableProperty]
    private bool _showRead = true;

    [ObservableProperty]
    private bool _showUpdate = true;

    [ObservableProperty]
    private bool _showDelete = true;

    [ObservableProperty]
    private string _programIdFilter = string.Empty;

    [RelayCommand]
    private void Apply()
    {
        // TODO: Apply filter to main window
        // - Store filter settings in GlobalState or return to parent window
        // - Close window
    }

    [RelayCommand]
    private void Clear()
    {
        TableFilter = string.Empty;
        TableCaseSensitive = false;
        ColumnFilter = string.Empty;
        ColumnCaseSensitive = false;
        ShowCreate = true;
        ShowRead = true;
        ShowUpdate = true;
        ShowDelete = true;
        ProgramIdFilter = string.Empty;
    }

    [RelayCommand]
    private void Close()
    {
        // TODO: Close window
    }
}
