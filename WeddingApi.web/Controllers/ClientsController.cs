using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

// ملحوظة: ده كنترولر إدارة يدوية للعملاء (Admin/Supervisor)، مش تسجيل عميل جديد.
// تسجيل العميل الفعلي بيتم عن طريق /api/Auth/register/client لأنه بيربط
// الـ Client بـ ApplicationUser تلقائيًا. الكنترولر ده لسه ناقص ربط UserId
// (راجع تعليق داخل Create) ولازم يتصلح قبل الاستخدام الفعلي في الإنتاج.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Supervisor")]
public class ClientsController : ControllerBase
{
    private readonly IUnitOfWorks _unitOfWork;

    public ClientsController(IUnitOfWorks unitOfWorks)
    {
        _unitOfWork = unitOfWorks;
    }

    [HttpGet(nameof(GetAll))]
    public async Task<IActionResult> GetAll(
       int pageNumber = 1,
       int pageSize = 10,
       string searchText = null)
    {
        var result = await _unitOfWork.Clients.SearchAsync(pageNumber, pageSize, searchText);

        if (!result.Data.Any())
            return NotFound();

        return Ok(result);
    }

    [HttpGet(nameof(GetAllWithoutPaging))]
    public async Task<IActionResult> GetAllWithoutPaging()
    {
        var clients = await _unitOfWork.Clients.GetAllWithoutPagingAsync();

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
            client.User.Email,
           
        };

        return Ok(result);
    }


    [HttpPost(nameof(Create))]
    public async Task<IActionResult> Create(CreateClientDto dto)
    {
        // TODO (خارج نطاق إصلاحات الأمان العاجلة): CreateClientDto مفيهوش UserId،
        // وClient.UserId مطلوب وفريد (FK لـ AspNetUsers). حاليًا الإندبوينت ده
        // هيفشل عند الحفظ لأن UserId هيبقى 0 (مفيش يوزر بالرقم ده). محتاج نضيف
        // UserId للـ DTO ونتأكد إن اليوزر موجود ومالوش Client مرتبط بيه قبليها،
        // أو نلغي الإندبوينت ده تمامًا ونخلي التسجيل يتم فقط عن طريق
        // /api/Auth/register/client. هرجعلها في مرحلة تصحيح النموذج (Phase 1).
        return await Task.FromResult<IActionResult>(BadRequest(new
        {
            message = "إنشاء عميل مباشر عبر هذا الإندبوينت غير مفعّل حاليًا. استخدم /api/Auth/register/client لتسجيل عميل جديد مرتبط بحساب مستخدم."
        }));
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
        existing.Budget = dto.Budget;

        var updated = await _unitOfWork.Clients.UpdateAsync(existing);
        return Ok(updated);
    }

    [HttpPost(nameof(Delete))]
    [Authorize(Roles = "Admin")]
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