namespace Rent.Contracts.DTOs.Owner
{
    public class PropertyHierarchyResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public int Capacity { get; set; }
        public string? Status { get; set; }
        public List<FloorResponse>? Floors { get; set; } = [];
    }
}
