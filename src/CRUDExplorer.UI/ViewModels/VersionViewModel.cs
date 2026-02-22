using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Media;

namespace CRUDExplorer.UI.ViewModels;

public partial class VersionViewModel : ViewModelBase
{
    private static readonly HttpClient _httpClient = new HttpClient();

    [ObservableProperty]
    private string _versionNumber = string.Empty;

    [ObservableProperty]
    private bool _isDemoMode = true;

    [ObservableProperty]
    private string _licenseKey = string.Empty;

    [ObservableProperty]
    private string _emailAddress = string.Empty;

    [ObservableProperty]
    private string _authenticationStatus = string.Empty;

    [ObservableProperty]
    private IBrush _authenticationStatusColor = Brushes.Black;

    [ObservableProperty]
    private string _licenseInfo = "ライセンスが認証されていません。デモモードで動作しています。";

    public VersionViewModel()
    {
        // Get version from assembly
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        VersionNumber = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";

        // Load saved license info
        LoadLicenseInfo();
    }

    [RelayCommand]
    private void Authenticate()
    {
        // Validate license key format (16 digits with optional hyphens)
        var licenseKeyPattern = @"^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$";
        var normalizedKey = LicenseKey.Replace("-", "");

        if (normalizedKey.Length != 16 || !Regex.IsMatch(LicenseKey, licenseKeyPattern))
        {
            AuthenticationStatus = "ライセンスキーの形式が正しくありません（XXXX-XXXX-XXXX-XXXX）";
            AuthenticationStatusColor = Brushes.Red;
            return;
        }

        // Validate email address format
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(EmailAddress, emailPattern))
        {
            AuthenticationStatus = "メールアドレスの形式が正しくありません";
            AuthenticationStatusColor = Brushes.Red;
            return;
        }

        // TODO: Call cloud authentication API
        AuthenticationStatus = "認証サーバーに接続中...";
        AuthenticationStatusColor = Brushes.Blue;

        // Simulate authentication (replace with actual API call)
        AuthenticateWithCloudApi();
    }

    private async void AuthenticateWithCloudApi()
    {
        try
        {
            // TODO: Replace with actual authentication server URL
            var authServerUrl = "https://localhost:5001/api/license/authenticate";

            var requestData = new
            {
                licenseKey = LicenseKey,
                email = EmailAddress,
                deviceId = GetDeviceId()
            };

            var jsonContent = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(authServerUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthenticationResponse>(responseData);

                if (authResponse?.Success == true)
                {
                    // Save authentication token
                    SaveAuthenticationToken(authResponse.Token ?? string.Empty);

                    IsDemoMode = false;
                    LicenseInfo = $"ライセンス認証済み\n登録メール: {EmailAddress}\nライセンスキー: {LicenseKey}";
                    AuthenticationStatus = "認証に成功しました！";
                    AuthenticationStatusColor = Brushes.Green;
                }
                else
                {
                    AuthenticationStatus = $"認証に失敗しました: {authResponse?.Message ?? "不明なエラー"}";
                    AuthenticationStatusColor = Brushes.Red;
                }
            }
            else
            {
                AuthenticationStatus = $"認証サーバーエラー: {response.StatusCode}";
                AuthenticationStatusColor = Brushes.Red;
            }
        }
        catch (HttpRequestException)
        {
            AuthenticationStatus = "認証サーバーに接続できません（オフラインまたはサーバー停止中）";
            AuthenticationStatusColor = Brushes.Orange;
        }
        catch (Exception ex)
        {
            AuthenticationStatus = $"エラーが発生しました: {ex.Message}";
            AuthenticationStatusColor = Brushes.Red;
        }
    }

    [RelayCommand]
    private void Close()
    {
        // TODO: Close window
    }

    private string GetDeviceId()
    {
        // Generate unique device ID based on machine name and username
        var machineInfo = $"{Environment.MachineName}_{Environment.UserName}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(machineInfo));
    }

    private void LoadLicenseInfo()
    {
        // TODO: Load saved license info from Settings.cs
    }

    private void SaveAuthenticationToken(string token)
    {
        // TODO: Save authentication token to Settings.cs
    }
}

internal class AuthenticationResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}
