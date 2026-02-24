using Microsoft.EntityFrameworkCore;
using CRUDExplorer.AuthServer.Models;

namespace CRUDExplorer.AuthServer.Data;

/// <summary>
/// 認証サーバーデータベースコンテキスト
/// </summary>
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// ユーザー
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// ライセンスキー
    /// </summary>
    public DbSet<LicenseKey> LicenseKeys { get; set; } = null!;

    /// <summary>
    /// デバイスアクティベーション
    /// </summary>
    public DbSet<DeviceActivation> DeviceActivations { get; set; } = null!;

    /// <summary>
    /// 監査ログ
    /// </summary>
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Userエンティティの設定
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EmailAddress).IsUnique();
            entity.Property(e => e.EmailAddress).IsRequired().HasMaxLength(256);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        });

        // LicenseKeyエンティティの設定
        modelBuilder.Entity<LicenseKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();
            entity.Property(e => e.Key).IsRequired().HasMaxLength(16);
            entity.Property(e => e.IssuedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.MaxDevices).IsRequired().HasDefaultValue(1);
            entity.Property(e => e.ProductType).IsRequired().HasMaxLength(50).HasDefaultValue("Standard");

            // User との1対多リレーション
            entity.HasOne(e => e.User)
                .WithMany(u => u.LicenseKeys)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DeviceActivationエンティティの設定
        modelBuilder.Entity<DeviceActivation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.LicenseKeyId, e.DeviceId }).IsUnique();
            entity.Property(e => e.DeviceId).IsRequired().HasMaxLength(256);
            entity.Property(e => e.DeviceName).HasMaxLength(256);
            entity.Property(e => e.ActivatedAt).IsRequired();
            entity.Property(e => e.LastSeenAt).IsRequired();

            // LicenseKey との1対多リレーション
            entity.HasOne(e => e.LicenseKey)
                .WithMany(lk => lk.DeviceActivations)
                .HasForeignKey(e => e.LicenseKeyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLogエンティティの設定
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.IpAddress).HasMaxLength(45); // IPv6対応
            entity.Property(e => e.Details).HasColumnType("jsonb"); // PostgreSQL JSONB型

            // User との1対多リレーション（nullable）
            entity.HasOne(e => e.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
