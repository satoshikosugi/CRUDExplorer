using CRUDExplorer.Core.Utilities;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.Core.Tests;

public class FileSystemHelperTests
{
    [Fact]
    public void ReadDictionary_NonExistentFile_ReturnsEmptyDict()
    {
        var result = FileSystemHelper.ReadDictionary("/nonexistent/path/file.tsv");
        Assert.Empty(result);
    }

    [Fact]
    public void ReadDictionary_ValidTsvFile_ReturnsDictionary()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "TABLE1\tテーブル1\nTABLE2\tテーブル2\n");
            var result = FileSystemHelper.ReadDictionary(tempFile);
            Assert.Equal(2, result.Count);
            Assert.Equal("テーブル1", result["TABLE1"]);
            Assert.Equal("テーブル2", result["TABLE2"]);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void ReadDictionary_IgnoresMalformedLines()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "VALID\tValue\nINVALID\n");
            var result = FileSystemHelper.ReadDictionary(tempFile);
            Assert.Single(result);
            Assert.Equal("Value", result["VALID"]);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void ReadTableDef_ValidFile_PopulatesDictionary()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile,
                "TABLE1\tCOL1\t列1\t1\tY\t\tY\tVARCHAR\t100\t0\n" +
                "TABLE1\tCOL2\t列2\t2\t\t\t\tINT\t10\t0\n");
            var dict = new Dictionary<string, TableDefinition>(StringComparer.OrdinalIgnoreCase);
            FileSystemHelper.ReadTableDef(tempFile, dict);

            Assert.Single(dict);
            Assert.True(dict.ContainsKey("TABLE1"));
            Assert.Equal(2, dict["TABLE1"].Columns.Count);
            Assert.Equal("列1", dict["TABLE1"].Columns["COL1"].AttributeName);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void DeleteComment_RemovesBlockComments()
    {
        var input = "SELECT /* comment */ * FROM table1";
        var result = FileSystemHelper.DeleteComment(input, false);
        Assert.Equal("SELECT  * FROM table1", result);
    }

    [Fact]
    public void DeleteComment_RemovesLineComments()
    {
        var input = "SELECT * FROM table1 --this is a comment\nWHERE col = 1";
        var result = FileSystemHelper.DeleteComment(input, false);
        Assert.Equal("SELECT * FROM table1 \nWHERE col = 1", result);
    }

    [Fact]
    public void DeleteComment_KeepsLineNumbers()
    {
        var input = "SELECT\n/* multi\nline\ncomment */\n* FROM table1";
        var result = FileSystemHelper.DeleteComment(input, true);
        // Should keep newlines from the block comment (3 newlines in the comment)
        Assert.Contains("\n\n\n", result);
    }
}
