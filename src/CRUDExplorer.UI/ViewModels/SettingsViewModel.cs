using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.UI.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly Action _closeWindow;
    private readonly Func<Task<string?>>? _filePicker;
    private readonly Func<string, Task>? _showError;

    // ─── 外部エディタ設定 ──────────────────────────────────────────

    [ObservableProperty]
    private bool _isNotepadSelected;

    [ObservableProperty]
    private bool _isSakuraEditorSelected;

    [ObservableProperty]
    private bool _isHidemaruSelected;

    [ObservableProperty]
    private bool _isVSCodeSelected;

    [ObservableProperty]
    private string _notepadPath = "notepad.exe";

    [ObservableProperty]
    private string _sakuraPath = @"C:\Program Files (x86)\sakura\sakura.exe";

    [ObservableProperty]
    private string _hidemaruPath = @"C:\Program Files\Hidemaru\Hidemaru.exe";

    [ObservableProperty]
    private string _vsCodePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "code" : "/usr/bin/code";

    // ─── ダブルクリック動作 ────────────────────────────────────────

    [ObservableProperty]
    private bool _isOpenInEditorMode = true;

    [ObservableProperty]
    private bool _isShowAnalysisMode;

    [ObservableProperty]
    private bool _isNoActionMode;

    // ─── CRUD解析設定 ──────────────────────────────────────────────

    [ObservableProperty]
    private string _programIdPattern = string.Empty;

    public ObservableCollection<ProgramIdPreset> ProgramIdPresets { get; } = new()
    {
        new("（プリセットから選択）", ""),
        new("ファイル名をそのまま使用（空欄）", ""),
        new("3文字+4桁 (例: ABC1234)", @"^([A-Z]{3}\d{4})"),
        new("アンダースコア前 (例: PROG_001→PROG)", @"^(\w+?)_"),
        new("先頭英字+数字 (例: SP001)", @"^([A-Za-z]+\d+)"),
        new("VB.NET例 (...[OB]Z\\d\\d\\d\\d)", @"(...[OB]Z\d\d\d\d)"),
    };

    [ObservableProperty]
    private ProgramIdPreset? _selectedPreset;

    partial void OnSelectedPresetChanged(ProgramIdPreset? value)
    {
        if (value != null && value != ProgramIdPresets[0])
        {
            ProgramIdPattern = value.Pattern;
        }
    }

    // ─── SQLエディタ設定 ───────────────────────────────────────────

    [ObservableProperty]
    private string _sqlEditorFontFamily = "Consolas";

    [ObservableProperty]
    private double _sqlEditorFontSize = 13;

    [ObservableProperty]
    private int _sqlEditorTabSize = 4;

    [ObservableProperty]
    private bool _sqlEditorWordWrap = true;

    // システムフォント一覧
    public ObservableCollection<string> SystemFontFamilies { get; } = new();

    // ─── カラー設定（5色） ────────────────────────────────────────

    [ObservableProperty]
    private string _sqlForegroundColor = "#000000";

    partial void OnSqlForegroundColorChanged(string value)
    {
        OnPropertyChanged(nameof(SqlForegroundPreviewBrush));
        NotifyPreviewChanged();
    }

    [ObservableProperty]
    private string _sqlBackgroundColor = "#FFFFFF";

    partial void OnSqlBackgroundColorChanged(string value)
    {
        OnPropertyChanged(nameof(SqlBackgroundPreviewBrush));
        NotifyPreviewChanged();
    }

    [ObservableProperty]
    private string _sqlKeywordColor = "#0000FF";

    partial void OnSqlKeywordColorChanged(string value)
    {
        OnPropertyChanged(nameof(SqlKeywordPreviewBrush));
        NotifyPreviewChanged();
    }

    [ObservableProperty]
    private string _sqlCommentColor = "#008000";

    partial void OnSqlCommentColorChanged(string value)
    {
        OnPropertyChanged(nameof(SqlCommentPreviewBrush));
        NotifyPreviewChanged();
    }

    [ObservableProperty]
    private string _sqlStringLiteralColor = "#FF0000";

    partial void OnSqlStringLiteralColorChanged(string value)
    {
        OnPropertyChanged(nameof(SqlStringLiteralPreviewBrush));
        NotifyPreviewChanged();
    }

    public IBrush SqlForegroundPreviewBrush => ParseBrush(SqlForegroundColor, Brushes.Black);
    public IBrush SqlBackgroundPreviewBrush => ParseBrush(SqlBackgroundColor, Brushes.White);
    public IBrush SqlKeywordPreviewBrush => ParseBrush(SqlKeywordColor, Brushes.Blue);
    public IBrush SqlStringLiteralPreviewBrush => ParseBrush(SqlStringLiteralColor, Brushes.Red);
    public IBrush SqlCommentPreviewBrush => ParseBrush(SqlCommentColor, Brushes.Green);

    /// <summary>プレビュー更新通知用イベント</summary>
    public event Action? PreviewChanged;

    private void NotifyPreviewChanged() => PreviewChanged?.Invoke();

    partial void OnSqlEditorFontFamilyChanged(string value) => NotifyPreviewChanged();
    partial void OnSqlEditorFontSizeChanged(double value) => NotifyPreviewChanged();
    partial void OnSqlEditorTabSizeChanged(int value) => NotifyPreviewChanged();
    partial void OnSqlEditorWordWrapChanged(bool value) => NotifyPreviewChanged();

    private static IBrush ParseBrush(string hex, IBrush fallback)
    {
        try
        {
            return Color.TryParse(hex, out var c) ? new SolidColorBrush(c) : fallback;
        }
        catch { return fallback; }
    }

    [RelayCommand]
    private void ResetColors()
    {
        SqlForegroundColor = "#000000";
        SqlBackgroundColor = "#FFFFFF";
        SqlKeywordColor = "#0000FF";
        SqlStringLiteralColor = "#FF0000";
        SqlCommentColor = "#008000";
    }

    // ─── 詳細設定 ─────────────────────────────────────────────────

    [ObservableProperty]
    private bool _debugMode;

    public SettingsViewModel(
        Action? closeWindow = null,
        Func<Task<string?>>? filePicker = null,
        Func<string, Task>? showError = null)
    {
        _closeWindow = closeWindow ?? (() => { });
        _filePicker = filePicker;
        _showError = showError;
        LoadSystemFonts();
        LoadSettings();
    }

    private void LoadSystemFonts()
    {
        try
        {
            var fonts = FontManager.Current.SystemFonts
                .Select(f => f.Name)
                .OrderBy(n => n)
                .ToList();
            foreach (var f in fonts)
                SystemFontFamilies.Add(f);
        }
        catch
        {
            SystemFontFamilies.Add("Consolas");
            SystemFontFamilies.Add("Courier New");
        }
    }

    // ─── コマンド ─────────────────────────────────────────────────

    [RelayCommand]
    private async Task BrowseNotepadPath()
    {
        var path = await PickFile();
        if (path != null) NotepadPath = path;
    }

    [RelayCommand]
    private async Task BrowseSakuraPath()
    {
        var path = await PickFile();
        if (path != null) SakuraPath = path;
    }

    [RelayCommand]
    private async Task BrowseHidemaruPath()
    {
        var path = await PickFile();
        if (path != null) HidemaruPath = path;
    }

    [RelayCommand]
    private async Task BrowseVSCodePath()
    {
        var path = await PickFile();
        if (path != null) VsCodePath = path;
    }

    private async Task<string?> PickFile()
    {
        if (_filePicker != null)
            return await _filePicker();
        return null;
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            var settings = Settings.Load();

            settings.TextEditor = GetSelectedEditor();
            settings.NotepadPath = NotepadPath;
            settings.SakuraPath = SakuraPath;
            settings.HidemaruPath = HidemaruPath;
            settings.VSCodePath = VsCodePath;

            settings.DoubleClickMode = IsShowAnalysisMode
                ? Settings.ListDoubleClickMode.AnalyzeQuery
                : IsNoActionMode
                    ? Settings.ListDoubleClickMode.NoAction
                    : Settings.ListDoubleClickMode.ExecTextEditor;

            settings.ProgramIdPattern = ProgramIdPattern;

            settings.SqlEditorFontFamily = SqlEditorFontFamily;
            settings.SqlEditorFontSize = SqlEditorFontSize;
            settings.SqlEditorTabSize = SqlEditorTabSize;
            settings.SqlEditorWordWrap = SqlEditorWordWrap;
            settings.SqlForegroundColor = SqlForegroundColor;
            settings.SqlBackgroundColor = SqlBackgroundColor;
            settings.SqlKeywordColor = SqlKeywordColor;
            settings.SqlStringLiteralColor = SqlStringLiteralColor;
            settings.SqlCommentColor = SqlCommentColor;

            settings.DebugMode = DebugMode;

            settings.Save();
            _closeWindow();
        }
        catch (Exception ex)
        {
            if (_showError != null)
                _ = _showError($"設定の保存に失敗しました: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _closeWindow();
    }

    // ─── 設定の読み込み ───────────────────────────────────────────

    private void LoadSettings()
    {
        var settings = Settings.Load();

        SetEditorSelection(settings.TextEditor);
        NotepadPath = settings.NotepadPath;
        SakuraPath = settings.SakuraPath;
        HidemaruPath = settings.HidemaruPath;
        VsCodePath = settings.VSCodePath;

        IsOpenInEditorMode = settings.DoubleClickMode == Settings.ListDoubleClickMode.ExecTextEditor;
        IsShowAnalysisMode = settings.DoubleClickMode == Settings.ListDoubleClickMode.AnalyzeQuery;
        IsNoActionMode = settings.DoubleClickMode == Settings.ListDoubleClickMode.NoAction;

        ProgramIdPattern = settings.ProgramIdPattern ?? string.Empty;

        SqlEditorFontFamily = settings.SqlEditorFontFamily;
        SqlEditorFontSize = settings.SqlEditorFontSize;
        SqlEditorTabSize = settings.SqlEditorTabSize;
        SqlEditorWordWrap = settings.SqlEditorWordWrap;
        SqlForegroundColor = settings.SqlForegroundColor;
        SqlBackgroundColor = settings.SqlBackgroundColor;
        SqlKeywordColor = settings.SqlKeywordColor;
        SqlStringLiteralColor = settings.SqlStringLiteralColor;
        SqlCommentColor = settings.SqlCommentColor;

        DebugMode = settings.DebugMode;
    }

    private string GetSelectedEditor()
    {
        if (IsSakuraEditorSelected) return "SakuraEditor";
        if (IsHidemaruSelected) return "Hidemaru";
        if (IsVSCodeSelected) return "VSCode";
        return "Notepad";
    }

    private void SetEditorSelection(string editor)
    {
        IsNotepadSelected = false;
        IsSakuraEditorSelected = false;
        IsHidemaruSelected = false;
        IsVSCodeSelected = false;

        switch (editor?.ToLowerInvariant())
        {
            case "sakuraeditor" or "sakura":
                IsSakuraEditorSelected = true;
                break;
            case "hidemaru":
                IsHidemaruSelected = true;
                break;
            case "vscode":
                IsVSCodeSelected = true;
                break;
            default:
                IsNotepadSelected = true;
                break;
        }
    }
}

public record ProgramIdPreset(string DisplayName, string Pattern)
{
    public override string ToString() => DisplayName;
}
