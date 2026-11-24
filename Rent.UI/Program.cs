using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Rent.UI;
using Rent.UI.Services.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


#region JWT Cookies & HTTP-CLIENT

// 1. Browser <-> MVC: cookie authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        //options.LogoutPath = "/Auth/Login";
        //options.AccessDeniedPath = "/Auth/AccessDenied";

        options.Cookie.Name = "RentPro.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = false;

        // fallback; real expiry set at login
        options.ExpireTimeSpan = TimeSpan.FromHours(1); 
    });

    // Every page needs login, unless it has [AllowAnonymous]
    builder.Services.AddAuthorization(option =>
    {
        option.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
    });


builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<BearerTokenHandler>();

// 2. HttpClient Setup - Read the API address
string? apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new InvalidOperationException("ApiSettings:BaseUrl is missing in appsettings.json");
}

Uri apiBaseUri = new Uri(apiBaseUrl);
builder.Services.AddHttpClient("RentApi", client =>
{
    client.BaseAddress = apiBaseUri;
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<BearerTokenHandler>();

#endregion


builder.Services.AddUiDI();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();


app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();