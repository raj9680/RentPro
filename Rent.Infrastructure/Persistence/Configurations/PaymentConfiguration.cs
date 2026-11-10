using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Configurations
{
    public static class PaymentConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .Property(x => x.Id)
                .UseIdentityColumn(seed: 400001, increment: 1);
        }
    }
}
