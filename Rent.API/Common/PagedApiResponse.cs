namespace Rent.API.Common
{
    public class PagedApiResponse<T>
    {
        public bool Success { get; set; }
        public List<T>? Data { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
