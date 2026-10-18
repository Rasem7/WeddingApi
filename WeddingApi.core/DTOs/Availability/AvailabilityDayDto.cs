namespace WeddingApi.core.DTOs.Availability
{
    public class AvailabilityDayDto
    {
        public DateTime Date { get; set; }

        // "Blocked" = مقفول من المزود، "Booked" = اكتملت السعة اليومية بحجوزات
        public string Status { get; set; } = string.Empty;

        // الـ Id والسبب بيظهروا لصاحب المزود والأدمن/المشرف بس (لفتح اليوم بعدين).
        public int? Id { get; set; }
        public string Reason { get; set; }
    }
}
