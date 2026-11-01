using Microsoft.AspNetCore.Mvc;

namespace Rent.UI.Controllers
{
    public class OwnerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
