using System;
using System.Threading.Tasks;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.AdminConfig.Entities;
using Lumle.Module.AdminConfig.Models;
using Lumle.Module.AdminConfig.Services;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Microsoft.Extensions.Localization;
using Lumle.Infrastructure.Constants.Localization;
using System.Linq;
using Lumle.Core.Attributes;
using Lumle.Core.Localizer;

namespace Lumle.Module.AdminConfig.Controllers
{
    [Authorize]
    [Route("adminconfig/[controller]")]
    public class CredentialController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICredentialCategoryService _credentialCategoryService;
        private readonly ICredentialService _credentialService;
        private readonly UserManager<User> _userManager;
        private readonly IAuditLogService _auditLogService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<ResourceString> _localizer;

        public CredentialController(
            ICredentialCategoryService credentialCategoryService,
            IAuditLogService auditLogService,
            ICredentialService credentialService,
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            IStringLocalizer<ResourceString> localizer
        )
        {
            _userManager = userManager;
            _credentialCategoryService = credentialCategoryService;
            _credentialService = credentialService;
            _auditLogService = auditLogService;
            _unitOfWork = unitOfWork;
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
        public IActionResult Edit(int credentialCategoryId)
        {
            try
            {
                var credential = _credentialService.GetAllCredential(x => x.CredentialCategoryId == credentialCategoryId).ToList();

                if (!credential.Any())
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                }

                return View(credential);
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
                var data = _credentialService.GetSingle(x => x.Id == credential.Id);
                if (data == null)
                    return Json(GetOperationFailedMessage());

                if (data.Value == credential.Value.Trim()) return Json(GetOperationSuccessMessage());

                var loggedUser = await GetCurrentUserAsync(); // Get current logged user
                // Add previous data in old record object for comparison
                var oldRecord = new Credential
                {
                    Id = data.Id,
                    CredentialCategoryId = data.CredentialCategoryId,
                    Slug = data.Slug,
                    DisplayName = data.DisplayName,
                    Value = data.Value
                };

                // update in database
                data.Value = credential.Value.Trim();
                _credentialService.Update(data);

                #region Credential Audit Log

                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Update,
                    KeyField = oldRecord.Id.ToString(),
                    OldObject = oldRecord,
                    NewObject = data,
                    LoggedUserEmail = loggedUser.Email,
                    ComparisonType = ComparisonType.ObjectCompare
                };

                _auditLogService.Add(auditLogModel);

                #endregion

                //Save
                _unitOfWork.Save();
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
