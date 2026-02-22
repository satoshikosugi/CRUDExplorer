using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using CRUDExplorer.AuthServer.Data;
using CRUDExplorer.AuthServer.Models;

namespace CRUDExplorer.IntegrationTests;

/// <summary>
/// 管理API統合テスト
/// </summary>
public class AdminApiTests : IClassFixture<AuthServerWebApplicationFactory>
{
    private readonly AuthServerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AdminApiTests(AuthServerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetUsers_ReturnsUserList()
    {
        // Arrange - テストユーザーを作成
        await SeedTestUsers();

        // Act
        var response = await _client.GetAsync("/api/admin/users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<User>>();
        Assert.NotNull(users);
        Assert.NotEmpty(users);
    }

    [Fact]
    public async Task GetLicenses_ReturnsLicenseList()
    {
        // Arrange
        await SeedTestLicenses();

        // Act
        var response = await _client.GetAsync("/api/admin/licenses");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var licenses = await response.Content.ReadFromJsonAsync<List<LicenseKey>>();
        Assert.NotNull(licenses);
        Assert.NotEmpty(licenses);
    }

    [Fact]
    public async Task CreateLicense_ValidData_ReturnsCreated()
    {
        // Arrange
        var userId = await SeedTestUser("newuser@example.com");

        var request = new
        {
            UserId = userId,
            MaxDevices = 3,
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/admin/licenses", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var license = await response.Content.ReadFromJsonAsync<LicenseKey>();
        Assert.NotNull(license);
        Assert.NotNull(license.Key);
        Assert.Equal(3, license.MaxDevices);
    }

    [Fact]
    public async Task GetDeviceActivations_ReturnsActivationList()
    {
        // Arrange
        await SeedTestDeviceActivations();

        // Act
        var response = await _client.GetAsync("/api/admin/devices");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var devices = await response.Content.ReadFromJsonAsync<List<DeviceActivation>>();
        Assert.NotNull(devices);
        Assert.NotEmpty(devices);
    }

    [Fact]
    public async Task DeactivateDevice_ValidDeviceId_ReturnsOk()
    {
        // Arrange
        var deviceId = await SeedTestDeviceActivation("device-to-deactivate");

        // Act
        var response = await _client.DeleteAsync($"/api/admin/devices/{deviceId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAuditLogs_ReturnsLogList()
    {
        // Arrange
        await SeedTestAuditLogs();

        // Act
        var response = await _client.GetAsync("/api/admin/audit-logs");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var logs = await response.Content.ReadFromJsonAsync<List<AuditLog>>();
        Assert.NotNull(logs);
        Assert.NotEmpty(logs);
    }

    [Fact]
    public async Task GetAuditLogs_WithFilters_ReturnsFilteredLogs()
    {
        // Arrange
        var userId = await SeedTestUser("filtertest@example.com");
        await SeedTestAuditLogsWithUserId(userId);

        // Act - ユーザーIDでフィルタ
        var response = await _client.GetAsync($"/api/admin/audit-logs?userId={userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var logs = await response.Content.ReadFromJsonAsync<List<AuditLog>>();
        Assert.NotNull(logs);
        Assert.All(logs, log => Assert.Equal(userId, log.UserId));
    }

    [Fact]
    public async Task RevokeLicense_ValidLicenseId_ReturnsOk()
    {
        // Arrange
        var licenseId = await SeedTestLicense("TEST-LICENSE-TO-REVOKE", true);

        // Act
        var response = await _client.PutAsync($"/api/admin/licenses/{licenseId}/revoke", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify license is revoked
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var license = await context.LicenseKeys.FindAsync(licenseId);
        Assert.NotNull(license);
        Assert.False(license.IsActive);
    }

    /// <summary>
    /// テスト用ユーザーを作成
    /// </summary>
    private async Task<Guid> SeedTestUser(string emailAddress)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            EmailAddress = emailAddress,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user.Id;
    }

    /// <summary>
    /// テスト用ユーザー複数作成
    /// </summary>
    private async Task SeedTestUsers()
    {
        await SeedTestUser("user1@example.com");
        await SeedTestUser("user2@example.com");
        await SeedTestUser("user3@example.com");
    }

    /// <summary>
    /// テスト用ライセンスを作成
    /// </summary>
    private async Task<Guid> SeedTestLicense(string key, bool isActive)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var userId = await SeedTestUser($"licenseuser-{key}@example.com");

        var license = new LicenseKey
        {
            Id = Guid.NewGuid(),
            Key = key,
            UserId = userId,
            IsActive = isActive,
            ExpiresAt = DateTime.UtcNow.AddYears(1),
            MaxDevices = 5,
            IssuedAt = DateTime.UtcNow
        };

        context.LicenseKeys.Add(license);
        await context.SaveChangesAsync();
        return license.Id;
    }

    /// <summary>
    /// テスト用ライセンス複数作成
    /// </summary>
    private async Task SeedTestLicenses()
    {
        await SeedTestLicense("LICENSE-001", true);
        await SeedTestLicense("LICENSE-002", true);
        await SeedTestLicense("LICENSE-003", false);
    }

    /// <summary>
    /// テスト用デバイスアクティベーションを作成
    /// </summary>
    private async Task<Guid> SeedTestDeviceActivation(string deviceId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var licenseId = await SeedTestLicense($"DEVICE-LICENSE-{deviceId}", true);

        var activation = new DeviceActivation
        {
            Id = Guid.NewGuid(),
            LicenseKeyId = licenseId,
            DeviceId = deviceId,
            DeviceName = "TestMachine",
            ActivatedAt = DateTime.UtcNow,
            LastSeenAt = DateTime.UtcNow
        };

        context.DeviceActivations.Add(activation);
        await context.SaveChangesAsync();
        return activation.Id;
    }

    /// <summary>
    /// テスト用デバイスアクティベーション複数作成
    /// </summary>
    private async Task SeedTestDeviceActivations()
    {
        await SeedTestDeviceActivation("device-001");
        await SeedTestDeviceActivation("device-002");
        await SeedTestDeviceActivation("device-003");
    }

    /// <summary>
    /// テスト用監査ログを作成
    /// </summary>
    private async Task SeedTestAuditLogs()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var logs = new[]
        {
            new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = null,
                Action = "SYSTEM_START",
                Details = "System started",
                Timestamp = DateTime.UtcNow.AddHours(-2),
                IpAddress = "192.168.1.1"
            },
            new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = null,
                Action = "LICENSE_ACTIVATED",
                Details = "License activated",
                Timestamp = DateTime.UtcNow.AddHours(-1),
                IpAddress = "192.168.1.1"
            },
            new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = null,
                Action = "LOGOUT",
                Details = "User logged out",
                Timestamp = DateTime.UtcNow,
                IpAddress = "192.168.1.2"
            }
        };

        context.AuditLogs.AddRange(logs);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 特定ユーザーIDで監査ログを作成
    /// </summary>
    private async Task SeedTestAuditLogsWithUserId(Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        var logs = new[]
        {
            new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = "LOGIN",
                Details = "User logged in",
                Timestamp = DateTime.UtcNow.AddHours(-1),
                IpAddress = "192.168.1.100"
            },
            new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = "DATA_ACCESS",
                Details = "User accessed data",
                Timestamp = DateTime.UtcNow,
                IpAddress = "192.168.1.100"
            }
        };

        context.AuditLogs.AddRange(logs);
        await context.SaveChangesAsync();
    }
}
