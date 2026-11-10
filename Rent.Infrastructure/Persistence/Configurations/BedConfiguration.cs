using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class BedConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bed>()
                .Property(b => b.Id)
                .UseIdentityColumn(seed: 10001, increment: 1);
        }
    }
}
