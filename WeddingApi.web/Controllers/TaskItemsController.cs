using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeddingApi.core.DTOs.TaskItems;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

// قائمة مهام الزفاف المشتركة بين العريس والعروسة (العملاء بس).
// قائمة الزوجين = مهام العميل + مهام شريكه المربوط (Client.LinkedPartnerId).
[ApiController]
[Route("api/Tasks")]
[Authorize(Roles = "Client")]
public class TaskItemsController : ControllerBase
{
    private readonly IUnitOfWorks _unitOfWork;
    public TaskItemsController(IUnitOfWorks unitOfWork) => _unitOfWork = unitOfWork;

    private int? MyId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    // Id العميل وشريكه المربوط (لو موجود)
    private async Task<List<int>> CoupleIdsAsync(int me)
    {
        var ids = new List<int> { me };
        var client = await _unitOfWork.Clients.GetByIdAsync(me);
        if (client != null && client.LinkedPartnerId > 0)
            ids.Add(client.LinkedPartnerId);
        return ids;
    }

    private static int PriorityRank(string priority) => priority switch
    {
        "High" => 3,
        "Medium" => 2,
        _ => 1
    };

    private static TaskItemDto ToDto(TaskItem t) => new()
    {
        Id = t.Id,
        OwnerClientId = t.OwnerClientId,
        Title = t.Title,
        Notes = t.Notes,
        Priority = t.Priority,
        Status = t.Status,
        EstimatedCost = t.EstimatedCost,
        DueDate = t.DueDate,
        AssignedTo = t.AssignedTo,
        IsOverdue = t.Status != "Done" && t.DueDate.HasValue && t.DueDate.Value.Date < DateTime.UtcNow.Date,
        CompletedAt = t.CompletedAt,
        CompletedByClientId = t.CompletedByClientId,
        CreatedAt = t.CreatedAt
    };

    // GET: api/Tasks/GetAll?status=Todo&priority=High&assignedTo=Bride&sortBy=priority
    // sortBy: priority (الأهم الأول، الافتراضي) | costAsc (الأرخص الأول) | costDesc (الأغلى الأول)
    //         | dueDate (الأقرب موعدًا) | created (الأحدث). المهام المكتملة دايمًا في الآخر.
    [HttpGet(nameof(GetAll))]
    public async Task<IActionResult> GetAll(string status = null, string priority = null,
        string assignedTo = null, string sortBy = "priority")
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var items = await _unitOfWork.TaskItems.GetByOwnersAsync(await CoupleIdsAsync(me.Value));

        IEnumerable<TaskItem> query = items;
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(t => t.Status == status);
        if (!string.IsNullOrWhiteSpace(priority)) query = query.Where(t => t.Priority == priority);
        if (!string.IsNullOrWhiteSpace(assignedTo)) query = query.Where(t => t.AssignedTo == assignedTo);

        // المكتملة في الآخر، وبعدين حسب الترتيب المطلوب. الـ null في التاريخ/السعر في الآخر.
        var ordered = query.OrderBy(t => t.Status == "Done");
        ordered = (sortBy ?? "priority") switch
        {
            "costAsc" => ordered.ThenBy(t => t.EstimatedCost.HasValue ? 0 : 1).ThenBy(t => t.EstimatedCost),
            "costDesc" => ordered.ThenBy(t => t.EstimatedCost.HasValue ? 0 : 1).ThenByDescending(t => t.EstimatedCost),
            "dueDate" => ordered.ThenBy(t => t.DueDate.HasValue ? 0 : 1).ThenBy(t => t.DueDate),
            "created" => ordered.ThenByDescending(t => t.CreatedAt),
            _ => ordered.ThenByDescending(t => PriorityRank(t.Priority))
                        .ThenBy(t => t.DueDate.HasValue ? 0 : 1).ThenBy(t => t.DueDate)
        };

