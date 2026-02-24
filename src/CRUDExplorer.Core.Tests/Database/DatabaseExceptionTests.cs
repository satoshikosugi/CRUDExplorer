using CRUDExplorer.Core.Database;

namespace CRUDExplorer.Core.Tests.Database;

public class DatabaseExceptionTests
{
    [Fact]
    public void DatabaseException_WithMessage_SetsMessage()
    {
        var ex = new DatabaseException("Test error");
        Assert.Equal("Test error", ex.Message);
    }

    [Fact]
    public void DatabaseException_WithInnerException_SetsInnerException()
    {
        var inner = new InvalidOperationException("Inner error");
        var ex = new DatabaseException("Test error", inner);

        Assert.Equal("Test error", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void DatabaseException_SetProperties_StoresValues()
    {
        var ex = new DatabaseException("Test error")
        {
            DatabaseType = DatabaseType.PostgreSQL,
            ConnectionName = "TestConnection"
        };

        Assert.Equal(DatabaseType.PostgreSQL, ex.DatabaseType);
        Assert.Equal("TestConnection", ex.ConnectionName);
    }

    [Fact]
    public void DatabaseConnectionException_InheritsFromDatabaseException()
    {
        var ex = new DatabaseConnectionException("Connection failed");
        Assert.IsAssignableFrom<DatabaseException>(ex);
    }

    [Fact]
    public void DatabaseConnectionException_SetConnectionString_StoresValue()
    {
        var ex = new DatabaseConnectionException("Connection failed")
        {
            ConnectionString = "Server=localhost",
            DatabaseType = DatabaseType.MySQL,
            ConnectionName = "TestDB"
        };

        Assert.Equal("Server=localhost", ex.ConnectionString);
        Assert.Equal(DatabaseType.MySQL, ex.DatabaseType);
        Assert.Equal("TestDB", ex.ConnectionName);
    }

    [Fact]
    public void DatabaseConnectionException_WithInnerException_SetsInnerException()
    {
        var inner = new TimeoutException("Connection timeout");
        var ex = new DatabaseConnectionException("Failed to connect", inner);

        Assert.Equal("Failed to connect", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void SchemaRetrievalException_InheritsFromDatabaseException()
    {
        var ex = new SchemaRetrievalException("Schema retrieval failed");
        Assert.IsAssignableFrom<DatabaseException>(ex);
    }

    [Fact]
    public void SchemaRetrievalException_SetProperties_StoresValues()
    {
        var ex = new SchemaRetrievalException("Schema retrieval failed")
        {
            TableName = "Users",
            SchemaName = "public",
            DatabaseType = DatabaseType.PostgreSQL
        };

        Assert.Equal("Users", ex.TableName);
        Assert.Equal("public", ex.SchemaName);
        Assert.Equal(DatabaseType.PostgreSQL, ex.DatabaseType);
    }

    [Fact]
    public void SchemaRetrievalException_WithInnerException_SetsInnerException()
    {
        var inner = new InvalidOperationException("Table not found");
        var ex = new SchemaRetrievalException("Failed to retrieve schema", inner);

        Assert.Equal("Failed to retrieve schema", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void DatabaseConfigurationException_InheritsFromDatabaseException()
    {
        var ex = new DatabaseConfigurationException("Configuration error");
        Assert.IsAssignableFrom<DatabaseException>(ex);
    }

    [Fact]
    public void DatabaseConfigurationException_WithInnerException_SetsInnerException()
    {
        var inner = new FormatException("Invalid configuration format");
        var ex = new DatabaseConfigurationException("Configuration error", inner);

        Assert.Equal("Configuration error", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void QueryExecutionException_InheritsFromDatabaseException()
    {
        var ex = new QueryExecutionException("Query failed");
        Assert.IsAssignableFrom<DatabaseException>(ex);
    }

    [Fact]
    public void QueryExecutionException_SetQuery_StoresValue()
    {
        var ex = new QueryExecutionException("Query failed")
        {
            Query = "SELECT * FROM Users",
            DatabaseType = DatabaseType.SqlServer
        };

        Assert.Equal("SELECT * FROM Users", ex.Query);
        Assert.Equal(DatabaseType.SqlServer, ex.DatabaseType);
    }

    [Fact]
    public void QueryExecutionException_WithInnerException_SetsInnerException()
    {
        var inner = new TimeoutException("Query timeout");
        var ex = new QueryExecutionException("Query execution failed", inner);

        Assert.Equal("Query execution failed", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void AllExceptions_CanBeCaughtAsDatabaseException()
    {
        var exceptions = new DatabaseException[]
        {
            new DatabaseConnectionException("Connection error"),
            new SchemaRetrievalException("Schema error"),
            new DatabaseConfigurationException("Config error"),
            new QueryExecutionException("Query error")
        };

        foreach (var ex in exceptions)
        {
            Assert.IsAssignableFrom<DatabaseException>(ex);
        }
    }
}
