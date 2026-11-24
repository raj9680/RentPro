using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rent.Application.Interfaces.IRepository;
using Rent.Infrastructure.Persistence.Repositories;

namespace Rent.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            return services;
        }
    }
}
