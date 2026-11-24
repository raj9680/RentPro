namespace Rent.Contracts.DTOs.Auth
{
    public class RegisterResponse
    {
        public int UserId { get; set; }
        public string? AccountStatus { get; set; }
        public bool IsExisting { get; set; }
    }
}
