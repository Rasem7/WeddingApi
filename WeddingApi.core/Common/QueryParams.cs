namespace WeddingApi.core.Common
{
    public class QueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? BudgetCategory { get; set; }
        public string? Status { get; set; }
    }
}
