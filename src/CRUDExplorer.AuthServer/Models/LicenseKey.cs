namespace CRUDExplorer.AuthServer.Models;

/// <summary>
/// ライセンスキーエンティティ
/// </summary>
public class LicenseKey
{
    /// <summary>
    /// ライセンスキーID（主キー）
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ユーザーID（外部キー）
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ライセンスキー（16桁、一意）
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 発行日時
    /// </summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>
    /// 有効期限
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// アクティブ状態
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 最大デバイス数
    /// </summary>
    public int MaxDevices { get; set; }

    /// <summary>
    /// 製品タイプ
    /// </summary>
    public string ProductType { get; set; } = "Standard";

    /// <summary>
    /// 関連ユーザー
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// このライセンスキーに関連するデバイスアクティベーション
    /// </summary>
    public ICollection<DeviceActivation> DeviceActivations { get; set; } = new List<DeviceActivation>();
}
