using System.Threading.Tasks;
using Lumle.Module.Schedular.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Lumle.Web.Infrastructures.Middlewares
{
    public class SchedularMiddleware : Controller
    {
        private readonly RequestDelegate _next;

        public SchedularMiddleware(RequestDelegate next, ISendSystemHealthReportViaEmailService sendSystemHealthReportViaEmailService)
        {
            _next = next;
            sendSystemHealthReportViaEmailService.SendHealthReportViaMail();
        }
        public async Task Invoke(HttpContext httpContext)
        {         
            await _next.Invoke(httpContext);
        }
    }
}
