using System.Data;
using CRUDExplorer.Core.Models;
using MySql.Data.MySqlClient;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// MySQLスキーマ情報取得
/// </summary>
public class MySqlSchemaProvider : ISchemaProvider
{
    public DatabaseType DatabaseType => DatabaseType.MySQL;

    public async Task<List<string>> GetTablesAsync(string connectionString, string? schemaName = null)
    {
        var tables = new List<string>();

        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        // Get database name from connection if schema not specified
        if (string.IsNullOrEmpty(schemaName))
        {
            schemaName = connection.Database;
        }

        var query = @"
            SELECT table_name
            FROM information_schema.tables
            WHERE table_schema = @schema
              AND table_type = 'BASE TABLE'
            ORDER BY table_name";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@schema", schemaName);

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

        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        if (string.IsNullOrEmpty(schemaName))
        {
            schemaName = connection.Database;
        }

        // Get columns
        var columnQuery = @"
            SELECT
                c.column_name,
                c.ordinal_position,
                c.data_type,
                c.character_maximum_length,
                c.numeric_precision,
                c.numeric_scale,
                c.is_nullable,
                c.column_key
            FROM information_schema.columns c
            WHERE c.table_name = @tableName
              AND c.table_schema = @schema
            ORDER BY c.ordinal_position";

        using var command = new MySqlCommand(columnQuery, connection);
        command.Parameters.AddWithValue("@tableName", tableName);
        command.Parameters.AddWithValue("@schema", schemaName);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var columnName = reader.GetString(0);
            var column = new ColumnDefinition
            {
                TableName = tableName,
                ColumnName = columnName,
                AttributeName = columnName,
                Sequence = reader.GetInt32(1).ToString(),
                DataType = reader.GetString(2),
                Digits = reader.IsDBNull(3) ? string.Empty : reader.GetValue(3).ToString()!,
                Accuracy = reader.IsDBNull(5) ? string.Empty : reader.GetValue(5).ToString()!,
                Required = reader.GetString(6) == "NO" ? "NOT NULL" : string.Empty,
                PrimaryKey = reader.GetString(7) == "PRI" ? "PK" : string.Empty,
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
