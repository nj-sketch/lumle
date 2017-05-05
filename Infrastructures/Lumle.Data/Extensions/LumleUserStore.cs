using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Lumle.Data.Models;
using Lumle.Data.Data;

namespace Lumle.Data.Extensions
{
    public class LumleUserStore : UserStore<User, Role, BaseContext, string, IdentityUserClaim<string>, UserRole,
        IdentityUserLogin<string>, IdentityUserToken<string>>
    {
        public LumleUserStore(BaseContext context) : base(context)
        {
        }

        protected override UserRole CreateUserRole(User user, Role role)
        {
            return new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
        }

        protected override IdentityUserClaim<string> CreateUserClaim(User user, Claim claim)
        {
            var userClaim = new IdentityUserClaim<string> { UserId = user.Id };
            userClaim.InitializeFromClaim(claim);
            return userClaim;
        }

        protected override IdentityUserLogin<string> CreateUserLogin(User user, UserLoginInfo login)
        {
            return new IdentityUserLogin<string>
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        protected override IdentityUserToken<string> CreateUserToken(User user, string loginProvider, string name, string value)
        {
            return new IdentityUserToken<string>
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }
    }
}
