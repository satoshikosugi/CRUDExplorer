using CRUDExplorer.AuthServer.Data;
using CRUDExplorer.AuthServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRUDExplorer.AuthServer.Services;

/// <summary>
/// 認証サービス
/// </summary>
public class AuthenticationService
{
    private readonly AuthDbContext _context;
    private readonly LicenseGenerationService _licenseService;
    private readonly AuditLogService _auditLogService;
    private readonly IConfiguration _configuration;

    public AuthenticationService(
        AuthDbContext context,
        LicenseGenerationService licenseService,
        AuditLogService auditLogService,
        IConfiguration configuration)
    {
        _context = context;
        _licenseService = licenseService;
        _auditLogService = auditLogService;
        _configuration = configuration;
    }

    /// <summary>
    /// ライセンスキーとデバイスIDで認証（メールアドレス不要版）
    /// </summary>
    public async Task<AuthenticationResult> AuthenticateWithLicenseKeyAsync(string licenseKey, string deviceId, string? deviceName = null, string? ipAddress = null)
    {
        // ライセンスキー検証
        var license = await _licenseService.ValidateLicenseKeyAsync(licenseKey);
        if (license == null)
        {
            await _auditLogService.LogActionAsync(null, "AuthenticationFailed", ipAddress, $"{{\"licenseKey\":\"{licenseKey}\",\"reason\":\"Invalid license key\"}}");
            return new AuthenticationResult { IsValid = false, Message = "Invalid license key" };
        }

        // デバイスアクティベーション確認・更新
        var activation = await _context.DeviceActivations
            .FirstOrDefaultAsync(da => da.LicenseKeyId == license.Id && da.DeviceId == deviceId);

        if (activation == null)
        {
            // 新規デバイス登録
            var activeDevicesCount = await _context.DeviceActivations
                .CountAsync(da => da.LicenseKeyId == license.Id);

            if (activeDevicesCount >= license.MaxDevices)
            {
                await _auditLogService.LogActionAsync(license.UserId, "AuthenticationFailed", ipAddress, $"{{\"licenseKey\":\"{licenseKey}\",\"reason\":\"Max devices reached\"}}");
                return new AuthenticationResult { IsValid = false, Message = "Maximum device limit reached", ErrorCode = "MAX_DEVICES_EXCEEDED" };
            }

            activation = new DeviceActivation
            {
                Id = Guid.NewGuid(),
                LicenseKeyId = license.Id,
                DeviceId = deviceId,
                DeviceName = deviceName,
                ActivatedAt = DateTime.UtcNow,
                LastSeenAt = DateTime.UtcNow
            };
            _context.DeviceActivations.Add(activation);
        }
        else
        {
            // 既存デバイス更新
            activation.LastSeenAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(deviceName))
            {
                activation.DeviceName = deviceName;
            }
        }

        await _context.SaveChangesAsync();

        // JWTトークン生成
        var token = GenerateJwtToken(license.User, license);

        await _auditLogService.LogActionAsync(license.UserId, "AuthenticationSuccess", ipAddress, $"{{\"licenseKey\":\"{licenseKey}\",\"deviceId\":\"{deviceId}\"}}");

        return new AuthenticationResult
        {
            IsValid = true,
            Token = token,
            ExpiresAt = license.ExpiresAt ?? DateTime.UtcNow.AddYears(100),
            Message = "Authentication successful"
        };
    }

    /// <summary>
    /// ライセンスキーとメールアドレスで認証
    /// </summary>
    public async Task<AuthenticationResult> AuthenticateAsync(string emailAddress, string licenseKey, string deviceId, string? ipAddress = null)
    {
        // ライセンスキー検証
        var license = await _licenseService.ValidateLicenseKeyAsync(licenseKey);
        if (license == null)
        {
            await _auditLogService.LogActionAsync(null, "AuthenticationFailed", ipAddress, $"{{\"email\":\"{emailAddress}\",\"reason\":\"Invalid license key\"}}");
            return new AuthenticationResult { IsValid = false, Message = "Invalid license key" };
        }

        // メールアドレス確認
        if (!string.Equals(license.User.EmailAddress, emailAddress, StringComparison.OrdinalIgnoreCase))
        {
            await _auditLogService.LogActionAsync(license.UserId, "AuthenticationFailed", ipAddress, $"{{\"email\":\"{emailAddress}\",\"reason\":\"Email mismatch\"}}");
            return new AuthenticationResult { IsValid = false, Message = "Email address does not match" };
        }

        // デバイスアクティベーション確認・更新
        var activation = await _context.DeviceActivations
            .FirstOrDefaultAsync(da => da.LicenseKeyId == license.Id && da.DeviceId == deviceId);

        if (activation == null)
        {
            // 新規デバイス登録
            var activeDevicesCount = await _context.DeviceActivations
                .CountAsync(da => da.LicenseKeyId == license.Id);

            if (activeDevicesCount >= license.MaxDevices)
            {
                await _auditLogService.LogActionAsync(license.UserId, "AuthenticationFailed", ipAddress, $"{{\"email\":\"{emailAddress}\",\"reason\":\"Max devices reached\"}}");
                return new AuthenticationResult { IsValid = false, Message = "Maximum device limit reached" };
            }

            activation = new DeviceActivation
            {
                Id = Guid.NewGuid(),
                LicenseKeyId = license.Id,
                DeviceId = deviceId,
                ActivatedAt = DateTime.UtcNow,
                LastSeenAt = DateTime.UtcNow
            };
            _context.DeviceActivations.Add(activation);
        }
        else
        {
            // 既存デバイス更新
            activation.LastSeenAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // JWTトークン生成
        var token = GenerateJwtToken(license.User, license);

        await _auditLogService.LogActionAsync(license.UserId, "AuthenticationSuccess", ipAddress, $"{{\"email\":\"{emailAddress}\",\"deviceId\":\"{deviceId}\"}}");

        return new AuthenticationResult
        {
            IsValid = true,
            Token = token,
            ExpiresAt = license.ExpiresAt ?? DateTime.UtcNow.AddYears(100),
            Message = "Authentication successful"
        };
    }

    /// <summary>
    /// JWTトークンを生成
    /// </summary>
    private string GenerateJwtToken(User user, LicenseKey license)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:SecretKey"] ?? "CRUDExplorer-Default-Secret-Key-Please-Change-In-Production-Min-32-Chars"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("license_key", license.Key),
            new Claim("product_type", license.ProductType)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "CRUDExplorer.AuthServer",
            audience: _configuration["Jwt:Audience"] ?? "CRUDExplorer.Client",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpirationHours"] ?? "24")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// トークンを検証
    /// </summary>
    public async Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecretKey"] ?? "CRUDExplorer-Default-Secret-Key-Please-Change-In-Production-Min-32-Chars"));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"] ?? "CRUDExplorer.AuthServer",
                ValidAudience = _configuration["Jwt:Audience"] ?? "CRUDExplorer.Client",
                IssuerSigningKey = securityKey
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            return new TokenValidationResult
            {
                IsValid = true,
                ExpiresAt = jwtToken.ValidTo
            };
        }
        catch
        {
            return new TokenValidationResult
            {
                IsValid = false
            };
        }
    }
}

/// <summary>
/// 認証結果
/// </summary>
public class AuthenticationResult
{
    public bool IsValid { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
}

/// <summary>
/// トークン検証結果
/// </summary>
public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
