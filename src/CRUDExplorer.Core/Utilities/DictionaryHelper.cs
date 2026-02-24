namespace CRUDExplorer.Core.Utilities;

/// <summary>
/// Dictionary操作ヘルパー（VB.NET CommonModule.vbのDict系関数から移植）
/// VB.NET Scripting.Dictionaryの大文字小文字不問動作をC#で再現
/// </summary>
public static class DictionaryHelper
{
    /// <summary>
    /// 辞書から値を取得（キーが存在しない場合はキー自体を返す）
    /// </summary>
    /// <param name="dict">対象辞書</param>
    /// <param name="key">検索キー</param>
    /// <param name="missingReturnKey">キーが存在しない場合にキー自体を返すか</param>
    /// <returns>辞書の値、またはキー自体/空文字列</returns>
    public static string GetDictValue(Dictionary<string, string> dict, string key, bool missingReturnKey = true)
    {
        if (dict == null || string.IsNullOrEmpty(key))
            return missingReturnKey ? (key ?? string.Empty) : string.Empty;

        if (dict.TryGetValue(key, out var value))
            return value;

        return missingReturnKey ? key : string.Empty;
    }

    /// <summary>
    /// 辞書からオブジェクトを取得
    /// </summary>
    /// <typeparam name="T">値の型</typeparam>
    /// <param name="dict">対象辞書</param>
    /// <param name="key">検索キー</param>
    /// <returns>辞書の値、またはdefault</returns>
    public static T? GetDictObject<T>(Dictionary<string, T> dict, string key) where T : class
    {
        if (dict == null || string.IsNullOrEmpty(key))
            return default;

        return dict.TryGetValue(key, out var value) ? value : default;
    }

    /// <summary>
    /// 辞書にキーが存在するか確認（大文字小文字不問は辞書のComparerに依存）
    /// </summary>
    /// <typeparam name="T">値の型</typeparam>
    /// <param name="dict">対象辞書</param>
    /// <param name="key">検索キー</param>
    /// <returns>存在する場合true</returns>
    public static bool DictExists<T>(Dictionary<string, T> dict, string key)
    {
        if (dict == null || string.IsNullOrEmpty(key))
            return false;

        return dict.ContainsKey(key);
    }

    /// <summary>
    /// 辞書に重複チェック付きで追加
    /// </summary>
    /// <typeparam name="T">値の型</typeparam>
    /// <param name="dict">対象辞書</param>
    /// <param name="key">キー</param>
    /// <param name="value">値</param>
    /// <returns>追加成功の場合true</returns>
    public static bool DictAdd<T>(Dictionary<string, T> dict, string key, T value)
    {
        if (dict == null || string.IsNullOrEmpty(key))
            return false;

        return dict.TryAdd(key, value);
    }

    /// <summary>
    /// 辞書をキーでソートした新しい辞書を返す
    /// </summary>
    /// <typeparam name="T">値の型</typeparam>
    /// <param name="dict">対象辞書</param>
    /// <returns>ソート済み辞書</returns>
    public static Dictionary<string, T> SortDictionary<T>(Dictionary<string, T> dict)
    {
        if (dict == null)
            return new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

        var sorted = new Dictionary<string, T>(dict.Comparer);
        foreach (var kvp in dict.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
        {
            sorted[kvp.Key] = kvp.Value;
        }
        return sorted;
    }
}
