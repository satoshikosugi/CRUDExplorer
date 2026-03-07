using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(CRUDExplorer.UI.Tests.TestAppBuilder))]

namespace CRUDExplorer.UI.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
