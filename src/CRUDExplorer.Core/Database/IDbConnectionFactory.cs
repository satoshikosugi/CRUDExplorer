using System.Data;

namespace CRUDExplorer.Core.Database;

/// <summary>
/// データベース接続ファクトリインターフェース
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// データベース接続を作成
    /// </summary>
    IDbConnection CreateConnection(string connectionString);

    /// <summary>
    /// データベースタイプ
    /// </summary>
    DatabaseType DatabaseType { get; }
}

/// <summary>
/// データベースタイプ列挙型
/// </summary>
public enum DatabaseType
{
    PostgreSQL,
    MySQL,
    SqlServer,
    Oracle,
    SQLite,
    MariaDB,
    Snowflake,
    BigQuery,
    Databricks,
    Redshift
}
