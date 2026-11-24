namespace Rent.Contracts.DTOs.Owner
{
    public class PropertyResponse
    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        public int EmptySpace { get; set; }
        public int Capacity { get; set; }
        public int Occupied { get; set; }

        public string? Status { get; set; }

        public List<FloorResponse>? Floors { get; set; } = [];
    }
}
