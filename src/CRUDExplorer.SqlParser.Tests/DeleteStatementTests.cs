using CRUDExplorer.SqlParser.Analyzers;

namespace CRUDExplorer.SqlParser.Tests;

public class DeleteStatementTests
{
    private readonly SqlAnalyzer _analyzer = new();

    [Fact]
    public void AnalyzeSql_SimpleDelete_ParsesCorrectly()
    {
        var sql = "DELETE FROM users WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("users", query.TableD.Keys);
        Assert.NotEmpty(query.ColumnWhere);
    }

    [Fact]
    public void AnalyzeSql_DeleteAll_ParsesCorrectly()
    {
        var sql = "DELETE FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("users", query.TableD.Keys);
    }

    [Fact]
    public void AnalyzeSql_DeleteWithComplexWhere_ParsesCorrectly()
    {
        var sql = "DELETE FROM users WHERE age > 60 AND status = 'inactive'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("users", query.TableD.Keys);
        Assert.NotEmpty(query.ColumnWhere);
    }

    [Fact]
    public void AnalyzeSql_DeleteWithSubquery_ParsesCorrectly()
    {
        var sql = "DELETE FROM users WHERE id IN (SELECT user_id FROM inactive_users)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("users", query.TableD.Keys);
        Assert.Contains("inactive_users", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_DeleteWithJoin_ParsesCorrectly()
    {
        var sql = "DELETE u FROM users u INNER JOIN orders o ON u.id = o.user_id WHERE o.status = 'cancelled'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("users", query.TableD.Keys);
    }

    [Fact]
    public void AnalyzeSql_DeleteWithNotExists_ParsesCorrectly()
    {
        var sql = "DELETE FROM users WHERE NOT EXISTS (SELECT 1 FROM orders WHERE orders.user_id = users.id)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("users", query.TableD.Keys);
    }

    [Fact]
    public void AnalyzeSql_DeleteWithBetween_ParsesCorrectly()
    {
        var sql = "DELETE FROM logs WHERE created_at BETWEEN '2020-01-01' AND '2020-12-31'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("logs", query.TableD.Keys);
    }

    [Fact]
    public void AnalyzeSql_DeleteWithLike_ParsesCorrectly()
    {
        var sql = "DELETE FROM users WHERE email LIKE '%@spam.com'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("users", query.TableD.Keys);
    }

    [Fact]
    public void AnalyzeSql_DeleteWithIsNull_ParsesCorrectly()
    {
        var sql = "DELETE FROM users WHERE email IS NULL";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("users", query.TableD.Keys);
    }

    [Fact]
    public void AnalyzeSql_DeleteWithLimit_ParsesCorrectly()
    {
        var sql = "DELETE FROM logs LIMIT 1000";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("DELETE", query.QueryKind);
        Assert.Contains("logs", query.TableD.Keys);
    }
}
