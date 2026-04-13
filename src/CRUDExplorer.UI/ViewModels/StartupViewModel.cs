using System;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CRUDExplorer.Core.Models;

namespace CRUDExplorer.UI.ViewModels;

public partial class StartupViewModel : ViewModelBase
{
    private readonly Action _closeWindow;

    [ObservableProperty]
    private string _versionText = string.Empty;

    [ObservableProperty]
    private string _loadingMessage = "初期化中...";

    [ObservableProperty]
    private bool _isDemoMode = false;

    public StartupViewModel(Action? closeWindow = null)
    {
        _closeWindow = closeWindow ?? (() => { });

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

            // ライセンス設定を読み込み（ライセンスキーが空ならデモモード）
            var settings = Settings.Load();
            IsDemoMode = string.IsNullOrEmpty(settings.LicenseKey);

            LoadingMessage = "データベース接続を確認しています...";
            await Task.Delay(500); // Simulate loading

            LoadingMessage = "ライセンスを確認しています...";
            await Task.Delay(500); // Simulate loading

            LoadingMessage = "準備完了";
            await Task.Delay(300);

            // スプラッシュ画面を閉じてメインウィンドウへ
            _closeWindow();
        }
        catch (Exception ex)
        {
            LoadingMessage = $"エラー: {ex.Message}";
        }
    }
}
