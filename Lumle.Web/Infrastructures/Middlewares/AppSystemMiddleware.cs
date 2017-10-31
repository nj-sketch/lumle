using System.Linq;
using System.Threading.Tasks;
using Lumle.Core.Services.Abstracts;
using Microsoft.AspNetCore.Http;
using Lumle.Infrastructure.Constants.SystemSetting;
using Microsoft.AspNetCore.Identity;
using Lumle.Data.Models;

namespace Lumle.Web.Infrastructures.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AppSystemMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserManager<User> _userManager;

        public AppSystemMiddleware(RequestDelegate next, UserManager<User> userManager)
        {
            _next = next;
            _userManager = userManager;
        }

        public async Task Invoke(HttpContext httpContext, RoleManager<Role> roleManager, UserManager<User> userManager, ISystemSettingService systemSettingService, SignInManager<User> signManager)

        {
            var user = await GetCurrentUserAsync(httpContext);
            if (user != null)
            {
                var systemMode = await systemSettingService.GetSingle(x => x.Slug == SystemSetting.MaintenanceMode);
                if (systemMode != null && systemMode.Status == SystemSetting.MaintenanceModeOn)
                {
                    var userRole = await userManager.GetRolesAsync(user);
                   
                    var role = userRole.Any() ? await roleManager.FindByNameAsync(userRole.First()) : null;
                    if (role != null)
                    {
                        if (role.IsBlocked)
                        {
                           await signManager.SignOutAsync();
                            httpContext.Response.Redirect("./maintenancemode", true);
                            return;
                        }
                    }
                }
            }  
                await _next.Invoke(httpContext);
        }

        #region Helpers
        private async Task<User> GetCurrentUserAsync(HttpContext httpContext)
        {

            return await _userManager.GetUserAsync(httpContext.User);
        }
        #endregion
    }

  
}


