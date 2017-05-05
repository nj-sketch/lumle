using System;
using System.Linq.Expressions;
using Lumle.Core.Models;

namespace Lumle.Core.Services.Abstracts
{
    public interface IProfileService
    {
        UserProfile GetSingle(Expression<Func<UserProfile, bool>> predicate);
        void Add(UserProfile entity);
        void Update(UserProfile entity);
        void DeleteWhere(Expression<Func<UserProfile, bool>> predicate);
    }
}
