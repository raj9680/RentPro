using Microsoft.EntityFrameworkCore;
using Rent.API;
using Rent.Infrastructure.Persistence;
using Rent.Infrastructure.Persistence.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DIs
builder.Services.AddApiDI(builder.Configuration);

// DB Connections
builder.Services.AddDbContext<RentProDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Initialise Database Settings
var databaseSettings = builder.Configuration
    .GetSection("DatabaseSettings")
    .Get<DatabaseSettings>()
    ?? throw new InvalidOperationException("DatabaseSettings configuration is missing");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RentProDbContext>();
    await DatabaseInitializer.InitializeAsync(context, databaseSettings, app.Configuration);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
