using Microsoft.AspNetCore.Mvc;
using CRUDExplorer.AuthServer.Services;
using CRUDExplorer.AuthServer.DTOs;

namespace CRUDExplorer.AuthServer.Controllers;

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
    /// ライセンスキーとデバイスIDで認証
    /// </summary>
    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
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
    /// トークンを検証（GET版）
    /// </summary>
    [HttpGet("validate")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [ProducesResponseType(200)]
    public IActionResult ValidateToken()
    {
        // Authorizeアトリビュートでトークン検証済み
        return Ok(new { valid = true });
    }

    /// <summary>
    /// トークンを検証（POST版）
    /// </summary>
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
