using System.Text.RegularExpressions;

namespace CRUDExplorer.Core.Utilities;

/// <summary>
/// 正規表現ユーティリティ（VB.NET CommonModule.vbのRegMatch系から移植）
/// </summary>
public static class RegexUtilities
{
    /// <summary>
    /// 正規表現マッチング
    /// </summary>
    /// <param name="input">対象文字列</param>
    /// <param name="pattern">正規表現パターン</param>
    /// <param name="options">正規表現オプション</param>
    /// <returns>マッチした場合true</returns>
    public static bool RegMatch(string input, string pattern, RegexOptions options = RegexOptions.None)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern))
            return false;

        try
        {
            return Regex.IsMatch(input, pattern, options);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 大文字小文字区別なし正規表現マッチング
    /// </summary>
    /// <param name="input">対象文字列</param>
    /// <param name="pattern">正規表現パターン</param>
    /// <returns>マッチした場合true</returns>
    public static bool RegMatchI(string input, string pattern)
    {
        return RegMatch(input, pattern, RegexOptions.IgnoreCase);
    }
}
