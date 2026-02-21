using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.Core.Tests;

public class DictionaryHelperTests
{
    [Fact]
    public void GetDictValue_ExistingKey_ReturnsValue()
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["KEY1"] = "Value1"
        };
        Assert.Equal("Value1", DictionaryHelper.GetDictValue(dict, "key1"));
    }

    [Fact]
    public void GetDictValue_MissingKey_ReturnsKey()
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Assert.Equal("missing", DictionaryHelper.GetDictValue(dict, "missing"));
    }

    [Fact]
    public void GetDictValue_MissingKey_ReturnsEmpty_WhenMissingReturnKeyFalse()
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Assert.Equal("", DictionaryHelper.GetDictValue(dict, "missing", false));
    }

    [Fact]
    public void GetDictObject_ExistingKey_ReturnsObject()
    {
        var obj = new object();
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            ["KEY"] = obj
        };
        Assert.Same(obj, DictionaryHelper.GetDictObject(dict, "key"));
    }

    [Fact]
    public void GetDictObject_MissingKey_ReturnsNull()
    {
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        Assert.Null(DictionaryHelper.GetDictObject(dict, "missing"));
    }

    [Fact]
    public void DictExists_ExistingKey_ReturnsTrue()
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["KEY"] = "val"
        };
        Assert.True(DictionaryHelper.DictExists(dict, "key"));
    }

    [Fact]
    public void DictExists_MissingKey_ReturnsFalse()
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Assert.False(DictionaryHelper.DictExists(dict, "missing"));
    }

    [Fact]
    public void DictAdd_NewKey_ReturnsTrue()
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        Assert.True(DictionaryHelper.DictAdd(dict, "key", "value"));
        Assert.Equal("value", dict["key"]);
    }

    [Fact]
    public void DictAdd_DuplicateKey_ReturnsFalse()
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["KEY"] = "existing"
        };
        Assert.False(DictionaryHelper.DictAdd(dict, "key", "new"));
        Assert.Equal("existing", dict["KEY"]);
    }

    [Fact]
    public void SortDictionary_ReturnsSortedKeys()
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["C"] = "3",
            ["A"] = "1",
            ["B"] = "2"
        };
        var sorted = DictionaryHelper.SortDictionary(dict);
        Assert.Equal(new[] { "A", "B", "C" }, sorted.Keys.ToArray());
    }
}
