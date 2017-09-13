using Lumle.Data.Data;
using Lumle.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace Lumle.Data.Extensions
{
    public class LumleRoleStore : RoleStore<Role, BaseContext, string, UserRole, IdentityRoleClaim<string>>
    {
        public LumleRoleStore(BaseContext context) : base(context)
        {
        }

        protected override IdentityRoleClaim<string> CreateRoleClaim(Role role, Claim claim)
        {
            return new IdentityRoleClaim<string> { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
        }
    }
}
