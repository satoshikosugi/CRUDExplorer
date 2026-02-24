namespace CRUDExplorer.Core.Database;

/// <summary>
/// データベース操作に関する例外の基底クラス
/// </summary>
public class DatabaseException : Exception
{
    public DatabaseType? DatabaseType { get; set; }
    public string? ConnectionName { get; set; }

    public DatabaseException(string message) : base(message)
    {
    }

    public DatabaseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// データベース接続エラー
/// </summary>
public class DatabaseConnectionException : DatabaseException
{
    public string? ConnectionString { get; set; }

    public DatabaseConnectionException(string message) : base(message)
    {
    }

    public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// スキーマ取得エラー
/// </summary>
public class SchemaRetrievalException : DatabaseException
{
    public string? TableName { get; set; }
    public string? SchemaName { get; set; }

    public SchemaRetrievalException(string message) : base(message)
    {
    }

    public SchemaRetrievalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// データベース設定エラー
/// </summary>
public class DatabaseConfigurationException : DatabaseException
{
    public DatabaseConfigurationException(string message) : base(message)
    {
    }

    public DatabaseConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// クエリ実行エラー
/// </summary>
public class QueryExecutionException : DatabaseException
{
    public string? Query { get; set; }

    public QueryExecutionException(string message) : base(message)
    {
    }

    public QueryExecutionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
