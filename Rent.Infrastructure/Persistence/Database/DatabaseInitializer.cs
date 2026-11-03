using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RentPro.Infrastructure.Persistence.Database;

namespace Rent.Infrastructure.Persistence.Database
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(RentProDbContext contex, DatabaseSettings settings, IConfiguration configuration)
        {
            // Update database schema
            if (settings.UpdateDatabase)
            {
                await contex.Database.MigrateAsync();
            }

            // Insert sample data
            if (settings.SeedSampleData)
            {
                await DatabaseSeeder.SeedAsync(contex, configuration);
            }
        }
    }
}
