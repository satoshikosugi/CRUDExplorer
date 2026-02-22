using CRUDExplorer.Core.Database;

namespace CRUDExplorer.Core.Tests.Database;

public class ConnectionStringManagerTests
{
    [Fact]
    public void AddConnection_NewConnection_AddsSuccessfully()
    {
        var manager = new ConnectionStringManager();
        manager.AddConnection("Test", DatabaseType.SQLite, "Data Source=test.db");

        var connection = manager.GetConnection("Test");
        Assert.NotNull(connection);
        Assert.Equal("Test", connection.Name);
        Assert.Equal(DatabaseType.SQLite, connection.DatabaseType);
        Assert.Equal("Data Source=test.db", connection.ConnectionString);
    }

    [Fact]
    public void AddConnection_CaseInsensitive_OverwritesExisting()
    {
        var manager = new ConnectionStringManager();
        manager.AddConnection("Test", DatabaseType.SQLite, "Data Source=test1.db");
        manager.AddConnection("TEST", DatabaseType.MySQL, "Server=localhost");

        var connection = manager.GetConnection("test");
        Assert.NotNull(connection);
        Assert.Equal(DatabaseType.MySQL, connection.DatabaseType);
        Assert.Equal("Server=localhost", connection.ConnectionString);
    }

    [Fact]
    public void GetConnection_NonExistent_ReturnsNull()
    {
        var manager = new ConnectionStringManager();
        var connection = manager.GetConnection("NonExistent");
        Assert.Null(connection);
    }

    [Fact]
    public void GetConnection_UpdatesLastUsedAt()
    {
        var manager = new ConnectionStringManager();
        manager.AddConnection("Test", DatabaseType.SQLite, "Data Source=test.db");

        var connection1 = manager.GetConnection("Test");
        var firstUsedAt = connection1!.LastUsedAt;

        Thread.Sleep(10);
        var connection2 = manager.GetConnection("Test");
        var secondUsedAt = connection2!.LastUsedAt;

        Assert.True(secondUsedAt > firstUsedAt);
    }

    [Fact]
    public void RemoveConnection_ExistingConnection_ReturnsTrue()
    {
        var manager = new ConnectionStringManager();
        manager.AddConnection("Test", DatabaseType.SQLite, "Data Source=test.db");

        var result = manager.RemoveConnection("Test");
        Assert.True(result);
        Assert.Null(manager.GetConnection("Test"));
    }

    [Fact]
    public void RemoveConnection_NonExistent_ReturnsFalse()
    {
        var manager = new ConnectionStringManager();
        var result = manager.RemoveConnection("NonExistent");
        Assert.False(result);
    }

    [Fact]
    public void GetAllConnections_ReturnsSortedByName()
    {
        var manager = new ConnectionStringManager();
        manager.AddConnection("Charlie", DatabaseType.SQLite, "Data Source=c.db");
        manager.AddConnection("Alpha", DatabaseType.MySQL, "Server=a");
        manager.AddConnection("Bravo", DatabaseType.PostgreSQL, "Host=b");

        var connections = manager.GetAllConnections().ToList();
        Assert.Equal(3, connections.Count);
        Assert.Equal("Alpha", connections[0].Name);
        Assert.Equal("Bravo", connections[1].Name);
        Assert.Equal("Charlie", connections[2].Name);
    }

    [Fact]
    public async Task SaveToFileAsync_CreatesDirectoryAndFile()
    {
        var manager = new ConnectionStringManager();
        manager.AddConnection("Test", DatabaseType.SQLite, "Data Source=test.db");

        var tempDir = Path.Combine(Path.GetTempPath(), $"crudexplorer-test-{Guid.NewGuid()}");
        var tempPath = Path.Combine(tempDir, "connections.json");
        try
        {
            await manager.SaveToFileAsync(tempPath);
            Assert.True(File.Exists(tempPath));

            var json = await File.ReadAllTextAsync(tempPath);
            Assert.Contains("Test", json);
            Assert.Contains("Data Source=test.db", json);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public async Task LoadFromFileAsync_NonExistentFile_DoesNotThrow()
    {
        var manager = new ConnectionStringManager();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"nonexistent-{Guid.NewGuid()}.json");

        await manager.LoadFromFileAsync(nonExistentPath);
        Assert.Empty(manager.GetAllConnections());
    }

    [Fact]
    public async Task SaveAndLoad_RoundTrip_PreservesData()
    {
        var manager1 = new ConnectionStringManager();
        manager1.AddConnection("SQLite1", DatabaseType.SQLite, "Data Source=test1.db");
        manager1.AddConnection("MySQL1", DatabaseType.MySQL, "Server=localhost;Database=test");
        manager1.AddConnection("PgSQL1", DatabaseType.PostgreSQL, "Host=localhost;Database=test");

        var tempPath = Path.Combine(Path.GetTempPath(), $"crudexplorer-test-{Guid.NewGuid()}.json");
        try
        {
            await manager1.SaveToFileAsync(tempPath);

            var manager2 = new ConnectionStringManager();
            await manager2.LoadFromFileAsync(tempPath);

            var connections = manager2.GetAllConnections().ToList();
            Assert.Equal(3, connections.Count);

            var sqlite = manager2.GetConnection("SQLite1");
            Assert.NotNull(sqlite);
            Assert.Equal(DatabaseType.SQLite, sqlite.DatabaseType);
            Assert.Equal("Data Source=test1.db", sqlite.ConnectionString);

            var mysql = manager2.GetConnection("MySQL1");
            Assert.NotNull(mysql);
            Assert.Equal(DatabaseType.MySQL, mysql.DatabaseType);

            var pgsql = manager2.GetConnection("PgSQL1");
            Assert.NotNull(pgsql);
            Assert.Equal(DatabaseType.PostgreSQL, pgsql.DatabaseType);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact]
    public async Task LoadFromFileAsync_InvalidJson_ThrowsDatabaseConfigurationException()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"invalid-{Guid.NewGuid()}.json");
        try
        {
            await File.WriteAllTextAsync(tempPath, "{ invalid json content");

            var manager = new ConnectionStringManager();
            await Assert.ThrowsAsync<DatabaseConfigurationException>(() =>
                manager.LoadFromFileAsync(tempPath));
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact]
    public async Task TestConnectionAsync_NonExistentConnection_ThrowsException()
    {
        var manager = new ConnectionStringManager();
        await Assert.ThrowsAsync<DatabaseConfigurationException>(() =>
            manager.TestConnectionAsync("NonExistent"));
    }

    [Fact]
    public async Task TestConnectionAsync_SQLite_ValidConnection_ReturnsTrue()
    {
        var manager = new ConnectionStringManager();
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.db");

        try
        {
            manager.AddConnection("TestSQLite", DatabaseType.SQLite, $"Data Source={tempDb}");
            var result = await manager.TestConnectionAsync("TestSQLite");
            Assert.True(result);
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
    public async Task TestConnectionAsync_InvalidConnectionString_ReturnsFalse()
    {
        var manager = new ConnectionStringManager();
        // Use a valid format but non-existent file in ReadOnly mode to trigger connection failure
        manager.AddConnection("Invalid", DatabaseType.SQLite, "Data Source=/nonexistent/path/db.sqlite;Mode=ReadOnly");

        var result = await manager.TestConnectionAsync("Invalid");
        Assert.False(result);
    }
}
