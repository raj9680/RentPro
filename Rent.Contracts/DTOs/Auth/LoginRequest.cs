using System.ComponentModel.DataAnnotations;

namespace Rent.Contracts.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email Required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        public string? Password { get; set; }
    }
}
