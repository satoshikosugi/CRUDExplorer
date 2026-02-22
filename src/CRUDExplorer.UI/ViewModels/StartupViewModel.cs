using System;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CRUDExplorer.UI.ViewModels;

public partial class StartupViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _versionText = string.Empty;

    [ObservableProperty]
    private string _loadingMessage = "初期化中...";

    public StartupViewModel()
    {
        // Get version from assembly
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        VersionText = version != null ? $"Version {version.Major}.{version.Minor}.{version.Build}" : "Version 1.0.0";

        // Start initialization
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            LoadingMessage = "設定を読み込んでいます...";
            await Task.Delay(500); // Simulate loading

            LoadingMessage = "データベース接続を確認しています...";
            await Task.Delay(500); // Simulate loading

            LoadingMessage = "ライセンスを確認しています...";
            await Task.Delay(500); // Simulate loading

            LoadingMessage = "準備完了";
            await Task.Delay(300);

            // TODO: Close splash screen and show main window
        }
        catch (Exception ex)
        {
            LoadingMessage = $"エラー: {ex.Message}";
        }
    }
}
