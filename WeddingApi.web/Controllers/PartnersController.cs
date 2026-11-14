using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeddingApi.core.Common;
using WeddingApi.core.DTOs.Partners;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

// ربط العريس بالعروسة: واحد يطلع كود دعوة والتاني يدخله. بعد الربط بيشوفوا حجوزات ومهام بعض.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Client")]
public class PartnersController : ControllerBase
{
    private static readonly TimeSpan InviteLifetime = TimeSpan.FromHours(48);

    private readonly IUnitOfWorks _unitOfWork;
    public PartnersController(IUnitOfWorks unitOfWork) => _unitOfWork = unitOfWork;

    private int? MyId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    // GET: api/Partners/Status — مربوط بمين، أو إيه كود الدعوة الشغّال.
    [HttpGet(nameof(Status))]
    public async Task<IActionResult> Status()
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var client = await _unitOfWork.Clients.GetByIdAsync(me.Value);
        if (client == null) return NotFound();

        if (client.LinkedPartnerId > 0)
        {
            var partner = await _unitOfWork.Clients.GetByIdAsync(client.LinkedPartnerId);
            return Ok(new
            {
                linked = true,
                partner = partner == null ? null : new
                {
                    clientId = partner.Id,
                    gender = partner.Gender,
                    name = string.Equals(partner.Gender, "groom", StringComparison.OrdinalIgnoreCase)
                        ? partner.GroomName : partner.BrideName
                }
            });
        }

        var invite = await _unitOfWork.Partners.GetActiveInviteAsync(me.Value);
        return Ok(new
        {
            linked = false,
            activeInvite = invite == null ? null : new { code = invite.Code, expiresAt = invite.ExpiresAt }
        });
    }

    // POST: api/Partners/Invite — يطلع كود دعوة جديد (بينتهي بعد 48 ساعة ويلغي أي كود قديم).
    [HttpPost(nameof(Invite))]
    public async Task<IActionResult> Invite()
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var client = await _unitOfWork.Clients.GetByIdAsync(me.Value);
        if (client == null) return NotFound();

        if (client.LinkedPartnerId > 0)
            return Conflict(new { message = "حسابك مربوط بالفعل. افصل الربط الأول لو عايز تربط حساب تاني." });

        var invite = await _unitOfWork.Partners.CreateInviteAsync(me.Value, InviteLifetime);
        return Ok(new
        {
            code = invite.Code,
            expiresAt = invite.ExpiresAt,
            message = "ابعت الكود ده لشريكك يدخله في حسابه. بينتهي بعد 48 ساعة ويتستخدم مرة واحدة."
        });
    }

    // POST: api/Partners/Accept — يدخل كود الدعوة اللي جاله من شريكه.
    [HttpPost(nameof(Accept))]
    public async Task<IActionResult> Accept(AcceptInviteDto dto)
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var result = await _unitOfWork.Partners.AcceptAsync(dto.Code, me.Value);

        return result switch
        {
            PartnerLinkResult.Success => Ok(new { message = "تم الربط بنجاح" }),
            PartnerLinkResult.SelfInvite => BadRequest(new { message = "ده الكود بتاعك، لازم شريكك هو اللي يدخله." }),
            PartnerLinkResult.AlreadyLinked => Conflict(new { message = "أحد الحسابين مربوط بالفعل." }),
            PartnerLinkResult.SameGender => BadRequest(new { message = "الربط بيكون بين عريس وعروسة." }),
            PartnerLinkResult.ClientNotFound => NotFound(new { message = "الحساب غير موجود." }),
            _ => BadRequest(new { message = "الكود غير صحيح أو منتهي." })
        };
    }

    // POST: api/Partners/Unlink — فصل الربط من الطرفين (كل واحد بيحتفظ بمهامه وحجوزاته).
    [HttpPost(nameof(Unlink))]
    public async Task<IActionResult> Unlink()
    {
        var me = MyId;
        if (me == null) return Unauthorized();

        var ok = await _unitOfWork.Partners.UnlinkAsync(me.Value);
        return ok
            ? Ok(new { message = "تم فصل الربط" })
            : NotFound(new { message = "حسابك مش مربوط بحد." });
    }
}
