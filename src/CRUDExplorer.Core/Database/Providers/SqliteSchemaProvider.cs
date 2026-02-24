using System.Data;
using CRUDExplorer.Core.Models;
using Microsoft.Data.Sqlite;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// SQLiteスキーマ情報取得
/// </summary>
public class SqliteSchemaProvider : ISchemaProvider
{
    public DatabaseType DatabaseType => DatabaseType.SQLite;

    public async Task<List<string>> GetTablesAsync(string connectionString, string? schemaName = null)
    {
        var tables = new List<string>();

        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT name
            FROM sqlite_master
            WHERE type = 'table'
              AND name NOT LIKE 'sqlite_%'
            ORDER BY name";

        using var command = new SqliteCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tables.Add(reader.GetString(0));
        }

        return tables;
    }

    public async Task<TableDefinition> GetTableDefinitionAsync(string connectionString, string tableName, string? schemaName = null)
    {
        var tableDef = new TableDefinition();

        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        // Get columns using PRAGMA table_info
        var pragmaQuery = $"PRAGMA table_info('{tableName}')";

        using var command = new SqliteCommand(pragmaQuery, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var columnName = reader.GetString(1);  // name
            var column = new ColumnDefinition
            {
                TableName = tableName,
                ColumnName = columnName,
                AttributeName = columnName,
                Sequence = reader.GetInt32(0).ToString(),  // cid
                DataType = reader.GetString(2),  // type
                Digits = string.Empty,  // SQLite doesn't enforce size constraints
                Accuracy = string.Empty,
                Required = reader.GetInt32(3) == 1 ? "NOT NULL" : string.Empty,  // notnull
                PrimaryKey = reader.GetInt32(5) > 0 ? "PK" : string.Empty,  // pk
                ForeignKey = string.Empty
            };
            tableDef.Columns.Add(columnName, column);
        }

        return tableDef;
    }

    public async Task<Dictionary<string, TableDefinition>> GetAllTableDefinitionsAsync(string connectionString, string? schemaName = null)
    {
        var tables = await GetTablesAsync(connectionString, schemaName);
        var tableDefinitions = new Dictionary<string, TableDefinition>(StringComparer.OrdinalIgnoreCase);

        foreach (var tableName in tables)
        {
            var tableDef = await GetTableDefinitionAsync(connectionString, tableName, schemaName);
            tableDefinitions[tableName] = tableDef;
        }

        return tableDefinitions;
    }
}
