using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Lumle.AuthServer.Data.Entities;
using Lumle.AuthServer.Data.Store;
using Lumle.AuthServer.Infrastructures.GrantTypes.Response;
using Lumle.AuthServer.Infrastructures.Helpers;
using Lumle.AuthServer.Infrastructures.Helpers.Constants;
using Lumle.AuthServer.Infrastructures.Helpers.Utilities;
using Lumle.AuthServer.Infrastructures.Providers;
using Lumle.AuthServer.Infrastructures.Security.CryptoService;

namespace Lumle.AuthServer.Infrastructures.GrantTypes
{
    public class FacebookAuthValidator : IExtensionGrantValidator
    {
        private readonly IUserStore _userStore;

        public FacebookAuthValidator(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public string GrantType => ExternalGrantTypes.Facebok;

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            try
            {
                var inputToken = context.Request.Raw.Get("input_token");
                var accessToken = context.Request.Raw.Get("access_token");

                if (string.IsNullOrEmpty(inputToken))
                {
                    context.Result = new GrantValidationResult(OidcConstants.TokenErrors.InvalidRequest, null);
                    return;
                }

                if (string.IsNullOrEmpty(accessToken))
                {
                    context.Result = new GrantValidationResult(OidcConstants.TokenErrors.InvalidRequest, null);
                    return;
                }

                using (var client = new HttpClient())
                {
                    var profileResponse = await client.GetAsync($"https://graph.facebook.com/me?fields=id,email,gender,picture&access_token={accessToken}");

                    if (profileResponse.IsSuccessStatusCode)
                    {
                        //Parse fb response
                        var responseData = await profileResponse.Content.ReadAsStringAsync();
                        var fbTokenResponse = TokenResponseDserializer.DserializeIdToken<FacebookAuthResponse>(responseData);


                        //Check if user is available in Auth Database yet.
                        var requestedUser = _userStore.FindByProviderAndSubjectId(ProviderConstants.Facebook, fbTokenResponse.Id);


                        if (requestedUser != null && requestedUser.IsBlocked)
                        {
                            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "user is blocked.");
                            return;
                        }

                        if (requestedUser == null)
                        {
                            //Check if user with this email is already exist or not [Does not depend on provider]
                            if (!string.IsNullOrEmpty(fbTokenResponse.Email))
                            {
                                if (_userStore.IsEmailExist(fbTokenResponse.Email.ToLower()))
                                {
                                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "User with this email already exist.");
                                    return;
                                }
                            }


                            #region Password Hash and Salt

                            var pwdSalt = CryptoService.GenerateSalt();
                            var pwdHash = CryptoService.ComputeHash(PasswordGenerator.GetRandomPassword(), pwdSalt);
                            #endregion

                            //Get User Information

                            //Create new User
                            var customUser = new MobileUser
                            {
                                SubjectId = fbTokenResponse.Id,
                                Email = EmailValidator.IsValidEmail(fbTokenResponse.Email) ? fbTokenResponse.Email.ToLower() : "",
                                UserName = EmailValidator.IsValidEmail(fbTokenResponse.Email) ? fbTokenResponse.Email.ToLower() : "",
                                PasswordHash = Convert.ToBase64String(pwdHash),
                                PasswordSalt = Convert.ToBase64String(pwdSalt),
                                PhoneNo = "",
                                FirstName = string.Empty,
                                LastName = string.Empty,
                                IsStaff = false,
                                ProfileUrl = fbTokenResponse.Picture.Data.Url,
                                Gender = GenderResolver.GetNumericGenderValue(fbTokenResponse.Gender),
                                IsEmailVerified = true, //Verified by default
                                IsBlocked = false,
                                Provider = ProviderConstants.Facebook,
                                CreatedDate = DateTime.UtcNow,
                                LastUpdated = DateTime.UtcNow

                            };

                            _userStore.AddNewUser(customUser);
                            //_userDbContext.SaveChanges();
                        }

                        context.Result = new GrantValidationResult(fbTokenResponse.Id, ExternalGrantTypes.Facebok);
                    }
                    else
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "unable to access user profile.");
                }

            }
            catch (Exception)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Internal Server Error.");
            }
        }
    }
}
