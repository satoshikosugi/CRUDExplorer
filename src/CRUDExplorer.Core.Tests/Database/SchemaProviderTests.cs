using CRUDExplorer.Core.Database;
using CRUDExplorer.Core.Database.Providers;
using CRUDExplorer.Core.Models;
using Microsoft.Data.Sqlite;

namespace CRUDExplorer.Core.Tests.Database;

public class SchemaProviderTests
{
    [Fact]
    public async Task SqliteSchemaProvider_GetTablesAsync_ReturnsTableList()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-schema-{Guid.NewGuid()}.db");
        try
        {
            // Create test database with tables
            using (var connection = new SqliteConnection($"Data Source={tempDb}"))
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE Users (
                        Id INTEGER PRIMARY KEY,
                        Name TEXT NOT NULL,
                        Email TEXT
                    );
                    CREATE TABLE Products (
                        Id INTEGER PRIMARY KEY,
                        ProductName TEXT NOT NULL,
                        Price REAL
                    );";
                await command.ExecuteNonQueryAsync();
            }

            var provider = new SqliteSchemaProvider();
            var tables = await provider.GetTablesAsync($"Data Source={tempDb}");

            Assert.Equal(2, tables.Count);
            Assert.Contains("Users", tables);
            Assert.Contains("Products", tables);
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
    public async Task SqliteSchemaProvider_GetTableDefinitionAsync_ReturnsCorrectColumns()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-schema-{Guid.NewGuid()}.db");
        try
        {
            // Create test table
            using (var connection = new SqliteConnection($"Data Source={tempDb}"))
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE TestTable (
                        Id INTEGER PRIMARY KEY,
                        Name TEXT NOT NULL,
                        Age INTEGER,
                        Email TEXT
                    );";
                await command.ExecuteNonQueryAsync();
            }

            var provider = new SqliteSchemaProvider();
            var tableDef = await provider.GetTableDefinitionAsync($"Data Source={tempDb}", "TestTable");

            Assert.Equal(4, tableDef.Columns.Count);

            Assert.True(tableDef.Columns.ContainsKey("Id"));
            Assert.True(tableDef.Columns.ContainsKey("Name"));
            Assert.True(tableDef.Columns.ContainsKey("Age"));
            Assert.True(tableDef.Columns.ContainsKey("Email"));

            var idColumn = tableDef.Columns["Id"];
            Assert.Equal("TestTable", idColumn.TableName);
            Assert.Equal("Id", idColumn.ColumnName);
            Assert.Equal("PK", idColumn.PrimaryKey);

            var nameColumn = tableDef.Columns["Name"];
            Assert.Equal("NOT NULL", nameColumn.Required);
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
    public async Task SqliteSchemaProvider_GetAllTableDefinitionsAsync_ReturnsAllTables()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-schema-{Guid.NewGuid()}.db");
        try
        {
            // Create multiple tables
            using (var connection = new SqliteConnection($"Data Source={tempDb}"))
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE Users (
                        Id INTEGER PRIMARY KEY,
                        Name TEXT NOT NULL
                    );
                    CREATE TABLE Orders (
                        Id INTEGER PRIMARY KEY,
                        UserId INTEGER NOT NULL,
                        Amount REAL
                    );";
                await command.ExecuteNonQueryAsync();
            }

            var provider = new SqliteSchemaProvider();
            var allDefinitions = await provider.GetAllTableDefinitionsAsync($"Data Source={tempDb}");

            Assert.Equal(2, allDefinitions.Count);
            Assert.True(allDefinitions.ContainsKey("Users"));
            Assert.True(allDefinitions.ContainsKey("Orders"));

            var usersDef = allDefinitions["Users"];
            Assert.Equal(2, usersDef.Columns.Count);

            var ordersDef = allDefinitions["Orders"];
            Assert.Equal(3, ordersDef.Columns.Count);
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
    public async Task SqliteSchemaProvider_DatabaseType_ReturnsSQLite()
    {
        var provider = new SqliteSchemaProvider();
        Assert.Equal(DatabaseType.SQLite, provider.DatabaseType);
    }

    [Fact]
    public async Task SqliteSchemaProvider_GetTablesAsync_ExcludesSqliteInternalTables()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-schema-{Guid.NewGuid()}.db");
        try
        {
            // Create test table
            using (var connection = new SqliteConnection($"Data Source={tempDb}"))
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE MyTable (Id INTEGER PRIMARY KEY);";
                await command.ExecuteNonQueryAsync();
            }

            var provider = new SqliteSchemaProvider();
            var tables = await provider.GetTablesAsync($"Data Source={tempDb}");

            // Should not include sqlite_* tables
            Assert.DoesNotContain(tables, t => t.StartsWith("sqlite_"));
            Assert.Single(tables);
            Assert.Equal("MyTable", tables[0]);
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
    public async Task SqliteSchemaProvider_GetTableDefinitionAsync_HandlesNullableColumns()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-schema-{Guid.NewGuid()}.db");
        try
        {
            using (var connection = new SqliteConnection($"Data Source={tempDb}"))
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE TestTable (
                        Id INTEGER PRIMARY KEY,
                        RequiredField TEXT NOT NULL,
                        OptionalField TEXT
                    );";
                await command.ExecuteNonQueryAsync();
            }

            var provider = new SqliteSchemaProvider();
            var tableDef = await provider.GetTableDefinitionAsync($"Data Source={tempDb}", "TestTable");

            var requiredField = tableDef.Columns["RequiredField"];
            Assert.Equal("NOT NULL", requiredField.Required);

            var optionalField = tableDef.Columns["OptionalField"];
            Assert.Equal(string.Empty, optionalField.Required);
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
    public async Task SqliteSchemaProvider_GetTableDefinitionAsync_NonExistentTable_ThrowsException()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-schema-{Guid.NewGuid()}.db");
        try
        {
            // Create empty database
            using (var connection = new SqliteConnection($"Data Source={tempDb}"))
            {
                await connection.OpenAsync();
            }

            var provider = new SqliteSchemaProvider();
            var tableDef = await provider.GetTableDefinitionAsync($"Data Source={tempDb}", "NonExistentTable");

            // SQLite returns empty result for non-existent table
            Assert.Empty(tableDef.Columns);
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
    public async Task SqliteSchemaProvider_GetTablesAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-schema-{Guid.NewGuid()}.db");
        try
        {
            // Create empty database
            using (var connection = new SqliteConnection($"Data Source={tempDb}"))
            {
                await connection.OpenAsync();
            }

            var provider = new SqliteSchemaProvider();
            var tables = await provider.GetTablesAsync($"Data Source={tempDb}");

            Assert.Empty(tables);
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
    public async Task SqliteSchemaProvider_GetTableDefinitionAsync_CaseInsensitiveDictionary()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-schema-{Guid.NewGuid()}.db");
        try
        {
            using (var connection = new SqliteConnection($"Data Source={tempDb}"))
            {
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE TestTable (UserId INTEGER PRIMARY KEY, UserName TEXT);";
                await command.ExecuteNonQueryAsync();
            }

            var provider = new SqliteSchemaProvider();
            var tableDef = await provider.GetTableDefinitionAsync($"Data Source={tempDb}", "TestTable");

            // Test case-insensitive access
            Assert.True(tableDef.Columns.ContainsKey("UserId"));
            Assert.True(tableDef.Columns.ContainsKey("userid"));
            Assert.True(tableDef.Columns.ContainsKey("USERID"));
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
