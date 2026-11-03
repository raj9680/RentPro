using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class UserIdentificationDocumentConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            // Configuration goes here
            modelBuilder.Entity<UserIdentificationDocument>()
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 300001, increment: 1);
        }
    }
}
