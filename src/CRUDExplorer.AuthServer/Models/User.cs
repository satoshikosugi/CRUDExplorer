namespace CRUDExplorer.AuthServer.Models;

/// <summary>
/// ユーザーエンティティ
/// </summary>
public class User
{
    /// <summary>
    /// ユーザーID（主キー）
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// メールアドレス（一意）
    /// </summary>
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// 作成日時
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新日時
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// アクティブ状態
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// このユーザーに関連するライセンスキー
    /// </summary>
    public ICollection<LicenseKey> LicenseKeys { get; set; } = new List<LicenseKey>();

    /// <summary>
    /// このユーザーに関連する監査ログ
    /// </summary>
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
