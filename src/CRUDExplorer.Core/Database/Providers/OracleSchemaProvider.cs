using System.Data;
using CRUDExplorer.Core.Models;
using Oracle.ManagedDataAccess.Client;

namespace CRUDExplorer.Core.Database.Providers;

/// <summary>
/// Oracleスキーマ情報取得
/// </summary>
public class OracleSchemaProvider : ISchemaProvider
{
    public DatabaseType DatabaseType => DatabaseType.Oracle;

    public async Task<List<string>> GetTablesAsync(string connectionString, string? schemaName = null)
    {
        var tables = new List<string>();

        using var connection = new OracleConnection(connectionString);
        await connection.OpenAsync();

        // If no schema specified, use current user
        string query;
        if (string.IsNullOrEmpty(schemaName))
        {
            query = @"
                SELECT table_name
                FROM user_tables
                ORDER BY table_name";
        }
        else
        {
            query = @"
                SELECT table_name
                FROM all_tables
                WHERE owner = :schema
                ORDER BY table_name";
        }

        using var command = new OracleCommand(query, connection);
        if (!string.IsNullOrEmpty(schemaName))
        {
            command.Parameters.Add(":schema", OracleDbType.Varchar2).Value = schemaName.ToUpper();
        }

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

        using var connection = new OracleConnection(connectionString);
        await connection.OpenAsync();

        // Get columns
        string columnQuery;
        if (string.IsNullOrEmpty(schemaName))
        {
            columnQuery = @"
                SELECT
                    c.column_name,
                    c.column_id,
                    c.data_type,
                    c.data_length,
                    c.data_precision,
                    c.data_scale,
                    c.nullable,
                    CASE WHEN pk.column_name IS NOT NULL THEN 'PK' ELSE '' END as is_primary_key
                FROM user_tab_columns c
                LEFT JOIN (
                    SELECT cols.column_name
                    FROM user_constraints cons
                    JOIN user_cons_columns cols
                        ON cons.constraint_name = cols.constraint_name
                    WHERE cons.constraint_type = 'P'
                        AND cons.table_name = :tableName
                ) pk ON c.column_name = pk.column_name
                WHERE c.table_name = :tableName
                ORDER BY c.column_id";
        }
        else
        {
            columnQuery = @"
                SELECT
                    c.column_name,
                    c.column_id,
                    c.data_type,
                    c.data_length,
                    c.data_precision,
                    c.data_scale,
                    c.nullable,
                    CASE WHEN pk.column_name IS NOT NULL THEN 'PK' ELSE '' END as is_primary_key
                FROM all_tab_columns c
                LEFT JOIN (
                    SELECT cols.column_name
                    FROM all_constraints cons
                    JOIN all_cons_columns cols
                        ON cons.constraint_name = cols.constraint_name
                        AND cons.owner = cols.owner
                    WHERE cons.constraint_type = 'P'
                        AND cons.table_name = :tableName
                        AND cons.owner = :schema
                ) pk ON c.column_name = pk.column_name
                WHERE c.table_name = :tableName
                  AND c.owner = :schema
                ORDER BY c.column_id";
        }

        using var command = new OracleCommand(columnQuery, connection);
        command.Parameters.Add(":tableName", OracleDbType.Varchar2).Value = tableName.ToUpper();
        if (!string.IsNullOrEmpty(schemaName))
        {
            command.Parameters.Add(":schema", OracleDbType.Varchar2).Value = schemaName.ToUpper();
        }

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
                Required = reader.GetString(6) == "N" ? "NOT NULL" : string.Empty,
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
