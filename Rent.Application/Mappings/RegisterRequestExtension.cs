using Rent.Domain.Entities;

namespace Rent.Application.Mappings
{
    public static class RegisterRequestExtension
    {
        public static User ToUser(this RegisterRequest request)
        {
            return new User
            {
                UserIdentification = new UserIdentification 
                { FullName = request.FullName, PhoneNumber = request.PhoneNumber },

                Email = request.Email,
                PasswordHash = request.Password,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
