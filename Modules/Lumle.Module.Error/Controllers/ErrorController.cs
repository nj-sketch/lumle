using Microsoft.AspNetCore.Mvc;

namespace Lumle.Module.Error.Controllers
{
    public class ErrorController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("~/404")]
        public IActionResult Error404()
        {
            return View("404");
        }

        [Route("~/403")]
        public IActionResult Error403()
        {
            return View("403");
        }

        [Route("~/500")]
        public IActionResult Error500()
        {
            return View("500");
        }

        [Route("~/maintenancemode")]
        public IActionResult SystemMaintenanceMode()
        {
            return View("SystemMaintenanceMode");
        }
    }
}
