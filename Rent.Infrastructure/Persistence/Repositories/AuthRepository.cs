using Microsoft.EntityFrameworkCore;
using Rent.Application.Interfaces.IRepository;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly RentProDbContext _dbContext;
        public AuthRepository(RentProDbContext dbContent)
        {
            _dbContext = dbContent;
        }

        public async Task<bool> IfExistinUser(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                return false;
            return true;
        }

        public async Task<User> LoginAsync(User user)
        {
            var s = await _dbContext.Users.Include(u => u.UserIdentification).FirstOrDefaultAsync(e => e.Email == user.Email);

            if (s == null || !BCrypt.Net.BCrypt.Verify(user.PasswordHash, s.PasswordHash))
                return new User(); // Not found or wrong password

            return new User
            {
                UserIdentification = s.UserIdentification is null ? null
                    : new UserIdentification { FullName = s.UserIdentification.FullName },
                Id = s.Id,
                Email = s.Email,
                Role = s.Role,
                Status = s.Status,
                PasswordHash = string.Empty
            };
        }

        public async Task<User> RegisterAsync(User user)
        {
            if(user.PasswordHash != null)
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
    }
}
