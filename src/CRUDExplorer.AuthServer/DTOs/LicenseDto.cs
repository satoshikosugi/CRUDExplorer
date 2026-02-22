namespace CRUDExplorer.AuthServer.DTOs;

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
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// トークン検証リクエスト
/// </summary>
public class ValidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// トークン検証レスポンス
/// </summary>
public class ValidateTokenResponse
{
    public bool IsValid { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// ライセンス作成リクエスト
/// </summary>
public class CreateLicenseRequest
{
    public string EmailAddress { get; set; } = string.Empty;
    public int MaxDevices { get; set; } = 1;
    public string ProductType { get; set; } = "Standard";
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// ライセンス作成レスポンス
/// </summary>
public class CreateLicenseResponse
{
    public Guid Id { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int MaxDevices { get; set; }
    public string ProductType { get; set; } = string.Empty;
}
