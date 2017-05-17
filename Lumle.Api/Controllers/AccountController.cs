using Microsoft.AspNetCore.Mvc;
using Lumle.Api.Data.Entities;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;
using Lumle.Api.ViewModels.Account;
using System;
using JsonApiDotNetCore.Internal;
using Lumle.Api.Infrastructures.Helpers;
using Lumle.Infrastructure.Utilities;
using System.Threading.Tasks;

namespace Lumle.Api.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AccountController : JsonApiController<MobileUser>
    {
        private readonly IResourceService<MobileUser> _mobileUserService;

        public AccountController(IJsonApiContext jsonApiContext, IResourceService<MobileUser> resourceService, ILoggerFactory loggerFactory) : base(jsonApiContext, resourceService, loggerFactory)
        {

            _mobileUserService = resourceService;

        }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody]SignupVM model)
        {

            try
            {
                if (!ModelState.IsValid)
                    return AppUtil.Error(new Error("400", "Bad Request.", "Please fill all required details."));


                var passwordSalt = CryptoService.GenerateSalt();
                var passwordHash = CryptoService.ComputeHash(model.Password, passwordSalt);

                var entity = new MobileUser
                {
                    SubjectId = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    PasswordSalt = Convert.ToBase64String(passwordSalt),
                    PasswordHash = Convert.ToBase64String(passwordHash),
                    IsStaff = false,
                    IsBlocked = false,
                    IsEmailVerified = false,
                    Provider = "application"

                };

                return await base.PostAsync(entity);
            }
            catch (Exception)
            {

                throw new JsonApiException(new Error("500", "Internal Server Error", "Internal server error. Please contact app admin. "));
            }
        }


    }
}
