using Lumle.Data.Models;
using Lumle.Module.Authorization.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.Authorization.Services
{
    public interface IPermissionService
    {
        IQueryable<Permission> GetAll(Expression<Func<Permission, bool>> predicate);
        IQueryable<Permission> GetAll();
        Task<Permission> GetSingleAsync(Expression<Func<Permission, bool>> predicate);
        IQueryable<Models.PermissionModels.Module> GetPermissionsIncludingAssigned(IEnumerable<BaseRoleClaim> roleClaims);
        List<User> GetAllUserOfRole(string roleId);
    }
}
