using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.Core.Tests;

public class LicenseClientTests
{
    [Theory]
    [InlineData("ABCD1234EFGH5678", true)]
    [InlineData("ABCD-1234-EFGH-5678", true)]
    [InlineData("abc", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void ValidateKeyFormat_ValidatesCorrectly(string? key, bool expected)
    {
        Assert.Equal(expected, LicenseClient.ValidateKeyFormat(key!));
    }

    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("test@test.co.jp", true)]
    [InlineData("invalid", false)]
    [InlineData("@missing.com", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void ValidateEmailFormat_ValidatesCorrectly(string? email, bool expected)
    {
        Assert.Equal(expected, LicenseClient.ValidateEmailFormat(email!));
    }
}
