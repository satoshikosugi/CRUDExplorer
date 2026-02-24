using Microsoft.AspNetCore.Mvc;
using CRUDExplorer.AuthServer.Services;
using CRUDExplorer.AuthServer.DTOs;

namespace CRUDExplorer.AuthServer.Controllers;

/// <summary>
/// ライセンス認証・検証API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LicenseController : ControllerBase
{
    private readonly AuthenticationService _authService;
    private readonly AuditLogService _auditLogService;

    public LicenseController(AuthenticationService authService, AuditLogService auditLogService)
    {
        _authService = authService;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// ライセンスキーとデバイスIDで認証し、JWTトークンを取得
    /// </summary>
    /// <param name="request">ライセンスキー、デバイスID、デバイス名を含む認証リクエスト</param>
    /// <returns>認証結果とJWTトークン</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/license/authenticate
    ///     {
    ///        "licenseKey": "XXXX-XXXX-XXXX-XXXX",
    ///        "deviceId": "unique-device-id",
    ///        "deviceName": "MyComputer"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">認証成功。JWTトークンを返却</response>
    /// <response code="401">認証失敗。無効なライセンスキーまたは期限切れ</response>
    /// <response code="409">最大デバイス数超過</response>
    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    [ProducesResponseType(typeof(AuthenticationResponse), 401)]
    [ProducesResponseType(typeof(AuthenticationResponse), 409)]
    public async Task<IActionResult> Authenticate([FromBody] LicenseAuthRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        // ライセンスキーからユーザー情報を取得して認証
        var result = await _authService.AuthenticateWithLicenseKeyAsync(
            request.LicenseKey,
            request.DeviceId,
            request.DeviceName,
            ipAddress);

        var response = new AuthenticationResponse
        {
            Success = result.IsValid,
            Token = result.Token,
            Message = result.Message
        };

        if (!result.IsValid)
        {
            // MAX_DEVICES_EXCEEDED の場合は 409 Conflict を返す
            if (result.ErrorCode == "MAX_DEVICES_EXCEEDED")
            {
                return Conflict(response);
            }
            return Unauthorized(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// JWTトークンを検証（GET版）
    /// </summary>
    /// <returns>トークンが有効な場合は200 OK</returns>
    /// <remarks>
    /// Authorizationヘッダーに"Bearer {token}"形式でJWTトークンを指定してください。
    /// </remarks>
    /// <response code="200">トークンが有効</response>
    /// <response code="401">トークンが無効または期限切れ</response>
    [HttpGet("validate")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public IActionResult ValidateToken()
    {
        // Authorizeアトリビュートでトークン検証済み
        return Ok(new { valid = true });
    }

    /// <summary>
    /// JWTトークンを検証（POST版）
    /// </summary>
    /// <param name="request">検証するトークンを含むリクエスト</param>
    /// <returns>トークンの検証結果と有効期限</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/license/validate
    ///     {
    ///        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    ///     }
    ///
    /// </remarks>
    /// <response code="200">検証結果を返却</response>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidateTokenResponse), 200)]
    public async Task<IActionResult> ValidateTokenPost([FromBody] ValidateTokenRequest request)
    {
        var result = await _authService.ValidateTokenAsync(request.Token);

        var response = new ValidateTokenResponse
        {
            IsValid = result.IsValid,
            ExpiresAt = result.ExpiresAt
        };

        return Ok(response);
    }
}
