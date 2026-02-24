using CRUDExplorer.Core.Utilities;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.Core.Tests;

public class LogicalNameResolverTests
{
    private LogicalNameResolver CreateResolver()
    {
        var tableNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["USERS"] = "ユーザー",
            ["ORDERS"] = "受注"
        };

        var tableDefs = new Dictionary<string, TableDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["USERS"] = new TableDefinition
            {
                Columns = new Dictionary<string, ColumnDefinition>(StringComparer.OrdinalIgnoreCase)
                {
                    ["USER_ID"] = new ColumnDefinition
                    {
                        TableName = "USERS",
                        ColumnName = "USER_ID",
                        AttributeName = "ユーザーID"
                    },
                    ["USER_NAME"] = new ColumnDefinition
                    {
                        TableName = "USERS",
                        ColumnName = "USER_NAME",
                        AttributeName = "ユーザー名"
                    }
                }
            }
        };

        return new LogicalNameResolver(tableNames, tableDefs);
    }

    [Fact]
    public void GetLogicalName_TableOnly_ReturnsLogicalTableName()
    {
        var resolver = CreateResolver();
        Assert.Equal("ユーザー", resolver.GetLogicalName("USERS"));
    }

    [Fact]
    public void GetLogicalName_TableAndColumn_ReturnsLogicalNames()
    {
        var resolver = CreateResolver();
        Assert.Equal("ユーザー.ユーザーID", resolver.GetLogicalName("USERS.USER_ID"));
    }

    [Fact]
    public void GetLogicalName_UnknownTable_ReturnsPhysicalName()
    {
        var resolver = CreateResolver();
        Assert.Equal("UNKNOWN_TABLE", resolver.GetLogicalName("UNKNOWN_TABLE"));
    }

    [Fact]
    public void GetLogicalName_CaseInsensitive()
    {
        var resolver = CreateResolver();
        Assert.Equal("ユーザー.ユーザー名", resolver.GetLogicalName("users.user_name"));
    }

    [Fact]
    public void GetTableDef_ExistingTable_ReturnsDefinition()
    {
        var resolver = CreateResolver();
        var tableDef = resolver.GetTableDef("USERS");
        Assert.NotNull(tableDef);
        Assert.Equal(2, tableDef.Columns.Count);
    }

    [Fact]
    public void GetTableDef_MissingTable_ReturnsNull()
    {
        var resolver = CreateResolver();
        Assert.Null(resolver.GetTableDef("NONEXISTENT"));
    }

    [Fact]
    public void GetColumnDef_ExistingColumn_ReturnsDefinition()
    {
        var resolver = CreateResolver();
        var colDef = resolver.GetColumnDef("USERS.USER_ID");
        Assert.NotNull(colDef);
        Assert.Equal("ユーザーID", colDef.AttributeName);
    }

    [Fact]
    public void GetColumnDef_MissingColumn_ReturnsNull()
    {
        var resolver = CreateResolver();
        Assert.Null(resolver.GetColumnDef("USERS.NONEXISTENT"));
    }

    [Fact]
    public void GetLogicalName_OutputParams_ReturnsCorrectNames()
    {
        var resolver = CreateResolver();
        resolver.GetLogicalName("USERS", "USER_NAME", out var entityName, out var attributeName);
        Assert.Equal("ユーザー", entityName);
        Assert.Equal("ユーザー名", attributeName);
    }
}
