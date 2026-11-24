using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Rent.Contracts.DTOs.Auth;

namespace Rent.UI.Helpers
{
    public static class AuthHelper
    {
        public static async Task SetupCookieAndJwt(HttpContext httpContext, LoginResponse login, LoginRequest model)
        {
            // 7. Who the user is (claims)
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Email, model.Email));
            claims.Add(new Claim(ClaimTypes.Name, login.User.FullName));
            claims.Add(new Claim(ClaimTypes.Role, login.User.Role));
            claims.Add(new Claim("Status", login.User.Status));

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            // 8. Cookie expires exactly when the token expires
            DateTime expiresAtUtc = DateTime.SpecifyKind(login.ExpiresAt, DateTimeKind.Utc);

            AuthenticationProperties props = new AuthenticationProperties();
            props.IsPersistent = true;
            props.ExpiresUtc = new DateTimeOffset(expiresAtUtc);
            props.AllowRefresh = false;

            // 9. JWT kept inside the encrypted cookie
            AuthenticationToken accessToken = new AuthenticationToken();
            accessToken.Name = "access_token";
            accessToken.Value = login.AccessToken;
            props.StoreTokens(new AuthenticationToken[] { accessToken });

            // 10. Write the cookie (httpContext is the parameter, not the controller property)
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
        }
    }
}