using Lumle.Module.PublicUser.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lumle.Module.PublicUser.Services
{
    public interface IPublicUserService
    {
        IEnumerable<CustomUser> GetAll();
        IEnumerable<CustomUser> GetAll(Expression<Func<CustomUser, bool>> predicate);
        CustomUser GetSingle(Expression<Func<CustomUser, bool>> predicate);
        void Add(CustomUser entity);
        void Update(CustomUser entity);
        void DeleteWhere(Expression<Func<CustomUser, bool>> predicate);
    }
}
