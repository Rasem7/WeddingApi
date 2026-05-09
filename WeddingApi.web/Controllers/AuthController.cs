using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeddingApi.core.DTOs.Auth;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    // ===== LOGIN =====
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try { return Ok(await _auth.LoginAsync(dto)); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ===== REGISTER CLIENT =====
    [HttpPost("register/client")]
    public async Task<IActionResult> RegisterClient(RegisterClientDto dto)
    {
        try { return Ok(await _auth.RegisterClientAsync(dto)); }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ===== REGISTER PROVIDER =====
    [HttpPost("register/provider")]
    public async Task<IActionResult> RegisterProvider(RegisterProviderDto dto)
    {
        try { return Ok(new { message = await _auth.RegisterProviderAsync(dto) }); }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ===== GET CURRENT USER =====
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _auth.GetProfileAsync(userId));
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ===== UPDATE PROFILE =====
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _auth.UpdateProfileAsync(userId, dto));
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ===== CHANGE PASSWORD =====
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _auth.ChangePasswordAsync(userId, dto);
            return Ok(new { message = "تم تغيير كلمة المرور بنجاح" });
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ===== PENDING PROVIDERS (Admin only) =====
    [HttpGet("providers/pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingProviders()
    {
        try { return Ok(await _auth.GetPendingProvidersAsync()); }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    // ===== APPROVE PROVIDER (Admin only) =====
    [HttpPatch("providers/{userId}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveProvider(int userId)
    {
        var result = await _auth.ApproveProviderAsync(userId);
        return result
            ? Ok(new { message = "تم قبول مزود الخدمة" })
            : NotFound(new { message = "مزود الخدمة مش موجود" });
    }

    // ===== REJECT PROVIDER (Admin only) =====
    [HttpPatch("providers/{userId}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectProvider(int userId)
    {
        var result = await _auth.RejectProviderAsync(userId);
        return result
            ? Ok(new { message = "تم رفض مزود الخدمة" })
            : NotFound(new { message = "مزود الخدمة مش موجود" });
    }
}