using System;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.UI.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    // Editor Selection
    [ObservableProperty]
    private bool _isNotepadSelected = false;

    [ObservableProperty]
    private bool _isSakuraEditorSelected = false;

    [ObservableProperty]
    private bool _isHidemaruSelected = false;

    [ObservableProperty]
    private bool _isVSCodeSelected = true;

    [ObservableProperty]
    private bool _isTextEditSelected = false;

    [ObservableProperty]
    private bool _isNanoSelected = false;

    [ObservableProperty]
    private bool _isVimSelected = false;

    [ObservableProperty]
    private string _editorPath = string.Empty;

    // Double Click Mode
    [ObservableProperty]
    private bool _isOpenInEditorMode = true;

    [ObservableProperty]
    private bool _isShowAnalysisMode = false;

    [ObservableProperty]
    private bool _isNoActionMode = false;

    // Program ID Pattern
    [ObservableProperty]
    private string _programIdPattern = @"^[A-Z]{3}[0-9]{4}$";

    // Debug Mode
    [ObservableProperty]
    private bool _debugMode = false;

    public SettingsViewModel()
    {
        LoadSettings();
        SetDefaultEditorBasedOnOS();
    }

    [RelayCommand]
    private void BrowseEditorPath()
    {
        // TODO: Implement file picker dialog for editor executable
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            // Load current settings
            var settings = Settings.Load();

            // Update editor selection
            settings.TextEditor = GetSelectedEditor();

            // Update editor paths based on selection
            if (IsNotepadSelected)
                settings.NotepadPath = string.IsNullOrEmpty(EditorPath) ? "notepad.exe" : EditorPath;
            else if (IsSakuraEditorSelected)
                settings.SakuraPath = EditorPath;
            else if (IsHidemaruSelected)
                settings.HidemaruPath = EditorPath;
            else if (IsVSCodeSelected)
                settings.VSCodePath = string.IsNullOrEmpty(EditorPath) ? "/usr/bin/code" : EditorPath;
            else if (IsTextEditSelected)
                settings.TextEditPath = EditorPath;

            // Update other settings
            settings.DoubleClickMode = (Settings.ListDoubleClickMode)GetDoubleClickMode();
            settings.ProgramIdPattern = ProgramIdPattern;
            settings.DebugMode = DebugMode;

            // Save settings
            settings.Save();

            // TODO: Close window with DialogResult = OK
        }
        catch (Exception ex)
        {
            // TODO: Show error dialog
            Console.WriteLine($"設定の保存に失敗しました: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        // TODO: Close window without saving
    }

    private void LoadSettings()
    {
        try
        {
            var settings = Settings.Load();

            // Set editor selection based on TextEditor
            SetEditorSelection(settings.TextEditor);

            // Load editor path based on selected editor
            switch (settings.TextEditor.ToLower())
            {
                case "notepad":
                    EditorPath = settings.NotepadPath;
                    break;
                case "sakuraeditor":
                    EditorPath = settings.SakuraPath;
                    break;
                case "hidemaru":
                    EditorPath = settings.HidemaruPath;
                    break;
                case "vscode":
                    EditorPath = settings.VSCodePath;
                    break;
                case "textedit":
                    EditorPath = settings.TextEditPath;
                    break;
            }

            // Load other settings
            SetDoubleClickModeSelection((int)settings.DoubleClickMode);
            ProgramIdPattern = settings.ProgramIdPattern ?? @"^[A-Z]{3}[0-9]{4}$";
            DebugMode = settings.DebugMode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"設定の読み込みに失敗しました: {ex.Message}");
        }
    }

    private void SetDefaultEditorBasedOnOS()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            IsNotepadSelected = true;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            IsTextEditSelected = true;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            IsNanoSelected = true;
        }
    }

    private string GetSelectedEditor()
    {
        if (IsNotepadSelected) return "Notepad";
        if (IsSakuraEditorSelected) return "SakuraEditor";
        if (IsHidemaruSelected) return "Hidemaru";
        if (IsVSCodeSelected) return "VSCode";
        if (IsTextEditSelected) return "TextEdit";
        if (IsNanoSelected) return "nano";
        if (IsVimSelected) return "vim";
        return "VSCode"; // Default
    }

    private void SetEditorSelection(string editor)
    {
        // Reset all
        IsNotepadSelected = false;
        IsSakuraEditorSelected = false;
        IsHidemaruSelected = false;
        IsVSCodeSelected = false;
        IsTextEditSelected = false;
        IsNanoSelected = false;
        IsVimSelected = false;

        // Set selected
        switch (editor)
        {
            case "Notepad": IsNotepadSelected = true; break;
            case "SakuraEditor": IsSakuraEditorSelected = true; break;
            case "Hidemaru": IsHidemaruSelected = true; break;
            case "VSCode": IsVSCodeSelected = true; break;
            case "TextEdit": IsTextEditSelected = true; break;
            case "nano": IsNanoSelected = true; break;
            case "vim": IsVimSelected = true; break;
            default: IsVSCodeSelected = true; break;
        }
    }

    private int GetDoubleClickMode()
    {
        if (IsOpenInEditorMode) return 0;
        if (IsShowAnalysisMode) return 1;
        if (IsNoActionMode) return 2;
        return 0; // Default
    }

    private void SetDoubleClickModeSelection(int mode)
    {
        IsOpenInEditorMode = mode == 0;
        IsShowAnalysisMode = mode == 1;
        IsNoActionMode = mode == 2;
    }
}
