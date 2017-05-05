using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lumle.Data.Extensions
{
    public class LumleSignInManager<TUser> : SignInManager<TUser> where TUser : class
    {
        //private readonly IMediator _mediator;

        public LumleSignInManager(UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger
            //IMediator mediator
            )
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
            //_mediator = mediator;
        }

        public override async Task SignInAsync(TUser user, bool isPersistent, string authenticationMethod = null)
        {
            var userId = await UserManager.GetUserIdAsync(user);
            //await _mediator.Publish(new UserSignedIn { UserId = userId });
            await base.SignInAsync(user, isPersistent, authenticationMethod);
        }

    }
}
