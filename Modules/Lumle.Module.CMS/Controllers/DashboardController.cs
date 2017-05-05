using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lumle.Module.CMS.Controllers
{
   
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ILogger _logger;
        public DashboardController(
                ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DashboardController>();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
