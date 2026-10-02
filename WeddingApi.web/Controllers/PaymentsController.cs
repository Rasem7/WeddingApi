using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeddingApi.core.DTOs.Payments;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IUnitOfWorks _unitOfWork;
    public PaymentsController(IUnitOfWorks unitOfWork) => _unitOfWork = unitOfWork;

    private static PaymentDto ToDto(Payment p) => new()
    {
        Id = p.Id,
        BookingId = p.BookingId,
        Amount = p.Amount,
        Method = p.Method,
        Notes = p.Notes,
        PaidAt = p.PaidAt
    };

    // GET: api/Payments/GetAll
    // تقرير مالي شامل — للأدمن والمشرف فقط.
    [HttpGet(nameof(GetAll))]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> GetAll()
    {
        var payments = await _unitOfWork.Payments.GetAllAsync();
        return Ok(payments.Select(ToDto));
    }

    // GET: api/Payments/GetById?id=5
    [HttpGet(nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);
        if (payment == null) return NotFound();
        return Ok(ToDto(payment));
    }

    // GET: api/Payments/GetByBooking?bookingId=5
    // بيرجع كل الدفعات الخاصة بحجز معيّن + الإجمالي المدفوع والمتبقي.
    [HttpGet(nameof(GetByBooking))]
    public async Task<IActionResult> GetByBooking(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) return NotFound(new { message = "الحجز غير موجود" });

        var payments = await _unitOfWork.Payments.GetByBookingIdAsync(bookingId);
        var totalPaid = payments.Sum(p => p.Amount);

        return Ok(new
        {
            bookingId,
            totalAmount = booking.TotalAmount,
            totalPaid,
            remaining = Math.Max(booking.TotalAmount - totalPaid, 0),
            payments = payments.Select(ToDto)
        });
    }

    // POST: api/Payments/Create
    // تسجيل دفعة جديدة على حجز، وتأكيد الحجز تلقائيًا لو اتغطى المبلغ بالكامل.
    [HttpPost(nameof(Create))]
    [Authorize(Roles = "Client,Admin,Supervisor,Provider")]
    public async Task<IActionResult> Create(CreatePaymentDto dto)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(dto.BookingId);
        if (booking == null)
            return NotFound(new { message = "الحجز غير موجود" });

        if (booking.Status is "Cancelled" or "Refunded")
            return BadRequest(new { message = "لا يمكن تسجيل دفعة على حجز ملغي أو مسترد." });

        var payment = new Payment
        {
            BookingId = dto.BookingId,
            Amount = dto.Amount,
            Method = dto.Method,
            Notes = dto.Notes,
            PaidAt = DateTime.UtcNow
        };

        var created = await _unitOfWork.Payments.CreateAsync(payment);

        // تأكيد الحجز تلقائيًا لو إجمالي المدفوعات غطّى قيمة الحجز بالكامل
        var totalPaid = await _unitOfWork.Payments.GetTotalPaidForBookingAsync(dto.BookingId);
        if (totalPaid >= booking.TotalAmount && booking.Status == "Pending")
        {
            await _unitOfWork.Bookings.UpdateStatusAsync(dto.BookingId, "Confirmed");
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new
        {
            payment = ToDto(created),
            totalPaid,
            bookingStatus = totalPaid >= booking.TotalAmount ? "Confirmed" : booking.Status
        });
    }

    // POST: api/Payments/Delete?id=5
    // للأدمن فقط — تصحيح دفعة اتسجلت غلط. لا يعيد حالة الحجز لـ Pending تلقائيًا (قرار يدوي مقصود).
    [HttpPost(nameof(Delete))]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);
        if (payment == null) return NotFound();

        await _unitOfWork.Payments.DeleteAsync(id);
        return Ok(new { message = "تم حذف الدفعة", deletedPaymentId = id });
    }
}
