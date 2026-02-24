using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// Oracle接続ファクトリ
/// </summary>
public class OracleConnectionFactory : IDbConnectionFactory
{
    public DatabaseType DatabaseType => DatabaseType.Oracle;

    public IDbConnection CreateConnection(string connectionString)
    {
        return new OracleConnection(connectionString);
    }
}
