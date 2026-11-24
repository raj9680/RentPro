namespace Rent.Contracts.DTOs.Owner
{
    public class FlatResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; } = "Flat";
        public int Capacity { get; set; }
        public decimal MonthlyRent { get; set; }
        public string? Status { get; set; }

        public List<RoomResponse>? Rooms { get; set; } = [];
    }
}
