using System.Text.Json;

namespace CRUDExplorer.Core.Models;

/// <summary>
/// アプリケーション設定（VB.NET clsSettings.vbから移植、Registry→JSON）
/// </summary>
public class Settings
{
    private const string DefaultEditor = "notepad";
    private const string DefaultNotepadPath = "notepad.exe";
    private const string DefaultSakuraPath = "C:\\Program Files\\sakura\\sakura.exe";
    private const string DefaultHidemaruPath = "C:\\Program Files\\Hidemaru\\Hidemaru.exe";
    private const string DefaultVSCodePath = "/usr/bin/code";  // Mac/Linux対応
    private const string DefaultTextEditPath = "/System/Applications/TextEdit.app/Contents/MacOS/TextEdit";  // Mac対応
    private const string DefaultProgramIdPattern = "";

    /// <summary>
    /// リストダブルクリック時の動作
    /// </summary>
    public enum ListDoubleClickMode
    {
        ExecTextEditor = 0,
        AnalyzeQuery = 1,
        NoAction = 2
    }

    /// <summary>
    /// 外部エディタ種別
    /// </summary>
    public string TextEditor { get; set; } = DefaultEditor;

    /// <summary>
    /// Notepadパス
    /// </summary>
    public string NotepadPath { get; set; } = DefaultNotepadPath;

    /// <summary>
    /// Sakuraエディタパス
    /// </summary>
    public string SakuraPath { get; set; } = DefaultSakuraPath;

    /// <summary>
    /// 秀丸エディタパス
    /// </summary>
    public string HidemaruPath { get; set; } = DefaultHidemaruPath;

    /// <summary>
    /// VS Codeパス（Mac/Linux対応）
    /// </summary>
    public string VSCodePath { get; set; } = DefaultVSCodePath;

    /// <summary>
    /// TextEditパス（Mac対応）
    /// </summary>
    public string TextEditPath { get; set; } = DefaultTextEditPath;

    /// <summary>
    /// リストダブルクリック時の動作
    /// </summary>
    public ListDoubleClickMode DoubleClickMode { get; set; } = ListDoubleClickMode.ExecTextEditor;

    /// <summary>
    /// プログラムID抽出パターン（正規表現）
    /// </summary>
    public string ProgramIdPattern { get; set; } = DefaultProgramIdPattern;

    /// <summary>
    /// デバッグモード
    /// </summary>
    public bool DebugMode { get; set; } = false;

    /// <summary>
    /// ライセンスキー
    /// </summary>
    public string LicenseKey { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// ウィンドウサイズ保存用
    /// </summary>
    public Dictionary<string, WindowState> WindowStates { get; set; } = new();

    /// <summary>
    /// 設定ファイルパス
    /// </summary>
    private static string GetSettingsFilePath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var settingsDir = Path.Combine(appDataPath, "CRUDExplorer");
        Directory.CreateDirectory(settingsDir);
        return Path.Combine(settingsDir, "settings.json");
    }

    /// <summary>
    /// 設定を読み込む
    /// </summary>
    public static Settings Load()
    {
        var filePath = GetSettingsFilePath();
        if (File.Exists(filePath))
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var settings = JsonSerializer.Deserialize<Settings>(json);
                return settings ?? new Settings();
            }
            catch
            {
                return new Settings();
            }
        }
        return new Settings();
    }

    /// <summary>
    /// 設定を保存する
    /// </summary>
    public void Save()
    {
        var filePath = GetSettingsFilePath();
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// ウィンドウサイズを保存
    /// </summary>
    public void SaveWindowState(string windowName, int width, int height, int left, int top, bool isMaximized)
    {
        WindowStates[windowName] = new WindowState
        {
            Width = width,
            Height = height,
            Left = left,
            Top = top,
            IsMaximized = isMaximized
        };
        Save();
    }

    /// <summary>
    /// ウィンドウサイズを読み込む
    /// </summary>
    public WindowState? LoadWindowState(string windowName)
    {
        return WindowStates.TryGetValue(windowName, out var state) ? state : null;
    }
}

/// <summary>
/// ウィンドウの状態
/// </summary>
public class WindowState
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int Left { get; set; }
    public int Top { get; set; }
    public bool IsMaximized { get; set; }
}
