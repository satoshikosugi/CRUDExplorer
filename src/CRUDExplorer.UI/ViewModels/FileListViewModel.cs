using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;
using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.UI.ViewModels;

public partial class FileListViewModel : ViewModelBase
{
    private readonly Action _closeWindow;

    [ObservableProperty]
    private ObservableCollection<FileItem> _files = new();

    [ObservableProperty]
    private FileItem? _selectedFile;

    [ObservableProperty]
    private string _filterPattern = string.Empty;

    [ObservableProperty]
    private int _fileCount = 0;

    public FileListViewModel(Action? closeWindow = null)
    {
        _closeWindow = closeWindow ?? (() => { });
        LoadFiles();
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadFiles();
        FileCount = Files.Count;
    }

    [RelayCommand]
    private void Open()
    {
        if (SelectedFile == null) return;

        var settings = Settings.Load();
        var launcher = new ExternalEditorLauncher(settings);
        launcher.RunTextEditor(SelectedFile.FilePath);
    }

    [RelayCommand]
    private void Close()
    {
        _closeWindow();
    }

    partial void OnFilterPatternChanged(string value)
    {
        Refresh();
    }

    private void LoadFiles()
    {
        Files.Clear();

        var state = GlobalState.Instance;
        var programNames = state.ProgramNames;

        foreach (var kvp in state.Files)
        {
            var fileName = kvp.Value?.ToString() ?? kvp.Key;

            // フィルタ適用
            if (!string.IsNullOrWhiteSpace(FilterPattern)
                && !fileName.Contains(FilterPattern, StringComparison.OrdinalIgnoreCase))
                continue;

            var programId = Path.GetFileNameWithoutExtension(fileName);
            if (programNames.TryGetValue(programId, out var programName))
                programId = $"{programName}({programId})";

            var fileInfo = new System.IO.FileInfo(fileName);
            Files.Add(new FileItem
            {
                FileName = programId,
                FilePath = fileName,
                FileSize = fileInfo.Exists ? $"{fileInfo.Length / 1024} KB" : string.Empty,
                LastModified = fileInfo.Exists
                    ? fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                    : string.Empty
            });
        }

        FileCount = Files.Count;
    }
}

public class FileItem
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileSize { get; set; } = string.Empty;
    public string LastModified { get; set; } = string.Empty;
}
