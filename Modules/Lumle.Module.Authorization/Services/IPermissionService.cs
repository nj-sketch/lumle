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
        int Count();
        int Count(Expression<Func<Permission, bool>> predicate);
        IQueryable<Permission> GetAll(Expression<Func<Permission, bool>> predicate);
        IQueryable<Permission> GetAll();
        Task<Permission> GetSingle(Expression<Func<Permission, bool>> predicate);
        Task Add(Permission entity);
        Task Update(Permission entity);
        Task DeleteWhere(Expression<Func<Permission, bool>> predicate);
        IQueryable<Models.PermissionModels.Module> GetPermissionsIncludingAssigned(IEnumerable<BaseRoleClaim> roleClaims);
        List<User> GetAllUserOfRole(string roleId);
    }
}
