using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using Lumle.Data.Extensions;
using Lumle.Infrastructure;
using Lumle.Infrastructure.Constants.Messenging;
using Lumle.Infrastructure.Utilities.Abstracts;
using Lumle.Module.Core.ViewModels.AccountViewModels;
using Microsoft.Extensions.Caching.Memory;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.IdentityConstants;
using Lumle.Module.AdminConfig.Services;
using System;
using Lumle.Data.Data;
using Lumle.Infrastructure.Utilities;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Infrastructure.Constants.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NLog;
using Lumle.Infrastructure.Constants.SystemSetting;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using Lumle.Core.Localizer;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Infrastructure.Constants.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Lumle.Infrastructure.Constants.Cache;
using Lumle.Infrastructure.Constants.Log;

namespace Lumle.Module.Core.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Class Variables
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly LumleSignInManager<User> _signInManager;
        private readonly IProfileService _profileService;
        private readonly IEmailService _emailService;
        private readonly ITwilioSmsService _twilioSmsService;
        private readonly TwilioSmsCredentials _twilioSmsCredentials;
        private readonly IMemoryCache _memoryCache;
        private readonly BaseContext _context;
        private readonly IMessagingService _messagingService;
        private readonly IApplicationTokenService _applicationTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly ISystemSettingService _systemSettingService;
        private readonly string _ipAddress;
        private readonly string _browserName;

        private IList<ResourceString> _resourceStrings;
        private readonly IRepository<Resource> _resourceRepository;
        private readonly IRepository<Culture> _cultureRepository;
        private readonly IStringLocalizer<ResourceString> _localizer;
        private readonly IdentityOptions _options;

        #endregion

        public AccountController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            LumleSignInManager<User> signInManager,
            IProfileService profileService,
            IEmailService emailService,
            ILoggerFactory loggerFactory,
            IMemoryCache memoryCache,
            BaseContext context,
            IMessagingService messagingService,
            IApplicationTokenService applicationTokenService,
            IUnitOfWork unitOfWork,
            IActionContextAccessor accessor,
            IHttpContextAccessor contextAccessor,
            ISystemSettingService systemSettingService,
            IUtilities utilities,
             IRepository<Resource> resourceRepository,
             IRepository<Culture> cultureRepository,
             IStringLocalizer<ResourceString> localizer,
             IOptions<IdentityOptions> optionsAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _profileService = profileService;
            _emailService = emailService;
            _twilioSmsService = new SmsService();
            _twilioSmsCredentials = new TwilioSmsCredentials
            {
                AccountSid = Twilio.AccountSid,
                Token = Twilio.AuthToken,
                From = Twilio.FromNumber
            };
            _memoryCache = memoryCache;
            _context = context;
            _messagingService = messagingService;
            _applicationTokenService = applicationTokenService;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _systemSettingService = systemSettingService;
            _ipAddress = utilities.GetClientInformation().Ip;
            _browserName = utilities.GetClientInformation().Browser;
            _resourceRepository = resourceRepository;
            _cultureRepository = cultureRepository;
            _localizer = localizer;
            _options = optionsAccessor?.Value?? new IdentityOptions();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var loggedUser = HttpContext.User.Identity.Name;

            return string.IsNullOrEmpty(loggedUser) ? RedirectToAction("Login") : RedirectToLocal(returnUrl);
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var loggedUser = HttpContext.User.Identity.Name;

            return string.IsNullOrEmpty(loggedUser) ? View() : RedirectToLocal(returnUrl);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            _memoryCache.Remove(CacheConstants.AuthorizationSidebarMenuCache);
            _memoryCache.Remove(CacheConstants.LocalizationCultureCache);
            _memoryCache.Remove(CacheConstants.LocalizationResourceCache);
            _memoryCache.Remove(CacheConstants.AuthorizationApplicationClaimsCache);




            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var user = await _userManager.FindByEmailAsync(model.Email);

            #region Global Setting

            InitializeGlobalLoggers(model.Email);

            #endregion

            if (user == null)
            {
                Logger.Error(CustomLogIdentifier.CustomLog + model.Email + CustomLogIdentifier.LoginFailureMessage);
                GlobalDiagnosticsContext.Clear();
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidLoginAttemptErrorMessage].Value;
                return View();
            }

            var systemMode = _systemSettingService.GetSingle(x => x.Slug == SystemSetting.MaintenanceMode);
            if (systemMode != null && systemMode.Status == SystemSetting.MaintenanceModeOn)
            {
                var userRole = await _userManager.GetRolesAsync(user);
                var role = userRole.Any() ? await _roleManager.FindByNameAsync(userRole.FirstOrDefault()) : null;
                if (role != null)
                {
                    if (role.IsBlocked)
                    {
                        return RedirectToAction("systemMaintenanceMode", "Error");
                    }
                }
            }
            //Check if account is Enabled/Disabled
            if (user.AccountStatus == (int)AccountStatus.Disable)
            {
                Logger.Error(CustomLogIdentifier.CustomLog + model.Email + CustomLogIdentifier.InactiveUserLoginMessage);
                GlobalDiagnosticsContext.Clear();
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidLoginAttemptErrorMessage].Value;
                return View();
            }
            if (!user.EmailConfirmed)
            {

                Logger.Error(CustomLogIdentifier.CustomLog + model.Email + CustomLogIdentifier.InactiveUserLoginMessage);
                GlobalDiagnosticsContext.Clear();
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidLoginAttemptErrorMessage].Value;
                return View();
            }

            var result =
                await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                #region Localization
                var supportedCulture = _cultureRepository.GetAll(x => x.IsActive && x.IsEnable && x.Name == user.Culture).FirstOrDefault();

                if (supportedCulture != null)
                {
                    Response.Cookies.Append(
                                  CookieRequestCultureProvider.DefaultCookieName,
                                  CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(user.Culture)),
                                  new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) }
                                  );
                }
                else
                {
                    Response.Cookies.Append(
                             CookieRequestCultureProvider.DefaultCookieName,
                             CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en-US")),
                             new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) }
                             );
                }

                _resourceStrings = _resourceRepository.AllIncluding(x => x.Culture).Where(x => !string.IsNullOrEmpty(x.Value) && x.Culture.Name == user.Culture).Select(x => new ResourceString
                {
                    Culture = x.Culture.Name,
                    Key = x.Key,
                    Value = x.Value
                }).ToList();

                var cacheOption = new MemoryCacheEntryOptions()
                {
                    Priority = CacheItemPriority.NeverRemove
                };
                _memoryCache.Set(CacheConstants.LocalizationResourceCache, _resourceStrings, cacheOption);

                #endregion

                Logger.Info(CustomLogIdentifier.CustomLog + model.Email + CustomLogIdentifier.LoginSuccessMessage);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                Logger.Error(CustomLogIdentifier.CustomLog + model.Email + CustomLogIdentifier.LockedAccountLoginMessage);
                GlobalDiagnosticsContext.Clear();
                return View("Lockout");
            }

            Logger.Error(CustomLogIdentifier.CustomLog + model.Email + CustomLogIdentifier.LoginFailureMessage);
            GlobalDiagnosticsContext.Clear();
            TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidLoginAttemptErrorMessage].Value;
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model, string returnUrl = null)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ViewData["ReturnUrl"] = returnUrl;
                    if (!ModelState.IsValid)
                    {
                        transaction.Rollback();
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                        return View(model);
                    }

                    var user = new User
                    {
                        UserName = model.Username,
                        Email = model.Email,
                        TimeZone ="Asia/Kathmandu", 
                        AccountStatus = (int)AccountStatus.Enable,
                        CreatedBy = model.Username,
                        CreatedDate = DateTime.UtcNow
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {

                        var role = await _roleManager.FindByNameAsync("superadmin"); //TODO: Made dynamic
                        if (role != null)
                        {
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
                                var callbackUrl = Url.Action(TokenType.ConfirmEmail, "Account",
                                    new { userId = user.Id, code = code, referal = "signup" },
                                    protocol: HttpContext.Request.Scheme);
                                await
                                    _messagingService.SendEmailConfirmationMailAsync(model.Email, model.Username.Trim(),
                                        callbackUrl);

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

                                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.AccountCreatedSuccessfully].Value;
                                return RedirectToAction("Login", "Account");
                            }
                            transaction.Rollback();
                            TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToAssignRoleToUserErrorMessage].Value;
                            return View(model);
                        }
                        transaction.Rollback();
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.RoleNotFoundErrorMessage].Value;
                        return View(model);

                    }

                    transaction.Rollback();
                    TempData["ErrorMsg"] =
                        _localizer[ActionMessageConstants.UnableToCreateUserErrorMessage].Value + $"{ result.Errors.FirstOrDefault()?.Description}";
                    return View(model);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Logger.Error(ex, ErrorLog.UserCreationError);
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToCreateUserErrorMessage].Value;
                    return View(model);
                }
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            var loggedUser = await GetCurrentUserAsync(); // Get logged in user
            #region Global Setting

            InitializeGlobalLoggers(loggedUser.Email);

            #endregion
            Logger.Info(CustomLogIdentifier.CustomLog + loggedUser.Email + CustomLogIdentifier.LogoutMessage);
            GlobalDiagnosticsContext.Clear();

            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Index), "Account");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, _localizer[ActionMessageConstants.FromExternalProviderErrorMessage].Value + $"{ remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
            if (result.Succeeded)
            {

                #region CleanUp before Signin
                ViewData["ReturnUrl"] = returnUrl;

                _memoryCache.Remove(CacheConstants.AuthorizationSidebarMenuCache);
                _memoryCache.Remove(CacheConstants.LocalizationCultureCache);
                _memoryCache.Remove(CacheConstants.LocalizationResourceCache);
                _memoryCache.Remove(CacheConstants.AuthorizationApplicationClaimsCache);

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                #region Global Setting

                InitializeGlobalLoggers(user.Email);

                #endregion

                //Check if account is Enabled/Disabled
                if (user.AccountStatus == (int)AccountStatus.Disable)
                {
                    Logger.Error(CustomLogIdentifier.CustomLog + user.Email + CustomLogIdentifier.InactiveUserLoginMessage);
                    GlobalDiagnosticsContext.Clear();
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidLoginAttemptErrorMessage].Value;
                    return RedirectToAction(nameof(Login));
                }
                if (!user.EmailConfirmed)
                {

                    Logger.Error(CustomLogIdentifier.CustomLog + user.Email + CustomLogIdentifier.InactiveUserLoginMessage);
                    GlobalDiagnosticsContext.Clear();
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidLoginAttemptErrorMessage].Value;
                    return RedirectToAction(nameof(Login));
                }
                #endregion

                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            // If the user does not have an account, then ask the user to create an account.
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationVM
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Username = Guid.NewGuid().ToString()
            });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationVM model,
            string returnUrl = null)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        transaction.Rollback();
                        await HttpContext.Authentication.SignOutAsync(_options.Cookies.ExternalCookieAuthenticationScheme);
                        TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                        return View(model);
                    }

                    // Get the information about the user from the external login provider
                    var info = await _signInManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        await HttpContext.Authentication.SignOutAsync(_options.Cookies.ExternalCookieAuthenticationScheme);
                        return View("ExternalLoginFailure");
                    }
                    var user = new User
                    {
                        UserName = model.Username,
                        EmailConfirmed = true, //Social login user are verified 
                        Email = model.Email,
                        TimeZone = "Nepal Standard Time",
                        AccountStatus = (int)AccountStatus.Enable,
                        CreatedBy = "Self",
                        CreatedDate = DateTime.UtcNow
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {

                            var role = await _roleManager.FindByNameAsync("guest"); //TODO: Made dynamic
                            if (role != null)
                            {
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
                                    _context.SaveChanges();
                                    transaction.Commit();

                                    #region CleanUp before Signin
                                    _memoryCache.Remove(CacheConstants.AuthorizationSidebarMenuCache);
                                    _memoryCache.Remove(CacheConstants.LocalizationCultureCache);
                                    _memoryCache.Remove(CacheConstants.LocalizationResourceCache);
                                    _memoryCache.Remove(CacheConstants.AuthorizationApplicationClaimsCache);
                                    #endregion

                                    await _signInManager.SignInAsync(user, false);
                                    return RedirectToLocal(returnUrl);
                                }
                                return await RedirectToLoginAsync(transaction, ActionMessageConstants.UnableToAssignRoleToUserErrorMessage);
                            }
                            return await RedirectToLoginAsync(transaction, ActionMessageConstants.RoleNotFoundErrorMessage);
                        }
                        return await RedirectToLoginAsync(transaction, ActionMessageConstants.UnableToSignInErrorMessage);
                    }
                    return await RedirectToLoginAsync(transaction, $"{ActionMessageConstants.UnableToCreateUserErrorMessage}. {result.Errors.FirstOrDefault()?.Description}");
                }
                catch (Exception)
                {
                    return await RedirectToLoginAsync(transaction, ActionMessageConstants.UnableToCreateUserErrorMessage);
                }
            }

        }



        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string referal = null)
        {
            if (userId == null || code == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Error404", "Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Error404", "Error");
            }
            if (user.EmailConfirmed)
            {
                await _signInManager.SignOutAsync();
                TempData["Info"] = _localizer[ActionMessageConstants.EmailAlreadyConfirmedErrorMessage].Value;
                return RedirectToAction("Login");
            }

            var appToken = _applicationTokenService.GetSingle(x => x.UserId == userId && x.Token == code && x.TokenType == TokenType.ConfirmEmail);

            if (appToken == null)
            {
                await _signInManager.SignOutAsync();
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidLinkErrorMessage].Value;
                return RedirectToAction("Login");
            }
            if (!appToken.IsUsed)
            {
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    appToken.IsUsed = true;
                    appToken.UsedDate = DateTime.UtcNow;
                    appToken.LastUpdated = DateTime.UtcNow;

                    _applicationTokenService.Update(appToken);
                    _unitOfWork.Save();

                    await _signInManager.SignOutAsync();
                    //redirects user to reset password after email confirmation
                    //make latest token valid
                    var unUsedCode = _applicationTokenService.GetSingle(x => x.IsUsed == false && x.UserId == user.Id && x.TokenType == TokenType.ResetPassword);
                    if (unUsedCode != null)
                    {
                        unUsedCode.IsUsed = true;
                        unUsedCode.UsedDate = DateTime.UtcNow;
                        unUsedCode.LastUpdated = DateTime.UtcNow;

                        _applicationTokenService.Update(unUsedCode);
                        _unitOfWork.Save();
                    }

                    //Referal means where does this confirmation come from.
                    //If manager made user they need to reset password for login else if from signup then they will navigate to login
                    if (string.IsNullOrEmpty(referal))
                    {
                        var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                        TempData["EmailConfirmMsg"] = _localizer[ActionMessageConstants.EmailConfirmedSuccessfully].Value;
                        var appResetToken = new ApplicationToken
                        {
                            TokenType = "ResetPassword",
                            Token = resetCode,
                            UserId = user.Id,
                            IsUsed = false,
                            CreatedDate = DateTime.UtcNow,
                            LastUpdated = DateTime.UtcNow
                        };

                        _applicationTokenService.Add(appResetToken);
                        _unitOfWork.Save();
                        return RedirectToAction("ResetPassword", new { userId = user.Id, code = resetCode });
                    }

                    TempData["SuccessMsg"] = _localizer[ActionMessageConstants.SignUpEmailconfirmedSuccessfully].Value;
                    return RedirectToAction("Login", "Account");

                }
            }
            TempData["Info"] = _localizer[ActionMessageConstants.LinkExpiredErrorMessage].Value;
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet("forgotpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            await _signInManager.SignOutAsync();
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            // set global variables for logging
            InitializeGlobalLoggers(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user) || user.AccountStatus == Convert.ToInt32(AccountStatus.Disable))
            {
                // Don't reveal that the user does not exist or is not confirmed
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InValidUserErrorMessage].Value;
                Logger.Error(CustomLogIdentifier.CustomLog + model.Email + CustomLogIdentifier.ForgotPasswordAttemptAndFailure);
                GlobalDiagnosticsContext.Clear();
                return RedirectToAction("ForgotPassword");
            }

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
            // Send an email with this link
            //make latest token valid
            var unUsedCode = _applicationTokenService.GetSingle(x => x.IsUsed == false && x.UserId == user.Id && x.TokenType == TokenType.ResetPassword);
            if (unUsedCode != null)
            {
                unUsedCode.IsUsed = true;
                unUsedCode.UsedDate = DateTime.UtcNow;
                unUsedCode.LastUpdated = DateTime.UtcNow;

                _applicationTokenService.Update(unUsedCode);
                _unitOfWork.Save();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action(TokenType.ResetPassword, "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);

            await _messagingService.SendForgotPasswordMailAsync(user.Email, user.UserName, callbackUrl);

            var appToken = new ApplicationToken
            {
                TokenType = TokenType.ResetPassword,
                Token = code,
                UserId = user.Id,
                IsUsed = false,
                CreatedDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };
            _applicationTokenService.Add(appToken);
            _unitOfWork.Save();

            Logger.Info(CustomLogIdentifier.CustomLog + model.Email + CustomLogIdentifier.ForgotPasswordAttemptAndEmailSent);
            GlobalDiagnosticsContext.Clear();
            return View("ForgotPasswordConfirmation");

            // If we got this far, something failed, redisplay form
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Error404", "Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }
            InitializeGlobalLoggers(user.Email);
            var appToken = _applicationTokenService.GetSingle(x => x.UserId == user.Id && x.Token == code && x.TokenType == TokenType.ResetPassword);
            if (appToken == null)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidLinkErrorMessage].Value;
                Logger.Error(CustomLogIdentifier.CustomLog + user.Email + CustomLogIdentifier.InvalidForgotPasswordLink);
                GlobalDiagnosticsContext.Clear();
                return RedirectToAction(nameof(AccountController.Index), "Account");

            }
            if (appToken.IsUsed)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.LinkExpiredErrorMessage].Value;
                Logger.Error(CustomLogIdentifier.CustomLog + user.Email + CustomLogIdentifier.ExpiredForgotPasswordLink);
                GlobalDiagnosticsContext.Clear();
                return RedirectToAction(nameof(AccountController.Index), "Account");

            }
            await _signInManager.SignOutAsync();
            GlobalDiagnosticsContext.Clear();
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost("resetpassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                if (model.UserId == null)
                {
                    return RedirectToAction("Error404", "Error");
                }
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;

                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }
            InitializeGlobalLoggers(user.Email);
            var appToken = _applicationTokenService.GetSingle(x => x.UserId == user.Id && x.Token == model.Code && x.TokenType == TokenType.ResetPassword);
            if (appToken == null)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.InvalidLinkErrorMessage].Value;
                Logger.Error(CustomLogIdentifier.CustomLog + user.Email + CustomLogIdentifier.InvalidForgotPasswordLink);
                GlobalDiagnosticsContext.Clear();
                return View();
            }
            if (!appToken.IsUsed)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    appToken.IsUsed = true;
                    appToken.UsedDate = DateTime.UtcNow;
                    appToken.LastUpdated = DateTime.UtcNow;

                    _applicationTokenService.Update(appToken);
                    _unitOfWork.Save();

                    TempData["SuccessMsg"] = _localizer[ActionMessageConstants.PasswordResetSuccessfully].Value;
                    Logger.Info(CustomLogIdentifier.CustomLog + user.Email + CustomLogIdentifier.PasswordResetSuccessfully);
                    return RedirectToAction("Index", "Dashboard");
                }
                AddErrors(result);
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PasswordCannotResetErrorMessage].Value;

                return View();

            }
            TempData["ErrorMsg"] = _localizer[ActionMessageConstants.LinkExpiredErrorMessage].Value;
            Logger.Error(CustomLogIdentifier.CustomLog + user.Email + CustomLogIdentifier.ExpiredForgotPasswordLink);
            GlobalDiagnosticsContext.Clear();
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet("resetpasswordconfirmation")]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeVM { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeVM model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = _localizer[ActionMessageConstants.SecurityCode].Value + code;
            switch (model.SelectedProvider)
            {
                case "Email":
                    return RedirectToAction(nameof(VerifyCode),
                        new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
                case "Phone":
                    await _twilioSmsService.SendMessageAsync(_twilioSmsCredentials,
                        await _userManager.GetPhoneNumberAsync(user), message);
                    break;
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, model.ReturnUrl, model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            return user == null ? View("Error") : View(new VerifyCodeVM { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                //_logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            ModelState.AddModelError(string.Empty, _localizer[ActionMessageConstants.InvalidCodeErrorMessage].Value);
            return View(model);
        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        private void InitializeGlobalLoggers(string username)
        {
            GlobalDiagnosticsContext.Clear();
            GlobalDiagnosticsContext.Set("UserEmail", username);
            GlobalDiagnosticsContext.Set("IPAddress", _ipAddress);
            GlobalDiagnosticsContext.Set("BrowserName", _browserName);
        }

        private async Task<IActionResult> RedirectToLoginAsync(IDbContextTransaction transaction, string message)
        {
            transaction.Rollback();
            await HttpContext.Authentication.SignOutAsync(_options.Cookies.ExternalCookieAuthenticationScheme);
            TempData["ErrorMsg"] = _localizer[message].Value;
            return RedirectToAction("Login", "Account");
        }

        #endregion
    }
}
