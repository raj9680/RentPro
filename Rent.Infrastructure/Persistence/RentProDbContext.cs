using Microsoft.EntityFrameworkCore;
using Rent.Domain.Entities;
using Rent.Infrastructure.Persistence.Configurations;

namespace Rent.Infrastructure.Persistence
{
    public class RentProDbContext: DbContext
    {
        public RentProDbContext(DbContextOptions<RentProDbContext> options)
            :base(options)
        {
            
        }


        // Register Entities as Table
        public DbSet<User> Users { get; set; }
        public DbSet<UserIdentification> UserIdentifications { get; set; }
        public DbSet<UserIdentificationDocument> UserIdentificationDocuments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // UserConfiguration
            UserConfiguration.Configure(modelBuilder);
        }
    }
}
