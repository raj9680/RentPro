namespace Rent.Contracts.DTOs.Owner
{
    public class FloorResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; } = "Floor";
        public int Capacity { get; set; }
        public decimal MonthlyRent { get; set; }
        public string? Status { get; set; }

        public List<FlatResponse>? Flats { get; set; }
    }
}
