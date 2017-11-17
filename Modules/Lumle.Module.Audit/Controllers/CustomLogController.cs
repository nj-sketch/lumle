using System;
using System.Threading.Tasks;
using Lumle.Core.Attributes;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Module.Audit.Helpers;
using Lumle.Module.Audit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Lumle.Infrastructure.Utilities.Abstracts;
using Lumle.Module.Audit.Services;

namespace Lumle.Module.Audit.Controllers
{
    [Route("audit/[controller]")]
    [Authorize]
    public class CustomLogController : Controller
    {
        private readonly ICustomLogService _customLogService;
        private readonly ITimeZoneHelper _timeZoneHelper;
        private readonly UserManager<User> _userManager;

        public CustomLogController(ICustomLogService customLogService, UserManager<User> userManager, ITimeZoneHelper timeZoneHelper)
        {       
            _customLogService = customLogService;
            _userManager = userManager;
            _timeZoneHelper = timeZoneHelper;
        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CustomLogView)]
        public ViewResult Index()
        {
            return View(new CustomLogFilter());
        }

        [HttpPost("DataHandler")]
        public async Task<JsonResult> DataHandler([FromBody] CustomLogDTParameter parameters)
        {
            try
            {
                var loggedUser = await GetCurrentUserAsync();

                // Get Datatable result
                var result = _customLogService.GetDataTableResult(loggedUser, parameters);
               
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #region Helpers

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    
        #endregion
    }
}
