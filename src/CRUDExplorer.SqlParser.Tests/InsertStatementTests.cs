using CRUDExplorer.SqlParser.Analyzers;

namespace CRUDExplorer.SqlParser.Tests;

public class InsertStatementTests
{
    private readonly SqlAnalyzer _analyzer = new();

    [Fact]
    public void AnalyzeSql_SimpleInsert_ParsesCorrectly()
    {
        var sql = "INSERT INTO users (id, name) VALUES (1, 'John')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("users", query.TableC.Keys);
        Assert.NotEmpty(query.ColumnInsert);
    }

    [Fact]
    public void AnalyzeSql_InsertWithAllColumns_ParsesCorrectly()
    {
        var sql = "INSERT INTO users (id, name, email, age) VALUES (1, 'John', 'john@example.com', 25)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("users", query.TableC.Keys);
        Assert.Equal(4, query.ColumnInsert.Count);
    }

    [Fact]
    public void AnalyzeSql_InsertWithoutColumnList_ParsesCorrectly()
    {
        var sql = "INSERT INTO users VALUES (1, 'John', 'john@example.com')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("users", query.TableC.Keys);
    }

    [Fact]
    public void AnalyzeSql_InsertMultipleRows_ParsesCorrectly()
    {
        var sql = "INSERT INTO users (id, name) VALUES (1, 'John'), (2, 'Jane'), (3, 'Bob')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("users", query.TableC.Keys);
    }

    [Fact]
    public void AnalyzeSql_InsertSelect_ParsesCorrectly()
    {
        var sql = "INSERT INTO users_backup SELECT * FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("users_backup", query.TableC.Keys);
        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_InsertSelectWithWhere_ParsesCorrectly()
    {
        var sql = "INSERT INTO active_users SELECT id, name FROM users WHERE status = 'active'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("active_users", query.TableC.Keys);
        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_InsertWithSubquery_ParsesCorrectly()
    {
        var sql = "INSERT INTO users (id, name) VALUES ((SELECT MAX(id) + 1 FROM users), 'New User')";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("users", query.TableC.Keys);
    }

    [Fact]
    public void AnalyzeSql_InsertWithNullValues_ParsesCorrectly()
    {
        var sql = "INSERT INTO users (id, name, email) VALUES (1, 'John', NULL)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("users", query.TableC.Keys);
    }

    [Fact]
    public void AnalyzeSql_InsertWithDefaultValues_ParsesCorrectly()
    {
        var sql = "INSERT INTO users DEFAULT VALUES";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("users", query.TableC.Keys);
    }

    [Fact]
    public void AnalyzeSql_InsertWithFunctions_ParsesCorrectly()
    {
        var sql = "INSERT INTO logs (id, created_at) VALUES (1, NOW())";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("INSERT", query.QueryKind);
        Assert.Contains("logs", query.TableC.Keys);
    }
}
