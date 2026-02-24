using Avalonia.Headless.XUnit;
using CRUDExplorer.UI.Tests.Stubs;
using CRUDExplorer.UI.ViewModels;
using Xunit;

namespace CRUDExplorer.UI.Tests;

public class MainWindowTests
{
    [AvaloniaFact]
    public void MainWindow_ViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new MainWindowViewModel(new NullWindowService());

        // Assert
        Assert.NotNull(viewModel);
    }

    [AvaloniaFact]
    public void MainWindow_ViewModel_HasExpectedProperties()
    {
        // Arrange
        var viewModel = new MainWindowViewModel(new NullWindowService());

        // Assert
        Assert.NotNull(viewModel.StatusMessage);
        Assert.NotNull(viewModel.MatrixRows);
        Assert.NotNull(viewModel.CrudListData);
    }
}
