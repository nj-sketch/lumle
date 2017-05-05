using Microsoft.AspNetCore.Mvc;

namespace Lumle.Module.Core.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
