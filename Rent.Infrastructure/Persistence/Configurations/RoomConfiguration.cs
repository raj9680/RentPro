using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class RoomConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .Property(x => x.Id)
                .UseIdentityColumn(seed: 900001, increment: 1);
        }
    }
}
