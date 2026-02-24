using CRUDExplorer.Core.Formatting;

namespace CRUDExplorer.Core.Tests;

public class QueryFormatterTests
{
    private readonly QueryFormatter _formatter = new();

    [Fact]
    public void Format_SimpleSelect_FormatsCorrectly()
    {
        var sql = "SELECT col1, col2 FROM table1 WHERE col1 = 1";
        var result = _formatter.Format(sql);

        Assert.Contains("SELECT", result);
        Assert.Contains("FROM", result);
        Assert.Contains("WHERE", result);
    }

    [Fact]
    public void Format_EmptyInput_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, _formatter.Format(""));
        Assert.Equal(string.Empty, _formatter.Format(null!));
    }

    [Fact]
    public void Format_SelectWithJoin_ContainsJoinKeywords()
    {
        var sql = "SELECT a.col1 FROM table1 a INNER JOIN table2 b ON a.id = b.id WHERE a.col1 = 1";
        var result = _formatter.Format(sql);

        Assert.Contains("INNER JOIN", result);
        Assert.Contains("ON", result);
    }
}
