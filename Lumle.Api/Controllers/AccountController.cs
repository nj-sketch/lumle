using Microsoft.AspNetCore.Mvc;
using JsonApiDotNetCore.Controllers;
using JsonApiDotNetCore.Services;
using Microsoft.Extensions.Logging;
using Lumle.Api.ViewModels.Account;
using System;
using JsonApiDotNetCore.Internal;
using Lumle.Api.Infrastructures.Helpers;
using Lumle.Infrastructure.Utilities;
using System.Threading.Tasks;
using Lumle.Api.Data.Entities;
using Lumle.Api.Service.Services.Abstracts;
using Lumle.Api.Data.Abstracts;

namespace Lumle.Api.Controllers
{
    [Route("[controller]")]
    [Produces("application/json","application/vnd.api+json")]
    [Consumes("application/json", "application/vnd.api+json")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IAccountService accountService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
        }


        [HttpPost("signup")]
        public IActionResult SignUp([FromBody]SignupVM model)
        {

            try
            {
                if (!ModelState.IsValid)
                    return AppUtil.Error(new Error("400", "Bad Request.", "Please fill all required details."));

                if (_accountService.IsUserAvailable(x => x.Email == model.Email))
                    return AppUtil.Error(new Error("400", "Duplicate user.", "User with this email already exist. Please try with new one."));

                var entity = AutoMapper.Mapper.Map<MobileUser>(model);
                _accountService.CreateSignupUser(entity, model.Password);

                return Created($"{HttpContext.Request.Host}", model);
            }
            catch (Exception)
            {

                throw new JsonApiException(new Error("500", "Internal Server Error", "Internal server error. Please contact app admin. "));
            }
        }


    }
}
