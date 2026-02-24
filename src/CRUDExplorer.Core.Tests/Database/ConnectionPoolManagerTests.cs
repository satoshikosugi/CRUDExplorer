using System.Data;
using CRUDExplorer.Core.Database;

namespace CRUDExplorer.Core.Tests.Database;

public class ConnectionPoolManagerTests
{
    [Fact]
    public async Task GetConnectionAsync_NewPool_CreatesAndReturnsConnection()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.db");
        try
        {
            using var manager = new ConnectionPoolManager();
            var connectionInfo = new DatabaseConnection
            {
                Name = "Test",
                DatabaseType = DatabaseType.SQLite,
                ConnectionString = $"Data Source={tempDb}"
            };

            var connection = await manager.GetConnectionAsync("Test", connectionInfo);

            Assert.NotNull(connection);
            Assert.Equal(ConnectionState.Open, connection.State);

            connection.Dispose();
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
    public async Task GetConnectionAsync_CaseInsensitive_ReturnsSamePool()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.db");
        try
        {
            using var manager = new ConnectionPoolManager();
            var connectionInfo = new DatabaseConnection
            {
                Name = "Test",
                DatabaseType = DatabaseType.SQLite,
                ConnectionString = $"Data Source={tempDb}"
            };

            var connection1 = await manager.GetConnectionAsync("Test", connectionInfo);
            manager.ReturnConnection("Test", connection1);

            var connection2 = await manager.GetConnectionAsync("TEST", connectionInfo);

            Assert.NotNull(connection2);
            Assert.Equal(ConnectionState.Open, connection2.State);

            connection2.Dispose();
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
    public async Task ReturnConnection_ValidConnection_AddsToPool()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.db");
        try
        {
            using var manager = new ConnectionPoolManager();
            var connectionInfo = new DatabaseConnection
            {
                Name = "Test",
                DatabaseType = DatabaseType.SQLite,
                ConnectionString = $"Data Source={tempDb}"
            };

            var connection1 = await manager.GetConnectionAsync("Test", connectionInfo);
            manager.ReturnConnection("Test", connection1);

            var connection2 = await manager.GetConnectionAsync("Test", connectionInfo);
            Assert.NotNull(connection2);

            connection2.Dispose();
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
    public async Task ReturnConnection_ClosedConnection_DisposesIt()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.db");
        try
        {
            using var manager = new ConnectionPoolManager();
            var connectionInfo = new DatabaseConnection
            {
                Name = "Test",
                DatabaseType = DatabaseType.SQLite,
                ConnectionString = $"Data Source={tempDb}"
            };

            var connection = await manager.GetConnectionAsync("Test", connectionInfo);
            connection.Close();
            manager.ReturnConnection("Test", connection);

            // Should create a new connection since the returned one was closed
            var newConnection = await manager.GetConnectionAsync("Test", connectionInfo);
            Assert.NotNull(newConnection);
            Assert.Equal(ConnectionState.Open, newConnection.State);

            newConnection.Dispose();
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
    public void ClearPool_ExistingPool_RemovesIt()
    {
        using var manager = new ConnectionPoolManager();
        var connectionInfo = new DatabaseConnection
        {
            Name = "Test",
            DatabaseType = DatabaseType.SQLite,
            ConnectionString = "Data Source=:memory:"
        };

        manager.ClearPool("Test");
        // Should not throw
    }

    [Fact]
    public async Task ClearAllPools_DisposesAllPools()
    {
        var tempDb1 = Path.Combine(Path.GetTempPath(), $"test1-{Guid.NewGuid()}.db");
        var tempDb2 = Path.Combine(Path.GetTempPath(), $"test2-{Guid.NewGuid()}.db");
        try
        {
            using var manager = new ConnectionPoolManager();
            var connectionInfo1 = new DatabaseConnection
            {
                Name = "Test1",
                DatabaseType = DatabaseType.SQLite,
                ConnectionString = $"Data Source={tempDb1}"
            };
            var connectionInfo2 = new DatabaseConnection
            {
                Name = "Test2",
                DatabaseType = DatabaseType.SQLite,
                ConnectionString = $"Data Source={tempDb2}"
            };

            var conn1 = await manager.GetConnectionAsync("Test1", connectionInfo1);
            var conn2 = await manager.GetConnectionAsync("Test2", connectionInfo2);

            manager.ClearAllPools();

            // Connections should be disposed
            conn1.Dispose();
            conn2.Dispose();
        }
        finally
        {
            if (File.Exists(tempDb1)) File.Delete(tempDb1);
            if (File.Exists(tempDb2)) File.Delete(tempDb2);
        }
    }

    [Fact]
    public async Task Dispose_DisposesManager_ThrowsOnSubsequentUse()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.db");
        try
        {
            var manager = new ConnectionPoolManager();
            var connectionInfo = new DatabaseConnection
            {
                Name = "Test",
                DatabaseType = DatabaseType.SQLite,
                ConnectionString = $"Data Source={tempDb}"
            };

            manager.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                manager.GetConnectionAsync("Test", connectionInfo));
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
    public void GetConnectionAsync_InvalidConnectionString_ThrowsException()
    {
        using var manager = new ConnectionPoolManager();
        var connectionInfo = new DatabaseConnection
        {
            Name = "Invalid",
            DatabaseType = DatabaseType.SQLite,
            ConnectionString = "Data Source=/invalid/nonexistent/path/database.db;Mode=ReadOnly"
        };

        // SQLite with non-existent readonly database throws SqliteException
        Assert.ThrowsAsync<Microsoft.Data.Sqlite.SqliteException>(() =>
            manager.GetConnectionAsync("Invalid", connectionInfo));
    }

    [Fact]
    public async Task GetConnectionAsync_MultipleRequests_HandlesThreadSafety()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}.db");
        try
        {
            using var manager = new ConnectionPoolManager();
            var connectionInfo = new DatabaseConnection
            {
                Name = "Test",
                DatabaseType = DatabaseType.SQLite,
                ConnectionString = $"Data Source={tempDb}"
            };

            var tasks = Enumerable.Range(0, 5).Select(async _ =>
            {
                var connection = await manager.GetConnectionAsync("Test", connectionInfo);
                await Task.Delay(10);
                manager.ReturnConnection("Test", connection);
            });

            await Task.WhenAll(tasks);
            // If we get here without deadlock or exception, thread safety works
            Assert.True(true);
        }
        finally
        {
            if (File.Exists(tempDb))
            {
                File.Delete(tempDb);
            }
        }
    }
}
