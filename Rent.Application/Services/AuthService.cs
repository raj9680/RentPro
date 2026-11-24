using Rent.Application.Interfaces.IRepository;
using Rent.Application.Interfaces.IService;
using Rent.Application.Mappings;
using Rent.Contracts.DTOs;
using Rent.Contracts.DTOs.Auth;
using Rent.Domain.Enums;

namespace Rent.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        public AuthService(IAuthRepository authRepository, ITokenService tokenService)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var data = loginRequest.ToUser();

            var result = await _authRepository.LoginAsync(data);
            TokenUser user = TokenUserExtension.ToUser(result);
            user.Id = result.Id;

            if (user.Id == 0)
                return new LoginResponse();

            var token = _tokenService.GenerateJsonWebToken(user);

            return new LoginResponse()
            {
                AccessToken = token.Token,
                ExpiresAt = token.ExpiresAt,
                User = new UserData
                {
                    FullName = result.UserIdentification.FullName,
                    Role = result.Role.ToString(),
                    Status = result.Status.ToString()
                }
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            var user = registerRequest.ToUser();

            user.Role = Role.Renter;
            user.Status = Status.Active;
            user.CreatedAt = DateTime.UtcNow;

            bool existingUser = await _authRepository.IfExistinUser(user.Email);

            if (existingUser)
            {
                return new RegisterResponse
                {
                    IsExisting = existingUser
                };
            }

            var IsRegistered = await _authRepository.RegisterAsync(user);

            return new RegisterResponse 
            { 
                UserId = IsRegistered.Id, 
                AccountStatus = IsRegistered.Status.ToString(),
                IsExisting = existingUser
            };
        }
    }
}
