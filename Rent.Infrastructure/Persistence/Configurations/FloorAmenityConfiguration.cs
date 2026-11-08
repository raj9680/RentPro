using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class FloorAmenityConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FloorAmenity>()
                .Property(x => x.Id)
                .UseIdentityColumn(seed: 400001, increment: 1);

            modelBuilder.Entity<FloorAmenity>()
                .HasIndex(x => new { x.FloorId, x.AmenityId })
                .IsUnique();
        }
    }
}
