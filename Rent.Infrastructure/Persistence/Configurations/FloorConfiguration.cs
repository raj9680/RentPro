using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class FloorConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Floor>()
                .Property(x => x.Id)
                .UseIdentityColumn(seed: 800001, increment: 1);
        }
    }
}
