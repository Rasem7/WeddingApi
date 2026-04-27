using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WeddingApi.core.DTOs.Auth;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly WeddingDbContext _db;
    private readonly IConfiguration _config;
    private readonly WeddingDbContext _db;

    public AuthService(WeddingDbContext db, IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var expiry = dto.RememberMe
            ? DateTime.UtcNow.AddDays(30)
            : DateTime.UtcNow.AddHours(8);

        var token = GenerateToken(user.Username, user.Role, expiry);

        if (user.UserType == "provider")
        {
            var providerProfile = await _db.ServiceProviders
                .FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (providerProfile?.Status == "pending")
                throw new UnauthorizedAccessException("طلب الاشتراك لسه قيد المراجعة");
            if (providerProfile?.Status == "rejected")
                throw new UnauthorizedAccessException("تم رفض طلب اشتراكك");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            throw new UnauthorizedAccessException("الحساب مقفول مؤقتاً بسبب محاولات كثيرة");

        if (!result.Succeeded)
            throw new UnauthorizedAccessException("البريد الإلكتروني أو كلمة المرور غلط");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateToken(user, roles, dto.RememberMe);

        int? clientId = null;
        int? serviceProviderId = null;

        if (user.UserType == "client")
        {
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
            clientId = client?.Id;
        }
        else if (user.UserType == "provider")
        {
            var provider = await _db.ServiceProviders.FirstOrDefaultAsync(s => s.UserId == user.Id);
            serviceProviderId = provider?.Id;
        }

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiry,
            Username = user.Username,
            Role = user.Role
        };
    }

        return new { id = user.Id, user.FullName, email = user.Email, user.UserType, user.IsActive };
    }

    // ===== UPDATE PROFILE =====
    public async Task<object> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new Exception("المستخدم مش موجود");

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.Phone;
        await _userManager.UpdateAsync(user);

        if (user.UserType == "client")
        {
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client != null)
            {
                if (!string.IsNullOrEmpty(dto.NationalId))
                    client.NationalId = dto.NationalId;
                await _db.SaveChangesAsync();
            }
        }
        else if (user.UserType == "provider")
        {
            var provider = await _db.ServiceProviders.FirstOrDefaultAsync(s => s.UserId == userId);
            if (provider != null)
            {
                provider.Location = dto.Location ?? provider.Location;
                provider.Description = dto.Description ?? provider.Description;
                provider.Phone = dto.Phone;
                await _db.SaveChangesAsync();
            }
        }

        return await GetProfileAsync(userId);
    }

    // ===== CHANGE PASSWORD =====
    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new Exception("المستخدم مش موجود");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            throw new Exception(string.Join("، ", result.Errors.Select(e => e.Description)));
    }

    // ===== GET PENDING PROVIDERS =====
    public async Task<List<object>> GetPendingProvidersAsync()
    {
        var providers = await _db.ServiceProviders
            .Where(s => s.Status == "pending")
            .Include(s => s.User)
            .Select(s => (object)new
            {
                id = s.Id,
                userId = s.UserId,
                fullName = s.User.FullName,
                email = s.User.Email,
                userPhone = s.User.PhoneNumber,
                businessName = s.BusinessName,
                category = s.Category,
                status = s.Status,
                providerPhone = s.Phone,
                createdAt = s.User.CreatedAt
            })
            .ToListAsync();

        return providers;
    }

    // ===== APPROVE PROVIDER =====
    public async Task<bool> ApproveProviderAsync(int userId)
    {
        var provider = await _db.ServiceProviders.FirstOrDefaultAsync(s => s.UserId == userId);
        if (provider == null) return false;
        provider.Status = "approved";
        await _db.SaveChangesAsync();
        return true;
    }

    // ===== REJECT PROVIDER =====
    public async Task<bool> RejectProviderAsync(int userId)
    {
        var provider = await _db.ServiceProviders.FirstOrDefaultAsync(s => s.UserId == userId);
        if (provider == null) return false;
        provider.Status = "rejected";
        await _db.SaveChangesAsync();
        return true;
    }

    // ===== LINK PARTNER =====
    private async Task LinkPartnerAsync(Client newClient, string nationalId)
    {
        var partner = await _db.Clients
            .FirstOrDefaultAsync(c => c.NationalId == nationalId && c.Id != newClient.Id);

        if (partner == null) return;

        newClient.LinkedPartnerId = partner.Id;
        partner.LinkedPartnerId = newClient.Id;
        await _db.SaveChangesAsync();
    }

    // ===== GENERATE TOKEN =====
    private string GenerateToken(ApplicationUser user, IList<string> roles, bool rememberMe)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new("fullName", user.FullName),
            new("userType", user.UserType),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(rememberMe ? 30 : 1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}