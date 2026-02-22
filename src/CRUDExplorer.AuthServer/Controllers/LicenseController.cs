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
    /// ライセンスキーとメールアドレスで認証
    /// </summary>
    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(AuthenticationResponse), 200)]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await _authService.AuthenticateAsync(
            request.EmailAddress,
            request.LicenseKey,
            request.DeviceId,
            ipAddress);

        var response = new AuthenticationResponse
        {
            IsValid = result.IsValid,
            Token = result.Token,
            ExpiresAt = result.ExpiresAt,
            Message = result.Message
        };

        if (!result.IsValid)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }

    /// <summary>
    /// トークンを検証
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidateTokenResponse), 200)]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
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
