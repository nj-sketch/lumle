using System;
using System.Linq;
using System.Threading.Tasks;
using Lumle.Data.Models;
using Lumle.Module.AdminConfig.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Lumle.Module.AdminConfig.ViewModels;
using Lumle.Infrastructure.Utilities.Abstracts;

namespace Lumle.Module.AdminConfig.Controllers
{
    [Route("adminconfig/[controller]")]
    public class SystemHealthController : Controller
    {
        private readonly ICredentialCategoryService _credentialCategoryService;
        private readonly UserManager<User> _userManager;
        private readonly ISystemHealthService _systemHealthService;
        private readonly ITimeZoneHelper _timeZoneHelper;

        public SystemHealthController
        (
            ICredentialCategoryService credentialCategoryService,
            UserManager<User> userManager,
            ISystemHealthService systemHealthService,
            ITimeZoneHelper timeZoneHelper
        )
        {
            _credentialCategoryService = credentialCategoryService;
            _userManager = userManager;
            _systemHealthService = systemHealthService;
            _timeZoneHelper = timeZoneHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var loggedUser = await GetCurrentUserAsync(); // Get logged in user details
            var credentialCategory = _credentialCategoryService.GetAllCredentialCategory();
            var systemHealth = _systemHealthService.GetAll().OrderByDescending(x=> x.CreatedDate).FirstOrDefault();
            var resultDate = systemHealth?.CreatedDate ?? DateTime.Now;

            var systemHealthVm = new SystemHealthVM
            {
                Categories = credentialCategory,
                LastCheckedDate = _timeZoneHelper.ConvertToLocalTime(resultDate, loggedUser.TimeZone).ToString("g")

            };            
            return View(systemHealthVm);
        }

        [HttpGet]
        [Route("status")]
        public async Task<JsonResult> GetStatusReport()
        {
            try
            {
                var loggedUser = await GetCurrentUserAsync(); // Get current logged user info
                var data =  await _systemHealthService.GetSystemHealthReport(loggedUser.Email);
                return Json(data);
            }

            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #region Helpers
        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion
    }
}
