using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class PropertyConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Property>()
                .Property(x => x.Id)
                .UseIdentityColumn(seed: 600001, increment: 1);
        }
    }
}
