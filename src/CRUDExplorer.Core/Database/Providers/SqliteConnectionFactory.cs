using System.Data;
using Microsoft.Data.Sqlite;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// SQLite接続ファクトリ
/// </summary>
public class SqliteConnectionFactory : IDbConnectionFactory
{
    public DatabaseType DatabaseType => DatabaseType.SQLite;

    public IDbConnection CreateConnection(string connectionString)
    {
        return new SqliteConnection(connectionString);
    }
}
