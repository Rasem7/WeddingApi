namespace WeddingApi.core.DTOs.Availability
{
    public class AvailabilityResult
    {
        public bool Available { get; set; }
        public string Reason { get; set; } = string.Empty;

        public static AvailabilityResult Yes() => new() { Available = true };
        public static AvailabilityResult No(string reason) => new() { Available = false, Reason = reason };
    }
}
