using System.Data;
using CRUDExplorer.Core.Models;
using Microsoft.Data.SqlClient;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// SQL Serverスキーマ情報取得
/// </summary>
public class SqlServerSchemaProvider : ISchemaProvider
{
    public DatabaseType DatabaseType => DatabaseType.SqlServer;

    public async Task<List<string>> GetTablesAsync(string connectionString, string? schemaName = null)
    {
        schemaName ??= "dbo";
        var tables = new List<string>();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT TABLE_NAME
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = @schema
              AND TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_NAME";

        using var command = new SqlCommand(query, connection);
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
        schemaName ??= "dbo";
        var tableDef = new TableDefinition();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Get columns
        var columnQuery = @"
            SELECT
                c.COLUMN_NAME,
                c.ORDINAL_POSITION,
                c.DATA_TYPE,
                c.CHARACTER_MAXIMUM_LENGTH,
                c.NUMERIC_PRECISION,
                c.NUMERIC_SCALE,
                c.IS_NULLABLE,
                CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'PK' ELSE '' END as IS_PRIMARY_KEY
            FROM INFORMATION_SCHEMA.COLUMNS c
            LEFT JOIN (
                SELECT ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                    ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                    AND tc.TABLE_SCHEMA = ku.TABLE_SCHEMA
                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                    AND tc.TABLE_NAME = @tableName
                    AND tc.TABLE_SCHEMA = @schema
            ) pk ON c.COLUMN_NAME = pk.COLUMN_NAME
            WHERE c.TABLE_NAME = @tableName
              AND c.TABLE_SCHEMA = @schema
            ORDER BY c.ORDINAL_POSITION";

        using var command = new SqlCommand(columnQuery, connection);
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
                Digits = reader.IsDBNull(3) ? string.Empty : reader.GetInt32(3).ToString(),
                Accuracy = reader.IsDBNull(5) ? string.Empty : reader.GetInt32(5).ToString(),
                Required = reader.GetString(6) == "NO" ? "NOT NULL" : string.Empty,
                PrimaryKey = reader.GetString(7),
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
