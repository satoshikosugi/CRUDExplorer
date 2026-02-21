using System.Diagnostics;
using System.Runtime.InteropServices;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.Core.Utilities;

/// <summary>
/// 外部テキストエディタ起動（VB.NET CommonModule.vbのRunTextEditor相当）
/// Mac/Linux/Windows対応
/// </summary>
public class ExternalEditorLauncher
{
    private readonly Settings _settings;

    public ExternalEditorLauncher(Settings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// テキストエディタでファイルを開く
    /// </summary>
    /// <param name="filePath">開くファイルのパス</param>
    /// <param name="lineNo">ジャンプ先行番号（0=指定なし）</param>
    /// <param name="search1">検索文字列1</param>
    /// <param name="search2">検索文字列2</param>
    /// <returns>起動成功の場合true</returns>
    public bool RunTextEditor(string filePath, int lineNo = 0, string search1 = "", string search2 = "")
    {
        try
        {
            switch (_settings.TextEditor.ToLowerInvariant())
            {
                case "sakura":
                    return LaunchSakuraEditor(filePath, lineNo, search1, search2);
                case "hidemaru":
                    return LaunchHidemaruEditor(filePath, lineNo, search1, search2);
                case "vscode":
                    return LaunchVSCode(filePath, lineNo);
                case "textedit":
                    return LaunchTextEdit(filePath);
                default:
                    return LaunchNotepad(filePath);
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// サクラエディタを起動
    /// </summary>
    private bool LaunchSakuraEditor(string filePath, int lineNo, string search1, string search2)
    {
        // サクラエディタのマクロファイルを作成
        var macroFile = Path.Combine(Path.GetTempPath(), "_sakura.mac");
        var search = BuildSearchPattern(search1, search2);

        var macroLines = new List<string>();
        if (lineNo > 1)
            macroLines.Add($"S_Jump({lineNo}, 1);");
        if (!string.IsNullOrEmpty(search))
            macroLines.Add($"S_SearchNext(\"{search}\",7);");
        if (lineNo > 1)
            macroLines.Add($"S_Jump({lineNo}, 1);");

        File.WriteAllLines(macroFile, macroLines);

        var psi = new ProcessStartInfo
        {
            FileName = _settings.SakuraPath,
            Arguments = $"-M={macroFile} \"{filePath}\"",
            UseShellExecute = false
        };
        Process.Start(psi);
        return true;
    }

    /// <summary>
    /// 秀丸エディタを起動
    /// </summary>
    private bool LaunchHidemaruEditor(string filePath, int lineNo, string search1, string search2)
    {
        var macroFile = Path.Combine(Path.GetTempPath(), "hidemaru.mac");
        var search = BuildSearchPattern(search1, search2);

        var macroLines = new List<string>();
        if (!string.IsNullOrEmpty(search))
            macroLines.Add($"searchdown \"{search}\",regular,nocasesense,hilight;");
        if (lineNo > 1)
            macroLines.Add($"movetolineno 1, {lineNo}; ");
        macroLines.Add("hilightfound 1 ;");

        File.WriteAllLines(macroFile, macroLines);

        var psi = new ProcessStartInfo
        {
            FileName = _settings.HidemaruPath,
            Arguments = $"/x\"{macroFile}\" \"{filePath}\"",
            UseShellExecute = false
        };
        Process.Start(psi);
        return true;
    }

    /// <summary>
    /// VS Codeを起動（Mac/Linux/Windows対応）
    /// </summary>
    private bool LaunchVSCode(string filePath, int lineNo)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _settings.VSCodePath,
            Arguments = lineNo > 0
                ? $"--goto \"{filePath}\":{lineNo}"
                : $"\"{filePath}\"",
            UseShellExecute = false
        };
        Process.Start(psi);
        return true;
    }

    /// <summary>
    /// TextEditを起動（Mac対応）
    /// </summary>
    private bool LaunchTextEdit(string filePath)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var psi = new ProcessStartInfo
            {
                FileName = "open",
                Arguments = $"-a TextEdit \"{filePath}\"",
                UseShellExecute = false
            };
            Process.Start(psi);
        }
        else
        {
            var psi = new ProcessStartInfo
            {
                FileName = _settings.TextEditPath,
                Arguments = $"\"{filePath}\"",
                UseShellExecute = false
            };
            Process.Start(psi);
        }
        return true;
    }

    /// <summary>
    /// メモ帳を起動（Windowsデフォルト）
    /// </summary>
    private bool LaunchNotepad(string filePath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _settings.NotepadPath,
            Arguments = $"\"{filePath}\"",
            UseShellExecute = false
        };
        Process.Start(psi);
        return true;
    }

    /// <summary>
    /// 検索パターンを構築
    /// </summary>
    private static string BuildSearchPattern(string search1, string search2)
    {
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(search1))
            parts.Add(StringUtilities.EscapeRegular(search1));
        if (!string.IsNullOrEmpty(search2))
            parts.Add(StringUtilities.EscapeRegular(search2));
        return string.Join("|", parts);
    }

    /// <summary>
    /// 現在のOSに適したデフォルトエディタ名を取得
    /// </summary>
    public static string GetDefaultEditorForOS()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "notepad";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "textedit";
        // Linux
        return "vscode";
    }
}
