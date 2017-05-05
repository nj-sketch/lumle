using Lumle.Data.Data.Abstracts;
using Lumle.Module.Authorization.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Module.Authorization.Models;
using System.Threading.Tasks;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Data;
using Lumle.Module.Authorization.Models.PermissionModels;
using Microsoft.Extensions.Caching.Memory;
using Lumle.Data.Models;
using Lumle.Infrastructure.Constants.LumleLog;
using NLog;
using Lumle.Infrastructure.Constants.Cache;

namespace Lumle.Module.Authorization.Services
{
    public class PermissionService : IPermissionService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IBaseRoleClaimService _baseRoleClaimService;
        private readonly BaseContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMemoryCache _memoryCache;

        public PermissionService(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IRepository<Permission> permissionRepository,
            IBaseRoleClaimService baseRoleClaimService,
            BaseContext context,
            IMemoryCache memoryCache)
        {
            _permissionRepository = permissionRepository;
            _baseRoleClaimService = baseRoleClaimService;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _memoryCache = memoryCache;
        }

        public int Count()
        {
            try
            {
                var permissionCount = _permissionRepository.Count();

                return permissionCount;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public int Count(Expression<Func<Permission, bool>> predicate)
        {
            try
            {
                var permissionCount = _permissionRepository.Count(predicate);
                return permissionCount;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Permission> GetAll(Expression<Func<Permission, bool>> predicate)
        {
            try
            {
                var permissions = _permissionRepository.GetAll(predicate);
                return permissions;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Permission> GetAll()
        {
            try
            {
                var permissions = _permissionRepository.GetAll();
                return permissions;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public Permission GetSingle(Expression<Func<Permission, bool>> predicate)
        {
            try
            {
                var permission = _permissionRepository.GetSingle(predicate);
                return permission;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public void Add(Permission entity)
        {
            try
            {
                _permissionRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw new Exception(ex.Message);
            }
        }

        public void Update(Permission entity)
        {
            try
            {
                _permissionRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<Permission, bool>> predicate)
        {
            try
            {
                _permissionRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DeleteError);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<SidebarMenuModel>> GetSideBarMenuAsync(User user)
        {
            List<SidebarMenuModel> menuList;

            if (_memoryCache.TryGetValue(CacheConstants.AuthorizationSidebarMenuCache, out menuList)) return menuList;

            if (user == null) return null;

            var role = await _userManager.GetRolesAsync(user);
            if (!role.Any()) return null;

            var roleModel = await _roleManager.FindByNameAsync(role.FirstOrDefault());

            var roleClaims = _baseRoleClaimService.GetAll(x => x.RoleId == roleModel.Id).ToList();
            //var roleClaims = await _roleManager.GetClaimsAsync(roleModel) as List<Claim>; //Use this if default roleclaim table is used
            if (!roleClaims.Any()) return null;

            var filteredPermissions = (from e in roleClaims
                                       join f in _permissionRepository.GetAll() on e.ClaimValue equals f.Slug
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


            var defaultMenuList = GetSideBarMenuDetails();
            menuList = new List<SidebarMenuModel>();

            foreach (var filteredMenu in filteredPermissions)
            {
                var menu = new SidebarMenuModel
                {
                    Menu = filteredMenu.Menu,
                    MenuDisplayName =
                        defaultMenuList.FirstOrDefault(x => x.Name == filteredMenu.Menu)?.DisplayName ??
                        filteredMenu.Menu,
                    Icon = defaultMenuList.FirstOrDefault(x => x.Name == filteredMenu.Menu)?.Icon ?? ""
                };

                var subMenuList = filteredMenu.SubMenu
                    .Select(filteredSubMenu => new SubMenuModel
                    {
                        SubMenu = filteredSubMenu.subMenu,
                        SubMenuDisplayName =
                            defaultMenuList.FirstOrDefault(x => x.Name == filteredSubMenu.subMenu)?.DisplayName ??
                            filteredSubMenu.subMenu,
                    }).ToList();

                menu.SubMenu = subMenuList;
                menuList.Add(menu);
            }

            var cacheOption = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.High
            };

            _memoryCache.Set(CacheConstants.AuthorizationSidebarMenuCache, menuList, cacheOption);

            return menuList;
        }

        private static List<SidebarMenuDetailsModel> GetSideBarMenuDetails()
        {
            return new List<SidebarMenuDetailsModel>
            {
                new SidebarMenuDetailsModel
                {
                    Name = "dashboard",
                    DisplayName = "Dashboard",
                    Icon = "glyph-icon icon-linecons-tv"
                },
                new SidebarMenuDetailsModel {Name = "blog", DisplayName = "Blog", Icon = "glyph-icon icon-rss"},
                new SidebarMenuDetailsModel {Name = "article", DisplayName = "Article", Icon = ""},
                new SidebarMenuDetailsModel
                {
                    Name = "authorization",
                    DisplayName = "Authorization",
                    Icon = "glyph-icon icon-unlock"
                },
                new SidebarMenuDetailsModel {Name = "permission", DisplayName = "Permission", Icon = ""},
                new SidebarMenuDetailsModel {Name = "role", DisplayName = "Role", Icon = ""},
                new SidebarMenuDetailsModel {Name = "user", DisplayName = "User", Icon = ""},
                new SidebarMenuDetailsModel
                {
                    Name = "localization",
                    DisplayName = "Localization",
                    Icon = "glyph-icon icon-language"
                },
                new SidebarMenuDetailsModel {Name = "culture", DisplayName = "Culture", Icon = ""},
                new SidebarMenuDetailsModel
                {
                    Name = "audit",
                    DisplayName = "Audit",
                    Icon = "glyph-icon icon-database"
                },
                new SidebarMenuDetailsModel {Name = "auditlog", DisplayName = "Audit Log", Icon = ""},
                new SidebarMenuDetailsModel {Name = "customlog", DisplayName = "Custom Log", Icon = ""},
                new SidebarMenuDetailsModel
                {
                    Name = "calendar",
                    DisplayName = "Calendar",
                    Icon = "glyph-icon icon-calendar"
                },
                new SidebarMenuDetailsModel {Name = "event", DisplayName = "Event", Icon = ""},
                new SidebarMenuDetailsModel
                {
                    Name = "adminconfig",
                    DisplayName = "Configuration",
                    Icon = "glyph-icon icon-gear"
                },
                new SidebarMenuDetailsModel {Name = "emailtemplate", DisplayName = "Email Template", Icon = ""},
                new SidebarMenuDetailsModel {Name = "credential", DisplayName = "Credential", Icon = ""},
                new SidebarMenuDetailsModel {Name = "systemsetting", DisplayName = "System Setting", Icon = ""},
                new SidebarMenuDetailsModel {Name = "systemhealth", DisplayName = "System Health", Icon = ""},
                new SidebarMenuDetailsModel {Name = "publicuser", DisplayName = "Public User", Icon = "glyph-icon icon-group"}
            };
        }

        public IEnumerable<Models.PermissionModels.Module> GetPermissionsIncludingAssigned(IEnumerable<BaseRoleClaim> roleClaims)
        {
            var rolePermissions = GetAll();


            var modules = from p in rolePermissions
                          orderby p.Id
                          group p by p.Menu
                          into o
                          select new Models.PermissionModels.Module
                          {
                              Name = o.Key,
                              SubModules = from j in o
                                           group j by j.SubMenu
                                  into k
                                           select
                                           new SubModule
                                           {
                                               Name = k.Key,
                                               ModulePermissions = k.ToList()
                                                   .Select(x =>
                                                       new ModulePermission
                                                       {
                                                           Id = x.Id,
                                                           Slug = x.Slug,
                                                           DisplayName = x.DisplayName,
                                                           IsAssigned = roleClaims.Select(h => h.ClaimValue).ToList().Contains(x.Slug)
                                                       })
                                           }
                          };

            return modules;
        }

        public List<User> GetAllUserOfRole(string roleId)
        {
            try
            {
                var users = _context.UserRoles.Where(x => x.RoleId == roleId).Select(x => x.User).ToList();
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
