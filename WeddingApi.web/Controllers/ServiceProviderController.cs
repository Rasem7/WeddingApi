using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;
using WeddingApi.infrastructure.UnitOfWorks;
using ServiceProvider = WeddingApi.core.Entities.ServiceProvider;
namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceProviderController : ControllerBase
{
    //private readonly WeddingDbContext _db;
	private readonly IUnitOfWorks _unitOfWork;// UnitOfWork injected
	public ServiceProviderController(/*WeddingDbContext db,*/ IUnitOfWorks unitOfWork)
    {
        //_db = db;
        _unitOfWork = unitOfWork;
	}

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
		/*var query = _db.ServiceProviders.Where(v => v.IsActive);
        if (!string.IsNullOrEmpty(category) && category != "الكل")
            query = query.Where(v => v.Category == category);
        return Ok(await query.OrderByDescending(v => v.Rating).ToListAsync());*/
		var providers = await _unitOfWork.ServiceProviders.GetActiveByCategoryAsync(category);
		return Ok(providers);
	}

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
		/*var serviceProvider = await _db.ServiceProviders.FindAsync(id);
        if (serviceProvider == null) return NotFound();*/
		var serviceProvider = await _unitOfWork.ServiceProviders.GetByIdAsync(id);
		if (serviceProvider == null) return NotFound();
		return Ok(serviceProvider);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(ServiceProvider sp)
    {
		/*_db.ServiceProviders.Add(serviceProvider);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = serviceProvider.Id }, serviceProvider);*/
		await _unitOfWork.ServiceProviders.AddAsync(sp);
		await _unitOfWork.CompleteAsync();
		return CreatedAtAction(nameof(GetById), new { id = sp.Id }, sp);
	}

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, ServiceProvider sp)
    {
		/* var existing = await _db.ServiceProviders.FindAsync(id);
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
		 return Ok(existing);*/
		var existing = await _unitOfWork.ServiceProviders.GetByIdAsync(id);
		if (existing == null) return NotFound();

		existing.Name = sp.Name;
		existing.Category = sp.Category;
		existing.Location = sp.Location;
		existing.Rating = sp.Rating;
		existing.ReviewCount = sp.ReviewCount;
		existing.PriceFrom = sp.PriceFrom;
		existing.Description = sp.Description;
		existing.Phone = sp.Phone;
		existing.IsActive = sp.IsActive;

		_unitOfWork.ServiceProviders.Update(existing);
		await _unitOfWork.CompleteAsync();
		return Ok(existing);

	}

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
		/* var ServiceProvider = await _db.ServiceProviders.FindAsync(id);
		 if (ServiceProvider == null) return NotFound();
		 ServiceProvider.IsActive = false; // Soft delete
		 await _db.SaveChangesAsync();
		 return NoContent();*/
		var sp = await _unitOfWork.ServiceProviders.GetByIdAsync(id);
		if (sp == null) return NotFound();

		sp.IsActive = false; // Soft delete
		_unitOfWork.ServiceProviders.Update(sp);
		await _unitOfWork.CompleteAsync();
		return NoContent();
	}
}
