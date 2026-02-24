namespace CRUDExplorer.AuthServer.Models;

/// <summary>
/// デバイスアクティベーションエンティティ
/// </summary>
public class DeviceActivation
{
    /// <summary>
    /// アクティベーションID（主キー）
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ライセンスキーID（外部キー）
    /// </summary>
    public Guid LicenseKeyId { get; set; }

    /// <summary>
    /// デバイスID（マシン固有の識別子）
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// デバイス名（オプション）
    /// </summary>
    public string? DeviceName { get; set; }

    /// <summary>
    /// アクティベーション日時
    /// </summary>
    public DateTime ActivatedAt { get; set; }

    /// <summary>
    /// 最終確認日時
    /// </summary>
    public DateTime LastSeenAt { get; set; }

    /// <summary>
    /// 関連ライセンスキー
    /// </summary>
    public LicenseKey LicenseKey { get; set; } = null!;
}
