using System;
using System.Threading.Tasks;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.AdminConfig.Models;
using Lumle.Module.AdminConfig.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Microsoft.Extensions.Localization;
using Lumle.Infrastructure.Constants.Localization;
using System.Linq;
using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Core.Services.Abstracts;
using Lumle.Module.AdminConfig.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using Lumle.Infrastructure.Constants.ActionConstants;

namespace Lumle.Module.AdminConfig.Controllers
{
    [Authorize]
    [Route("adminconfig/[controller]")]
    public class CredentialController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly ICredentialCategoryService _credentialCategoryService;
        private readonly ICredentialService _credentialService;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<ResourceString> _localizer;

        public CredentialController(
            IBaseRoleClaimService baseRoleClaimService,
            ICredentialCategoryService credentialCategoryService,
            ICredentialService credentialService,
            UserManager<User> userManager,          
            IStringLocalizer<ResourceString> localizer
        )
        {
            _baseRoleClaimService = baseRoleClaimService;
            _credentialCategoryService = credentialCategoryService;
            _credentialService = credentialService;
            _userManager = userManager;           
            _localizer = localizer;
        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigCredentialView)]
        public IActionResult Index()
        {
            var credentialCategory = _credentialCategoryService.GetAllCredentialCategory().ToList();

            return View(credentialCategory);
        }

        [HttpGet]
        [Route("{credentialCategoryId:int}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigCredentialView)]
        public async Task<IActionResult> Edit(int credentialCategoryId)
        {
            try
            {
                #region template-edit-permission
                // Make map to check for the action previleges
                var map = new Dictionary<string, Claim>
                {
                    { OperationActionConstant.UpdateAction, new Claim("permission", Permissions.AdminConfigCredentialUpdate) }
                };

                // Get action previlege according to actions provided
                var actionClaimResult = await _baseRoleClaimService.GetActionPrevilegeAsync(map, User);
                #endregion

                var credentials = _credentialService.GetAllCredential(x => x.CredentialCategoryId == credentialCategoryId).ToList();

                if (!credentials.Any())
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                }

                var credentialVm = new CredentialVM
                {
                    CredentialModels = credentials,
                    UpdateAction = actionClaimResult[OperationActionConstant.UpdateAction]
                };

                return View(credentialVm);
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value;
                return RedirectToAction("Index");
            }
        }

        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigCredentialUpdate)]
        public async Task<JsonResult> Edit([FromBody] CredentialModel credential)
        {
            try
            {
                var loggedUser = await GetCurrentUserAsync(); // Get current logged user
                await _credentialService.Update(credential, loggedUser);
                return Json(GetOperationSuccessMessage());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                return Json(GetOperationFailedMessage());
            }

        }

        #region Helpers
        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private object GetOperationSuccessMessage()
        {
            return new
            {
                success = true,
                messageTitle = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value,
                message = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value
            };
        }

        private object GetOperationFailedMessage()
        {
            return new
            {
                success = false,
                messageTitle = _localizer[ActionMessageConstants.ErrorOccured].Value,
                message = _localizer[ActionMessageConstants.InternalServerErrorMessage].Value
            };
        }

        #endregion
    }
}
