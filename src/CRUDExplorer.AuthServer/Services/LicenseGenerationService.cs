using CRUDExplorer.AuthServer.Data;
using CRUDExplorer.AuthServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CRUDExplorer.AuthServer.Services;

/// <summary>
/// ライセンスキー生成・管理サービス
/// </summary>
public class LicenseGenerationService
{
    private readonly AuthDbContext _context;

    public LicenseGenerationService(AuthDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 16桁のライセンスキーを生成
    /// </summary>
    public string GenerateLicenseKey()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // 紛らわしい文字を除外(I,O,0,1)
        var random = new byte[12];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
        }

        var result = new char[16];
        for (int i = 0; i < 16; i++)
        {
            result[i] = chars[random[i % 12] % chars.Length];
        }

        // 4文字ごとにハイフンを入れる形式: XXXX-XXXX-XXXX-XXXX
        return $"{new string(result, 0, 4)}-{new string(result, 4, 4)}-{new string(result, 8, 4)}-{new string(result, 12, 4)}";
    }

    /// <summary>
    /// ライセンスキーフォーマット検証（XXXX-XXXX-XXXX-XXXX形式）
    /// </summary>
    public bool ValidateKeyFormat(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        var parts = key.Split('-');
        if (parts.Length != 4)
            return false;

        return parts.All(part => part.Length == 4 && part.All(c => char.IsLetterOrDigit(c)));
    }

    /// <summary>
    /// 新しいライセンスキーを作成
    /// </summary>
    public async Task<LicenseKey> CreateLicenseKeyAsync(Guid userId, int maxDevices = 1, string productType = "Standard", DateTime? expiresAt = null)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        string key;
        do
        {
            key = GenerateLicenseKey();
        } while (await _context.LicenseKeys.AnyAsync(lk => lk.Key == key));

        var licenseKey = new LicenseKey
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Key = key,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            IsActive = true,
            MaxDevices = maxDevices,
            ProductType = productType
        };

        _context.LicenseKeys.Add(licenseKey);
        await _context.SaveChangesAsync();

        return licenseKey;
    }

    /// <summary>
    /// ライセンスキーを検証
    /// </summary>
    public async Task<LicenseKey?> ValidateLicenseKeyAsync(string key)
    {
        if (!ValidateKeyFormat(key))
            return null;

        var licenseKey = await _context.LicenseKeys
            .Include(lk => lk.User)
            .Include(lk => lk.DeviceActivations)
            .FirstOrDefaultAsync(lk => lk.Key == key);

        if (licenseKey == null || !licenseKey.IsActive || !licenseKey.User.IsActive)
            return null;

        if (licenseKey.ExpiresAt.HasValue && licenseKey.ExpiresAt.Value < DateTime.UtcNow)
            return null;

        return licenseKey;
    }
}
