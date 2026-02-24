using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.Core.Tests;

public class RegexUtilitiesTests
{
    [Theory]
    [InlineData("Hello World", "World", true)]
    [InlineData("Hello World", "world", false)]
    [InlineData("Hello World", "xyz", false)]
    [InlineData("", "test", false)]
    [InlineData("test", "", false)]
    public void RegMatch_ReturnsExpectedResult(string input, string pattern, bool expected)
    {
        Assert.Equal(expected, RegexUtilities.RegMatch(input, pattern));
    }

    [Theory]
    [InlineData("Hello World", "world", true)]
    [InlineData("Hello World", "WORLD", true)]
    [InlineData("Hello World", "xyz", false)]
    public void RegMatchI_CaseInsensitive(string input, string pattern, bool expected)
    {
        Assert.Equal(expected, RegexUtilities.RegMatchI(input, pattern));
    }

    [Fact]
    public void RegMatch_InvalidPattern_ReturnsFalse()
    {
        Assert.False(RegexUtilities.RegMatch("test", "[invalid"));
    }
}
