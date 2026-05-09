using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly WeddingDbContext _db;
    public DashboardController(WeddingDbContext db) => _db = db;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var now = DateTime.UtcNow;
        var firstOfMonth = new DateTime(now.Year, now.Month, 1);

        var totalBookings = await _db.Bookings.CountAsync();
        var bookingsThisMonth = await _db.Bookings
            .CountAsync(b => b.CreatedAt >= firstOfMonth);
        var totalRevenue = await _db.Payments.SumAsync(p => p.Amount);
        var revenueThisMonth = await _db.Payments
            .Where(p => p.PaidAt >= firstOfMonth).SumAsync(p => p.Amount);
        var totalClients = await _db.Clients.CountAsync();

        var recentBookings = await _db.Bookings
    .Include(b => b.Client)
    .OrderByDescending(b => b.CreatedAt)
    .Take(5)
    .Select(b => new {
        b.Id,
        b.ClientId,
        client = new
        {
            id = b.Client.Id,
            groomName = b.Client.GroomName,
            brideName = b.Client.BrideName,
            groomPhone = b.Client.GroomPhone,
            budget = b.Client.Budget,
            budgetCategory = b.Client.BudgetCategory,
        },
        weddingDate = b.WeddingDate,
        weddingTime = b.WeddingTime ?? "",
        venue = b.Venue ?? "",
        guestCount = b.GuestCount,
        eventType = b.EventType,
        status = b.Status,
        totalAmount = b.TotalAmount,
        createdAt = b.CreatedAt
    })
    .ToListAsync();

        return Ok(new
        {
            totalBookings,
            bookingsThisMonth,
            totalRevenue,
            revenueThisMonth,
            totalClients,
            averageRating = 4.8,
            recentBookings
        });
    }
}