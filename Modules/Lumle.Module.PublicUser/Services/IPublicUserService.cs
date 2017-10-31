using Lumle.Module.PublicUser.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Module.PublicUser.Services
{
    public interface IPublicUserService
    {
        IQueryable<CustomUser> GetAll();
        IQueryable<CustomUser> GetAll(Expression<Func<CustomUser, bool>> predicate);
        Task<CustomUser> GetSingle(Expression<Func<CustomUser, bool>> predicate);
        Task Add(CustomUser entity);
        Task Update(CustomUser entity);
        Task DeleteWhere(Expression<Func<CustomUser, bool>> predicate);
    }
}
