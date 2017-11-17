using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.ActionConstants;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Infrastructure.Constants.Localization;
using Lumle.Module.PublicUser.Helpers;
using Lumle.Module.PublicUser.Services;
using Lumle.Module.PublicUser.ViewModels.PublicUserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lumle.Module.PublicUser.Controllers
{
    [Route("publicuser")]
    [Authorize]
    public class ManageUserController : Controller
    {
        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly IPublicUserService _publicUserService;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<ResourceString> _localizer;

        public ManageUserController(
            IBaseRoleClaimService baseRoleClaimService,
            IPublicUserService publicUserService, 
            UserManager<User> userManager,
            IStringLocalizer<ResourceString> localizer)
        {
            _baseRoleClaimService = baseRoleClaimService;
            _publicUserService = publicUserService;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.PublicUserView)]
        public async Task<IActionResult> Index()
        {
            // Make map to check for the action previleges
            var map = new Dictionary<string, Claim>
            {
                { OperationActionConstant.UpdateAction, new Claim("permission", Permissions.PublicUserUpdate) }
            };

            // Get action previlege according to actions provided
            var actionClaimResult = await _baseRoleClaimService.GetActionPrevilegeAsync(map, User);

            ViewBag.UpdateAction = actionClaimResult[OperationActionConstant.UpdateAction];

            return View();
        }

        [HttpPost("update")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update([FromBody] PublicUserEditVM user)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, messageTitle = _localizer[ActionMessageConstants.UnableToUpdateErrorMessage].Value, message = _localizer[ActionMessageConstants.UnableToUpdateErrorMessage].Value });

            var currentUser = await GetCurrentUserAsync(); // Get current logged in user

            await _publicUserService.Update(user, currentUser);

            return Json(new { success = true, messageTitle = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value, message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value });
        }

        [HttpPost("DataHandler")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.PublicUserView)]
        public async Task<JsonResult> DataHandler([FromBody] PublicUserDTParamaters parameters)
        {
            try
            {
                var currentUser = await GetCurrentUserAsync();

                // Datatable result
                var result = _publicUserService.GetDataTableResult(currentUser, parameters);
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
