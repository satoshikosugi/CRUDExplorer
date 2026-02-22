using Avalonia.Headless.XUnit;
using CRUDExplorer.UI.ViewModels;
using Xunit;

namespace CRUDExplorer.UI.Tests;

public class MainWindowTests
{
    [AvaloniaFact]
    public void MainWindow_ViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new MainWindowViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [AvaloniaFact]
    public void MainWindow_ViewModel_HasExpectedProperties()
    {
        // Arrange
        var viewModel = new MainWindowViewModel();

        // Assert
        Assert.NotNull(viewModel);
        // Check that observable properties are initialized
        Assert.NotNull(viewModel.StatusMessage);
    }
}
