using System;
using System.Collections.Generic;
using Lumle.Module.Authorization.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lumle.Data.Data.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.IdentityConstants;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Infrastructure.Utilities;
using Lumle.Module.AdminConfig.Services;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Services;
using NLog;
using Lumle.Data.Data;
using static Lumle.Infrastructure.Helpers.DataTableHelper;
using System.Linq.Expressions;
using System.Security.Claims;
using Lumle.Module.Authorization.Helpers;
using Lumle.Infrastructure.Constants.Token;
using Lumle.Module.Audit.Models;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Microsoft.AspNetCore.Hosting;
using Lumle.Infrastructure.Constants.Localization;
using Lumle.Infrastructure.Utilities.Abstracts;

namespace Lumle.Module.Authorization.Controllers
{

    [Route("authorization/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IProfileService _profileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessagingService _messagingService;
        private readonly IAuditLogService _auditLogService;
        private readonly BaseContext _context;
        private readonly IApplicationTokenService _applicationTokenService;
        private readonly IStringLocalizer<ResourceString> _localizer;
        private readonly ITimeZoneHelper _timeZoneHelper;

        public UserController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IProfileService profileService,
            IUnitOfWork unitOfWork,
            IMessagingService messagingService,
            ICredentialService credentialService,
            IAuditLogService auditLogService,
            BaseContext context,
            IApplicationTokenService applicationTokenService,
            IStringLocalizer<ResourceString> localizer,
            IHostingEnvironment env,
            ITimeZoneHelper timeZoneHelper)

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _profileService = profileService;
            _unitOfWork = unitOfWork;
            _messagingService = messagingService;
            _auditLogService = auditLogService;
            _context = context;
            _applicationTokenService = applicationTokenService;
            _localizer = localizer;
            _timeZoneHelper = timeZoneHelper;

        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserView)]
        public IActionResult Index()
        {
            return View();

        }

        [HttpGet("add")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserCreate)]
        public async Task<IActionResult> AddUser()
        {
            await InitializeRolesAndTimeZonesAsync();
            return View("Add");
        }

        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserCreate)]
        public async Task<IActionResult> AddUser(UserVM model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region Validation Logic

                    if (!ModelState.IsValid)
                    {
                        await InitializeRolesAndTimeZonesAsync();
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                        return View("Add");
                    }
                    #endregion

                    var loggedUser = await GetCurrentUserAsync();
                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email.ToLower(),
                        CreatedBy = loggedUser.Email,
                        TimeZone = model.TimeZone,
                        AccountStatus = Convert.ToInt32(AccountStatus.Enable),
                        CreatedDate = DateTime.UtcNow
                    };
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        #region User Audit Log

                        // For audit log of User Details
                        var dummyCoreUser = new User(); // Storage of this null object shows data before creation = nothing!
                        var userAuditLogModel = new AuditLogModel
                        {
                            AuditActionType = AuditActionType.Create,
                            OldObject = dummyCoreUser,
                            NewObject = user,
                            KeyField = user.Id,
                            ComparisonType = ComparisonType.ObjectCompare,
                            LoggedUserEmail = loggedUser.Email
                        };

                        _auditLogService.Add(userAuditLogModel);
                        #endregion

                        var role = await _roleManager.FindByIdAsync(model.RoleId);
                        if (role != null)
                        {

                            #region Role Audit
                            // For audit log of user role details
                            var roleAuditLogModel = new AuditLogModel
                            {
                                AuditActionType = AuditActionType.Update,
                                KeyField = user.Id,
                                OldString = "",
                                NewString = role.Name,
                                ModuleList = ModuleList.User,
                                LoggedUserEmail = loggedUser.Email,
                                ComparisonType = ComparisonType.StringCompare
                            };

                            _auditLogService.Add(roleAuditLogModel);
                            #endregion

                            var roleResult = await _userManager.AddToRoleAsync(user, role.Name);
                            if (roleResult.Succeeded)
                            {
                                var userProfile = new UserProfile
                                {
                                    UserId = user.Id,
                                    Email = user.Email,
                                    UserName = user.UserName,
                                    FirstName = model.FirstName,
                                    LastName = model.LastName,
                                    CreatedDate = DateTime.UtcNow
                                };
                                _profileService.Add(userProfile);

                                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                                var callbackUrl = Url.Action(TokenType.ConfirmEmail, "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);
                                await _messagingService.SendEmailConfirmationMailAsync(model.Email, model.UserName.Trim(), callbackUrl);

                                var appToken = new ApplicationToken
                                {
                                    TokenType = TokenType.ConfirmEmail,
                                    Token = code,
                                    UserId = user.Id,
                                    IsUsed = false,
                                    CreatedDate = DateTime.UtcNow,
                                    LastUpdated = DateTime.UtcNow
                                };

                                _applicationTokenService.Add(appToken);

                                _context.SaveChanges();
                                transaction.Commit();

                                #region User Profile Audit
                                // For audit log of User Profile Details
                                var dummyProfile = new UserProfile(); // Storage of this null object shows data before creation = nothing!

                                var userProfileAuditLogModel = new AuditLogModel
                                {
                                    AuditActionType = AuditActionType.Create,
                                    OldObject = dummyProfile,
                                    NewObject = userProfile,
                                    KeyField = userProfile.Id.ToString(),
                                    ComparisonType = ComparisonType.ObjectCompare,
                                    LoggedUserEmail = loggedUser.Email
                                };

                                _auditLogService.Add(userProfileAuditLogModel);
                                _unitOfWork.Save();
                                #endregion


                                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.AddedSuccessfully].Value;
                                return RedirectToAction("Index");
                            }
                            transaction.Rollback();
                            TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToAssignRoleToUserErrorMessage].Value;
                            return RedirectToAction("Index");
                        }
                        transaction.Rollback();
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.RoleNotFoundErrorMessage].Value;
                        return RedirectToAction("Index");
                    }
                    transaction.Rollback();
                    await InitializeRolesAndTimeZonesAsync();
                    TempData["ErrorMsg"] = $"{_localizer[ActionMessageConstants.ErrorOccured].Value}. {result.Errors.FirstOrDefault()?.Description}";
                    return View("Add", model);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Logger.Error(ex, ErrorLog.UserCreationError);
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value;
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpGet("edit/{id}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserUpdate)]
        public async Task<IActionResult> EditUser(string id)
        {
            #region User Validation
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.SelectValidItemErrorMessage].Value;
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                return RedirectToAction("Index");
            }

            var requestedUser = _profileService.GetSingle(x => x.UserId == user.Id);
            if (requestedUser == null)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                return RedirectToAction("Index");
            }

            #endregion

            var requestedUserRoles = await _userManager.GetRolesAsync(user);
            var model = new EditUserVM
            {
                UserName = user.UserName,
                Email = user.Email,
                TimeZone = user.TimeZone,
                AccountStatus = user.AccountStatus,
                FirstName = requestedUser.FirstName,
                LastName = requestedUser.LastName,
                RoleId = requestedUserRoles.Any()
                    ? _roleManager.Roles
                        .FirstOrDefault(r => string.Equals(r.Name, requestedUserRoles.FirstOrDefault(),
                            StringComparison.CurrentCultureIgnoreCase)).Id
                    : ""
            };

            await InitializeRolesAndTimeZonesAsync();

            return View("Edit", model);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserUpdate)]
        public async Task<IActionResult> EditUser(EditUserVM model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region Validation

                    if (!ModelState.IsValid)
                    {
                        await InitializeRolesAndTimeZonesAsync();
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                        return View("Edit", model);
                    }

                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (user == null)
                    {
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                        return RedirectToAction("Index");
                    }

                    var requestedUser = _profileService.GetSingle(x => x.UserId == model.Id);
                    if (requestedUser == null)
                    {
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                        return RedirectToAction("Index");
                    }

                    var requestedUserRoles = await _userManager.GetRolesAsync(user);
                    if (!requestedUserRoles.Any())
                    {
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.RoleNotFoundErrorMessage].Value;
                        return RedirectToAction("Index");
                    }

                    var role = await _roleManager.FindByIdAsync(model.RoleId);
                    if (role == null)
                    {
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.RoleNotFoundErrorMessage].Value;
                        return RedirectToAction("Index");
                    }
                    #endregion

                    var loggedUser = await GetCurrentUserAsync(); //Get current logged user
                    // Add previous user identity data in old record object for comparison
                    var oldUserRecord = new User
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        NormalizedEmail = user.NormalizedEmail,
                        NormalizedUserName = user.NormalizedUserName,
                        LockoutEnabled = user.LockoutEnabled,
                        CreatedBy = user.CreatedBy,
                        TimeZone = user.TimeZone,
                        AccountStatus = user.AccountStatus
                    };


                    //Update user in Identity
                    user.UserName = model.UserName;
                    user.Email = model.Email.ToLower();
                    user.TimeZone = model.TimeZone;
                    user.AccountStatus = model.AccountStatus;

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        await InitializeRolesAndTimeZonesAsync();
                        TempData["ErrorMsg"] = $"{_localizer[ActionMessageConstants.ErrorOccured]}. {result.Errors.FirstOrDefault().Description}";
                        return View("Edit", model);
                    }

                    //Update security timestamp and force logout for user if their account status is disabled
                    if (user.AccountStatus != model.AccountStatus &&
                        model.AccountStatus == (int)AccountStatus.Disable)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                    }

                    #region Audit Log of User Identity

                    var userAuditLogModel = new AuditLogModel
                    {
                        AuditActionType = AuditActionType.Update,
                        OldObject = oldUserRecord,
                        NewObject = user,
                        KeyField = oldUserRecord.Id,
                        ComparisonType = ComparisonType.ObjectCompare,
                        LoggedUserEmail = loggedUser.Email
                    };
                    _auditLogService.Add(userAuditLogModel);

                    #endregion

                    // Add previous user profile data in old record object for comparison
                    var oldUserProfileRecord = new UserProfile
                    {
                        Id = requestedUser.Id,
                        FirstName = requestedUser.FirstName,
                        LastName = requestedUser.LastName,
                        UserId = requestedUser.UserId,
                        Email = requestedUser.Email,
                        UserName = requestedUser.UserName
                    };

                    //Update user in UserProfile
                    requestedUser.FirstName = model.FirstName;
                    requestedUser.LastName = model.LastName;

                    _profileService.Update(requestedUser);

                    // For audit log of user identity
                    #region User Profile Audit
                    var userProfileAuditLogModel = new AuditLogModel
                    {
                        AuditActionType = AuditActionType.Update,
                        OldObject = oldUserProfileRecord,
                        NewObject = requestedUser,
                        KeyField = oldUserProfileRecord.Id.ToString(),
                        ComparisonType = ComparisonType.ObjectCompare,
                        LoggedUserEmail = loggedUser.Email
                    };

                    _auditLogService.Add(userProfileAuditLogModel);
                    #endregion

                    var currentRole = requestedUserRoles.FirstOrDefault();
                    var requestedUserRoleId = _roleManager.Roles.FirstOrDefault(r => r.Name == currentRole).Id;

                    if (requestedUserRoleId != model.RoleId)
                    {
                        var roleUpdateResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                        if (!roleUpdateResult.Succeeded)
                        {

                            await InitializeRolesAndTimeZonesAsync();
                            TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToManageUserRoleErrorMessage].Value;
                            return View("Edit", model);
                        }

                        await _userManager.AddToRoleAsync(user, role.Name);
                        await _userManager.UpdateSecurityStampAsync(user); //Update security timestamp and force logout for user if their role is changed
                    }

                    #region Role Audit
                    var roleAuditLogModel = new AuditLogModel
                    {
                        AuditActionType = AuditActionType.Update,
                        KeyField = user.Id,
                        OldString = currentRole,
                        NewString = role.Name,
                        ModuleList = ModuleList.User,
                        LoggedUserEmail = loggedUser.Email,
                        ComparisonType = ComparisonType.StringCompare
                    };

                    _auditLogService.Add(roleAuditLogModel);
                    #endregion

                    _context.SaveChanges();
                    transaction.Commit();

                    TempData["SuccessMsg"] = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Logger.Error(ex, ErrorLog.UpdateError);
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value;
                    return RedirectToAction("Index");
                }
            }

        }

        /// <summary>
        /// to resend activation link to user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("ResendActivationEmail")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserCreate)]
        public async Task<IActionResult> ResendActivationEmail([FromBody] string userId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(userId))
                    {
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.SelectValidItemErrorMessage].Value;
                        return RedirectToAction("Index");
                    }
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                        return RedirectToAction("Index");
                    }

                    //make latest token valid
                    var unUsedLastToken = _applicationTokenService.GetSingle(x => x.IsUsed == false &&
                                                                                x.UserId == user.Id &&
                                                                                x.TokenType == TokenType.ConfirmEmail);
                    if (unUsedLastToken != null)
                    {
                        unUsedLastToken.IsUsed = true;
                        unUsedLastToken.UsedDate = DateTime.UtcNow;
                        unUsedLastToken.LastUpdated = DateTime.UtcNow;

                        _applicationTokenService.Update(unUsedLastToken);
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(TokenType.ConfirmEmail, "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);
                    await _messagingService.SendEmailConfirmationMailAsync(user.Email, user.UserName, callbackUrl);

                    var appToken = new ApplicationToken
                    {
                        TokenType = TokenType.ConfirmEmail,
                        Token = code,
                        UserId = user.Id,
                        IsUsed = false,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };

                    _applicationTokenService.Add(appToken);
                    _context.SaveChanges();
                    transaction.Commit();

                    return Json(new { success = true, messageTitle = _localizer[ActionMessageConstants.Success].Value, message = _localizer[ActionMessageConstants.EmailSentSuccessfully].Value });
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToSendActivationEmailErrorMessage].Value;
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpPost("delete/{id}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserDelete)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region Validation

                    if (string.IsNullOrEmpty(id))
                    {
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.SelectValidItemErrorMessage].Value;
                        return RedirectToAction("Index");
                    }

                    var user = await _userManager.FindByIdAsync(id);
                    var loggedUser = await GetCurrentUserAsync(); // For audit log purpose
                    if (user == null)
                    {
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                        return RedirectToAction("Index");
                    }
                    #endregion

                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToDeleteErrorMessage].Value;
                        return RedirectToAction("Index");
                    }

                    #region Delete Identity User Audit
                    var dummyCoreUser = new User(); // Storage of this null object shows data before creation = nothing!
                    var auditLogModel = new AuditLogModel
                    {
                        AuditActionType = AuditActionType.Delete,
                        OldObject = user,
                        NewObject = dummyCoreUser,
                        KeyField = user.Id,
                        ComparisonType = ComparisonType.ObjectCompare,
                        LoggedUserEmail = loggedUser.Email
                    };
                    _auditLogService.Add(auditLogModel);
                    #endregion

                    //Soft Delete
                    //User will removed from AspNet Identity but will remain in "UserProfile" table with flag IsDeleted TRUE.
                    var requestedDeletedUser = _profileService.GetSingle(x => x.UserId == user.Id);

                    // Set old user profile data to an object for audit
                    var oldrequestedDeletedUser = new UserProfile
                    {
                        UserId = requestedDeletedUser.UserId,
                        Email = requestedDeletedUser.Email,
                        UserName = requestedDeletedUser.UserName,
                        FirstName = requestedDeletedUser.FirstName,
                        LastName = requestedDeletedUser.LastName,
                        Phone = requestedDeletedUser.Phone,
                        Website = requestedDeletedUser.Website,
                        AboutMe = requestedDeletedUser.AboutMe,
                        IsDeleted = requestedDeletedUser.IsDeleted,
                        DeletedDate = requestedDeletedUser.DeletedDate
                    };

                    // Update attributes after soft delete
                    requestedDeletedUser.IsDeleted = true; //Soft Delete
                    requestedDeletedUser.DeletedDate = DateTime.UtcNow;
                    requestedDeletedUser.LastUpdated = DateTime.UtcNow;

                    _profileService.Update(requestedDeletedUser);

                    #region User Profile Update Audit                  
                    var userProfileAuditModel = new AuditLogModel
                    {
                        AuditActionType = AuditActionType.Update,
                        OldObject = oldrequestedDeletedUser,
                        NewObject = requestedDeletedUser,
                        KeyField = oldrequestedDeletedUser.Id.ToString(),
                        ComparisonType = ComparisonType.ObjectCompare,
                        LoggedUserEmail = loggedUser.Email
                    };
                    _auditLogService.Add(userProfileAuditModel);
                    #endregion

                    _context.SaveChanges();
                    transaction.Commit();

                    TempData["SuccessMsg"] = _localizer[ActionMessageConstants.DeletedSuccessfully].Value;
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value;
                    Logger.Error(ex, ErrorLog.DeleteError);
                    return RedirectToAction("Index");
                }
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profileEntity = _profileService.GetSingle(x => x.UserId == user.Id);
            if (profileEntity != null)
            {
                var profileVm = Mapper.Map<ProfileVM>(profileEntity);
                profileVm.FirstName = profileVm.FirstName ?? "";
                profileVm.LastName = profileVm.LastName ?? "";
                profileVm.Phone = profileVm.Phone ?? "";
                profileVm.AboutMe = profileVm.AboutMe ?? "";
                profileVm.Website = profileVm.Website ?? "";
                profileVm.TimeZone = user.TimeZone;
                profileVm.Email = user.Email;
                profileVm.UserName = user.UserName;

                return View("Profile", profileVm);
            }

            var profile = new ProfileVM();
            {
                profile.Email = user.Email;
                profile.UserName = user.UserName;
            }

            return View("Profile", profile);
        }

        [HttpGet("profile/edit/{id:int}")]
        public async Task<IActionResult> EditProfile(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                return RedirectToAction("Profile");
            }

            var profileEntity = _profileService.GetSingle(x => x.Id == id);
            if (profileEntity == null)
            {
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                    return RedirectToAction("Profile");
                }
            }

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var profileVm = Mapper.Map<ProfileVM>(profileEntity);
            profileVm.Email = user.Email;
            profileVm.UserName = user.UserName;
            profileVm.TimeZone = user.TimeZone;
            profileVm.TimeZone = user.TimeZone;

            ViewBag.TimeZoneList = GetAllSupportedTimeZones(); //Timezone Init
            return View("EditProfile", profileVm);
        }

        [HttpPost("profile/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileVM model)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.TimeZoneList = GetAllSupportedTimeZones(); //Timezone Init
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                        return View("EditProfile", model);
                    }

                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    if (user.TimeZone != model.TimeZone.Trim())
                    {
                        user.TimeZone = model.TimeZone;
                        await _userManager.UpdateAsync(user);
                    }

                    var requestedUser = _profileService.GetSingle(x => x.UserId == model.UserId);
                    if (requestedUser == null)
                    {
                        ViewBag.TimeZoneList = GetAllSupportedTimeZones(); //Timezone Init
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                        return View("EditProfile", model);
                    }

                    var profileEntity = Mapper.Map<UserProfile>(model);
                    _profileService.Update(profileEntity);

                    _context.SaveChanges();
                    transaction.Commit();

                    TempData["SuccessMsg"] = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value;
                    return RedirectToAction("Profile");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value;
                    Logger.Error(ex, ErrorLog.UpdateError);
                    return View("EditProfile", model);
                }
            }

        }

        /// <summary>
        /// Server Side DataTable Handler
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost("DataHandler")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserView)]
        public async Task<JsonResult> DataHandler([FromBody] UserDTParameters parameters)
        {
            try
            {                     
                var users = _userManager.Users.Select(u => new UserListVM
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Email = u.Email,
                    AccountStatus = u.AccountStatus,
                    EmailConfirmed = u.EmailConfirmed
                });

                List<UserListVM> filteredUser;

                int totalUser;

                if (!string.IsNullOrEmpty(parameters.Search.Value.Trim())
                   && !string.IsNullOrWhiteSpace(parameters.Search.Value.Trim()))
                {
                    Expression<Func<UserListVM, bool>> search =
                        x => (x.Name ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.Email ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower());

                    totalUser = users.Count(search);
                    users = users.Where(search);
                    users = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), users);
                    filteredUser = users.Skip(parameters.Start).Take(parameters.Length).ToList();

                }
                else
                {
                    totalUser = users.Count();
                    users = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), users);
                    filteredUser = users.Skip(parameters.Start).Take(parameters.Length).ToList();
                }

                var counter = parameters.Start + 1;
                var userListVms = filteredUser.OrderByDescending(x => x.CreatedDate).Select(x => { x.SN = counter++; return x; }).ToList();

                #region  User with role priority
                var selectedUserLists = new List<UserListVM>();
                foreach (var userListVm in userListVms)
                {
                    var targetUser = await _userManager.FindByIdAsync(userListVm.Id);
                    var targetUserRole = await _userManager.GetRolesAsync(targetUser);
                    userListVm.RoleName = FirstCharToUpper(targetUserRole.FirstOrDefault());
                    userListVm.RolePriority = await GetRolePriorityByNameAsync(targetUserRole.FirstOrDefault());
                    selectedUserLists.Add(userListVm);
                }
                #endregion


                var user = new DTResult<UserListVM>
                {
                    Draw = parameters.Draw,
                    Data = selectedUserLists,
                    RecordsFiltered = totalUser,
                    RecordsTotal = totalUser
                };
                return Json(user);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet("exportexcel/{searchString?}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserView)]
        public async Task<FileContentResult> ExportExcel(string searchString)
        {
            var loggedUser = await GetCurrentUserAsync();
            List<UserListVM> userList;

            var users = _userManager.Users.Select(u => new UserListVM
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email,
                AccountStatus = u.AccountStatus,
                EmailConfirmed = u.EmailConfirmed
            });
            if (string.IsNullOrEmpty(searchString))
            {
                userList = users.ToList();
            }
            else
            {
                Expression<Func<UserListVM, bool>> search =
                        x => (x.Name ?? "").ToString().ToLower().Contains(searchString.Trim().ToLower()) ||
                             (x.Email ?? "").ToString().ToLower().Contains(searchString.Trim().ToLower());
                userList = users.Where(search).ToList();
            }

            // get role name of each users
            foreach (var user in userList)
            {
                var targetUser = await _userManager.FindByIdAsync(user.Id);
                var targetUserRole = await _userManager.GetRolesAsync(targetUser);
                user.RoleName = FirstCharToUpper(targetUserRole.FirstOrDefault());
            }


            //column Header name
            var columnsHeader = new List<string>{
                "S/N",
                "User Name",
                "Email",
                "Role",
                "Status",
                "Email Confirmed"
            };
            var filecontent = ExportExcell(userList, columnsHeader, "Users");
            return File(filecontent, "application/ms-excel", "users.xlsx"); ;
        }

        [HttpGet("exportpdf/{searchString?}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AuthorizationUserView)]
        public async Task<FileContentResult> ExportPdf(string searchString)     
        {
            var loggedUser = await GetCurrentUserAsync();
            List<UserListVM> userList;

            var users = _userManager.Users.Select(u => new UserListVM
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email,
                AccountStatus = u.AccountStatus,
                EmailConfirmed = u.EmailConfirmed
            });

            if (string.IsNullOrEmpty(searchString))
            {
                userList = users.ToList();
            }
            else
            {
                Expression<Func<UserListVM, bool>> search =
                        x => (x.Name ?? "").ToString().ToLower().Contains(searchString.Trim().ToLower()) ||
                             (x.Email ?? "").ToString().ToLower().Contains(searchString.Trim().ToLower());
                userList = users.Where(search).ToList();
            }

            // get role name of each users
            foreach (var user in userList)
            {
                var targetUser = await _userManager.FindByIdAsync(user.Id);
                var targetUserRole = await _userManager.GetRolesAsync(targetUser);
                user.RoleName = FirstCharToUpper(targetUserRole.FirstOrDefault());
            }

            var columnsHeader = new List<string>{
                "S/N",
                "User Name",
                "Email",
                "Role",
                "Status",
                "Email Confirmed"
            };

            var reportedDate = _timeZoneHelper.ConvertToLocalTime(DateTime.UtcNow, loggedUser.TimeZone).ToString("g"); 

            var filecontent = ExportPdf(userList, columnsHeader, reportedDate, "Users");
            return File(filecontent, "application/pdf", "users.pdf"); ;
        }

        #region Helpers

        /// <summary>
        /// Serverside dataTable Sorting
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDirection"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IQueryable<UserListVM> SortByColumnWithOrder(int order, string orderDirection, IQueryable<UserListVM> data)
        {
            try
            {
                switch (order)
                {
                    case 2:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Name) : data.OrderBy(p => p.Name);

                    case 3:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Email) : data.OrderBy(p => p.Email);

                    case 4:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.RoleName) : data.OrderBy(p => p.RoleName);

                    case 5:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.AccountStatus) : data.OrderBy(p => p.AccountStatus);

                    case 6:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.EmailConfirmed) : data.OrderBy(p => p.EmailConfirmed);

                    default:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Name) : data.OrderBy(p => p.Name);
                }
            }
            catch (Exception)
            {
                return data;
            }
        }

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task<List<SelectListItem>> GetAllSupportedRolesAsync()
        {
            var availableRoles = new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Text = "Select Role",
                            Value = ""
                        }
                    };

            var currentUserRole = await _roleManager.FindByNameAsync(User.Claims.
                                FirstOrDefault(x => x.Type == ClaimTypes.Role).Value);

            var supportedRoles = _roleManager.Roles
                                .Where(x => x.Priority >= currentUserRole.Priority)
                                .Select(r => new SelectListItem
                                {
                                    Text = r.Name,
                                    Value = r.Id
                                }).ToList();

            availableRoles.AddRange(supportedRoles);
            return availableRoles;
        }

        private List<SelectListItem> GetAllSupportedTimeZones()
        {
            var timeZoneList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = _localizer[LabelConstants.PleaseSelect].Value,
                    Value = ""
                }
            };

            timeZoneList.AddRange(_timeZoneHelper.GetTimeZoneSelectListItem());
            return timeZoneList;
        }

        private static List<SelectListItem> GetAllAccountStatus()
        {
            var availableStatus = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "-- Select Status --",
                    Value = ""
                },
                new SelectListItem
                {
                    Text = "Enable",
                    Value = "1"
                },
                new SelectListItem
                {
                    Text = "Disable",
                    Value = "0"
                }
            };

            return availableStatus;
        }

        private async Task InitializeRolesAndTimeZonesAsync()
        {
            ViewBag.AvailableRoles = await GetAllSupportedRolesAsync(); //Role Init
            ViewBag.TimeZoneList = GetAllSupportedTimeZones(); //Timezone Init
            ViewBag.AccountStatus = GetAllAccountStatus(); //Account Status Init
        }

        private async Task<int> GetRolePriorityByIdAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            return role.Priority;
        }

        private async Task<int> GetRolePriorityByNameAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            return role.Priority;
        }

        private static byte[] ExportExcell(List<UserListVM> data, List<string> columns, string heading)
        {
            byte[] result;

            using (var package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook
                var worksheet = package.Workbook.Worksheets.Add(heading);
                using (var cells = worksheet.Cells[1, 1, 1, 6])
                {
                    cells.Style.Font.Bold = true;

                    #region css for header of cexel (commented for linux)
                    // cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //cells.Style.Fill.BackgroundColor.SetColor(Color.Green);
                    #endregion

                    //cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //cells.Style.Fill.BackgroundColor.SetColor(Color.Green);
                }
                //First add the headers
                for (var i = 0; i < columns.Count(); i++)
                {
                    worksheet.Cells[1, i + 1].Value = columns[i];
                }

                //Add values
                var j = 2;
                var count = 1;
                foreach (var item in data)
                {
                    worksheet.Cells["A" + j].Value = count;
                    worksheet.Cells["B" + j].Value = item.Name;
                    worksheet.Cells["C" + j].Value = item.Email;
                    worksheet.Cells["D" + j].Value = item.RoleName;
                    worksheet.Cells["E" + j].Value = item.AccountStatus == 1 ? "Active" : "Not Active";
                    worksheet.Cells["F" + j].Value = item.EmailConfirmed ? "Confirmed" : "Not confirmed";

                    j++;
                    count++;
                }
                result = package.GetAsByteArray();
            }

            return result;
        }

        /// <summary>
        /// convert list of users to pdf
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columns"></param>
        /// <param name="reportedDate"></param>
        /// <param name="heading"></param>
        /// <returns></returns>
        private static byte[] ExportPdf(List<UserListVM> data, List<string> columns, string reportedDate, string heading)
        {
            var document = new Document();
            var outputMs = new MemoryStream();
            PdfWriter.GetInstance(document, outputMs);
            document.Open();
            var font5 = FontFactory.GetFont(FontFactory.HELVETICA, 11);

            //var logo = iTextSharp.text.Image.GetInstance(_env.WebRootPath+"/assets/image-resources/logo-admin.png");
            //logo.BackgroundColor = BaseColor.Blue;
            //logo.Alignment = Element.ALIGN_LEFT;
            //document.Add(logo);

            document.Add(new Paragraph("Report Created On: " + reportedDate));

            document.Add(new Phrase(Environment.NewLine));

            //var count = typeof(UserListVM).GetProperties().Count();
            var count = columns.Count();
            var table = new PdfPTable(count);
            var widths = new[] { 1.5f, 4f, 6f, 3.5f, 3f, 4f };

            table.SetWidths(widths);

            table.WidthPercentage = 100;
            var pdfPCell = new PdfPCell(new Phrase(heading)) {Colspan = count};

            for (var i = 0; i < count; i++)
            {
                var headerCell = new PdfPCell(new Phrase(columns[i], font5)) {BackgroundColor = BaseColor.Gray};
                table.AddCell(headerCell);
            }
            if (data.Count > 0)
            {
                var sn = 1;
                foreach (var item in data)
                {
                    table.AddCell(new Phrase(sn.ToString(), font5));
                    table.AddCell(new Phrase(item.Name, font5));
                    table.AddCell(new Phrase(item.Email, font5));
                    table.AddCell(new Phrase(item.RoleName, font5));
                    table.AddCell(new Phrase(item.AccountStatus == 1 ? "Active" : "Not Active", font5));
                    table.AddCell(new Phrase(item.EmailConfirmed ? "Confirmed" : "Not Confirmed", font5));

                    sn++;
                }
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase(" No data available", font5)) { Colspan = count });
            }

            document.Add(table);
            document.Close();
            var result = outputMs.ToArray();

            return result;
        }

        /// <summary>
        /// for converting first letter of string to upper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
        #endregion
    }
}
