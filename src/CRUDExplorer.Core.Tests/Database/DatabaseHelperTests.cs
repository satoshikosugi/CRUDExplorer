using System.Data;
using CRUDExplorer.Core.Database;
using Microsoft.Data.Sqlite;

namespace CRUDExplorer.Core.Tests.Database;

public class DatabaseHelperTests
{
    [Fact]
    public async Task OpenConnectionAsync_AlreadyOpen_ReturnsConnection()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.db");
        try
        {
            using var connection = new SqliteConnection($"Data Source={tempDb}");
            await connection.OpenAsync();

            var result = await DatabaseHelper.OpenConnectionAsync(connection, DatabaseType.SQLite);

            Assert.Same(connection, result);
            Assert.Equal(ConnectionState.Open, result.State);
        }
        finally
        {
            if (File.Exists(tempDb))
            {
                File.Delete(tempDb);
            }
        }
    }

    [Fact]
    public async Task OpenConnectionAsync_Closed_OpensSuccessfully()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.db");
        try
        {
            using var connection = new SqliteConnection($"Data Source={tempDb}");

            var result = await DatabaseHelper.OpenConnectionAsync(connection, DatabaseType.SQLite);

            Assert.Equal(ConnectionState.Open, result.State);
        }
        finally
        {
            if (File.Exists(tempDb))
            {
                File.Delete(tempDb);
            }
        }
    }

    [Fact]
    public async Task OpenConnectionAsync_InvalidConnectionString_ThrowsDatabaseConnectionException()
    {
        using var connection = new SqliteConnection("Data Source=/invalid/nonexistent/path/database.db;Mode=ReadOnly");

        var ex = await Assert.ThrowsAsync<DatabaseConnectionException>(() =>
            DatabaseHelper.OpenConnectionAsync(connection, DatabaseType.SQLite, "TestConnection"));

        Assert.Equal(DatabaseType.SQLite, ex.DatabaseType);
        Assert.Equal("TestConnection", ex.ConnectionName);
        Assert.Contains("Failed to open connection", ex.Message);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_SucceedsFirstAttempt_ReturnsResult()
    {
        var result = await DatabaseHelper.ExecuteWithRetryAsync(async () =>
        {
            await Task.Delay(10);
            return 42;
        });

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_FailsThenSucceeds_ReturnsResult()
    {
        int attempts = 0;

        var result = await DatabaseHelper.ExecuteWithRetryAsync(async () =>
        {
            attempts++;
            await Task.Delay(10);
            if (attempts < 2)
            {
                throw new InvalidOperationException("Temporary failure");
            }
            return "success";
        }, maxRetries: 3, delayMilliseconds: 50);

        Assert.Equal("success", result);
        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task ExecuteWithRetryAsync_AlwaysFails_ThrowsQueryExecutionException()
    {
        int attempts = 0;

        var ex = await Assert.ThrowsAsync<QueryExecutionException>(() =>
            DatabaseHelper.ExecuteWithRetryAsync<string>(async () =>
            {
                attempts++;
                await Task.Delay(10);
                throw new InvalidOperationException("Always fails");
            }, maxRetries: 3, delayMilliseconds: 10));

        Assert.Equal(3, attempts);
        Assert.Contains("failed after 3 attempts", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Theory]
    [InlineData("Server=localhost;Password=secret123;Database=test", "Server=localhost;Password=****;Database=test")]
    [InlineData("Host=localhost;pwd=mypass;Database=test", "Host=localhost;pwd=****;Database=test")]
    [InlineData("Server=localhost;User=admin;Password=p@ssw0rd", "Server=localhost;User=admin;Password=****")]
    [InlineData("ApiKey=abc123;Server=test", "ApiKey=****;Server=test")]
    [InlineData("Server=localhost;Database=test", "Server=localhost;Database=test")]
    public void MaskConnectionString_MasksSecrets(string input, string expected)
    {
        var result = DatabaseHelper.MaskConnectionString(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MaskConnectionString_CaseInsensitive_MasksSecrets()
    {
        var input = "Server=localhost;PASSWORD=secret;Database=test";
        var result = DatabaseHelper.MaskConnectionString(input);
        Assert.Contains("PASSWORD=****", result);
        Assert.DoesNotContain("secret", result);
    }

    [Theory]
    [InlineData(DatabaseType.PostgreSQL, "Host=localhost;Database=test", true)]
    [InlineData(DatabaseType.PostgreSQL, "Host=localhost", false)]
    [InlineData(DatabaseType.PostgreSQL, "", false)]
    [InlineData(DatabaseType.MySQL, "Server=localhost;Database=test", true)]
    [InlineData(DatabaseType.MySQL, "Server=localhost", false)]
    [InlineData(DatabaseType.SqlServer, "Server=localhost;Database=test", true)]
    [InlineData(DatabaseType.SqlServer, "Database=test", false)]
    [InlineData(DatabaseType.Oracle, "Data Source=mydb", true)]
    [InlineData(DatabaseType.Oracle, "Server=localhost", false)]
    [InlineData(DatabaseType.SQLite, "Data Source=test.db", true)]
    [InlineData(DatabaseType.SQLite, "Database=test", false)]
    [InlineData(DatabaseType.MariaDB, "Server=localhost;Database=test", true)]
    [InlineData(DatabaseType.MariaDB, "Server=localhost", false)]
    public void ValidateConnectionString_VariousFormats_ValidatesCorrectly(
        DatabaseType dbType, string connectionString, bool expected)
    {
        var result = DatabaseHelper.ValidateConnectionString(connectionString, dbType);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ValidateConnectionString_CaseInsensitive_Validates()
    {
        var result = DatabaseHelper.ValidateConnectionString(
            "HOST=localhost;DATABASE=test",
            DatabaseType.PostgreSQL);
        Assert.True(result);
    }

    [Fact]
    public void ValidateConnectionString_WhitespaceOnly_ReturnsFalse()
    {
        var result = DatabaseHelper.ValidateConnectionString("   ", DatabaseType.PostgreSQL);
        Assert.False(result);
    }
}
