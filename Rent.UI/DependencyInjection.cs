using Rent.UI.Services;

namespace Rent.UI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUiDI(this IServiceCollection services)
        {
            services.AddScoped<AuthService, AuthService>();
            services.AddScoped<OwnerService, OwnerService>();
            return services;
        }
    }
}
