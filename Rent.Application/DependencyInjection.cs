using Microsoft.Extensions.DependencyInjection;
using Rent.Application.Interfaces.IService;
using Rent.Application.Services;

namespace Rent.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOwnerService, OwnerService>();

            return services;
        }
    }
}
