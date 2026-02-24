namespace CRUDExplorer.AuthServer.Models;

/// <summary>
/// 監査ログエンティティ
/// </summary>
public class AuditLog
{
    /// <summary>
    /// ログID（主キー）
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ユーザーID（外部キー、nullable）
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// アクション種別
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// タイムスタンプ
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// IPアドレス
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 詳細情報（JSON形式）
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// 関連ユーザー
    /// </summary>
    public User? User { get; set; }
}
