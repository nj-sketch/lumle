using Lumle.Module.Audit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Lumle.Infrastructure.Utilities;
using Lumle.Module.Audit.Helpers;
using System.Threading.Tasks;
using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Module.Audit.Models;
using Microsoft.AspNetCore.Identity;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Infrastructure.Constants.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace Lumle.Module.Audit.Controllers
{
    [Route("audit/[controller]")]
    [Authorize]  
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;
        private readonly UserManager<User> _userManager;
        private static IStringLocalizer<ResourceString> _localizer;

        public AuditLogController(
            IAuditLogService auditLogService,
            UserManager<User> userManager,
            IStringLocalizer<ResourceString> localizer
            )
        {
            _auditLogService = auditLogService;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuditLogView)]
        public ViewResult Index()
        {
            var auditLogFilter = new AuditLogFilter
            {
                ModuleList = new List<SelectListItem>
                {
                    new SelectListItem {Value = "", Text = "Select Module"},
                    new SelectListItem {Value = "Article", Text = _localizer[LabelConstants.Article]},
                    new SelectListItem {Value = "User", Text = _localizer[LabelConstants.User]},
                    new SelectListItem {Value = "UserProfile", Text = _localizer[LabelConstants.UserProfile]},
                    new SelectListItem {Value = "Resource", Text = _localizer[LabelConstants.Resource]},
                    new SelectListItem {Value = "EmailTemplate", Text = _localizer[LabelConstants.EmailTemplate]},
                    new SelectListItem {Value = "Role", Text = _localizer[LabelConstants.Role]},
                    new SelectListItem {Value = "UserRole", Text = _localizer[LabelConstants.UserRole]},
                    new SelectListItem {Value = "Credential", Text = _localizer[LabelConstants.Credential]},
                    new SelectListItem {Value = "Calendar", Text = _localizer[LabelConstants.Calendar]},
                    new SelectListItem {Value = "AppSystem", Text = _localizer[LabelConstants.SystemSetting]}
                },
                ActionList = new List<SelectListItem>
                {
                    new SelectListItem {Value = "", Text = "Select Action"},
                    new SelectListItem {Value = "Create", Text = _localizer[LabelConstants.Create]},
                    new SelectListItem {Value = "Update", Text = _localizer[LabelConstants.Update]},
                    new SelectListItem {Value = "Delete", Text = _localizer[LabelConstants.Delete]}
                }
            };
            return View(auditLogFilter);
        }

        [HttpGet("Report")]
        public async Task<JsonResult> Report(string id, string tn)
        {
            var loggedUser = await GetCurrentUserAsync();
            IQueryable<AuditChange> result;
            if (tn.Equals(ModuleList.UserRole.ToString()))
            {
                result = _auditLogService.GetListRecords(x => x.KeyField == id && x.TableName == tn, loggedUser.TimeZone);
            }
            else
            {
                result = _auditLogService.GetAll(x => x.KeyField == id && x.TableName == tn, loggedUser.TimeZone);
            }

            return Json(result.OrderByDescending(x => x.CreatedDate));
        }


        [HttpPost("DataHandler")]
        public async Task<JsonResult> DataHandler([FromBody] AuditLogDTParameter parameters)
        {
            try
            {
                var loggedUser = await GetCurrentUserAsync();

                // Get Datatable result
                var result = _auditLogService.GetDataTableResult(loggedUser, parameters);

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
