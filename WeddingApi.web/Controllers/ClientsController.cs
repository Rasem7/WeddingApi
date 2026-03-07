using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingApi.core.Common;
using WeddingApi.core.DTOs.Clients;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _repo;
    public ClientsController(IClientRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams q)
    {
        var result = await _repo.GetAllAsync(q);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var client = await _repo.GetByIdAsync(id);
        if (client == null) return NotFound();
        return Ok(client);
    }

    [HttpPost]
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
        var created = await _repo.CreateAsync(client);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateClientDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        existing.GroomName = dto.GroomName;
        existing.BrideName = dto.BrideName;
        existing.GroomPhone = dto.GroomPhone;
        existing.BridePhone = dto.BridePhone;
        existing.Email = dto.Email;
        existing.Address = dto.Address;
        existing.Budget = dto.Budget;

        var updated = await _repo.UpdateAsync(existing);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }
}