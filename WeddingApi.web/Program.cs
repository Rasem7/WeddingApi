using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;
using WeddingApi.infrastructure.Repositories;
using WeddingApi.infrastructure.Services;
using WeddingApi.infrastructure.UnitOfWorks;

// NOTE: If you still get CS0234 for Microsoft.OpenApi.Models, ensure the Swagger/OpenAPI package is referenced:
// dotnet add package Swashbuckle.AspNetCore

var builder = WebApplication.CreateBuilder(args);

// ===== Controllers =====
builder.Services.AddControllers();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins(
            "http://localhost:4200",
            "https://wedding-frontend-two.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// ===== Database =====
builder.Services.AddDbContext<WeddingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ===== Identity =====
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<WeddingDbContext>()
.AddDefaultTokenProviders();

// ===== JWT Auth =====
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

// ===== Swagger =====
builder.Services.AddEndpointsApiExplorer();
// ===== Swagger =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// ===== Dependency Injection =====
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUnitOfWorks, UnitOfWork>();

// ===== File Upload =====
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 100_000_000;
});

var app = builder.Build();

// ===== Middleware — الترتيب مهم جداً =====
app.UseCors("AllowAngular");
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/hash", () => BCrypt.Net.BCrypt.HashPassword("Admin@123"));

// ===== Migrations =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeddingDbContext>();
    db.Database.EnsureCreated();
}

// ===== Seed Data =====
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

app.Run();