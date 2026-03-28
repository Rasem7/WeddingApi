using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Common;
using WeddingApi.core.DTOs.Bookings;
using WeddingApi.core.DTOs.Clients;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class ClientsController : ControllerBase
{
    private readonly WeddingDbContext _Context;
    private readonly IUnitOfWorks _unitOfWork;

    public ClientsController(WeddingDbContext Context , IUnitOfWorks unitOfWorks) {
        _Context = Context;
        _unitOfWork = unitOfWorks;
    
    }


    [HttpGet(nameof(GetAll))]
    public async Task<ActionResult<IEnumerable<Client>>> GetAll(
       int pageNumber = 1,
       int pageSize = 10,
       string? searchText = null
       )
    {
        var query = _Context.Clients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(x =>
                (x.BrideName ?? "").Contains(searchText.Trim()) || (x.GroomName ?? "").Contains(searchText.Trim()) ||
                (x.BridePhone ?? "").Contains(searchText.Trim()) || (x.GroomPhone ?? "").Contains(searchText.Trim()));

        }
      

        var clients = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (!clients.Any())
            return NotFound();

        return Ok(clients);
    }
    [HttpGet(nameof(GetAllWithoutPaging))]
    public async Task<ActionResult<IEnumerable<Client>>> GetAllWithoutPaging(
 )
    {
        var query = _Context.Clients.AsQueryable();

        var clients = await query
            .OrderByDescending(x => x.Id)
            .ToListAsync(); 

        if (!clients.Any())
            return NotFound();

        return Ok(clients);
    }

    
    [HttpGet(nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id);
        if (client == null) return NotFound();

        var result = new
        {
            client.Id,
            client.BrideName,
            client.GroomName,
            client.GroomPhone,
            client.BridePhone,
            client.Email,
           
        };

        return Ok(result);
    }
  

    [HttpPost(nameof(Create))]
    public async Task<IActionResult> Create(CreateClientDto dto)
    {
        var client = new Client
        {
            GroomName = dto.GroomName,
            BrideName = dto.BrideName,
            GroomPhone = dto.GroomPhone,
            BridePhone = dto.BridePhone,
            Email = dto.Email,
            Address = dto.Address,
            Budget = dto.Budget
        };
        var created = await _unitOfWork.Clients.CreateAsync(client);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    [HttpPost(nameof(Update))]
    public async Task<IActionResult> Update([FromQuery] int id, [FromBody] CreateClientDto dto)
    {
        var existing = await _unitOfWork.Clients.GetByIdAsync(id);
        if (existing == null) return NotFound();

        existing.GroomName = dto.GroomName;
        existing.BrideName = dto.BrideName;
        existing.GroomPhone = dto.GroomPhone;
        existing.BridePhone = dto.BridePhone;
        existing.Email = dto.Email;
        existing.Address = dto.Address;
        existing.Budget = dto.Budget;

        var updated = await _unitOfWork.Clients.UpdateAsync(existing);
        return Ok(updated);
    }

    [HttpPost(nameof(Delete))]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id);

        if (client == null)
            return NotFound(new { message = "Client not found" });

        await _unitOfWork.Clients.DeleteAsync(id);

        return Ok(new
        {
            message = "Client deleted successfully",
            deletedClient = new
            {
                client.Id,
                client.BrideName,
                client.GroomName
            }
        });
    }
}