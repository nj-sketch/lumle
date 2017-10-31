using AutoMapper;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Infrastructure.Constants.SystemSetting;
using Lumle.Module.AdminConfig.ViewModels;
using Lumle.Module.Audit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Lumle.Core.Attributes;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Models;
using Lumle.Infrastructure.Constants.Localization;
using Microsoft.Extensions.Localization;
using Lumle.Core.Localizer;
using Lumle.Infrastructure.Constants.ActionConstants;

namespace Lumle.Module.AdminConfig.Controllers
{
    [Route("adminconfig/[controller]")]
    [Authorize]
    public class SystemSettingController : Controller
    {
        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly ISystemSettingService _systemSettingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IAuditLogService _auditLogService;
        private readonly RoleManager<Role> _roleManager;
        private readonly IStringLocalizer<ResourceString> _localizer;

        public SystemSettingController(
            IBaseRoleClaimService baseRoleClaimService,
            ISystemSettingService systemSettingService,
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            IAuditLogService auditLogService,
            RoleManager<Role> roleManager,
            IStringLocalizer<ResourceString> localizer
        )
        {
            _baseRoleClaimService = baseRoleClaimService;
            _systemSettingService = systemSettingService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _auditLogService = auditLogService;
            _roleManager = roleManager;
            _localizer = localizer;
        }


        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigSystemSettingView)]
        public async Task<IActionResult> Index()
        {
            #region action-permission
            // Make map to check for the action previleges
            var map = new Dictionary<string, Claim>
            {
                { OperationActionConstant.UpdateAction, new Claim("permission", Permissions.AdminConfigSystemSettingUpdate) }
            };

            // Get action previlege according to actions provided
            var actionClaimResult = await _baseRoleClaimService.GetActionPrevilegeAsync(map, User);
            #endregion

            var systemSettingEntity = await _systemSettingService.GetSingle(x => x.Slug == SystemSetting.MaintenanceMode);
            var systemSettingModel = Mapper.Map<SystemSettingVM>(systemSettingEntity);

            systemSettingModel.Roles = await GetAllSupportedRolesAsync();
            systemSettingModel.UpdateAction = actionClaimResult[OperationActionConstant.UpdateAction];

            return View(systemSettingModel);
        }

        [HttpPost("UpdateSystemMaintenanceMode")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigSystemSettingUpdate)]
        public async Task<JsonResult> UpdateSystemMaintenanceMode([FromBody] SystemSettingUpdateVM model)
        {
            if (string.IsNullOrEmpty(model.Status) && model.Id <= 0)
                return Json(new { success = false, messageTitle = _localizer[ActionMessageConstants.UnableToUpdateErrorMessage].Value, message = _localizer[ActionMessageConstants.UnableToUpdateErrorMessage].Value });

            var data = await _systemSettingService.GetSingle(x => x.Id == model.Id);
            if (data == null) return Json(new { success = false, messageTitle = _localizer[ActionMessageConstants.ErrorOccured].Value, message = _localizer[ActionMessageConstants.ErrorOccured].Value });

            var oldAppSytemRecord = new AppSystem
            {
                Id = data.Id,
                Slug = data.Slug,
                Status = data.Status,
                LastUpdated = data.LastUpdated,
                LastUpdatedBy = data.LastUpdatedBy
            };
            var oldRoles = await GetAllSupportedRolesAsync();
            var oldBlockedRoles = oldRoles.Where(x => x.IsBlocked); // Get the roles which have been blocked previously
            var newblockedRoleList = new List<string>();
            if (int.Parse(model.Status.Trim()) == 0) // OFF
            {
                foreach (var item in oldRoles)
                {
                    var role = await _roleManager.FindByIdAsync(item.RoleId);
                    role.IsBlocked = false;
                    await _roleManager.UpdateAsync(role);
                }
            }
            else //ON
            {
                if (model.Roles.Length > 0)
                {
                    var blockedRoleIds = model.Roles.ToList();

                    foreach (var item in oldRoles)
                    {
                        var role = await _roleManager.FindByIdAsync(item.RoleId);
                        role.IsBlocked = !(blockedRoleIds.Any(blockedRoleId => blockedRoleId.Equals(item.RoleId)));
                        // add roles to blocked roles list for audit purpose
                        if (blockedRoleIds.Contains(item.RoleId))
                        {
                            newblockedRoleList.Add(item.RoleName);
                        }                   
                        await _roleManager.UpdateAsync(role);
                    }
                }
                else
                {
                    foreach (var item in oldRoles)
                    {
                        var role = await _roleManager.FindByIdAsync(item.RoleId);
                        role.IsBlocked = true;
                        // add roles to blocked roles list for audit purpose                     
                        newblockedRoleList.Add(item.RoleName);
                        await _roleManager.UpdateAsync(role);
                    }
                }
            }
            
            var currentUser = await GetCurrentUserAsync(); // Get current logged in user
            data.Status = model.Status;
            data.LastUpdated = DateTime.UtcNow;
            data.LastUpdatedBy = currentUser.Email;

            await _systemSettingService.Update(data);

            #region Maintainance Mode Audit Log
                var oldBlockedRoleList = oldBlockedRoles.Select(item => item.RoleName).ToList();
                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Update,
                    KeyField = data.Id.ToString(),
                    OldObject = oldAppSytemRecord,
                    NewObject = data,
                    LoggedUserEmail = currentUser.Email,
                    AdditionalInfo = "RolesWithAccess",
                    OldStringList = oldBlockedRoleList,
                    NewStringList = newblockedRoleList,
                    ComparisonType = ComparisonType.ObjectListCompare,
                };

                await _auditLogService.Add(auditLogModel);

            #endregion

            await _unitOfWork.SaveAsync();

            return Json(new { success = true, messageTitle = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value, message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value });

        }

        [HttpPost("CheckUserCredential")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigSystemSettingUpdate)]
        public async Task<JsonResult> CheckUserCredential([FromBody] UserCredentialVM userCredentail)
        {
            var user = await GetCurrentUserAsync();

            if (user == null) return Json(new { status = false });
            var validUser = await _userManager.CheckPasswordAsync(user, userCredentail.Credential);

            return Json(validUser ? new { status = true } : new { status = false });
        }


        #region Helpers
        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task<List<RoleHelper>> GetAllSupportedRolesAsync()
        {

            var currentUserRole = await _roleManager.FindByNameAsync(User.Claims.
                                FirstOrDefault(x => x.Type == ClaimTypes.Role).Value);

            var supportedRoles = _roleManager.Roles
                                .Where(x => x.Priority >= currentUserRole.Priority && x.Id != currentUserRole.Id)
                                .Select(r => new RoleHelper
                                {
                                    RoleName = r.Name,
                                    RoleId = r.Id,
                                    IsBlocked = r.IsBlocked
                                }).ToList();

            return supportedRoles;
        }
        
        #endregion
    }
}
