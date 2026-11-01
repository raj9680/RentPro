using Microsoft.AspNetCore.Mvc;

namespace Rent.UI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
