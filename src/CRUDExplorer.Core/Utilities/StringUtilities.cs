using System.Text;
using System.Text.RegularExpressions;

namespace CRUDExplorer.Core.Utilities;

/// <summary>
/// 文字列操作ユーティリティ（VB.NET CommonModule.vbから移植）
/// </summary>
public static class StringUtilities
{
    static StringUtilities()
    {
        // CodePagesEncodingProviderの登録は1回のみ実行
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    /// <summary>
    /// 文字列の右端から指定した長さの文字列を取得（VB.NET Right関数相当）
    /// </summary>
    /// <param name="target">対象文字列</param>
    /// <param name="length">取得する文字数</param>
    /// <returns>右端からの文字列</returns>
    public static string GetRight(string target, int length)
    {
        if (string.IsNullOrEmpty(target))
            return string.Empty;

        if (length <= 0)
            return string.Empty;

        if (length >= target.Length)
            return target;

        return target.Substring(target.Length - length);
    }

    /// <summary>
    /// 文字列のバイト長を取得（Shift_JIS基準）
    /// </summary>
    /// <param name="target">対象文字列</param>
    /// <returns>バイト長</returns>
    public static int LenB(string target)
    {
        if (string.IsNullOrEmpty(target))
            return 0;

        // Shift_JIS（CodePage 932）でのバイト長を返す
        return Encoding.GetEncoding("Shift_JIS").GetByteCount(target);
    }

    /// <summary>
    /// 正規表現の特殊文字をエスケープ（英数字以外を[]で囲む）
    /// </summary>
    /// <param name="value">エスケープする文字列</param>
    /// <returns>エスケープされた文字列</returns>
    public static string EscapeRegular(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var sb = new StringBuilder();
        foreach (var c in value)
        {
            if (Regex.IsMatch(c.ToString(), "[a-z0-9]", RegexOptions.IgnoreCase))
            {
                sb.Append(c);
            }
            else
            {
                sb.Append('[').Append(c).Append(']');
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 配列から安全にインデックスの値を取得（VB.NET GetStringArrayByIndex相当）
    /// </summary>
    /// <param name="array">対象配列</param>
    /// <param name="index">インデックス</param>
    /// <returns>値（範囲外の場合は空文字列）</returns>
    public static string GetStringArrayByIndex(string[] array, int index)
    {
        if (array == null || index < 0 || index >= array.Length)
            return string.Empty;

        return array[index];
    }
}
