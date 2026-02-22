using System.Data;
using Npgsql;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// PostgreSQL接続ファクトリ
/// </summary>
public class PostgreSqlConnectionFactory : IDbConnectionFactory
{
    public DatabaseType DatabaseType => DatabaseType.PostgreSQL;

    public IDbConnection CreateConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }
}
