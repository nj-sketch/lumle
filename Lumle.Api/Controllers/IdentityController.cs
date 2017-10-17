using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Lumle.Api.Controllers
{

    [NonController]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
