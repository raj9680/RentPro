using Rent.Contracts.DTOs;
using Rent.Domain.Entities;

namespace Rent.Application.Mappings
{
    public static class TokenUserExtension
    {
        public static TokenUser ToUser(User user)
        {
            return new TokenUser
            {
                Id = user.Id,
                Email = user.Email.ToString(),
                Role = user.Role.ToString(),
                Status = user.Status.ToString(),
            };
        }
    }
}
