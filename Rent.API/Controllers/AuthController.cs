using Microsoft.AspNetCore.Mvc;
using Rent.API.Common;
using Rent.Application.Interfaces.IService;
using Rent.Contracts.DTOs.Auth;

namespace Rent.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost]
        public async Task<ActionResult> Register(RegisterRequest register)
        {
            RegisterResponse data = await _authService.RegisterAsync(register);

            if(data.UserId != 0)
            {
                return Ok(new ApiResponse<RegisterResponse>
                {
                    Success = true,
                    Message = "NA",
                    Data = data,
                    Errors = []
                });
            }

            return BadRequest(new ApiResponse<RegisterResponse>
            {
                Success = false,
                Message = "User not created",
                Data = data,
                Errors = []
            }); 
        }


        [HttpPost]
        public async Task<ActionResult> Login(LoginRequest login)
        {
            LoginResponse data = await _authService.LoginAsync(login);

            if (data != null && data.AccessToken != null && data.AccessToken != "")
            {
                return Ok(new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Message = "Logged in successfully",
                    Data = data,
                    Errors = []
                });
            }

            return NotFound(new ApiResponse<LoginRequest>
            {
                Success = false,
                Message = "Incorrect credentials",
                Data = null,
                Errors = []
            });
        }
    }
}