using CRUDExplorer.SqlParser.Analyzers;

namespace CRUDExplorer.SqlParser.Tests;

public class UpdateStatementTests
{
    private readonly SqlAnalyzer _analyzer = new();

    [Fact]
    public void AnalyzeSql_SimpleUpdate_ParsesCorrectly()
    {
        var sql = "UPDATE users SET name = 'John' WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("users", query.TableU.Keys);
        Assert.NotEmpty(query.ColumnSetCond);
        Assert.NotEmpty(query.ColumnWhere);
    }

    [Fact]
    public void AnalyzeSql_UpdateMultipleColumns_ParsesCorrectly()
    {
        var sql = "UPDATE users SET name = 'John', email = 'john@example.com', age = 25 WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("users", query.TableU.Keys);
        Assert.Equal(3, query.ColumnSetCond.Count);
    }

    [Fact]
    public void AnalyzeSql_UpdateWithoutWhere_ParsesCorrectly()
    {
        var sql = "UPDATE users SET status = 'active'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("users", query.TableU.Keys);
        Assert.NotEmpty(query.ColumnSetCond);
    }

    [Fact]
    public void AnalyzeSql_UpdateWithSubquery_ParsesCorrectly()
    {
        var sql = "UPDATE users SET status = (SELECT status FROM settings WHERE id = 1) WHERE id = 2";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("users", query.TableU.Keys);
        // settingsはサブクエリ内のテーブル → GetAllTableR経由で取得
        var allTableR = query.GetAllTableR();
        Assert.True(allTableR.Values.Any(v => v.Contains("SETTINGS", StringComparison.OrdinalIgnoreCase)),
            "settings table should be found in subquery's TableR via GetAllTableR");
    }

    [Fact]
    public void AnalyzeSql_UpdateWithJoin_ParsesCorrectly()
    {
        var sql = "UPDATE users u SET u.status = o.status FROM users u INNER JOIN orders o ON u.id = o.user_id";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("users", query.TableU.Keys);
    }

    [Fact]
    public void AnalyzeSql_UpdateWithCalculation_ParsesCorrectly()
    {
        var sql = "UPDATE products SET price = price * 1.1 WHERE category = 'electronics'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("products", query.TableU.Keys);
    }

    [Fact]
    public void AnalyzeSql_UpdateWithNull_ParsesCorrectly()
    {
        var sql = "UPDATE users SET email = NULL WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("users", query.TableU.Keys);
    }

    [Fact]
    public void AnalyzeSql_UpdateWithComplexWhere_ParsesCorrectly()
    {
        var sql = "UPDATE users SET status = 'inactive' WHERE age > 60 AND last_login < '2020-01-01'";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("users", query.TableU.Keys);
    }

    [Fact]
    public void AnalyzeSql_UpdateWithCaseExpression_ParsesCorrectly()
    {
        var sql = "UPDATE users SET category = CASE WHEN age < 18 THEN 'minor' ELSE 'adult' END";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("users", query.TableU.Keys);
    }

    [Fact]
    public void AnalyzeSql_UpdateWithFunction_ParsesCorrectly()
    {
        var sql = "UPDATE logs SET updated_at = NOW() WHERE id = 1";
        var query = _analyzer.AnalyzeSql(sql);

        Assert.Equal("UPDATE", query.QueryKind);
        Assert.Contains("logs", query.TableU.Keys);
    }
}
