namespace Rent.Contracts.DTOs.Owner
{
    public class RoomResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; } = "Room";
        public int Capacity { get; set; }
        public decimal MonthlyRent { get; set; }
        public string? Status { get; set; }

        public List<BedResponse>? Beds { get; set; } = [];
    }
}
