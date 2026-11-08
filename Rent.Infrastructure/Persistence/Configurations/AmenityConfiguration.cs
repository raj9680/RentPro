using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class AmenityConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Amenity>()
                .Property(x => x.Id)
                .UseIdentityColumn(seed: 500001, increment: 1);
        }
    }
}
