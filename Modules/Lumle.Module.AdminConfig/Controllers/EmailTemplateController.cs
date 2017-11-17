using System;
using Microsoft.AspNetCore.Mvc;
using Lumle.Module.AdminConfig.Services;
using Lumle.Module.AdminConfig.ViewModels;
using System.Threading.Tasks;
using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Lumle.Infrastructure.Constants.LumleLog;
using Microsoft.AspNetCore.Identity;
using NLog;
using Microsoft.Extensions.Localization;
using Lumle.Infrastructure.Constants.Localization;
using Lumle.Core.Services.Abstracts;
using System.Security.Claims;
using System.Collections.Generic;
using Lumle.Infrastructure.Constants.ActionConstants;
using System.Linq;

namespace Lumle.Module.AdminConfig.Controllers
{
    [Route("adminconfig/[controller]")]
    [Authorize]
    public class EmailTemplateController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IBaseRoleClaimService _baseRoleClaimService;       
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<ResourceString> _localizer;

        public EmailTemplateController(
            IBaseRoleClaimService baseRoleClaimService,
            IEmailTemplateService emailTemplateService,
            UserManager<User> userManager,          
            IStringLocalizer<ResourceString> localizer
        )
        {
            _baseRoleClaimService = baseRoleClaimService;
            _emailTemplateService = emailTemplateService;
            _userManager = userManager;         
            _localizer = localizer;
        }

        [HttpGet]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigEmailTemplateView)]
        public IActionResult Index()
        {
            var emailTemplates = _emailTemplateService.GetAllEmailTemplate().ToList();

            ViewBag.EmailTemplateUpdated = TempData["EmailTemplateUpdated"]; // for email template edit case
            return View(emailTemplates);
        }

        [HttpGet]
        [Route("edit/{emailTemplateId:int}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigEmailTemplateView)]
        public async Task<IActionResult> GetEmailTemplate(int emailTemplateId)
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

            var emailTemplate = await _emailTemplateService.GetSingleAsync(x => x.Id == emailTemplateId);
            if (emailTemplate == null)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.ResourceNotFoundErrorMessage].Value;
                return RedirectToAction("Index");
            }

            if (TempData["UseDefaultTemplate"] != null)
            {
                emailTemplate.SlugDisplayName = emailTemplate.DefaultSlugDisplayName;
                emailTemplate.Subject = emailTemplate.DefaultSubject;
                emailTemplate.Body = emailTemplate.DefaultBody;
            }

            if (TempData["UseLastTemplate"]!=null)
            {
                emailTemplate.SlugDisplayName = emailTemplate.LastSlugDisplayName;
                emailTemplate.Subject = emailTemplate.LastSubject;
                emailTemplate.Body = emailTemplate.LastBody;
            }

            var emailTemplateVm = AutoMapper.Mapper.Map<EmailTemplateVM>(emailTemplate);

            // Add permission for edit
            emailTemplateVm.UpdateAction = actionClaimResult[OperationActionConstant.UpdateAction];

            return View("Edit", emailTemplateVm);
        }

        [HttpPost]
        [Route("edit/{emailTemplateId:int}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigEmailTemplateUpdate)]
        public async Task<IActionResult> EditEmailTemplate(EmailTemplateVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMsg"] =_localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                    return View("Edit", model);
                }

                var loggedUser = await GetCurrentUserAsync(); //Get current logged in user

                await _emailTemplateService.Update(model, loggedUser);
               
                TempData["EmailTemplateUpdated"] = true;

                TempData["SuccessMsg"] =_localizer[ActionMessageConstants.UpdatedSuccessfully].Value;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                TempData["ErrorMsg"] =_localizer[ActionMessageConstants.InternalServerErrorMessage].Value;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Route("default/{emailTemplateId:int}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigEmailTemplateUpdate)]
        public IActionResult LoadDefaultTemplate(int emailTemplateId)
        {
            TempData["UseDefaultTemplate"] = true;
            return RedirectToAction("EditEmailTemplate", emailTemplateId);
        }

        [HttpGet]
        [Route("last/{emailTemplateId:int}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.AdminConfigEmailTemplateUpdate)]
        public IActionResult LastDefaultTemplate(int emailTemplateId)
        {
            TempData["UseLastTemplate"] = true;
            return RedirectToAction("EditEmailTemplate", emailTemplateId);
        }

        #region Helpers
        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        #endregion

    }
}
