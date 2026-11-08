using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Rent.Domain.Entities;
using Rent.Infrastructure.Persistence;

namespace RentPro.Infrastructure.Persistence.Database;

public static class DatabaseSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static IConfiguration _configuration = default!;
    public static async Task SeedAsync(RentProDbContext context, IConfiguration configuration)
    {
        _configuration = configuration;
        // Check whether any relevant table contains data
        bool hasData =
            await context.Users.AnyAsync() ||
            await context.UserIdentifications.AnyAsync() ||
            await context.UserIdentificationDocuments.AnyAsync();

        if (hasData)
            return;

        // Read JSON files
        var users = await ReadJsonAsync<User>("users.json");

        var identifications =
            await ReadJsonAsync<UserIdentification>(
                "user-identifications.json");

        var documents =
            await ReadJsonAsync<UserIdentificationDocument>(
                "user-identification-documents.json");

        var properties = await ReadJsonAsync<Property>("properties.json");

        var amenities = await ReadJsonAsync<Amenity>("amenities.json");

        // Insert users
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var now = DateTime.UtcNow;

        foreach (var user in users)
        {
            user.CreatedAt = now;
            user.UpdatedAt = now;
        }

        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Users ON");
        context.Users.AddRange(users);
        await context.SaveChangesAsync();
        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Users OFF");


        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.UserIdentifications ON");
        context.UserIdentifications.AddRange(identifications);
        await context.SaveChangesAsync();
        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.UserIdentifications OFF");


        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.UserIdentificationDocuments ON");
        context.UserIdentificationDocuments.AddRange(documents);
        await context.SaveChangesAsync();
        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.UserIdentificationDocuments OFF");


        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Properties ON");
        context.Properties.AddRange(properties);
        await context.SaveChangesAsync();
        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Properties OFF");


        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Amenities ON");
        context.Amenities.AddRange(amenities);
        await context.SaveChangesAsync();
        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Amenities OFF");


        await transaction.CommitAsync();
    }

    private static async Task<List<T>> ReadJsonAsync<T>(string fileName)
    {
        var dataPath = _configuration["SeedSettings:DataPath"];
        var filePath = Path.Combine(dataPath!, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException(
                $"Seed file not found: {filePath}");

        var json = await File.ReadAllTextAsync(filePath);

        return JsonSerializer.Deserialize<List<T>>(
            json, JsonOptions) ?? new List<T>();
    }
}