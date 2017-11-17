using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;

namespace Lumle.Core.Services
{
    public class BaseRoleClaimService : IBaseRoleClaimService
    {
        private readonly IRepository<BaseRoleClaim> _baseRoleClaimRepository;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public BaseRoleClaimService(
            IRepository<BaseRoleClaim> baseRoleClaimRepository,
            RoleManager<Role> roleManager,
            UserManager<User> userManager
        )
        {
            _baseRoleClaimRepository = baseRoleClaimRepository;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IQueryable<BaseRoleClaim> GetAll(Expression<Func<BaseRoleClaim, bool>> predicate)
        {
            try
            {
                var claims = _baseRoleClaimRepository.GetAll(predicate);
                return claims;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteWhere(Expression<Func<BaseRoleClaim, bool>> predicate)
        {
            try
            {
                 _baseRoleClaimRepository.DeleteWhere(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsClaimExist(Claim claim, string roleId)
        {
            try
            {
                var claims = _baseRoleClaimRepository.GetAll().ToList();

                var roleClaim = claims.FirstOrDefault(x => x.RoleId == roleId &&
                                                                   x.ClaimType == claim.Type &&
                                                                   x.ClaimValue == claim.Value);

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

        private Boolean CheckRoleClaimForAction(List<BaseRoleClaim> baseRoleClaims, Claim claim, string roleId)
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

        public async Task<BaseRoleClaim> AddAsync(BaseRoleClaim entity)
        {
            try
            {
                return await _baseRoleClaimRepository.AddAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
