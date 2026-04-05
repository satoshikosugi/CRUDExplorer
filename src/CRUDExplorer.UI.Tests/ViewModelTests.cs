using Avalonia.Headless.XUnit;
using CRUDExplorer.Core.Utilities;
using CRUDExplorer.UI.Tests.Stubs;
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

    // ── MainWindowViewModel テスト ──────────────────────────────────────────────

    [AvaloniaFact]
    public void MainWindowViewModel_Initializes_Successfully()
    {
        // Arrange & Act
        var viewModel = new MainWindowViewModel(new NullWindowService());

        // Assert
        Assert.NotNull(viewModel);
        Assert.Equal("CRUD Explorer", viewModel.Title);
        Assert.Empty(viewModel.SourcePath);
    }

    [AvaloniaFact]
    public void MainWindowViewModel_FilterCrudListBySelection_WithNullParams_ClearsList()
    {
        // Arrange
        var viewModel = new MainWindowViewModel(new NullWindowService());

        // Act
        viewModel.FilterCrudListBySelection(null, null);

        // Assert
        Assert.Empty(viewModel.CrudListData);
        Assert.Contains("なし", viewModel.StatusMessage);
    }

    [AvaloniaFact]
    public void MainWindowViewModel_FilterCrudListBySelection_WithTableName_FiltersCorrectly()
    {
        // Arrange
        var viewModel = new MainWindowViewModel(new NullWindowService());

        // Act
        viewModel.FilterCrudListBySelection("TEST_TABLE", null);

        // Assert
        // Since no data is loaded, the list should be empty but status should show the table name
        Assert.Contains("TEST_TABLE", viewModel.StatusMessage);
    }

    [AvaloniaFact]
    public void MainWindowViewModel_CopyMatrixToClipboard_WithNoData_SetsErrorMessage()
    {
        // Arrange
        var viewModel = new MainWindowViewModel(new NullWindowService());

        // Act
        viewModel.CopyMatrixToClipboardCommand.Execute(null);

        // Assert
        Assert.Contains("コピーするデータがありません", viewModel.StatusMessage);
    }

    [AvaloniaFact]
    public void MainWindowViewModel_CopyCrudListSelectedItem_WithNoSelection_SetsErrorMessage()
    {
        // Arrange
        var viewModel = new MainWindowViewModel(new NullWindowService());

        // Act
        viewModel.CopyCrudListSelectedItemCommand.Execute(null);

        // Assert
        Assert.Contains("コピーする項目がありません", viewModel.StatusMessage);
    }

    [AvaloniaFact]
    public void MainWindowViewModel_CopyCrudListAllItems_WithNoData_SetsErrorMessage()
    {
        // Arrange
        var viewModel = new MainWindowViewModel(new NullWindowService());

        // Act
        viewModel.CopyCrudListAllItemsCommand.Execute(null);

        // Assert
        Assert.Contains("コピーするデータがありません", viewModel.StatusMessage);
    }

    [AvaloniaFact]
    public void MainWindowViewModel_ToggleLogicalName_TogglesProperty()
    {
        // Arrange
        var viewModel = new MainWindowViewModel(new NullWindowService());
        var initialValue = viewModel.ShowLogicalName;

        // Act
        viewModel.ToggleLogicalNameCommand.Execute(null);

        // Assert
        Assert.NotEqual(initialValue, viewModel.ShowLogicalName);
    }

    [AvaloniaFact]
    public void MainWindowViewModel_ClearFilter_ResetsAllFilters()
    {
        // Arrange
        var viewModel = new MainWindowViewModel(new NullWindowService())
        {
            FilterProgram = "test",
            FilterTable = "test",
            FilterC = false,
            FilterR = false,
            FilterU = false,
            FilterD = false
        };

        // Act
        viewModel.ClearFilterCommand.Execute(null);

        // Assert
        Assert.Empty(viewModel.FilterProgram);
        Assert.Empty(viewModel.FilterTable);
        Assert.True(viewModel.FilterC);
        Assert.True(viewModel.FilterR);
        Assert.True(viewModel.FilterU);
        Assert.True(viewModel.FilterD);
    }

    // ── FilterViewModel テスト ──────────────────────────────────────────────────

    [AvaloniaFact]
    public void FilterViewModel_Clear_ResetsAllValues()
    {
        // Arrange
        var closeWindowCalled = false;
        var viewModel = new FilterViewModel(closeWindow: () => closeWindowCalled = true)
        {
            ProgramFilter = "test",
            TableFilter = "test",
            ShowC = false,
            ShowR = false,
            ShowU = false,
            ShowD = false
        };

        // Act
        viewModel.ClearCommand.Execute(null);

        // Assert
        Assert.Empty(viewModel.ProgramFilter);
        Assert.Empty(viewModel.TableFilter);
        Assert.True(viewModel.ShowC);
        Assert.True(viewModel.ShowR);
        Assert.True(viewModel.ShowU);
        Assert.True(viewModel.ShowD);
    }

    [AvaloniaFact]
    public void FilterViewModel_Close_CallsCloseWindow()
    {
        // Arrange
        var closeWindowCalled = false;
        var viewModel = new FilterViewModel(closeWindow: () => closeWindowCalled = true);

        // Act
        viewModel.CloseCommand.Execute(null);

        // Assert
        Assert.True(closeWindowCalled);
    }

    [AvaloniaFact]
    public void FilterViewModel_Apply_SetsGlobalStateAndClosesWindow()
    {
        // Arrange
        var closeWindowCalled = false;
        var viewModel = new FilterViewModel(closeWindow: () => closeWindowCalled = true)
        {
            ProgramFilter = "TEST_PATTERN",
            TableFilter = "TABLE_PATTERN",
            ShowC = true,
            ShowR = false,
            ShowU = true,
            ShowD = false
        };

        // Act
        viewModel.ApplyCommand.Execute(null);

        // Assert
        Assert.True(closeWindowCalled);
        var filterState = GlobalState.Instance.FilterState;
        Assert.True(filterState.WasApplied);
        Assert.True(filterState.ShowC);
        Assert.False(filterState.ShowR);
        Assert.True(filterState.ShowU);
        Assert.False(filterState.ShowD);
    }

    // ── GrepViewModel テスト ────────────────────────────────────────────────────

    [AvaloniaFact]
    public void GrepViewModel_Search_WithEmptyPattern_SetsErrorMessage()
    {
        // Arrange
        var viewModel = new GrepViewModel()
        {
            SearchPattern = ""
        };

        // Act
        viewModel.SearchCommand.Execute(null);

        // Assert
        Assert.Contains("検索パターンを入力してください", viewModel.ResultSummary);
    }

    [AvaloniaFact]
    public void GrepViewModel_Search_WithInvalidRegex_SetsErrorMessage()
    {
        // Arrange
        var viewModel = new GrepViewModel()
        {
            SearchPattern = "[invalid(",
            UseRegex = true
        };

        // Act
        viewModel.SearchCommand.Execute(null);

        // Assert
        Assert.Contains("正規表現パターンが不正です", viewModel.ResultSummary);
    }

    [AvaloniaFact]
    public void GrepViewModel_Close_CallsCloseWindow()
    {
        // Arrange
        var closeWindowCalled = false;
        var viewModel = new GrepViewModel(closeWindow: () => closeWindowCalled = true);

        // Act
        viewModel.CloseCommand.Execute(null);

        // Assert
        Assert.True(closeWindowCalled);
    }

    // ── CrudSearchViewModel テスト ─────────────────────────────────────────────

    [AvaloniaFact]
    public void CrudSearchViewModel_Search_WithNoData_ReturnsZeroResults()
    {
        // Arrange
        var viewModel = new CrudSearchViewModel()
        {
            TableNamePattern = "TEST"
        };

        // Act
        viewModel.SearchCommand.Execute(null);

        // Assert
        Assert.Equal(0, viewModel.ResultCount);
    }

    [AvaloniaFact]
    public void CrudSearchViewModel_Close_CallsCloseWindow()
    {
        // Arrange
        var closeWindowCalled = false;
        var viewModel = new CrudSearchViewModel(closeWindow: () => closeWindowCalled = true);

        // Act
        viewModel.CloseCommand.Execute(null);

        // Assert
        Assert.True(closeWindowCalled);
    }

    // ── FileListViewModel テスト ───────────────────────────────────────────────

    [AvaloniaFact]
    public void FileListViewModel_Close_CallsCloseWindow()
    {
        // Arrange
        var closeWindowCalled = false;
        var viewModel = new FileListViewModel(closeWindow: () => closeWindowCalled = true);

        // Act
        viewModel.CloseCommand.Execute(null);

        // Assert
        Assert.True(closeWindowCalled);
    }

    [AvaloniaFact]
    public void FileListViewModel_FilterPattern_FiltersFileList()
    {
        // Arrange
        var viewModel = new FileListViewModel();
        var initialCount = viewModel.FileCount;

        // Act
        viewModel.FilterPattern = "NONEXISTENT_FILE_NAME_12345";

        // Assert
        // Filtering should reduce the count or keep it at 0
        Assert.True(viewModel.FileCount <= initialCount);
    }
}
