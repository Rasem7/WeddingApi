using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly WeddingDbContext _db;
    public PaymentRepository(WeddingDbContext db) => _db = db;

    public async Task<Payment?> GetByIdAsync(int id) =>
        await _db.Payments.Include(p => p.Booking).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Payment>> GetByBookingIdAsync(int bookingId) =>
        await _db.Payments
            .Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync();

    public async Task<List<Payment>> GetAllAsync() =>
        await _db.Payments
            .Include(p => p.Booking)
            .OrderByDescending(p => p.PaidAt)
            .ToListAsync();

    public async Task<decimal> GetTotalPaidForBookingAsync(int bookingId) =>
        await _db.Payments
            .Where(p => p.BookingId == bookingId)
            .SumAsync(p => (decimal?)p.Amount) ?? 0m;

    public async Task<Payment> CreateAsync(Payment payment)
    {
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();
        return payment;
    }

    public async Task DeleteAsync(int id)
    {
        var payment = await _db.Payments.FindAsync(id);
        if (payment != null)
        {
            _db.Payments.Remove(payment);
            await _db.SaveChangesAsync();
        }
    }
}
