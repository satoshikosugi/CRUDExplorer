using Avalonia.Headless.XUnit;
using CRUDExplorer.UI.ViewModels;
using CRUDExplorer.UI.Views;
using Xunit;

namespace CRUDExplorer.UI.Tests;

public class ViewModelTests
{
    [AvaloniaFact]
    public void MakeCrudViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new MakeCrudViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [AvaloniaFact]
    public void AnalyzeQueryViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new AnalyzeQueryViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [AvaloniaFact]
    public void VersionViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new VersionViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [AvaloniaFact]
    public void CrudSearchViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new CrudSearchViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [AvaloniaFact]
    public void FileListViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new FileListViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [AvaloniaFact]
    public void FilterViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new FilterViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [AvaloniaFact]
    public void GrepViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new GrepViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }

    [AvaloniaFact]
    public void TableDefinitionViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new TableDefinitionViewModel();

        // Assert
        Assert.NotNull(viewModel);
    }
}
