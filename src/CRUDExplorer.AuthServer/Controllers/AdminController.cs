using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CRUDExplorer.AuthServer.Data;
using CRUDExplorer.AuthServer.Services;
using CRUDExplorer.AuthServer.DTOs;
using CRUDExplorer.AuthServer.Models;

namespace CRUDExplorer.AuthServer.Controllers;

/// <summary>
/// 管理者向けAPI（ライセンス管理、ユーザー管理、監査ログ等）
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize] // 統合テスト用に一時的に無効化 - TODO: 本番環境では有効化すること
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
    /// 新規ライセンスを作成
    /// </summary>
    /// <param name="request">ライセンス作成リクエスト（ユーザーID or メールアドレス、最大デバイス数、製品タイプ、有効期限）</param>
    /// <returns>作成されたライセンス情報</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/admin/licenses
    ///     {
    ///        "emailAddress": "user@example.com",
    ///        "maxDevices": 3,
    ///        "productType": "Standard",
    ///        "expiresAt": "2025-12-31T23:59:59Z"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">ライセンス作成成功</response>
    /// <response code="400">不正なリクエスト（ユーザーIDまたはメールアドレスが必要）</response>
    [HttpPost("licenses")]
    [ProducesResponseType(typeof(CreateLicenseResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateLicense([FromBody] CreateLicenseRequest request)
    {
        User user;

        // UserIdが指定されている場合は既存ユーザーを検索
        if (request.UserId.HasValue)
        {
            user = await _context.Users.FindAsync(request.UserId.Value);
            if (user == null)
            {
                return BadRequest(new { error = "User not found" });
            }
        }
        // EmailAddressが指定されている場合は検索または作成
        else if (!string.IsNullOrEmpty(request.EmailAddress))
        {
            user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress);
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
        }
        else
        {
            return BadRequest(new { error = "Either UserId or EmailAddress must be provided" });
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

        return StatusCode(201, response);
    }

    /// <summary>
    /// ライセンス一覧を取得（ページネーション対応）
    /// </summary>
    /// <param name="page">ページ番号（デフォルト: 1）</param>
    /// <param name="pageSize">1ページあたりの件数（デフォルト: 20）</param>
    /// <returns>ライセンス一覧</returns>
    /// <response code="200">ライセンス一覧を返却</response>
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
    /// ユーザー一覧を取得（ページネーション対応）
    /// </summary>
    /// <param name="page">ページ番号（デフォルト: 1）</param>
    /// <param name="pageSize">1ページあたりの件数（デフォルト: 20）</param>
    /// <returns>ユーザー一覧</returns>
    /// <response code="200">ユーザー一覧を返却</response>
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
    /// 監査ログを取得（ページネーション対応、ユーザーIDでフィルタ可能）
    /// </summary>
    /// <param name="userId">フィルタ対象のユーザーID（省略可能）</param>
    /// <param name="page">ページ番号（デフォルト: 1）</param>
    /// <param name="pageSize">1ページあたりの件数（デフォルト: 50）</param>
    /// <returns>監査ログ一覧</returns>
    /// <response code="200">監査ログ一覧を返却</response>
    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] Guid? userId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.AuditLogs.Include(al => al.User).AsQueryable();

        // ユーザーIDでフィルタ
        if (userId.HasValue)
        {
            query = query.Where(al => al.UserId == userId.Value);
        }

        var logs = await query
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

    /// <summary>
    /// デバイスアクティベーション一覧を取得（ページネーション対応）
    /// </summary>
    /// <param name="page">ページ番号（デフォルト: 1）</param>
    /// <param name="pageSize">1ページあたりの件数（デフォルト: 20）</param>
    /// <returns>デバイスアクティベーション一覧</returns>
    /// <response code="200">デバイスアクティベーション一覧を返却</response>
    [HttpGet("devices")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<IActionResult> GetDevices([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var devices = await _context.DeviceActivations
            .Include(da => da.LicenseKey)
            .ThenInclude(lk => lk.User)
            .OrderByDescending(da => da.ActivatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(da => new
            {
                da.Id,
                da.DeviceId,
                da.DeviceName,
                da.ActivatedAt,
                da.LastSeenAt,
                LicenseKey = da.LicenseKey.Key,
                EmailAddress = da.LicenseKey.User.EmailAddress
            })
            .ToListAsync();

        return Ok(devices);
    }

    /// <summary>
    /// デバイスアクティベーションを無効化（削除）
    /// </summary>
    /// <param name="id">デバイスアクティベーションID</param>
    /// <returns>無効化結果</returns>
    /// <response code="200">デバイス無効化成功</response>
    /// <response code="404">指定されたデバイスアクティベーションが見つからない</response>
    [HttpDelete("devices/{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeactivateDevice(Guid id)
    {
        var device = await _context.DeviceActivations.FindAsync(id);
        if (device == null)
        {
            return NotFound();
        }

        _context.DeviceActivations.Remove(device);
        await _context.SaveChangesAsync();

        await _auditLogService.LogActionAsync(
            null,
            "DeviceDeactivated",
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            $"{{\"deviceId\":\"{device.DeviceId}\",\"activationId\":\"{id}\"}}");

        return Ok(new { message = "Device deactivated successfully" });
    }

    /// <summary>
    /// ライセンスを取り消し（無効化）
    /// </summary>
    /// <param name="id">ライセンスID</param>
    /// <returns>取り消し結果</returns>
    /// <response code="200">ライセンス取り消し成功</response>
    /// <response code="404">指定されたライセンスが見つからない</response>
    [HttpPut("licenses/{id}/revoke")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RevokeLicense(Guid id)
    {
        var license = await _context.LicenseKeys.FindAsync(id);
        if (license == null)
        {
            return NotFound();
        }

        license.IsActive = false;
        await _context.SaveChangesAsync();

        await _auditLogService.LogActionAsync(
            license.UserId,
            "LicenseRevoked",
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            $"{{\"licenseId\":\"{id}\",\"licenseKey\":\"{license.Key}\"}}");

        return Ok(new { message = "License revoked successfully" });
    }
}
