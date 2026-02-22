using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRUDExplorer.UI.ViewModels;

public partial class FileListViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<FileItem> _files = new();

    [ObservableProperty]
    private FileItem? _selectedFile;

    [ObservableProperty]
    private string _filterPattern = string.Empty;

    [ObservableProperty]
    private int _fileCount = 0;

    public FileListViewModel()
    {
        // TODO: Load initial file list
    }

    [RelayCommand]
    private void Refresh()
    {
        // TODO: Refresh file list from source folder
        // - Use GlobalState.SourcePath or similar
        // - Apply FilterPattern
        // - Populate Files collection

        FileCount = Files.Count;
    }

    [RelayCommand]
    private void Open()
    {
        if (SelectedFile == null) return;

        // TODO: Open selected file in external editor
        // - Use ExternalEditorLauncher
    }

    [RelayCommand]
    private void Close()
    {
        // TODO: Close window
    }

    partial void OnFilterPatternChanged(string value)
    {
        Refresh();
    }
}

public class FileItem
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileSize { get; set; } = string.Empty;
    public string LastModified { get; set; } = string.Empty;
}
