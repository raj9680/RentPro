using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Rent.Domain.Entities;

namespace Rent.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task SeedAsync(RentProDbContext context, IConfiguration configuration)
    {
        var dataPath = configuration["SeedSettings:DataPath"];
        if (string.IsNullOrWhiteSpace(dataPath))
            throw new InvalidOperationException("SeedSettings:DataPath is not configured.");

        await using var transaction = await context.Database.BeginTransactionAsync();

        // check if already contains data to entity
        await SeedIfEmptyAsync<User>(context, dataPath, "users.json");
        await SeedIfEmptyAsync<UserIdentification>(context, dataPath, "user-identifications.json");
        await SeedIfEmptyAsync<UserIdentificationDocument>(context, dataPath, "user-identification-documents.json");
        await SeedIfEmptyAsync<Property>(context, dataPath, "properties.json");
        await SeedIfEmptyAsync<Floor>(context, dataPath, "floors.json");
        await SeedIfEmptyAsync<Flat>(context, dataPath, "flats.json");
        await SeedIfEmptyAsync<Room>(context, dataPath, "rooms.json");
        await SeedIfEmptyAsync<Bed>(context, dataPath, "beds.json");
        await SeedIfEmptyAsync<Amenity>(context, dataPath, "amenities.json");
        await SeedIfEmptyAsync<Occupancy>(context, dataPath, "occupancies.json");
        await SeedIfEmptyAsync<Bill>(context, dataPath, "bills.json");
        await SeedIfEmptyAsync<Payment>(context, dataPath, "payments.json");
        await SeedIfEmptyAsync<FloorAmenity>(context, dataPath, "floor-amenities.json");

        await transaction.CommitAsync();
    }

    private static async Task SeedIfEmptyAsync<T>(
        RentProDbContext context, string dataPath, string fileName) where T : class
    {
        if (await context.Set<T>().AnyAsync())
            return;

        var path = Path.Combine(dataPath, fileName);
        if (!File.Exists(path))
            return;

        var json = await File.ReadAllTextAsync(path);
        var items = JsonSerializer.Deserialize<List<T>>(json, JsonOptions);
        if (items is null || items.Count == 0)
            return;

        context.Set<T>().AddRange(items);

        // Does this table have an identity column?
        var entityType = context.Model.FindEntityType(typeof(T))!;
        var hasIdentity = entityType.GetProperties().Any(p =>
            p.GetValueGenerationStrategy() == SqlServerValueGenerationStrategy.IdentityColumn);

        if (!hasIdentity)
        {
            await context.SaveChangesAsync();
            return;
        }

        var table = $"[{entityType.GetSchema() ?? "dbo"}].[{entityType.GetTableName()}]";
        var sqlOn = "SET IDENTITY_INSERT " + table + " ON";
        var sqlOff = "SET IDENTITY_INSERT " + table + " OFF";

        await context.Database.ExecuteSqlRawAsync(sqlOn);
        try
        {
            await context.SaveChangesAsync();
        }
        finally
        {
            await context.Database.ExecuteSqlRawAsync(sqlOff);
        }
    }
}