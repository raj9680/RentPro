using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rent.Contracts.DTOs.Owner;
using Rent.UI.Services;
using Rent.UI.Services.Api;

namespace Rent.UI.Controllers
{
    public class OwnerController : Controller
    {
        private readonly OwnerService _ownerService;
        public OwnerController(OwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet]
        [Authorize(Roles = "Owner")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Properties([FromQuery] int page = 1)
        {
            PropertySummaryResponse res = await _ownerService.ListPropertiesAsync(page);
            if(res != null)
                return View(res);

            return View();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Property(int id)
        {
            ApiResult<PropertyHierarchyResponse> result = await _ownerService.GetPropertyHierarchyAsync(id);

            if(!result.Success || result.Data == null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Properties");
            }

            return View(result.Data);
        }
    }
}
