using Microsoft.AspNetCore.Mvc;
using Lumle.Api.ViewModels.Account;
using System;
using System.Net;
using Lumle.Api.BusinessRules.Abstracts;
using Lumle.Api.Infrastructures.Abstracts;
using Lumle.Api.Infrastructures.Extensions;
using Lumle.Api.Infrastructures.Handlers.ApiResponse;
using Lumle.Api.Infrastructures.Handlers.ApiResponse.Models;
using Lumle.Api.Infrastructures.Helpers;

namespace Lumle.Api.Controllers
{
    
    [NonController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountController : LumleBaseController
    {
        private readonly IAccountBusinessRule _accountBusinessRule;
        private readonly IActionResponse _actionResponse;

        public AccountController(IAccountBusinessRule accountBusinessRule,
            IActionResponse actionResponse) : base(actionResponse)
        {
            _accountBusinessRule = accountBusinessRule;
            _actionResponse = actionResponse;
        }


        [HttpPost("signup")]
        public IActionResult SignUp([FromBody]SignupVM model)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(_actionResponse.GetResponse(null, ActionResponseHelper.GetAllErrors(ModelState.GetErrors())));

                if (_accountBusinessRule.IsUserAvailable(x => string.Equals(x.Email, model.Email, StringComparison.CurrentCultureIgnoreCase)))
                    return BadRequest(new[]
                    {
                        new Message
                        {
                            Title = "Duplicate User",
                            Detail = "User with this email already exist. Please try with new one."
                        }
                    });

                _accountBusinessRule.RegisterUser(model);

                return StatusCode((int)HttpStatusCode.Created,
                    _actionResponse.GetResponse(null, null, new[]
                    {
                        new Message
                        {
                            Title = "Your account has been created. Please verify your email to login."
                        }
                    }));
            }
            catch (Exception)
            {

                return InternalServerError(new[]
                {
                    new Message
                    {
                        Title = "Internal server error."
                    }
                });
            }
        }


    }
}
