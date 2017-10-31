using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Lumle.Core.Localizer;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure;
using Lumle.Infrastructure.Constants.Messenging;
using Lumle.Infrastructure.Utilities.Abstracts;
using Lumle.Module.Core.ViewModels.ManageViewModels;
using Lumle.Data.Models;
using Lumle.Infrastructure.Utilities;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Services;
using Lumle.Infrastructure.Constants.Localization;
using Microsoft.Extensions.Localization;

namespace Lumle.Module.Core.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ITwilioSmsService _twilioSmsService;
        private readonly ILogger _logger;
        private readonly TwilioSmsCredentials _twilioSmsCredentials;
        private readonly IAuditLogService _auditLogService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<ResourceString> _localizer;

        public ManageController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailService emailService,
            IAuditLogService auditLogService,
            IUnitOfWork unitOfWork,
            ILoggerFactory loggerFactory,
            IStringLocalizer<ResourceString> localizer
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _twilioSmsService = new SmsService();
            _auditLogService = auditLogService;
            _unitOfWork = unitOfWork;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _twilioSmsCredentials = new TwilioSmsCredentials {
                AccountSid = Twilio.AccountSid,
                Token = Twilio.AuthToken,
                From = Twilio.FromNumber };
            _localizer = localizer;
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess ? _localizer[ActionMessageConstants.PasswordChangedSucessfully].Value
                : message == ManageMessageId.SetPasswordSuccess ? _localizer[ActionMessageConstants.PasswordSetSucessfully].Value
                : message == ManageMessageId.SetTwoFactorSuccess ? _localizer[ActionMessageConstants.TwoFactorAuthenticationProviderSetSucessfully].Value
                : message == ManageMessageId.Error ? _localizer[ActionMessageConstants.ErrorOccured].Value
                : message == ManageMessageId.AddPhoneSuccess ? _localizer[ActionMessageConstants.PhoneNumberAddedSucessfully].Value
                : message == ManageMessageId.RemovePhoneSuccess ? _localizer[ActionMessageConstants.PhoneNumberRemovedSucessfully].Value
                : "";

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var model = new IndexVM
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                //PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                //TwoFactor = await _userManager.GetTwoFactorEnabledAsync(user),
                Logins = await _userManager.GetLoginsAsync(user),
                BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user)
            };

            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginVM account)
        {
            ManageMessageId? message = ManageMessageId.Error;
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction(nameof(ManageLogins), new {Message = message});
            var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
            if (!result.Succeeded) return RedirectToAction(nameof(ManageLogins), new {Message = message});
            await _signInManager.SignInAsync(user, isPersistent: false);
            message = ManageMessageId.RemoveLoginSuccess;

            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await _twilioSmsService.SendMessageAsync(_twilioSmsCredentials, model.PhoneNumber, _localizer[ActionMessageConstants.SecurityCode].Value + code);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction(nameof(Index), "Manage");
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation(1, "User enabled two-factor authentication.");
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction(nameof(Index), "Manage");
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation(2, "User disabled two-factor authentication.");
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            // Send an SMS to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberVM { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }
            // If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, _localizer[ActionMessageConstants.FailedToVerifyPhoneNumberErrorMessage].Value);
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction(nameof(Index), new {Message = ManageMessageId.Error});
            var result = await _userManager.SetPhoneNumberAsync(user, null);
            if (!result.Succeeded) return RedirectToAction(nameof(Index), new {Message = ManageMessageId.Error});
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        ////
        //// GET: /Manage/ChangePassword
        //[HttpGet]
        //public IActionResult ChangePassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Manage/ChangePassword
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        //        if (result.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            _logger.LogInformation(3, "User changed their password successfully.");
        //            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
        //        }
        //        AddErrors(result);
        //        return View(model);
        //    }
        //    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        //}



        // GET: /Account/ChangePassword
        [HttpGet("changepassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("changepassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user == null)
                return RedirectToAction(nameof(Index), new { Message = ManageController.ManageMessageId.Error });
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                #region AuditLog
                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Update,
                    KeyField = user.Id,
                    OldString = "Old Password",
                    NewString = "New Password",
                    ModuleList = ModuleList.User,
                    LoggedUserEmail = user.Email,
                    ComparisonType = ComparisonType.StringCompare
                };

                await _auditLogService.Add(auditLogModel);
                await _unitOfWork.SaveAsync();
                #endregion

                //Logout and redirect to login
                await _signInManager.SignOutAsync();
                _logger.LogInformation(4, "User logged out.");
                return RedirectToAction(nameof(AccountController.Index), "Account");
            }

            TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToChangePasswordErrorMessage].Value + $"{result.Errors.FirstOrDefault().Description}";
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user == null) return RedirectToAction(nameof(Index), new {Message = ManageMessageId.Error});
            var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //GET: /Manage/ManageLogins
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.RemoveLoginSuccess ? _localizer[ActionMessageConstants.ExternalLoginRemovedErrorMessage].Value
                : message == ManageMessageId.AddLoginSuccess ? _localizer[ActionMessageConstants.ExternalLoginAddedSuccessfully].Value
                : message == ManageMessageId.Error ? _localizer[ActionMessageConstants.ErrorOccured].Value
                : "";
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();
            ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsVM
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return Challenge(properties, provider);
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
            if (info == null)
            {
                return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
            }
            var result = await _userManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;

            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
