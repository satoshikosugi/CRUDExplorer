using System.Data;
using MySqlConnector;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// MariaDB接続ファクトリ（MySqlConnector使用）
/// </summary>
public class MariaDbConnectionFactory : IDbConnectionFactory
{
    public DatabaseType DatabaseType => DatabaseType.MariaDB;

    public IDbConnection CreateConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }
}
