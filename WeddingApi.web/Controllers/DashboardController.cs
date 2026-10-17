using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Supervisor")]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWorks _unitOfWork;
    public DashboardController(IUnitOfWorks unitOfWork) => _unitOfWork = unitOfWork;

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var now = DateTime.UtcNow;
        var firstOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalBookings = await _unitOfWork.Bookings.CountAllAsync();
        var bookingsThisMonth = await _unitOfWork.Bookings.CountSinceAsync(firstOfMonth);
        var totalRevenue = await _unitOfWork.Payments.GetTotalAsync();
        var revenueThisMonth = await _unitOfWork.Payments.GetTotalSinceAsync(firstOfMonth);
        var totalClients = await _unitOfWork.Clients.CountAllAsync();

        var recentBookings = (await _unitOfWork.Bookings.GetRecentWithClientAsync(5))
            .Select(b => new
            {
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
            });

        return Ok(new
        {
            totalBookings,
            bookingsThisMonth,
            totalRevenue,
            revenueThisMonth,
            totalClients,
            // TODO: averageRating لازم يتحسب فعليًا لما يتضاف نظام Review حقيقي (راجع المرحلة 2 في خطة العمل)
            averageRating = 4.8,
            recentBookings
        });
    }
}
