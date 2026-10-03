using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingApi.core.DTOs.Services;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IUnitOfWorks _unitOfWork;
    public ServicesController(IUnitOfWorks unitOfWork) => _unitOfWork = unitOfWork;

    private static ServiceDto ToDto(Service s) => new()
    {
        Id = s.Id,
        ServiceProviderId = s.ServiceProviderId,
        Name = s.Name,
        Description = s.Description,
        Price = s.Price,
        IsActive = s.IsActive,
        CreatedAt = s.CreatedAt
    };

    // GET: api/Services/GetByProvider?serviceProviderId=5
    // عامة (بدون تسجيل دخول) — عشان العريس/العروسة يقدروا يشوفوا الباقات المتاحة.
    [HttpGet(nameof(GetByProvider))]
    public async Task<IActionResult> GetByProvider(int serviceProviderId)
    {
        var services = await _unitOfWork.Services.GetByProviderIdAsync(serviceProviderId);
        return Ok(services.Select(ToDto));
    }

    // GET: api/Services/GetById?id=5
    [HttpGet(nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(id);
        if (service == null) return NotFound();
        return Ok(ToDto(service));
    }

    // POST: api/Services/Create
    // TODO: لازم نتحقق إن ServiceProviderId بتاع الـ dto فعلًا ملك اليوزر صاحب
    // التوكن (أو إن اليوزر Admin/Supervisor) — نفس ملاحظة MediaController.
    [HttpPost(nameof(Create))]
    [Authorize(Roles = "Provider,Admin,Supervisor")]
    public async Task<IActionResult> Create(CreateServiceDto dto)
    {
        var service = new Service
        {
            ServiceProviderId = dto.ServiceProviderId,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price
        };

        var created = await _unitOfWork.Services.CreateAsync(service);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    // POST: api/Services/Update?id=5
    [HttpPost(nameof(Update))]
    [Authorize(Roles = "Provider,Admin,Supervisor")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateServiceDto dto)
    {
        var existing = await _unitOfWork.Services.GetByIdAsync(id);
        if (existing == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Name)) existing.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.Description)) existing.Description = dto.Description;
        if (dto.Price.HasValue) existing.Price = dto.Price.Value;
        if (dto.IsActive.HasValue) existing.IsActive = dto.IsActive.Value;

        var updated = await _unitOfWork.Services.UpdateAsync(existing);
        return Ok(ToDto(updated));
    }

    // POST: api/Services/Delete?id=5
    [HttpPost(nameof(Delete))]
    [Authorize(Roles = "Provider,Admin,Supervisor")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _unitOfWork.Services.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _unitOfWork.Services.DeleteAsync(id);
        return Ok(new { message = "تم حذف الخدمة", deletedServiceId = id });
    }
}
