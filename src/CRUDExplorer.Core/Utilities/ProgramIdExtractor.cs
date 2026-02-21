using System.Text.RegularExpressions;

namespace CRUDExplorer.Core.Utilities;

/// <summary>
/// プログラムID抽出（VB.NET CommonModule.vbのGetProgramId相当）
/// </summary>
public static class ProgramIdExtractor
{
    /// <summary>
    /// モジュールIDからプログラムIDを正規表現で抽出
    /// </summary>
    /// <param name="moduleId">モジュールID</param>
    /// <param name="pattern">プログラムID抽出パターン（正規表現）</param>
    /// <returns>抽出されたプログラムID（マッチしない場合は空文字列）</returns>
    public static string GetProgramId(string moduleId, string pattern)
    {
        if (string.IsNullOrEmpty(moduleId) || string.IsNullOrEmpty(pattern))
            return string.Empty;

        try
        {
            var matches = Regex.Matches(moduleId, pattern, RegexOptions.IgnoreCase);
            if (matches.Count > 0)
            {
                if (matches[0].Groups.Count > 1)
                {
                    return matches[0].Groups[1].Value;
                }
                return matches[0].Value;
            }
        }
        catch
        {
            // 正規表現パターンエラー時は空文字列を返す
        }

        return string.Empty;
    }
}
