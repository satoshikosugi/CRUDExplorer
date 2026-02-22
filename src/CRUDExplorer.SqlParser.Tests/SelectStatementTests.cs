using CRUDExplorer.SqlParser.Analyzers;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.SqlParser.Tests;

public class SelectStatementTests
{
    private readonly SqlAnalyzer _analyzer = new();

    [Fact]
    public void AnalyzeSql_SimpleSelect_ParsesCorrectly()
    {
        var sql = "SELECT id, name FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Equal(sql, query.QueryText);
        Assert.NotNull(query.ColumnSelect);
        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithWhere_ParsesCorrectly()
    {
        var sql = "SELECT id, name FROM users WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.NotEmpty(query.ColumnWhere);
    }

    [Fact]
    public void AnalyzeSql_SelectWithJoin_ParsesCorrectly()
    {
        var sql = "SELECT u.id, u.name, o.order_id FROM users u INNER JOIN orders o ON u.id = o.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("orders", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithGroupBy_ParsesCorrectly()
    {
        var sql = "SELECT department, COUNT(*) FROM employees GROUP BY department";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("employees", query.TableR.Keys);
        Assert.NotEmpty(query.ColumnGroupBy);
    }

    [Fact]
    public void AnalyzeSql_SelectWithHaving_ParsesCorrectly()
    {
        var sql = "SELECT department, COUNT(*) FROM employees GROUP BY department HAVING COUNT(*) > 5";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.NotEmpty(query.ColumnGroupBy);
        Assert.NotEmpty(query.ColumnHaving);
    }

    [Fact]
    public void AnalyzeSql_SelectWithOrderBy_ParsesCorrectly()
    {
        var sql = "SELECT id, name FROM users ORDER BY name ASC";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.NotEmpty(query.ColumnOrderBy);
    }

    [Fact]
    public void AnalyzeSql_SelectStar_ParsesCorrectly()
    {
        var sql = "SELECT * FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithAlias_ParsesCorrectly()
    {
        var sql = "SELECT id AS user_id, name AS user_name FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.NotEmpty(query.ColumnSelect);
    }

    [Fact]
    public void AnalyzeSql_SelectWithMultipleTables_ParsesCorrectly()
    {
        var sql = "SELECT u.id, p.product_name FROM users u, products p WHERE u.id = p.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("products", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithSubquery_ParsesCorrectly()
    {
        var sql = "SELECT id, name FROM users WHERE id IN (SELECT user_id FROM orders)";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.NotEmpty(query.SubQueries);
    }

    [Fact]
    public void AnalyzeSql_SelectWithLeftJoin_ParsesCorrectly()
    {
        var sql = "SELECT u.id, o.order_id FROM users u LEFT JOIN orders o ON u.id = o.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("orders", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithRightJoin_ParsesCorrectly()
    {
        var sql = "SELECT u.id, o.order_id FROM users u RIGHT JOIN orders o ON u.id = o.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("orders", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithFullJoin_ParsesCorrectly()
    {
        var sql = "SELECT u.id, o.order_id FROM users u FULL JOIN orders o ON u.id = o.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("orders", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithCrossJoin_ParsesCorrectly()
    {
        var sql = "SELECT u.id, p.product_name FROM users u CROSS JOIN products p";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("products", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithComplexWhere_ParsesCorrectly()
    {
        var sql = "SELECT id, name FROM users WHERE age > 18 AND city = 'Tokyo' OR status = 'active'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.NotEmpty(query.ColumnWhere);
    }

    [Fact]
    public void AnalyzeSql_SelectWithAggregateFunctions_ParsesCorrectly()
    {
        var sql = "SELECT COUNT(*), SUM(amount), AVG(age), MIN(price), MAX(price) FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithDistinct_ParsesCorrectly()
    {
        var sql = "SELECT DISTINCT department FROM employees";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("employees", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithLimit_ParsesCorrectly()
    {
        var sql = "SELECT id, name FROM users LIMIT 10";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithOffset_ParsesCorrectly()
    {
        var sql = "SELECT id, name FROM users LIMIT 10 OFFSET 20";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithUnion_ParsesCorrectly()
    {
        var sql = "SELECT id, name FROM users UNION SELECT id, name FROM employees";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
        Assert.Contains("employees", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_SelectWithCaseExpression_ParsesCorrectly()
    {
        var sql = "SELECT id, CASE WHEN age > 18 THEN 'adult' ELSE 'minor' END AS age_group FROM users";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("SELECT", query.QueryKind);
        Assert.Contains("users", query.TableR.Keys);
    }

    [Fact]
    public void AnalyzeSql_EmptyQuery_ReturnsQueryWithQueryText()
    {
        var sql = "";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.NotNull(query);
        Assert.Equal(sql, query.QueryText);
    }

    [Fact]
    public void AnalyzeSql_SelectWithFileNameAndLineNo_StoresMetadata()
    {
        var sql = "SELECT id FROM users";
        var fileName = "test.sql";
        var lineNo = 42;

        var query = _analyzer.AnalyzeSql(sql, fileName, lineNo);

        Assert.Equal(fileName, query.FileName);
        Assert.Equal(lineNo, query.LineNo);
    }
}
