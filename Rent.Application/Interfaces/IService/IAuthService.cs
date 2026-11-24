using Rent.Contracts.DTOs.Auth;

namespace Rent.Application.Interfaces.IService
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);
    }
}