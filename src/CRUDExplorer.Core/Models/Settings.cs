using System.Runtime.InteropServices;
using System.Text.Json;

namespace CRUDExplorer.Core.Models;

/// <summary>
/// アプリケーション設定（VB.NET clsSettings.vbから移植、Registry→JSON）
/// </summary>
public class Settings
{
    private const string DefaultEditor = "notepad";
    private const string DefaultNotepadPath = "notepad.exe";
    private const string DefaultSakuraPath = @"C:\Program Files (x86)\sakura\sakura.exe";
    private const string DefaultHidemaruPath = @"C:\Program Files\Hidemaru\Hidemaru.exe";
    private static readonly string DefaultVSCodePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "code"
        : "/usr/bin/code";
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
    /// VS Codeパス
    /// </summary>
    public string VSCodePath { get; set; } = DefaultVSCodePath;

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
    /// 認証サーバーURL
    /// </summary>
    public string AuthServerUrl { get; set; } = "https://localhost:5001/api/license/authenticate";

    /// <summary>
    /// SQLエディタのフォントファミリー
    /// </summary>
    public string SqlEditorFontFamily { get; set; } = "Consolas";

    /// <summary>
    /// SQLエディタのフォントサイズ
    /// </summary>
    public double SqlEditorFontSize { get; set; } = 13;

    /// <summary>
    /// SQLエディタの文字色（Hex）
    /// </summary>
    public string SqlForegroundColor { get; set; } = "#000000";

    /// <summary>
    /// SQLエディタの背景色（Hex）
    /// </summary>
    public string SqlBackgroundColor { get; set; } = "#FFFFFF";

    /// <summary>
    /// SQL予約語の色（Hex）
    /// </summary>
    public string SqlKeywordColor { get; set; } = "#0000FF";

    /// <summary>
    /// SQL文字列リテラルの色（Hex）
    /// </summary>
    public string SqlStringLiteralColor { get; set; } = "#FF0000";

    /// <summary>
    /// SQLコメントの色（Hex）
    /// </summary>
    public string SqlCommentColor { get; set; } = "#008000";

    /// <summary>
    /// SQLエディタのタブ文字数
    /// </summary>
    public int SqlEditorTabSize { get; set; } = 4;

    /// <summary>
    /// SQLエディタの行折り返し
    /// </summary>
    public bool SqlEditorWordWrap { get; set; } = true;

    /// <summary>
    /// ウィンドウサイズ保存用
    /// </summary>
    public Dictionary<string, WindowState> WindowStates { get; set; } = new();

    /// <summary>
    /// テーブル定義取込の接続設定（最後に使用した設定）
    /// </summary>
    public DbConnectionSettings? ImportDbConnection { get; set; }

    /// <summary>
    /// テーブル定義取込の接続履歴リスト
    /// </summary>
    public List<DbConnectionSettings> ConnectionHistory { get; set; } = new();

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

/// <summary>
/// DB接続設定（テーブル定義取込用）
/// </summary>
public class DbConnectionSettings
{
    public int DbTypeIndex { get; set; }
    public string Server { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// 暗号化されたパスワード（Base64）
    /// </summary>
    public string EncryptedPassword { get; set; } = string.Empty;

    /// <summary>
    /// 表示名（プルダウン用: "DB種別 UserName@Server:Port/Database"）
    /// </summary>
    public string DisplayName
    {
        get
        {
            var dbTypes = new[] { "MySQL", "PostgreSQL", "SQL Server", "Oracle", "SQLite", "MariaDB" };
            var dbType = DbTypeIndex >= 0 && DbTypeIndex < dbTypes.Length ? dbTypes[DbTypeIndex] : "Unknown";
            return $"{dbType} {UserName}@{Server}:{Port}/{Database}";
        }
    }

    public void SetPassword(string plainPassword)
    {
        if (string.IsNullOrEmpty(plainPassword))
        {
            EncryptedPassword = string.Empty;
            return;
        }
        var bytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
        var encrypted = System.Security.Cryptography.ProtectedData.Protect(
            bytes, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
        EncryptedPassword = Convert.ToBase64String(encrypted);
    }

    public string GetPassword()
    {
        if (string.IsNullOrEmpty(EncryptedPassword))
            return string.Empty;
        try
        {
            var encrypted = Convert.FromBase64String(EncryptedPassword);
            var decrypted = System.Security.Cryptography.ProtectedData.Unprotect(
                encrypted, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return System.Text.Encoding.UTF8.GetString(decrypted);
        }
        catch
        {
            return string.Empty;
        }
    }
}
