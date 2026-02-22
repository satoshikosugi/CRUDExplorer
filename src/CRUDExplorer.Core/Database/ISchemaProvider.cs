using CRUDExplorer.Core.Models;

namespace CRUDExplorer.Core.Database;

/// <summary>
/// データベーススキーマ情報取得インターフェース
/// </summary>
public interface ISchemaProvider
{
    /// <summary>
    /// データベースタイプ
    /// </summary>
    DatabaseType DatabaseType { get; }

    /// <summary>
    /// テーブル一覧を取得
    /// </summary>
    Task<List<string>> GetTablesAsync(string connectionString, string? schemaName = null);

    /// <summary>
    /// テーブル定義を取得
    /// </summary>
    Task<TableDefinition> GetTableDefinitionAsync(string connectionString, string tableName, string? schemaName = null);

    /// <summary>
    /// すべてのテーブル定義を取得
    /// </summary>
    Task<Dictionary<string, TableDefinition>> GetAllTableDefinitionsAsync(string connectionString, string? schemaName = null);
}
