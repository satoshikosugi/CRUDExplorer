using System.Data;
using MySql.Data.MySqlClient;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// MySQL接続ファクトリ
/// </summary>
public class MySqlConnectionFactory : IDbConnectionFactory
{
    public DatabaseType DatabaseType => DatabaseType.MySQL;

    public IDbConnection CreateConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }
}
