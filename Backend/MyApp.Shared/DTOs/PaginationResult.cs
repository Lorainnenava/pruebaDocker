namespace MyApp.Shared.DTOs
{
    public class PaginationResult<T> where T : class
    {
        public int RowsCount { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public IEnumerable<T> Results { get; set; } = [];
    }
}