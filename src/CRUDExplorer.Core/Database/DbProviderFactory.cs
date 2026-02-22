using CRUDExplorer.Core.Database.Providers;

namespace CRUDExplorer.Core.Database;

/// <summary>
/// データベースプロバイダーファクトリ
/// </summary>
public static class DbProviderFactory
{
    /// <summary>
    /// 接続ファクトリを取得
    /// </summary>
    public static IDbConnectionFactory GetConnectionFactory(DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.PostgreSQL => new PostgreSqlConnectionFactory(),
            DatabaseType.MySQL => new MySqlConnectionFactory(),
            DatabaseType.SqlServer => new SqlServerConnectionFactory(),
            DatabaseType.Oracle => new OracleConnectionFactory(),
            _ => throw new NotSupportedException($"Database type {databaseType} is not supported yet.")
        };
    }

    /// <summary>
    /// スキーマプロバイダーを取得
    /// </summary>
    public static ISchemaProvider GetSchemaProvider(DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.PostgreSQL => new PostgreSqlSchemaProvider(),
            DatabaseType.MySQL => new MySqlSchemaProvider(),
            DatabaseType.SqlServer => new SqlServerSchemaProvider(),
            DatabaseType.Oracle => new OracleSchemaProvider(),
            _ => throw new NotSupportedException($"Database type {databaseType} is not supported yet.")
        };
    }

    /// <summary>
    /// 文字列からデータベースタイプを取得
    /// </summary>
    public static DatabaseType ParseDatabaseType(string databaseTypeName)
    {
        return databaseTypeName.ToLowerInvariant() switch
        {
            "postgresql" or "postgres" or "pgsql" => DatabaseType.PostgreSQL,
            "mysql" => DatabaseType.MySQL,
            "sqlserver" or "mssql" or "sql server" => DatabaseType.SqlServer,
            "oracle" => DatabaseType.Oracle,
            "sqlite" => DatabaseType.SQLite,
            "mariadb" => DatabaseType.MariaDB,
            "snowflake" => DatabaseType.Snowflake,
            "bigquery" => DatabaseType.BigQuery,
            "databricks" => DatabaseType.Databricks,
            "redshift" => DatabaseType.Redshift,
            _ => throw new ArgumentException($"Unknown database type: {databaseTypeName}", nameof(databaseTypeName))
        };
    }
}
