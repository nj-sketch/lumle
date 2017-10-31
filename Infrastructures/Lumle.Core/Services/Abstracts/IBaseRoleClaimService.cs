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
        int Count(Expression<Func<BaseRoleClaim, bool>> predicate);
        IQueryable<BaseRoleClaim> GetAll(Expression<Func<BaseRoleClaim, bool>> predicate);
        IQueryable<BaseRoleClaim> GetAll();
        Task<BaseRoleClaim> GetSingle(Expression<Func<BaseRoleClaim, bool>> predicate);
        Task Add(BaseRoleClaim entity);
        Task Update(BaseRoleClaim entity);
        Task DeleteWhere(Expression<Func<BaseRoleClaim, bool>> predicate);
        bool IsClaimExist(Claim claim, string roleId);
        Task<Dictionary<string, bool>> GetActionPrevilegeAsync(Dictionary<string, Claim> claims, ClaimsPrincipal claimPrincipal);
    }
}
