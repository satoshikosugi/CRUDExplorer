using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CRUDExplorer.UI.Tests.Stubs;
using CRUDExplorer.UI.ViewModels;
using CRUDExplorer.UI.Views;
using Xunit;
using Xunit.Abstractions;

namespace CRUDExplorer.UI.Tests;

public class MainWindowTests
{
    private readonly ITestOutputHelper _output;

    public MainWindowTests(ITestOutputHelper output)
    {
        _output = output;
    }

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

    [AvaloniaFact]
    public void MainWindow_DataGrid_HasTemplate()
    {
        // Arrange
        var window = new MainWindow();
        var service = new NullWindowService { MainWindow = window };
        var vm = new MainWindowViewModel(service);
        window.DataContext = vm;

        // Force layout
        window.Show();
        window.Width = 1200;
        window.Height = 800;

        // Act - Find the DataGrid
        var grid = window.FindControl<DataGrid>("CrudMatrixGrid");

        _output.WriteLine($"DataGrid found: {grid != null}");
        _output.WriteLine($"DataGrid Template: {grid?.Template != null}");
        _output.WriteLine($"DataGrid IsVisible: {grid?.IsVisible}");
        _output.WriteLine($"DataGrid Bounds: {grid?.Bounds}");

        // Assert
        Assert.NotNull(grid);
        Assert.True(grid.Template != null, "DataGrid has no Template - the DataGrid theme may be missing. Add <StyleInclude Source=\"avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml\"/> to App.axaml");

        window.Close();
    }

    [AvaloniaFact]
    public async System.Threading.Tasks.Task MainWindow_DataGrid_ShowsColumnsAfterLoad()
    {
        // Arrange - Run analysis pipeline first to generate TSV files
        var testSampleDir = FindTestSampleDir();
        if (testSampleDir == null)
        {
            _output.WriteLine("TestSampleが見つかりません。スキップします。");
            return;
        }

        var outputDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"CRUDExplorer_MatrixTest_{System.Guid.NewGuid():N}");
        System.IO.Directory.CreateDirectory(outputDir);

        try
        {
            // Run analysis pipeline
            var makeCrudVm = new MakeCrudViewModel();
            makeCrudVm.SourcePath = testSampleDir;
            makeCrudVm.DestPath = outputDir;
            makeCrudVm.ProcessAll = true;
            await makeCrudVm.ExecuteAnalysisCommand.ExecuteAsync(null);

            // Create MainWindow and load the data
            var window = new MainWindow();
            var service = new NullWindowService { MainWindow = window };
            var mainVm = new MainWindowViewModel(service);
            window.DataContext = mainVm;
            window.Show();
            window.Width = 1200;
            window.Height = 800;

            // Track if MatrixColumnsChanged fires
            bool columnsChanged = false;
            mainVm.MatrixColumnsChanged += (s, e) => columnsChanged = true;

            // Load data
            mainVm.SourcePath = outputDir;
            var loadMethod = typeof(MainWindowViewModel).GetMethod(
                "LoadCrudDataAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.NotNull(loadMethod);
            await (System.Threading.Tasks.Task)loadMethod.Invoke(mainVm, null)!;

            _output.WriteLine($"MatrixColumnsChanged fired: {columnsChanged}");
            _output.WriteLine($"MatrixRows count: {mainVm.MatrixRows.Count}");
            _output.WriteLine($"MatrixHeaders count: {mainVm.MatrixHeaders.Length}");

            // Find the DataGrid and check its state
            var grid = window.FindControl<DataGrid>("CrudMatrixGrid");
            Assert.NotNull(grid);

            _output.WriteLine($"DataGrid Template: {grid.Template != null}");
            _output.WriteLine($"DataGrid Columns count: {grid.Columns.Count}");
            _output.WriteLine($"DataGrid ItemsSource type: {grid.ItemsSource?.GetType().Name}");
            _output.WriteLine($"DataGrid IsVisible: {grid.IsVisible}");
            _output.WriteLine($"DataGrid Bounds: {grid.Bounds}");
            _output.WriteLine($"StatusMessage: {mainVm.StatusMessage}");

            // Assert
            Assert.True(columnsChanged, "MatrixColumnsChanged event was NOT fired");
            Assert.True(mainVm.MatrixRows.Count > 0, "MatrixRows is empty");
            Assert.True(grid.Columns.Count > 0, "DataGrid has no columns - RebuildMatrixColumns failed");
            Assert.NotNull(grid.ItemsSource);

            window.Close();
        }
        finally
        {
            try { System.IO.Directory.Delete(outputDir, recursive: true); } catch { }
        }
    }

    private static string? FindTestSampleDir()
    {
        var dir = System.AppContext.BaseDirectory;
        for (int i = 0; i < 8; i++)
        {
            var candidate = System.IO.Path.Combine(dir, "TestSample");
            if (System.IO.Directory.Exists(candidate)) return candidate;
            var parent = System.IO.Directory.GetParent(dir)?.FullName;
            if (parent == null) break;
            dir = parent;
        }
        return null;
    }
}
