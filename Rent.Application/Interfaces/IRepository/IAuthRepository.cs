using Rent.Domain.Entities;

namespace Rent.Application.Interfaces.IRepository
{
    public interface IAuthRepository
    {
        Task<User> LoginAsync(User user);
        Task<User> RegisterAsync(User user);
        Task<bool> IfExistinUser(string email);
    }
}
