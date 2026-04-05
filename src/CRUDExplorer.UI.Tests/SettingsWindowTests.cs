using Avalonia.Headless.XUnit;
using CRUDExplorer.UI.ViewModels;
using CRUDExplorer.UI.Views;
using Xunit;

namespace CRUDExplorer.UI.Tests;

public class SettingsWindowTests
{
    [AvaloniaFact]
    public void SettingsWindow_Initializes_Successfully()
    {
        // Arrange & Act
        var window = new SettingsWindow
        {
            DataContext = new SettingsViewModel()
        };

        // Assert
        Assert.NotNull(window);
        Assert.NotNull(window.DataContext);
        Assert.IsType<SettingsViewModel>(window.DataContext);
    }

    [AvaloniaFact]
    public void SettingsViewModel_LoadsDefaultSettings()
    {
        // Arrange & Act
        var viewModel = new SettingsViewModel();

        // Assert
        Assert.NotNull(viewModel);
        // Check that editor path property is initialized
        Assert.NotNull(viewModel.NotepadPath);
        Assert.NotNull(viewModel.ProgramIdPattern);
    }

    [AvaloniaFact]
    public void SettingsViewModel_SaveCommand_Exists()
    {
        // Arrange
        var viewModel = new SettingsViewModel();

        // Assert
        Assert.NotNull(viewModel.SaveCommand);
    }
}
