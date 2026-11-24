using Rent.Contracts.DTOs.Auth;
using Rent.Domain.Entities;

namespace Rent.Application.Mappings
{
    public static class LoginRequestExtension
    {
        public static User ToUser(this LoginRequest loginRequest)
        {
            return new User
            {
                Email = loginRequest.Email,
                PasswordHash = loginRequest.Password,
            };
        }
    }
}
