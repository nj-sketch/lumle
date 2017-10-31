using Lumle.Infrastructure.Constants.LumleLog;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Lumle.Module.Core.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/Home/ErrorWithCode/{statusCode}")]
        public IActionResult ErrorWithCode(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("404");
            }

            return View("Error");
        }

        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;

            if (error != null)
            {
                Logger.Error(error, ErrorLog.Undefinederror);
            }

            return View("Error");
        }

        [Route("~/maintenancemode")]
        public IActionResult SystemMaintenanceMode()
        {
            return View("Maintainance");
        }
    }
}
