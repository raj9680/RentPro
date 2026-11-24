namespace Rent.Contracts.DTOs.Owner
{
    public class BedResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; } = "Bed";
        public int Capacity { get; set; }
        public decimal MonthlyRent { get; set; }
        public string? Status { get; set; }
    }
}
