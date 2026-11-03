using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class UserIdentificationConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // Configuration goes here
            modelBuilder.Entity<UserIdentification>()
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 200001, increment: 1);
        }
    }
}
