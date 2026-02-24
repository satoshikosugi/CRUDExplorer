using System.Data.Common;
using System.Text.Json;

namespace CRUDExplorer.Core.Database;

/// <summary>
/// データベース接続文字列管理
/// </summary>
public class ConnectionStringManager
{
    private readonly Dictionary<string, DatabaseConnection> _connections = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string DefaultConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CRUDExplorer",
        "database-connections.json"
    );

    /// <summary>
    /// 接続情報を追加
    /// </summary>
    public void AddConnection(string name, DatabaseType databaseType, string connectionString)
    {
        _connections[name] = new DatabaseConnection
        {
            Name = name,
            DatabaseType = databaseType,
            ConnectionString = connectionString,
            CreatedAt = DateTime.UtcNow,
            LastUsedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// 接続情報を取得
    /// </summary>
    public DatabaseConnection? GetConnection(string name)
    {
        if (_connections.TryGetValue(name, out var connection))
        {
            connection.LastUsedAt = DateTime.UtcNow;
            return connection;
        }
        return null;
    }

    /// <summary>
    /// 全接続情報を取得
    /// </summary>
    public IEnumerable<DatabaseConnection> GetAllConnections()
    {
        return _connections.Values.OrderBy(c => c.Name);
    }

    /// <summary>
    /// 接続情報を削除
    /// </summary>
    public bool RemoveConnection(string name)
    {
        return _connections.Remove(name);
    }

    /// <summary>
    /// 接続情報をファイルから読み込み
    /// </summary>
    public async Task LoadFromFileAsync(string? filePath = null)
    {
        filePath ??= DefaultConfigPath;

        if (!File.Exists(filePath))
        {
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var connections = JsonSerializer.Deserialize<List<DatabaseConnection>>(json);

            if (connections != null)
            {
                _connections.Clear();
                foreach (var conn in connections)
                {
                    _connections[conn.Name] = conn;
                }
            }
        }
        catch (Exception ex)
        {
            throw new DatabaseConfigurationException($"Failed to load connections from {filePath}", ex);
        }
    }

    /// <summary>
    /// 接続情報をファイルに保存
    /// </summary>
    public async Task SaveToFileAsync(string? filePath = null)
    {
        filePath ??= DefaultConfigPath;

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        try
        {
            var json = JsonSerializer.Serialize(_connections.Values, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            throw new DatabaseConfigurationException($"Failed to save connections to {filePath}", ex);
        }
    }

    /// <summary>
    /// 接続をテスト
    /// </summary>
    public async Task<bool> TestConnectionAsync(string name)
    {
        var connection = GetConnection(name);
        if (connection == null)
        {
            throw new DatabaseConfigurationException($"Connection '{name}' not found");
        }

        var factory = DbProviderFactory.GetConnectionFactory(connection.DatabaseType);
        using var dbConnection = factory.CreateConnection(connection.ConnectionString);

        try
        {
            if (dbConnection is DbConnection db)
            {
                await db.OpenAsync();
            }
            else
            {
                dbConnection.Open();
            }
            return dbConnection.State == System.Data.ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// データベース接続情報
/// </summary>
public class DatabaseConnection
{
    public string Name { get; set; } = string.Empty;
    public DatabaseType DatabaseType { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastUsedAt { get; set; }
}
