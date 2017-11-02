using Lumle.Core.Services.Abstracts;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.Cache;
using Lumle.Module.Authorization.Services;
using Lumle.Module.Authorization.ViewModels.SideBarViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.CMS.Components
{
    public class SideBarViewComponent : ViewComponent
    {
        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly IPermissionService _permissionService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMemoryCache _memoryCache;

        public SideBarViewComponent
        (
            IBaseRoleClaimService baseRoleClaimService,
            IPermissionService permissionService,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IMemoryCache memoryCache
        )
        {           
            _baseRoleClaimService = baseRoleClaimService;
            _permissionService = permissionService;
            _userManager = userManager;
            _roleManager = roleManager;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var orderedMenuList = new List<SideBarMenuVM>();
            var user = await GetCurrentUserAsync();

            if (user == null) return View("Sidebar", orderedMenuList);

            var role = await _userManager.GetRolesAsync(user);

            if (!role.Any()) return View("Sidebar", orderedMenuList);

            // Get role associated with the user
            var roleModel = await _roleManager.FindByNameAsync(role.FirstOrDefault());

            if (_memoryCache.TryGetValue(CacheConstants.AuthorizationSidebarMenuCache + roleModel, out orderedMenuList)) return View("Sidebar",orderedMenuList);

            var roleClaims = _baseRoleClaimService.GetAll(x => x.RoleId == roleModel.Id).ToList();

            if (!roleClaims.Any()) return null;

            var filteredPermissions = (from e in roleClaims
                                       join f in _permissionService.GetAll() on e.ClaimValue equals f.Slug
                                       orderby f.Id
                                       group f by f.Menu
                                       into g
                                       select new
                                       {
                                           Menu = g.Key,
                                           SubMenu = (from h in g
                                                      where !string.IsNullOrEmpty(h.SubMenu)
                                                      group h by h.SubMenu
                                               into i
                                                      select new { subMenu = i.Key }).ToList()
                                       }).ToList();

           
            // Get default sidebar list
            var sideBar = new SideBarVM();
            var defaultMenuList = sideBar.SideBarItems;

            var menuList = new List<SideBarMenuVM>();

            foreach (var filteredMenu in filteredPermissions)
            {
                var menuInfo = defaultMenuList.FirstOrDefault(x => x.Name == filteredMenu.Menu && x.MenuLevel == 0);
                var menu = new SideBarMenuVM
                {
                    Menu = filteredMenu.Menu,
                    MenuDisplayName =
                        menuInfo?.DisplayName ??
                        filteredMenu.Menu,
                    Icon = menuInfo?.Icon ?? "",
                    Sequence = menuInfo?.Sequence ?? 1
                };

                var subMenuList = filteredMenu.SubMenu
                    .Select(filteredSubMenu => new SideBarSubMenuVM
                    {
                        SubMenu = filteredSubMenu.subMenu,
                        SubMenuDisplayName =
                            defaultMenuList.FirstOrDefault(x => x.Name == filteredSubMenu.subMenu && x.MenuLevel == 1)?.DisplayName ??
                            filteredSubMenu.subMenu,
                        Sequence = defaultMenuList.FirstOrDefault(x => x.Name == filteredSubMenu.subMenu && x.MenuLevel == 1)?.Sequence ?? 1
                    });

                menu.SubMenu = subMenuList.OrderBy(x => x.Sequence).ToList();
                menuList.Add(menu);
            }

            orderedMenuList = (from e in menuList orderby e.Sequence select e).ToList();

            var cacheOption = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.High
            };

            _memoryCache.Set(CacheConstants.AuthorizationSidebarMenuCache + roleModel, orderedMenuList, cacheOption);

            return View("Sidebar",orderedMenuList);
        }

        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
