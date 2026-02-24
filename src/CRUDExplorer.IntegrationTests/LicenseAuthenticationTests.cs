using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using CRUDExplorer.AuthServer.Data;
using CRUDExplorer.AuthServer.Models;

namespace CRUDExplorer.IntegrationTests;

/// <summary>
/// ライセンス認証API統合テスト
/// </summary>
public class LicenseAuthenticationTests : IClassFixture<AuthServerWebApplicationFactory>
{
    private readonly AuthServerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public LicenseAuthenticationTests(AuthServerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Authenticate_ValidLicense_ReturnsToken()
    {
        // Arrange - テストデータの準備
        await SeedTestLicense("TEST-VALI-DLIC-EN01", true, 5);

        var request = new
        {
            LicenseKey = "TEST-VALI-DLIC-EN01",
            DeviceId = "device-001",
            DeviceName = "TestMachine"
        };

        // Act - API呼び出し
        var response = await _client.PostAsJsonAsync("/api/license/authenticate", request);

        // Assert - 検証
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Authenticate_InvalidLicense_ReturnsUnauthorized()
    {
        // Arrange
        var request = new
        {
            LicenseKey = "INVA-LIDL-ICEN-SKEY",
            DeviceId = "device-001",
            DeviceName = "TestMachine"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/license/authenticate", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Authenticate_ExpiredLicense_ReturnsUnauthorized()
    {
        // Arrange - 期限切れライセンス
        await SeedTestLicense("TEST-EXPI-REDL-IC01", false, 5);

        var request = new
        {
            LicenseKey = "TEST-EXPI-REDL-IC01",
            DeviceId = "device-001",
            DeviceName = "TestMachine"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/license/authenticate", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Authenticate_MaxDevicesExceeded_ReturnsConflict()
    {
        // Arrange - 最大デバイス数が1のライセンスに2台目を追加
        var licenseKey = "TEST-MAXD-EVIC-ES01";
        await SeedTestLicense(licenseKey, true, 1);

        // 1台目のデバイスを認証
        await _client.PostAsJsonAsync("/api/license/authenticate", new
        {
            LicenseKey = licenseKey,
            DeviceId = "device-001",
            DeviceName = "Device1"
        });

        // Act - 2台目のデバイスで認証試行
        var response = await _client.PostAsJsonAsync("/api/license/authenticate", new
        {
            LicenseKey = licenseKey,
            DeviceId = "device-002",
            DeviceName = "Device2"
        });

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Authenticate_SameDeviceTwice_ReturnsToken()
    {
        // Arrange
        var licenseKey = "TEST-SAME-DEVI-CE01";
        await SeedTestLicense(licenseKey, true, 1);

        var request = new
        {
            LicenseKey = licenseKey,
            DeviceId = "device-001",
            DeviceName = "TestMachine"
        };

        // Act - 同じデバイスで2回認証
        var response1 = await _client.PostAsJsonAsync("/api/license/authenticate", request);
        var response2 = await _client.PostAsJsonAsync("/api/license/authenticate", request);

        // Assert - 両方とも成功する
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
    }

    [Fact]
    public async Task Validate_ValidToken_ReturnsOk()
    {
        // Arrange - トークン取得
        var licenseKey = "TEST-VALI-DATO-KEN1";
        await SeedTestLicense(licenseKey, true, 5);

        var authResponse = await _client.PostAsJsonAsync("/api/license/authenticate", new
        {
            LicenseKey = licenseKey,
            DeviceId = "device-001",
            DeviceName = "TestMachine"
        });

        var authResult = await authResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
        Assert.NotNull(authResult?.Token);

        // Act - トークン検証
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.Token);
        var response = await _client.GetAsync("/api/license/validate");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Validate_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        var response = await _client.GetAsync("/api/license/validate");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Validate_NoToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/license/validate");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// テスト用ライセンスキーをデータベースにシード
    /// </summary>
    private async Task SeedTestLicense(string licenseKey, bool isActive, int maxDevices)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var license = new LicenseKey
        {
            Id = Guid.NewGuid(),
            Key = licenseKey,
            UserId = user.Id,
            IsActive = isActive,
            ExpiresAt = isActive ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddDays(-1),
            MaxDevices = maxDevices,
            IssuedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        context.LicenseKeys.Add(license);
        await context.SaveChangesAsync();
    }
}

/// <summary>
/// 認証レスポンスモデル
/// </summary>
public class AuthenticationResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}
