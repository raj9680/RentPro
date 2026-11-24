using Rent.Contracts.DTOs;

namespace Rent.Application.Interfaces.IService
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) GenerateJsonWebToken(TokenUser tokenUser);
    }
}
