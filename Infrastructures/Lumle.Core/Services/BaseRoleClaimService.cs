using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Infrastructure.Constants.Cache;

namespace Lumle.Core.Services
{
    public class BaseRoleClaimService : IBaseRoleClaimService
    {
        private readonly IRepository<BaseRoleClaim> _baseRoleClaimRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public BaseRoleClaimService(
            IRepository<BaseRoleClaim> baseRoleClaimRepository,
            IMemoryCache memoryCache,
            RoleManager<Role> roleManager,
            UserManager<User> userManager
        )
        {
            _baseRoleClaimRepository = baseRoleClaimRepository;
            _memoryCache = memoryCache;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public int Count(Expression<Func<BaseRoleClaim, bool>> predicate)
        {
            try
            {
                var claims = _baseRoleClaimRepository.Count(predicate);
                return claims;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<BaseRoleClaim> GetAll(Expression<Func<BaseRoleClaim, bool>> predicate)
        {
            try
            {
                var claims = _baseRoleClaimRepository.GetAll(predicate);
                return claims;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<BaseRoleClaim> GetAll()
        {
            try
            {
                var claims = _baseRoleClaimRepository.GetAll();
                return claims;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public BaseRoleClaim GetSingle(Expression<Func<BaseRoleClaim, bool>> predicate)
        {
            try
            {
                var claim = _baseRoleClaimRepository.GetSingle(predicate);
                return claim;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(BaseRoleClaim entity)
        {
            try
            {
                _baseRoleClaimRepository.Add(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(BaseRoleClaim entity)
        {
            try
            {
                _baseRoleClaimRepository.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<BaseRoleClaim, bool>> predicate)
        {
            try
            {
                _baseRoleClaimRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsClaimExist(Claim claim, string roleId)
        {
            try
            {
                BaseRoleClaim roleClaim;

                if (_memoryCache.TryGetValue(CacheConstants.AuthorizationApplicationClaimsCache, out List<BaseRoleClaim> claims))
                {
                     roleClaim= claims.FirstOrDefault(x => x.RoleId == roleId &&
                                                                         x.ClaimType == claim.Type &&
                                                                         x.ClaimValue == claim.Value);
                }
                else
                {
                    var cacheOption = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High
                    };

                     claims = _baseRoleClaimRepository.GetAll().ToList();
                    _memoryCache.Set(CacheConstants.AuthorizationApplicationClaimsCache, claims, cacheOption);

                     roleClaim = claims.FirstOrDefault(x => x.RoleId == roleId &&
                                                                          x.ClaimType == claim.Type &&
                                                                          x.ClaimValue == claim.Value);
                   
                }

                return roleClaim != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsClaimExist(Claim claim, ClaimsPrincipal claimPrinciple)
        {
            try
            {
                var user = await _userManager.GetUserAsync(claimPrinciple);
                var roles = await _userManager.GetRolesAsync(user);
                var roleModel = await _roleManager.FindByNameAsync(roles.FirstOrDefault());

                var roleId = await _roleManager.GetRoleIdAsync(roleModel);

                BaseRoleClaim roleClaim;

                if (_memoryCache.TryGetValue(CacheConstants.AuthorizationApplicationClaimsCache, out List<BaseRoleClaim> claims))
                {
                    roleClaim = claims.FirstOrDefault(x => x.RoleId == roleId &&
                                                                         x.ClaimType == claim.Type &&
                                                                         x.ClaimValue == claim.Value);
                }
                else
                {
                    var cacheOption = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High
                    };

                    claims = _baseRoleClaimRepository.GetAll().ToList();
                    _memoryCache.Set(CacheConstants.AuthorizationApplicationClaimsCache, claims, cacheOption);

                    roleClaim = claims.FirstOrDefault(x => x.RoleId == roleId &&
                                                                         x.ClaimType == claim.Type &&
                                                                         x.ClaimValue == claim.Value);

                }

                return roleClaim != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Dictionary<string, bool>> GetActionPrevilegeAsync(Dictionary<string, Claim> claims, ClaimsPrincipal claimPrincipal)
        {
            try
            {
                // Get current user thorough claims
                var user = await _userManager.GetUserAsync(claimPrincipal);
                
                // Get role assigned to user
                var userRole = await _userManager.GetRolesAsync(user);

                var roleModel = await _roleManager.FindByNameAsync(userRole.FirstOrDefault());

                var roleId = await _roleManager.GetRoleIdAsync(roleModel);

                var roleClaims = _baseRoleClaimRepository.GetAll().ToList();
                // dictionary that contains return value
                var map = new Dictionary<string, bool>();

                // Check for the action previleges
                foreach(var pair in claims)
                {
                    var actionClaim = pair.Value;
                    map.Add(pair.Key, CheckRoleClaimForAction(roleClaims, actionClaim, roleId));
                }

                return map;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CheckRoleClaimForAction(List<BaseRoleClaim> baseRoleClaims, Claim claim, string roleId)
        {
            bool flag = false;
            var roleClaim = baseRoleClaims.FirstOrDefault(x => x.RoleId == roleId &&
                                                                     x.ClaimType == claim.Type &&
                                                                     x.ClaimValue == claim.Value);
            if(roleClaim != null)
            {
                flag = true;
            }

            return flag;
        }
    }
}
