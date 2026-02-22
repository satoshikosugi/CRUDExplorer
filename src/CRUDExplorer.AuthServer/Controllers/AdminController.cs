using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CRUDExplorer.AuthServer.Data;
using CRUDExplorer.AuthServer.Services;
using CRUDExplorer.AuthServer.DTOs;
using CRUDExplorer.AuthServer.Models;

namespace CRUDExplorer.AuthServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // 管理者機能は認証必須
public class AdminController : ControllerBase
{
    private readonly AuthDbContext _context;
    private readonly LicenseGenerationService _licenseService;
    private readonly AuditLogService _auditLogService;

    public AdminController(
        AuthDbContext context,
        LicenseGenerationService licenseService,
        AuditLogService auditLogService)
    {
        _context = context;
        _licenseService = licenseService;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// 新規ライセンス作成
    /// </summary>
    [HttpPost("licenses")]
    [ProducesResponseType(typeof(CreateLicenseResponse), 200)]
    public async Task<IActionResult> CreateLicense([FromBody] CreateLicenseRequest request)
    {
        // ユーザーを検索または作成
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                EmailAddress = request.EmailAddress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // ライセンスキー作成
        var license = await _licenseService.CreateLicenseKeyAsync(
            user.Id,
            request.MaxDevices,
            request.ProductType,
            request.ExpiresAt);

        await _auditLogService.LogActionAsync(
            user.Id,
            "LicenseCreated",
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            $"{{\"licenseKey\":\"{license.Key}\",\"productType\":\"{license.ProductType}\"}}");

        var response = new CreateLicenseResponse
        {
            Id = license.Id,
            LicenseKey = license.Key,
            EmailAddress = user.EmailAddress,
            IssuedAt = license.IssuedAt,
            ExpiresAt = license.ExpiresAt,
            MaxDevices = license.MaxDevices,
            ProductType = license.ProductType
        };

        return Ok(response);
    }

    /// <summary>
    /// ライセンス一覧取得
    /// </summary>
    [HttpGet("licenses")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetLicenses([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var licenses = await _context.LicenseKeys
            .Include(lk => lk.User)
            .Include(lk => lk.DeviceActivations)
            .OrderByDescending(lk => lk.IssuedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(lk => new
            {
                lk.Id,
                lk.Key,
                EmailAddress = lk.User.EmailAddress,
                lk.IssuedAt,
                lk.ExpiresAt,
                lk.IsActive,
                lk.MaxDevices,
                lk.ProductType,
                ActiveDevices = lk.DeviceActivations.Count
            })
            .ToListAsync();

        return Ok(licenses);
    }

    /// <summary>
    /// ユーザー一覧取得
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var users = await _context.Users
            .Include(u => u.LicenseKeys)
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new
            {
                u.Id,
                u.EmailAddress,
                u.CreatedAt,
                u.UpdatedAt,
                u.IsActive,
                LicenseCount = u.LicenseKeys.Count
            })
            .ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// 監査ログ取得
    /// </summary>
    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetAuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var logs = await _context.AuditLogs
            .Include(al => al.User)
            .OrderByDescending(al => al.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(al => new
            {
                al.Id,
                UserId = al.UserId,
                EmailAddress = al.User != null ? al.User.EmailAddress : null,
                al.Action,
                al.Timestamp,
                al.IpAddress,
                al.Details
            })
            .ToListAsync();

        return Ok(logs);
    }
}
