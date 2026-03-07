using Microsoft.AspNetCore.Mvc;
using WeddingApi.core.DTOs.Auth;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _auth.LoginAsync(dto);
        if (result == null)
            return Unauthorized(new { message = "اسم المستخدم أو كلمة المرور غلط" });
        return Ok(result);
    }
}