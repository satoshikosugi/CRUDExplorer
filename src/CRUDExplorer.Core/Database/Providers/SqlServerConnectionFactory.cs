using System.Data;
using Microsoft.Data.SqlClient;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// SQL Server接続ファクトリ
/// </summary>
public class SqlServerConnectionFactory : IDbConnectionFactory
{
    public DatabaseType DatabaseType => DatabaseType.SqlServer;

    public IDbConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }
}
