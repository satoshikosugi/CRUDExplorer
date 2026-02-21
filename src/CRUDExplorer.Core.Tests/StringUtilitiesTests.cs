using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.Core.Tests;

public class StringUtilitiesTests
{
    [Theory]
    [InlineData("Hello", 3, "llo")]
    [InlineData("Hello", 5, "Hello")]
    [InlineData("Hello", 10, "Hello")]
    [InlineData("Hello", 0, "")]
    [InlineData("", 3, "")]
    public void GetRight_ReturnsExpectedResult(string target, int length, string expected)
    {
        Assert.Equal(expected, StringUtilities.GetRight(target, length));
    }

    [Fact]
    public void GetRight_NullTarget_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, StringUtilities.GetRight(null!, 3));
    }

    [Theory]
    [InlineData("abc", 3)]       // ASCII: 1 byte each
    [InlineData("", 0)]
    public void LenB_AsciiChars_ReturnsCorrectByteCount(string target, int expected)
    {
        Assert.Equal(expected, StringUtilities.LenB(target));
    }

    [Fact]
    public void LenB_NullString_ReturnsZero()
    {
        Assert.Equal(0, StringUtilities.LenB(null!));
    }

    [Theory]
    [InlineData("abc", "abc")]           // Alphanumerics pass through
    [InlineData("a.b", "a[.]b")]         // Dot is escaped
    [InlineData("a*b", "a[*]b")]         // Star is escaped
    [InlineData("", "")]
    public void EscapeRegular_EscapesNonAlphanumeric(string input, string expected)
    {
        Assert.Equal(expected, StringUtilities.EscapeRegular(input));
    }

    [Theory]
    [InlineData(new[] { "a", "b", "c" }, 0, "a")]
    [InlineData(new[] { "a", "b", "c" }, 2, "c")]
    [InlineData(new[] { "a", "b", "c" }, 5, "")]
    [InlineData(new[] { "a", "b", "c" }, -1, "")]
    public void GetStringArrayByIndex_ReturnsExpected(string[] array, int index, string expected)
    {
        Assert.Equal(expected, StringUtilities.GetStringArrayByIndex(array, index));
    }

    [Fact]
    public void GetStringArrayByIndex_NullArray_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, StringUtilities.GetStringArrayByIndex(null!, 0));
    }
}
