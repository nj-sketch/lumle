using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Lumle.AuthServer.Data.Entities;
using Lumle.AuthServer.Data.Store;
using Lumle.AuthServer.Infrastructures.Enums;
using Lumle.AuthServer.Infrastructures.Extensions;
using Lumle.AuthServer.Infrastructures.Helpers.Constants;
using Lumle.AuthServer.Infrastructures.Helpers.Tokens;
using Lumle.AuthServer.Infrastructures.Helpers.Utilities;
using Lumle.AuthServer.Infrastructures.Providers;
using Lumle.AuthServer.Infrastructures.Security.CryptoService;

namespace Lumle.AuthServer.Infrastructures.GrantTypes
{
    public class GoogleFirebaseAuthValidator : IExtensionGrantValidator
    {
        private readonly IUserStore _userStore;

        public GoogleFirebaseAuthValidator(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public string GrantType => ExternalGrantTypes.GoogleFireBase;

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            try
            {
                var idToken = context.Request.Raw.Get("access_token");

                if (string.IsNullOrEmpty(idToken))
                {
                    context.Result = new GrantValidationResult(OidcConstants.TokenErrors.InvalidRequest, null);
                    return;
                }

                //1. Validate token

                var validatedJwtToken = await AuthTokenValidator.ValidateGoogleIdTokenAsync(idToken);

                //2. Get Issuer data and add security layer

                var googleAuthResponse = validatedJwtToken.MapToGoogleAuthResponse();

                if (!string.Equals(googleAuthResponse.Iss, GoogleAuthConstants.Issuer, StringComparison.CurrentCultureIgnoreCase))
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Invalid issuer.");
                    return;
                }

                //3. Get User data and process for authentication

                var requestedUser = _userStore.FindByProviderAndEmail(ProviderConstants.Google, googleAuthResponse.Email);

                if (requestedUser != null && requestedUser.IsBlocked)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Requested user is blocked.");
                    return;
                }

                if (requestedUser == null)
                {
                    //Check if user with this email is already exist or not [Does not depend on provider]
                    if (_userStore.IsEmailExist(googleAuthResponse.Email.ToLower()))
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "User with this email already exist.");
                        return;
                    }

                    #region Password Hash and Salt

                    var pwdSalt = CryptoService.GenerateSalt();
                    var pwdHash = CryptoService.ComputeHash(PasswordGenerator.GetRandomPassword(), pwdSalt);
                    #endregion

                    //Create new User
                    var customUser = new MobileUser
                    {
                        SubjectId = googleAuthResponse.Sub,
                        Email = googleAuthResponse.Email.ToLower(),
                        UserName = string.Empty,
                        ProfileUrl = googleAuthResponse.Picture,
                        PhoneNo = string.Empty,
                        Gender = (int)AuthEnums.Gender.Unknown,
                        FirstName = string.Empty,
                        LastName = string.Empty,
                        PasswordHash = Convert.ToBase64String(pwdHash),
                        PasswordSalt = Convert.ToBase64String(pwdSalt),
                        IsStaff = false,
                        IsEmailVerified = true, //Verified by default
                        IsBlocked = false,
                        Provider = ProviderConstants.Google,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow

                    };

                    _userStore.AddNewUser(customUser);
                    //_userDbContext.SaveChanges();
                }

                context.Result = new GrantValidationResult(googleAuthResponse.Sub, ExternalGrantTypes.Google);
            }
            catch (TimeoutException)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Token lifetime expired. Please re-login with your google account.");
            }
            catch (Exception)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Internal Server Error.");
            }

        }
    }
}
