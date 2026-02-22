using CRUDExplorer.AuthServer.Data;
using CRUDExplorer.AuthServer.Models;

namespace CRUDExplorer.AuthServer.Services;

/// <summary>
/// 監査ログサービス
/// </summary>
public class AuditLogService
{
    private readonly AuthDbContext _context;

    public AuditLogService(AuthDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// アクションをログに記録
    /// </summary>
    public async Task LogActionAsync(Guid? userId, string action, string? ipAddress = null, string? details = null)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            Timestamp = DateTime.UtcNow,
            IpAddress = ipAddress,
            Details = details
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