        return Ok(ordered.Select(ToDto));
    }

    // GET: api/Tasks/Summary — تقدّم الزوجين في التجهيزات.
    [HttpGet(nameof(Summary))]
    public async Task<IActionResult> Summary()
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var items = await _unitOfWork.TaskItems.GetByOwnersAsync(await CoupleIdsAsync(me.Value));

        var total = items.Count;
        var done = items.Count(t => t.Status == "Done");
        var today = DateTime.UtcNow.Date;

        return Ok(new
        {
            total,
            todo = items.Count(t => t.Status == "Todo"),
            inProgress = items.Count(t => t.Status == "InProgress"),
            done,
            overdue = items.Count(t => t.Status != "Done" && t.DueDate.HasValue && t.DueDate.Value.Date < today),
            completionPercent = total == 0 ? 0 : Math.Round(done * 100.0 / total, 1),
            totalEstimatedCost = items.Sum(t => t.EstimatedCost ?? 0),
            doneEstimatedCost = items.Where(t => t.Status == "Done").Sum(t => t.EstimatedCost ?? 0),
            remainingEstimatedCost = items.Where(t => t.Status != "Done").Sum(t => t.EstimatedCost ?? 0)
        });
    }

    // GET: api/Tasks/GetById?id=5
    [HttpGet(nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var item = await _unitOfWork.TaskItems.GetByIdAsync(id);
        if (item == null) return NotFound();

        if (!(await CoupleIdsAsync(me.Value)).Contains(item.OwnerClientId)) return Forbid();

        return Ok(ToDto(item));
    }

    // POST: api/Tasks/Create
    [HttpPost(nameof(Create))]
    public async Task<IActionResult> Create(CreateTaskItemDto dto)
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var item = new TaskItem
        {
            OwnerClientId = me.Value,
            Title = dto.Title.Trim(),
            Notes = dto.Notes ?? string.Empty,
            Priority = dto.Priority,
            EstimatedCost = dto.EstimatedCost,
            DueDate = dto.DueDate?.Date,
            AssignedTo = dto.AssignedTo
        };

        var created = await _unitOfWork.TaskItems.CreateAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    // POST: api/Tasks/Update?id=5 — تعديل أي حقل، وتغيير الحالة (Status) بنفس الإندبوينت.
    [HttpPost(nameof(Update))]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskItemDto dto)
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var item = await _unitOfWork.TaskItems.GetByIdAsync(id);
        if (item == null) return NotFound();

        if (!(await CoupleIdsAsync(me.Value)).Contains(item.OwnerClientId)) return Forbid();

        if (!string.IsNullOrWhiteSpace(dto.Title)) item.Title = dto.Title.Trim();
        if (dto.Notes != null) item.Notes = dto.Notes;
        if (!string.IsNullOrWhiteSpace(dto.Priority)) item.Priority = dto.Priority;
        if (!string.IsNullOrWhiteSpace(dto.AssignedTo)) item.AssignedTo = dto.AssignedTo;

        if (dto.ClearEstimatedCost == true) item.EstimatedCost = null;
        else if (dto.EstimatedCost.HasValue) item.EstimatedCost = dto.EstimatedCost;

        if (dto.ClearDueDate == true) item.DueDate = null;
        else if (dto.DueDate.HasValue) item.DueDate = dto.DueDate.Value.Date;

        if (!string.IsNullOrWhiteSpace(dto.Status) && dto.Status != item.Status)
        {
            item.Status = dto.Status;
            if (dto.Status == "Done")
            {
                item.CompletedAt = DateTime.UtcNow;
                item.CompletedByClientId = me.Value;
            }
            else
            {
                item.CompletedAt = null;
                item.CompletedByClientId = null;
            }
        }

        var updated = await _unitOfWork.TaskItems.UpdateAsync(item);
        return Ok(ToDto(updated));
    }

    // POST: api/Tasks/Delete?id=5
    [HttpPost(nameof(Delete))]
    public async Task<IActionResult> Delete(int id)
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var item = await _unitOfWork.TaskItems.GetByIdAsync(id);
        if (item == null) return NotFound();

        if (!(await CoupleIdsAsync(me.Value)).Contains(item.OwnerClientId)) return Forbid();

        await _unitOfWork.TaskItems.DeleteAsync(id);
        return Ok(new { message = "تم حذف المهمة", deletedTaskId = id });
    }
}
