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
        public DbSet<Property> Properties { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Flat> Flats { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<FloorAmenity> FloorAmenities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Entities Configurations
            UserConfiguration.Configure(modelBuilder);
            UserIdentificationConfiguration.Configure(modelBuilder);
            UserIdentificationDocumentConfiguration.Configure(modelBuilder);
            FloorAmenityConfiguration.Configure(modelBuilder);
        }
    }
}
