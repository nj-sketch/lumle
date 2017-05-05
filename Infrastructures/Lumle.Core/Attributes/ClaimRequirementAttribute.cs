using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lumle.Core.Attributes
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class ClaimRequirementFilter : IAsyncActionFilter
    {
        private readonly Claim _claim;
        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly RoleManager<Role> _roleManager;

        public ClaimRequirementFilter(Claim claim, 
            IBaseRoleClaimService baseRoleClaimService, 
            RoleManager<Role> roleManager)
        {
            _claim = claim;
            _roleManager = roleManager;
            _baseRoleClaimService = baseRoleClaimService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var role = await _roleManager.FindByNameAsync(context.HttpContext.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Role).Value);
            var hasClaim = _baseRoleClaimService.IsClaimExist(_claim, role.Id);
            if (!hasClaim)
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            else
            await next();
        }
    }
}
