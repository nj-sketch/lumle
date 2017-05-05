using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Module.Authorization.Services;
using Lumle.Module.Authorization.ViewModels.RoleViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Lumle.Data.Models;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Services;
using Lumle.Infrastructure.Constants.Log;
using Lumle.Infrastructure.Utilities;
using Lumle.Module.Audit.Models;
using NLog;
using Microsoft.Extensions.Localization;
using Lumle.Infrastructure.Constants.Localization;

namespace Lumle.Module.Authorization.Controllers
{
    [Route("authorization/[controller]")]
    [Authorize]
    public class RoleController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionService _permissionService;
        private readonly IAuditLogService _auditLogService;
        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly IStringLocalizer<ResourceString> _localizer;

        public RoleController(
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            IAuditLogService auditLogService,
            IPermissionService permissionService,
            IBaseRoleClaimService baseRoleClaimService,
             IStringLocalizer<ResourceString> localizer
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _permissionService = permissionService;
            _auditLogService = auditLogService;
            _baseRoleClaimService = baseRoleClaimService;
            _localizer = localizer;
        }


        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationRoleView)]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.Select(r => new RoleListVM
            {
                RoleName = r.Name,
                Id = r.Id,
                Description = r.Description,
                NumberOfUsers = r.Users.Count,
                Priority = r.Priority
            }).ToList();

            return View(roles);
        }

        [HttpGet("add")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationRoleCreate)]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationRoleCreate)]
        public async Task<IActionResult> Add(RoleAddVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                return View(model);
            }
            var roleExist = await _roleManager.RoleExistsAsync(model.RoleName.Trim());
            if (roleExist)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.RoleAlreadyExistErrorMessage].Value;
                return View(model);
            }
            var role = new Role
            {
                Name = model.RoleName,
                Description = model.Description,
                Priority = model.Priority,
                IsBlocked = false
                
            };
            var roleResult = await _roleManager.CreateAsync(role);

            if (roleResult.Succeeded)
            {
                var loggedUser = await GetCurrentUserAsync(); // Get current logged user

                #region Role Create Audit Log
                var oldRole = new Role(); // Storage of this null object shows data before creation = nothing!
                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Create,
                    KeyField = role.Id,
                    OldObject = oldRole,
                    NewObject = role,
                    LoggedUserEmail = loggedUser.Email,
                    ComparisonType = ComparisonType.ObjectCompare
                };

                _auditLogService.Add(auditLogModel);
                _unitOfWork.Save();
                #endregion

                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.AddedSuccessfully].Value;
                return RedirectToAction("Index");
            }

            TempData["ErrorMsg"] =$"{ _localizer[ActionMessageConstants.ErrorOccured].Value}.{roleResult.Errors.FirstOrDefault().Description} ";
            return View(model);
        }

        [HttpGet("edit/{id}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationRoleUpdate)]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMsg"] =_localizer[ActionMessageConstants.SelectValidItemErrorMessage].Value;
                return RedirectToAction("Index");
            }

            var applicationRole = await _roleManager.FindByIdAsync(id);
            if (applicationRole == null)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                return RedirectToAction("Index");
            }

            var model = new RoleEditVM
            {
                Id = applicationRole.Id,
                RoleName = applicationRole.Name,
                Description = applicationRole.Description,
                Priority = applicationRole.Priority
            };

            return View(model);
        }


        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationRoleUpdate)]
        public async Task<IActionResult> Edit(RoleEditVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                    return View(model);
                }

                var applicationRole = await _roleManager.FindByIdAsync(model.Id);

                // Add previous data in old record object for comparison
                var oldRecord = new Role
                {
                    Id = applicationRole.Id,
                    Name = applicationRole.Name,
                    Description = applicationRole.Description,
                    NormalizedName = applicationRole.NormalizedName,
                    Priority = applicationRole.Priority,
                    IsBlocked = false
                };

                // Update in the database
                applicationRole.Name = model.RoleName;
                applicationRole.Description = model.Description;
                applicationRole.Priority = model.Priority;

                var roleResult = await _roleManager.UpdateAsync(applicationRole);
                if (roleResult.Succeeded)
                {
                    var loggedUser = await GetCurrentUserAsync(); // Get current logged in user

                    #region Role Update Audit Log

                    var auditLogModel = new AuditLogModel
                    {
                        AuditActionType = AuditActionType.Update,
                        KeyField = oldRecord.Id,
                        OldObject = oldRecord,
                        NewObject = applicationRole,
                        LoggedUserEmail = loggedUser.Email,
                        ComparisonType = ComparisonType.ObjectCompare
                    };
                    _auditLogService.Add(auditLogModel);
                    _unitOfWork.Save();
                    #endregion

                    TempData["SuccessMsg"] = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value;
                    return RedirectToAction("Index");
                }

                TempData["ErrorMsg"] = $"{ _localizer[ActionMessageConstants.ErrorOccured].Value}.{roleResult.Errors.FirstOrDefault().Description} ";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value;
                Logger.Error(ex, ErrorLog.UpdateError);
                return RedirectToAction("Index");
            }
        }


        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationRoleDelete)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            try
            {
                var loggedUser = await GetCurrentUserAsync(); // Get current logged in user details
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                    return RedirectToAction("Index");
                }

                var applicationRole = await _roleManager.FindByIdAsync(id);
                if (applicationRole == null)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                    return RedirectToAction("Index");
                }

                var roleResult = await _roleManager.DeleteAsync(applicationRole);
                if (roleResult.Succeeded)
                {
                   
                    #region Delete Audit Log
                        var auditLogModel = new AuditLogModel
                        {
                            AuditActionType = AuditActionType.Delete,
                            KeyField = applicationRole.Id,
                            OldObject = applicationRole,
                            NewObject = new Role(),
                            LoggedUserEmail = loggedUser.Email,
                            ComparisonType = ComparisonType.ObjectCompare
                        };
                        _auditLogService.Add(auditLogModel);

                    #endregion
                    _unitOfWork.Save();

                    TempData["SuccessMsg"] = _localizer[ActionMessageConstants.DeletedSuccessfully].Value;
                    return RedirectToAction("Index");
                }
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ErrorOccured];
                return View("Index");

            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value;
                return RedirectToAction("Index");
            }
        }

        [HttpGet("permission/{id}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationRoleUpdate)]
        public async Task<IActionResult> AddEditRolePermission(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                return RedirectToAction("Index");
            }

            ViewData["role"] = id;
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                return RedirectToAction("Index");
            }

            var roleClaims = _baseRoleClaimService.GetAll(x => x.RoleId == role.Id && x.ClaimType == "permission").ToList();
            var modules = _permissionService.GetPermissionsIncludingAssigned(roleClaims);

            return View("RolePermissionConfig", modules);
        }

        [HttpPost("permission")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationRoleUpdate)]
        public async Task<IActionResult> SaveRolePermission([FromBody] RoleClaimVM model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.RoleId) || model.ClaimValues.Length <= 0)
                    return Json(new { status = false });

                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role == null)
                    return Json(new { status = false });

                var loggedUser = await GetCurrentUserAsync(); //Get current logged user

                var roleClaims = _baseRoleClaimService.GetAll(x => x.RoleId == model.RoleId);
                var oldClaimsList = new List<string>();
                var baseRoleClaims = roleClaims as BaseRoleClaim[] ?? roleClaims.ToArray();
                if (baseRoleClaims.Any())
                {
                    foreach (var item in baseRoleClaims)
                    {
                        var permission = _permissionService.GetSingle(x => x.Slug == item.ClaimValue);
                        oldClaimsList.Add(permission.DisplayName);
                    }

                    _baseRoleClaimService.DeleteWhere(x => x.RoleId == model.RoleId);
                    _unitOfWork.Save();
                }
                        
                var claimsDisplayNameList = new List<string>();
                foreach (var claim in model.ClaimValues)
                {
                    var permission = _permissionService.GetSingle(x => x.Slug == claim);
                    claimsDisplayNameList.Add(permission.DisplayName);
                    var newClaim = new BaseRoleClaim
                    {
                        ClaimType = CustomClaimtypes.Permission,
                        ClaimValue = claim.ToLower(),
                        RoleId = role.Id,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };

                    _baseRoleClaimService.Add(newClaim);
                    _unitOfWork.Save();
                }
      
                #region Role Claim Audit Log            

                if (oldClaimsList.Any())
                {
                    var updateAuditLogModel = new AuditLogModel
                    {
                        AuditActionType = AuditActionType.Update,
                        KeyField = role.Id,
                        OldStringList = oldClaimsList,
                        NewStringList = claimsDisplayNameList,
                        AdditionalInfo = role.Name,
                        LoggedUserEmail = loggedUser.Email,
                        ComparisonType = ComparisonType.ListCompare,
                        ModuleList = ModuleList.UserRole
                    };

                    _auditLogService.Add(updateAuditLogModel);

                }
                else
                {
                    var dummyClaimList = new List<string>();
                    var createAuditLogModel = new AuditLogModel
                    {
                        AuditActionType = AuditActionType.Create,
                        KeyField = role.Id,
                        OldStringList = dummyClaimList,
                        NewStringList = claimsDisplayNameList,
                        AdditionalInfo = role.Name,
                        LoggedUserEmail = loggedUser.Email,
                        ComparisonType = ComparisonType.ListCompare,
                        ModuleList = ModuleList.UserRole
                    };

                    _auditLogService.Add(createAuditLogModel);
                }

                _unitOfWork.Save();

                #endregion

                await UpdateSecurityStampOfRoleUser(role);
               
                return Json(new { success = true, message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value });

            }
            catch (Exception)
            {
                return Json(new { success = false, message = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value });
            }
        }


        #region Helpers
        private async Task UpdateSecurityStampOfRoleUser(Role role)
        {
            var roleUsers = _permissionService.GetAllUserOfRole(role.Id);
            foreach (var roleUser in roleUsers)
            {
                await _userManager.UpdateSecurityStampAsync(roleUser);
            }
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion


    }
}