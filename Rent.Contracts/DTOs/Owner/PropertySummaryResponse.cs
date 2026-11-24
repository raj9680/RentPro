namespace Rent.Contracts.DTOs.Owner
{
    public class PropertySummaryResponse
    {
        public List<PropertyResponse>? Items { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
