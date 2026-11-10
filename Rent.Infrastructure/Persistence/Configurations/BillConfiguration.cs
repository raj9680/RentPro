using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class BillConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bill>()
                .Property(x => x.Id)
                .UseIdentityColumn(seed: 300001, increment: 1);

            // OI and BM cannot be repeated in multiple rows
            modelBuilder.Entity<Bill>()
                .HasIndex(b => new { b.OccupancyId, b.BillingMonth })
                .IsUnique();
        }
    }
}
