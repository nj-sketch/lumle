using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Lumle.Core.Models;
using System.Linq;

namespace Lumle.Core.Services.Abstracts
{
    public interface IBaseRoleClaimService
    {
        Task<BaseRoleClaim> AddAsync(BaseRoleClaim entity);
        void DeleteWhere(Expression<Func<BaseRoleClaim, bool>> predicate);
        IQueryable<BaseRoleClaim> GetAll(Expression<Func<BaseRoleClaim, bool>> predicate);
        bool IsClaimExist(Claim claim, string roleId);
        Task<Dictionary<string, bool>> GetActionPrevilegeAsync(Dictionary<string, Claim> claims, ClaimsPrincipal claimPrincipal);
    }
}
