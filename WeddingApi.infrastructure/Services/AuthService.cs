using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeddingApi.core.DTOs.Auth;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;
    private readonly WeddingDbContext _db;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config,
        WeddingDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _db = db;
    }

    // ===== LOGIN =====
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
                   ?? await _userManager.FindByNameAsync(dto.Email);

        if (user == null)
            throw new UnauthorizedAccessException("البريد الإلكتروني أو كلمة المرور غلط");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("الحساب موقوف");

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
            ExpiresAt = DateTime.UtcNow.AddDays(dto.RememberMe ? 30 : 1),
            Username = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? string.Empty,
            UserType = user.UserType,
            UserId = user.Id,
            ClientId = clientId,
            ServiceProviderId = serviceProviderId
        };
    }

    // ===== REGISTER CLIENT =====
    public async Task<AuthResponseDto> RegisterClientAsync(RegisterClientDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            throw new Exception("البريد الإلكتروني مستخدم بالفعل");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.Phone,
            UserType = "client",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join("، ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Client");

        var client = new Client
        {
            UserId = user.Id,
            Gender = dto.Gender,
            NationalId = dto.NationalId,
            GroomName = dto.Gender == "groom" ? dto.FullName : null,
            BrideName = dto.Gender == "bride" ? dto.FullName : null,
            GroomPhone = dto.Gender == "groom" ? dto.Phone : null,
            BridePhone = dto.Gender == "bride" ? dto.Phone : null,
            CreatedAt = DateTime.UtcNow
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrEmpty(dto.NationalId))
            await LinkPartnerAsync(client, dto.NationalId);

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateToken(user, roles, false);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            Username = user.FullName,
            Email = user.Email!,
            Role = "Client",
            UserType = "client",
            UserId = user.Id,
            ClientId = client.Id
        };
    }

    // ===== REGISTER PROVIDER =====
    public async Task<string> RegisterProviderAsync(RegisterProviderDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            throw new Exception("البريد الإلكتروني مستخدم بالفعل");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.Phone,
            UserType = "provider",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join("، ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Provider");

        var provider = new ServiceProvider
        {
            UserId = user.Id,
            BusinessName = dto.BusinessName,
            Category = dto.Category,
            Status = "pending",
            Phone = dto.Phone,
            Rating = 0,
            ReviewCount = 0,
            PriceFrom = 0
        };

        _db.ServiceProviders.Add(provider);
        await _db.SaveChangesAsync();

        return "تم تسجيل طلبك بنجاح، في انتظار موافقة الإدارة";
    }

    // ===== GET PROFILE =====
    public async Task<object> GetProfileAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new Exception("المستخدم مش موجود");

        if (user.UserType == "client")
        {
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            return new
            {
                id = user.Id,
                user.FullName,
                email = user.Email,
                user.PhoneNumber,
                user.UserType,
                user.IsActive,
                clientId = client == null ? (int?)null : client.Id,
                gender = client == null ? null : client.Gender,
                nationalId = client == null ? null : client.NationalId,
                linkedPartnerId = client == null ? (int?)null : client.LinkedPartnerId,
                budget = client == null ? (decimal?)null : client.Budget,
                groomName = client == null ? null : client.GroomName,
                brideName = client == null ? null : client.BrideName,
                groomPhone = client == null ? null : client.GroomPhone,
                bridePhone = client == null ? null : client.BridePhone
            };
        }
        else if (user.UserType == "provider")
        {
            var provider = await _db.ServiceProviders.FirstOrDefaultAsync(s => s.UserId == userId);
            return new
            {
                id = user.Id,
                user.FullName,
                email = user.Email,
                user.PhoneNumber,
                user.UserType,
                user.IsActive,
                serviceProviderId = provider == null ? (int?)null : provider.Id,
                businessName = provider == null ? null : provider.BusinessName,
                category = provider == null ? null : provider.Category,
                status = provider == null ? null : provider.Status,
                location = provider == null ? null : provider.Location,
                description = provider == null ? null : provider.Description,
                phone = provider == null ? null : provider.Phone,
                rating = provider == null ? (decimal?)null : provider.Rating,
                priceFrom = provider == null ? (decimal?)null : provider.PriceFrom
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