using CRUDExplorer.Core.Utilities;

namespace CRUDExplorer.Core.Tests;

public class GlobalStateTests
{
    [Fact]
    public void Instance_ReturnsSameInstance()
    {
        var instance1 = GlobalState.Instance;
        var instance2 = GlobalState.Instance;
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Reset_ClearsAllCollections()
    {
        var state = GlobalState.Instance;
        state.TableNames["TEST"] = "テスト";
        state.IsDemoMode = false;

        state.Reset();

        Assert.Empty(state.TableNames);
        Assert.True(state.IsDemoMode);
    }
}
