using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class UserConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // Configuration goes here
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
            .Property(x => x.Id)
            .UseIdentityColumn(seed: 100001, increment: 1);
        }
    }
}
