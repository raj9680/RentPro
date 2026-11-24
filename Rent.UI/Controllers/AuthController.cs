using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Contracts.DTOs.Auth;
using Rent.UI.Helpers;
using Rent.UI.Services;
using Rent.UI.Services.Api;
using System.Text.Json;

namespace Rent.UI.Controllers
{
    public class AuthController : Controller
    {
         private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            // C# Email -> JSON "email"
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            // JSON "fullName" -> C# FullName
            options.PropertyNameCaseInsensitive = true;   
            
            return options;
        }


        #region Login

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null, string? message = null)
        {
            if(User.Identity != null && User.Identity.IsAuthenticated == true)
                return RedirectToAction("Dashboard","Owner");

            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.Message = message;

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest model, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApiResult<LoginResponse> body = await _authService.LoginAsync(model);

            if (body == null || body.Message == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Email or Password");
            }
            
            LoginResponse login = body.Data;

            if (login != null)
                await AuthHelper.SetupCookieAndJwt(HttpContext, login, model);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Dashboard", "Owner", new { message = "LoggedIn" });
        }

        #endregion


        #region Register

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            model.ConfirmPassword = model.Password;
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            ApiResult<RegisterResponse> body = await _authService.RegisterAsync(model);


            if(body == null || body.Message == null)
            {
                ModelState.AddModelError(string.Empty, body.Message);
                return View(model);
            }

            RegisterResponse register = body.Data;
                
            return RedirectToAction("Login", new { message = register.AccountStatus });
        }

        #endregion


        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        #endregion

    }
}