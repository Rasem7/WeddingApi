namespace WeddingApi.core.Common
{
    // سياسة السعة اليومية لكل فئة خدمة: أقصى عدد حجوزات (Pending/Confirmed) في اليوم الواحد.
    // الفئات اللي مش مذكورة هنا (كاترينج، تجميل، فساتين...) بدون حد أقصى.
    // لو عايز تغيّر السياسة، عدّل الجدول ده بس.
    public static class AvailabilityPolicy
    {
        private static readonly Dictionary<string, int> DailyCapacityByCategory =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Venue"] = 1,
                ["Photography"] = 1,
                ["Music"] = 1,
                ["Transportation"] = 1
            };

        // null = بدون حد أقصى
        public static int? DailyCapacity(string category) =>
            category != null && DailyCapacityByCategory.TryGetValue(category, out var capacity)
                ? capacity
                : null;
    }
}
