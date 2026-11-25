using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.API.Common;
using Rent.Application.Interfaces.IService;
using Rent.Contracts.DTOs.Auth;
using Rent.Contracts.DTOs.Owner;
using System.Security.Claims;

namespace Rent.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerService _ownerService;
        public OwnerController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }


        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> Property(string? propType, int page = 1, int pageSize = 4)
        {
            Claim? idClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null)
                return Unauthorized(new ApiResponse<LoginResponse>
                {
                     Success = false,
                     Message = "Session Expired. Login again!"
                });

            var data = await _ownerService.ListProperties(propType, page, pageSize, Convert.ToInt32(idClaim.Value));

            return Ok(new ApiResponse<PropertySummaryResponse>
            {
                Success = true,
                Data = data,
                Message = "NA",
                Errors = []
            });
        }


        [Authorize(Roles = "Owner")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetPropertyHierarchy(int id, int page = 1, int pageSize = 2)
        {
            var data = await _ownerService.ListPropertiesHierarchy(id, page, pageSize);

            if(data != null)
            {
                return Ok(new ApiResponse<PropertyHierarchyResponse>
                {
                    Success = true,
                    Data = data,
                    Message = "NA",
                    Errors = []
                });
            }

            return BadRequest(new ApiResponse<PropertyHierarchyResponse>
            {
                Success = false,
                Data = new(),
                Message = "Nothing found",
                Errors = []
            });
        }
    }
}
