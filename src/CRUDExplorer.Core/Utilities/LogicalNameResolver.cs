using CRUDExplorer.Core.Models;

namespace CRUDExplorer.Core.Utilities;

/// <summary>
/// 論理名解決（VB.NET CommonModule.vbのGetLogicalName, GetTableDef, GetColumnDef相当）
/// テーブル名・カラム名の物理名→論理名変換
/// </summary>
public class LogicalNameResolver
{
    private readonly Dictionary<string, string> _tableNames;
    private readonly Dictionary<string, TableDefinition> _tableDefinitions;

    public LogicalNameResolver(
        Dictionary<string, string> tableNames,
        Dictionary<string, TableDefinition> tableDefinitions)
    {
        _tableNames = tableNames;
        _tableDefinitions = tableDefinitions;
    }

    /// <summary>
    /// テーブル名またはテーブル名.カラム名の論理名を取得
    /// </summary>
    /// <param name="tableColumnName">テーブル名 または テーブル名.カラム名</param>
    /// <returns>論理名</returns>
    public string GetLogicalName(string tableColumnName)
    {
        if (string.IsNullOrEmpty(tableColumnName))
            return string.Empty;

        var parts = tableColumnName.Split('.');
        var tableName = parts[0];

        // テーブル論理名を取得
        var logicalTableName = DictionaryHelper.DictExists(_tableNames, tableName)
            ? DictionaryHelper.GetDictValue(_tableNames, tableName)
            : tableName;

        if (parts.Length <= 1)
            return logicalTableName;

        // カラム論理名を取得
        var columnName = parts[1];
        var attributeName = columnName;

        var tableDef = GetTableDef(tableName);
        if (tableDef != null && DictionaryHelper.DictExists(tableDef.Columns, columnName))
        {
            var colDef = DictionaryHelper.GetDictObject(tableDef.Columns, columnName);
            if (colDef != null)
            {
                attributeName = colDef.AttributeName;
            }
        }

        return $"{logicalTableName}.{attributeName}";
    }

    /// <summary>
    /// テーブル名とカラム名から論理名を取得（出力パラメータ版）
    /// </summary>
    /// <param name="tableName">テーブル名</param>
    /// <param name="columnName">カラム名</param>
    /// <param name="entityName">テーブル論理名（出力）</param>
    /// <param name="attributeName">カラム論理名（出力）</param>
    public void GetLogicalName(string tableName, string columnName, out string entityName, out string attributeName)
    {
        var logicalName = GetLogicalName($"{tableName}.{columnName}");
        var parts = logicalName.Split('.');
        entityName = parts[0];
        attributeName = parts.Length > 1 ? parts[1] : columnName;
    }

    /// <summary>
    /// テーブル定義を取得（VB.NET GetTableDef相当）
    /// </summary>
    /// <param name="tableName">テーブル名</param>
    /// <returns>テーブル定義（存在しない場合null）</returns>
    public TableDefinition? GetTableDef(string tableName)
    {
        return DictionaryHelper.GetDictObject(_tableDefinitions, tableName);
    }

    /// <summary>
    /// カラム定義を取得（VB.NET GetColumnDef相当）
    /// </summary>
    /// <param name="tableColumnName">テーブル名.カラム名</param>
    /// <returns>カラム定義（存在しない場合null）</returns>
    public ColumnDefinition? GetColumnDef(string tableColumnName)
    {
        var parts = tableColumnName.Split('.');
        if (parts.Length < 2)
            return null;

        var tableDef = GetTableDef(parts[0]);
        if (tableDef == null)
            return null;

        return DictionaryHelper.GetDictObject(tableDef.Columns, parts[1]);
    }
}
