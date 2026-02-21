using System.Net.Http.Json;

namespace CRUDExplorer.Core.Utilities;

/// <summary>
/// ライセンス認証クライアント（VB.NET CommonLicence.vb → クラウド認証に変更）
/// 認証サーバーと通信してライセンスキーの認証を行う
/// </summary>
public class LicenseClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private string? _cachedToken;
    private DateTime _tokenExpiry;

    /// <summary>
    /// 認証サーバーのベースURL
    /// </summary>
    public string BaseUrl { get; }

    public LicenseClient(string? baseUrl = null)
    {
        _httpClient = new HttpClient();
        BaseUrl = !string.IsNullOrEmpty(baseUrl) ? baseUrl : "https://auth.crudexplorer.com";
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    /// <summary>
    /// メールアドレスとライセンスキーで認証
    /// </summary>
    /// <param name="emailAddress">メールアドレス</param>
    /// <param name="licenseKey">ライセンスキー</param>
    /// <param name="deviceId">デバイスID（マシン固有のID）</param>
    /// <returns>認証結果</returns>
    public async Task<AuthenticationResult> AuthenticateAsync(string emailAddress, string licenseKey, string? deviceId = null)
    {
        try
        {
            var request = new AuthenticationRequest
            {
                EmailAddress = emailAddress,
                LicenseKey = licenseKey,
                DeviceId = deviceId ?? GetDeviceId()
            };

            var response = await _httpClient.PostAsJsonAsync("/api/license/authenticate", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
                if (result != null && result.IsValid)
                {
                    _cachedToken = result.Token;
                    _tokenExpiry = result.ExpiresAt;
                    return new AuthenticationResult
                    {
                        IsValid = true,
                        Token = result.Token,
                        ExpiresAt = result.ExpiresAt,
                        Message = "認証に成功しました。"
                    };
                }
                return new AuthenticationResult
                {
                    IsValid = false,
                    Message = result?.Message ?? "認証に失敗しました。"
                };
            }

            return new AuthenticationResult
            {
                IsValid = false,
                Message = $"認証サーバーとの通信に失敗しました。ステータス: {response.StatusCode}"
            };
        }
        catch (HttpRequestException ex)
        {
            return new AuthenticationResult
            {
                IsValid = false,
                Message = $"認証サーバーに接続できません: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResult
            {
                IsValid = false,
                Message = $"認証エラー: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// トークンの有効性を確認
    /// </summary>
    public async Task<bool> ValidateTokenAsync()
    {
        if (string.IsNullOrEmpty(_cachedToken) || DateTime.UtcNow >= _tokenExpiry)
            return false;

        try
        {
            var request = new { Token = _cachedToken };
            var response = await _httpClient.PostAsJsonAsync("/api/license/validate", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ValidateResponse>();
                return result?.IsValid ?? false;
            }
        }
        catch
        {
            // 通信エラー時はオフライン検証にフォールバック
        }

        return ValidateOffline();
    }

    /// <summary>
    /// オフライン検証（キャッシュされたトークンの有効期限で判定）
    /// </summary>
    public bool ValidateOffline()
    {
        return !string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry;
    }

    /// <summary>
    /// ライセンスキーのフォーマット検証（16桁）
    /// </summary>
    /// <param name="licenseKey">ライセンスキー</param>
    /// <returns>フォーマットが正しい場合true</returns>
    public static bool ValidateKeyFormat(string licenseKey)
    {
        if (string.IsNullOrEmpty(licenseKey))
            return false;

        var cleanKey = licenseKey.Replace("-", "").Replace(" ", "");
        return cleanKey.Length == 16 && cleanKey.All(c => char.IsLetterOrDigit(c));
    }

    /// <summary>
    /// メールアドレスのフォーマット検証
    /// </summary>
    public static bool ValidateEmailFormat(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        return System.Text.RegularExpressions.Regex.IsMatch(
            email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    /// <summary>
    /// デバイスIDを取得（マシン固有のID）
    /// </summary>
    private static string GetDeviceId()
    {
        return Environment.MachineName + "-" + Environment.UserName;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 認証リクエスト
/// </summary>
public class AuthenticationRequest
{
    public string EmailAddress { get; set; } = string.Empty;
    public string LicenseKey { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
}

/// <summary>
/// 認証レスポンス
/// </summary>
public class AuthenticationResponse
{
    public bool IsValid { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 認証結果
/// </summary>
public class AuthenticationResult
{
    public bool IsValid { get; set; }
    public string? Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// トークン検証レスポンス
/// </summary>
public class ValidateResponse
{
    public bool IsValid { get; set; }
    public DateTime ExpiresAt { get; set; }
}
