using Lumle.Module.Audit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Lumle.Module.Audit.Entities;
using System.Collections.Generic;
using Lumle.Infrastructure.Utilities;
using Lumle.Module.Audit.Helpers;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Infrastructure.Helpers;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.ViewModels;
using Microsoft.AspNetCore.Identity;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Infrastructure.Constants.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Lumle.Infrastructure.Utilities.Abstracts;

namespace Lumle.Module.Audit.Controllers
{
    [Route("audit/[controller]")]
    [Authorize]  
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;
        private readonly UserManager<User> _userManager;
        private static IStringLocalizer<ResourceString> _localizer;
        private readonly ITimeZoneHelper _timeZoneHelper;

        public AuditLogController(
            IAuditLogService auditLogService,
            UserManager<User> userManager,
            IStringLocalizer<ResourceString> localizer,
            ITimeZoneHelper timeZoneHelper
            )
        {
            _auditLogService = auditLogService;
            _userManager = userManager;
            _localizer = localizer;
            _timeZoneHelper = timeZoneHelper;
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
            if (tn.Equals(ModuleList.UserRole.ToString()))
            {
                var recordList = _auditLogService.GetListRecords(x => x.KeyField == id && x.TableName == tn, loggedUser.TimeZone);
                return Json(recordList.OrderByDescending(x => x.CreatedDate)); 
            }
            var allrecords = _auditLogService.GetAll(x => x.KeyField == id && x.TableName == tn, loggedUser.TimeZone);
            return Json(allrecords.OrderByDescending(x => x.CreatedDate));
        }


        [HttpPost("DataHandler")]
        public async Task<JsonResult> DataHandler([FromBody] AuditLogDTParameter parameters)
        {
            try
            {
                var loggedUser = await GetCurrentUserAsync();
                var startDate = parameters.StartDate;
                var endDate = parameters.EndDate;
                var moduleName = parameters.ModuleName;
                var actionName = parameters.ActionName;
                var username = parameters.Username;
                var fieldName = parameters.FieldName;

                // Filter the records
                var auditLogs = GetFilteredRecords(moduleName, actionName, startDate, endDate, username, fieldName);
                int totalAuditRecord;
                if (!string.IsNullOrEmpty(parameters.Search.Value.Trim())
                   && !string.IsNullOrWhiteSpace(parameters.Search.Value.Trim()))
                {
                    Expression<Func<AuditLog, bool>> search =
                        x => (x.AuditType ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.TableName ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.UserId ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower());

                    totalAuditRecord = auditLogs.Count(search);
                    auditLogs = auditLogs.Where(search);
                    auditLogs = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), auditLogs);
                    auditLogs = auditLogs.Skip(parameters.Start).Take(parameters.Length);
                }
                else
                {
                    totalAuditRecord = auditLogs.Count();
                    auditLogs = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), auditLogs);
                    auditLogs = auditLogs.Skip(parameters.Start).Take(parameters.Length);
                }
                var i = parameters.Start + 1;
                var auditLogVms = Mapper.Map<List<AuditLogVM>>(auditLogs);
                auditLogVms = auditLogVms.Select(x =>
                {
                    x.Sn = i++;
                    x.ConvertedCreatedDate = _timeZoneHelper.ConvertToLocalTime(x.CreatedDate, loggedUser.TimeZone).ToString("g");
                    return x;
                }).ToList();

                var audit = new DataTableHelper.DTResult<AuditLogVM>
                {
                    Draw = parameters.Draw,
                    Data = auditLogVms,
                    RecordsFiltered = totalAuditRecord,
                    RecordsTotal = totalAuditRecord
                };
                return Json(audit);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #region Helpers
        /// <summary>
        /// Serverside dataTable Sorting
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDirection"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IQueryable<AuditLog> SortByColumnWithOrder(int order, string orderDirection, IQueryable<AuditLog> data)
        {
            try
            {
                switch (order)
                {
                    case 2:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.TableName) : data.OrderBy(p => p.TableName);
                    case 3:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.UserId) : data.OrderBy(p => p.UserId);
                    case 4:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.CreatedDate) : data.OrderBy(p => p.CreatedDate);
                    default:
                        return data.OrderByDescending(p => p.CreatedDate);
                }
            }
            catch (Exception)
            {
                return data;
            }
        }

        private IQueryable<AuditLog> GetFilteredRecords(string modelSearch, string actionSearch, string startDate, string endDate, string userSearch, string fieldSearch)
        {

            var auditEntites = _auditLogService.GetAll()
                .Select(auditLog => new AuditLog
                {
                    Id = auditLog.Id,
                    AuditType = auditLog.AuditType,
                    AuditSummary = auditLog.AuditSummary,
                    CreatedDate = auditLog.CreatedDate,
                    TableName = auditLog.TableName,
                    ModuleInfo = auditLog.ModuleInfo,
                    Changes = auditLog.Changes,
                    UserId = auditLog.UserId,
                    KeyField = auditLog.KeyField
                }).AsQueryable();
            if (!string.IsNullOrEmpty(modelSearch))
            {
                auditEntites = auditEntites.Where(model => model.TableName.Contains(modelSearch.ToLower()));
            }

            if (!string.IsNullOrEmpty(actionSearch))
            {
                auditEntites = auditEntites.Where(action => action.AuditType.Contains(actionSearch.ToLower()));
            }

            if (!string.IsNullOrEmpty(userSearch))
            {
                auditEntites = auditEntites.Where(user => user.UserId.Contains(userSearch.ToLower()));
            }

            if (!string.IsNullOrEmpty(startDate))
            {
                auditEntites = auditEntites.Where(sD => sD.CreatedDate >= DateTime.Parse(startDate));
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                auditEntites = auditEntites.Where(eD => eD.CreatedDate <= DateTime.Parse(endDate).AddDays(1));
            }

            if (!string.IsNullOrEmpty(fieldSearch))
            {
                auditEntites = auditEntites.Where(fS => fS.Changes.Contains("\"FieldName\":") && fS.Changes.ToLower().Contains(fieldSearch.ToLower()));
            }

            return auditEntites;
        }

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion
    }
}
