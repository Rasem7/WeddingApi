using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingApi.infrastructure.Data;
using ServiceProvider = WeddingApi.core.Entities.ServiceProvider;
namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceProviderController : ControllerBase
{
    //private readonly WeddingDbContext _db;
	private readonly IUnitOfWork _unit;
	public ServiceProviderController(WeddingDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        var query = _db.ServiceProviders.Where(v => v.IsActive);
        if (!string.IsNullOrEmpty(category) && category != "الكل")
            query = query.Where(v => v.Category == category);
        return Ok(await query.OrderByDescending(v => v.Rating).ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var serviceProvider = await _db.ServiceProviders.FindAsync(id);
        if (serviceProvider == null) return NotFound();
        return Ok(serviceProvider);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(ServiceProvider serviceProvider)
    {
        _db.ServiceProviders.Add(serviceProvider);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = serviceProvider.Id }, serviceProvider);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, ServiceProvider ServiceProvider)
    {
        var existing = await _db.ServiceProviders.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = ServiceProvider.Name;
        existing.Category = ServiceProvider.Category;
        existing.Location = ServiceProvider.Location;
        existing.Rating = ServiceProvider.Rating;
        existing.ReviewCount = ServiceProvider.ReviewCount;
        existing.PriceFrom = ServiceProvider.PriceFrom;
        existing.Description = ServiceProvider.Description;
        existing.Phone = ServiceProvider.Phone;
        existing.IsActive = ServiceProvider.IsActive;

        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var ServiceProvider = await _db.ServiceProviders.FindAsync(id);
        if (ServiceProvider == null) return NotFound();
        ServiceProvider.IsActive = false; // Soft delete
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
