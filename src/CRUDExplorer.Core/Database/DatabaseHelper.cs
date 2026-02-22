using System.Data;
using System.Data.Common;

namespace CRUDExplorer.Core.Database;

/// <summary>
/// データベース操作のヘルパークラス
/// </summary>
public static class DatabaseHelper
{
    /// <summary>
    /// 接続を安全に開く
    /// </summary>
    public static async Task<IDbConnection> OpenConnectionAsync(IDbConnection connection, DatabaseType databaseType, string? connectionName = null)
    {
        try
        {
            if (connection.State != ConnectionState.Open)
            {
                if (connection is DbConnection dbConnection)
                {
                    await dbConnection.OpenAsync();
                }
                else
                {
                    connection.Open();
                }
            }
            return connection;
        }
        catch (Exception ex)
        {
            throw new DatabaseConnectionException($"Failed to open connection to {databaseType} database", ex)
            {
                DatabaseType = databaseType,
                ConnectionName = connectionName
            };
        }
    }

    /// <summary>
    /// クエリを安全に実行
    /// </summary>
    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = 3,
        int delayMilliseconds = 1000)
    {
        int attempts = 0;
        Exception? lastException = null;

        while (attempts < maxRetries)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                lastException = ex;
                attempts++;

                if (attempts < maxRetries)
                {
                    await Task.Delay(delayMilliseconds * attempts);
                }
            }
        }

        throw new QueryExecutionException($"Operation failed after {maxRetries} attempts", lastException!);
    }

    /// <summary>
    /// 接続文字列をマスク（ログ用）
    /// </summary>
    public static string MaskConnectionString(string connectionString)
    {
        var keywords = new[] { "password", "pwd", "secret", "key" };
        var result = connectionString;

        foreach (var keyword in keywords)
        {
            var pattern = $@"({keyword}\s*=\s*)([^;]+)";
            result = System.Text.RegularExpressions.Regex.Replace(
                result,
                pattern,
                $"$1****",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }

        return result;
    }

    /// <summary>
    /// 接続文字列を検証
    /// </summary>
    public static bool ValidateConnectionString(string connectionString, DatabaseType databaseType)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        // Basic validation - check for required keywords based on database type
        var requiredKeywords = databaseType switch
        {
            DatabaseType.PostgreSQL => new[] { "host", "database" },
            DatabaseType.MySQL => new[] { "server", "database" },
            DatabaseType.SqlServer => new[] { "server", "database" },
            DatabaseType.Oracle => new[] { "data source" },
            DatabaseType.SQLite => new[] { "data source" },
            DatabaseType.MariaDB => new[] { "server", "database" },
            _ => Array.Empty<string>()
        };

        var lowerConnectionString = connectionString.ToLowerInvariant();
        return requiredKeywords.All(keyword => lowerConnectionString.Contains(keyword));
    }
}
