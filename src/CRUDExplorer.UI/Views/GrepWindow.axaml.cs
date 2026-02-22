using Avalonia.Controls;
using Avalonia.Input;
using CRUDExplorer.UI.ViewModels;

namespace CRUDExplorer.UI.Views;

public partial class GrepWindow : Window
{
    public GrepWindow()
    {
        InitializeComponent();
        DataContext = new GrepViewModel();
    }

    private void OnResultDoubleClicked(object? sender, TappedEventArgs e)
    {
        if (DataContext is GrepViewModel viewModel && viewModel.SelectedResult != null)
        {
            viewModel.OpenInEditorCommand.Execute(null);
        }
    }
}
