using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Infrastructure.Constants.Localization;
using Lumle.Infrastructure.Utilities.Abstracts;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Services;
using Lumle.Module.PublicUser.Entities;
using Lumle.Module.PublicUser.Helpers;
using Lumle.Module.PublicUser.Services;
using Lumle.Module.PublicUser.ViewModels.PublicUserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Lumle.Infrastructure.Helpers.DataTableHelper;

namespace Lumle.Module.PublicUser.Controllers
{
    [Route("publicuser")]
    [Authorize]
    public class ManageUserController : Controller
    {
        private readonly IPublicUserService _publicUserService;
        private readonly UserManager<User> _userManager;
        private readonly ITimeZoneHelper _timeZoneHelper;
        private readonly IUnitOfWork _unitOfwork;
        private readonly IAuditLogService _auditLogService;
        private readonly IStringLocalizer<ResourceString> _localizer;

        public ManageUserController(IPublicUserService publicUserService, 
            UserManager<User> userManager,
            ITimeZoneHelper timeZoneHelper,
            IUnitOfWork unitOfwork,
            IAuditLogService auditLogService,
            IStringLocalizer<ResourceString> localizer)
        {
            _publicUserService = publicUserService;
            _userManager = userManager;
            _timeZoneHelper = timeZoneHelper;
            _unitOfwork = unitOfwork;
            _auditLogService = auditLogService;
            _localizer = localizer;
        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.PublicUserView)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update([FromBody] PublicUserEditVM user)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, messageTitle = _localizer[ActionMessageConstants.UnableToUpdateErrorMessage].Value, message = _localizer[ActionMessageConstants.UnableToUpdateErrorMessage].Value });

            var publicUser = _publicUserService.GetSingle(x => x.Id == Convert.ToInt32(user.Id));
            if(publicUser == null)
                return Json(new { success = false, messageTitle = _localizer[ActionMessageConstants.ErrorOccured].Value, message = _localizer[ActionMessageConstants.ErrorOccured].Value });

            var oldRecord = new CustomUser
            {
                Id = publicUser.Id,
                IsBlocked = publicUser.IsBlocked,
                IsStaff = publicUser.IsStaff,
                LastUpdated = publicUser.LastUpdated,
            };

            publicUser.IsBlocked = Convert.ToBoolean(user.IsBlocked);
            publicUser.IsStaff = Convert.ToBoolean(user.IsStaff);
            publicUser.LastUpdated = DateTime.UtcNow;

            _publicUserService.Update(publicUser);

            var currentUser = await GetCurrentUserAsync(); // Get current logged in user

            #region Public User Audit log
            var auditLogModel = new AuditLogModel
            {
                AuditActionType = AuditActionType.Update,
                KeyField = publicUser.Id.ToString(),
                OldObject = oldRecord,
                NewObject = publicUser,
                LoggedUserEmail = currentUser.Email,
                ComparisonType = ComparisonType.ObjectCompare
            };
            _auditLogService.Add(auditLogModel);
            #endregion

            _unitOfwork.Save();

            return Json(new { success = true, messageTitle = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value, message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value });
        }

        [HttpPost("DataHandler")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.PublicUserView)]
        public async Task<JsonResult> DataHandler([FromBody] PublicUserDTParamaters parameters)
        {
            try
            {
                var currentUser = await GetCurrentUserAsync();
                var publicUsers = _publicUserService.GetAll().Select(usr => new PublicUserIndexVM
                {
                    Id = usr.Id,
                    UserName = usr.UserName,
                    ProfileUrl = usr.ProfileUrl,
                    Email = usr.Email,
                    Gender = usr.Gender,
                    IsStaff = usr.IsStaff,
                    IsEmailVerified = usr.IsEmailVerified,
                    IsBlocked = usr.IsBlocked,
                    Provider = usr.Provider,
                    CreatedDate = usr.CreatedDate
                }).AsQueryable();

                List<PublicUserIndexVM> filteredUsers;
                int totalUser;
                if (!string.IsNullOrEmpty(parameters.Search.Value.Trim())
                   && !string.IsNullOrWhiteSpace(parameters.Search.Value.Trim()))
                {
                    Expression<Func<PublicUserIndexVM, bool>> search =
                        x => (x.UserName ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.Email ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower()) ||
                             (x.Gender.ToString()).Contains(parameters.Search.Value.ToLower()) ||
                             (x.Provider ?? "").ToString().ToLower().Contains(parameters.Search.Value.ToLower());
                    totalUser = publicUsers.Count(search);
                    publicUsers = publicUsers.Where(search);
                    publicUsers = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), publicUsers);
                    filteredUsers = publicUsers.Skip(parameters.Start).Take(parameters.Length).ToList();
                }
                else
                {
                    totalUser = publicUsers.Count();
                    publicUsers = SortByColumnWithOrder(parameters.Order[0].Column, parameters.Order[0].Dir.ToString(), publicUsers);
                    filteredUsers = publicUsers.Skip(parameters.Start).Take(parameters.Length).ToList();
                }
                var counter = parameters.Start + 1;
                filteredUsers = filteredUsers.Select(x => {
                    x.SN = counter++;
                    x.FormatedCreatedDate = _timeZoneHelper.ConvertToLocalTime(x.CreatedDate, currentUser.TimeZone).ToString("g");
                    return x;
                }).ToList();

                var users = new DTResult<PublicUserIndexVM>
                {
                    Draw = parameters.Draw,
                    Data = filteredUsers,
                    RecordsFiltered = totalUser,
                    RecordsTotal = totalUser
                };
                return Json(users);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #region Helpers
        private static IQueryable<PublicUserIndexVM> SortByColumnWithOrder(int order, string orderDirection, IQueryable<PublicUserIndexVM> data)
        {
            try
            {
                switch (order)
                {
                    case 2:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.UserName) : data.OrderBy(p => p.UserName);
                    case 3:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Email) : data.OrderBy(p => p.Email);
                    case 4:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Gender) : data.OrderBy(p => p.Gender);
                    case 5:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.IsStaff) : data.OrderBy(p => p.IsStaff);
                    case 6:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.IsEmailVerified) : data.OrderBy(p => p.IsEmailVerified);
                    case 7:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.IsBlocked) : data.OrderBy(p => p.IsBlocked);
                    case 8:
                        return orderDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Provider) : data.OrderBy(p => p.Provider);
                    case 9:
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

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion
    }
}
