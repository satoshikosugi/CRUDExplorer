using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.Core.Tests;

public class ProgramIdExtractorTests
{
    [Fact]
    public void GetProgramId_WithPattern_ExtractsId()
    {
        var result = ProgramIdExtractor.GetProgramId("PRG001_Module.vb", @"(PRG\d+)");
        Assert.Equal("PRG001", result);
    }

    [Fact]
    public void GetProgramId_NoMatch_ReturnsEmpty()
    {
        var result = ProgramIdExtractor.GetProgramId("Module.vb", @"(PRG\d+)");
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetProgramId_EmptyInputs_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, ProgramIdExtractor.GetProgramId("", "pattern"));
        Assert.Equal(string.Empty, ProgramIdExtractor.GetProgramId("input", ""));
        Assert.Equal(string.Empty, ProgramIdExtractor.GetProgramId(null!, "pattern"));
    }

    [Fact]
    public void GetProgramId_InvalidPattern_ReturnsEmpty()
    {
        var result = ProgramIdExtractor.GetProgramId("test", "[invalid");
        Assert.Equal(string.Empty, result);
    }
}
