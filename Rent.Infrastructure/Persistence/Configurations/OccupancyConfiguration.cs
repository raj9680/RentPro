using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class OccupancyConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Occupancy>()
                .Property(p => p.Id)
                .UseIdentityColumn(seed: 110001, increment: 1);
        }
    }
}
