using System;
using System.Linq.Expressions;
using Lumle.Core.Models;
using System.Threading.Tasks;

namespace Lumle.Core.Services.Abstracts
{
    public interface IProfileService
    {
        Task<UserProfile> GetSingle(Expression<Func<UserProfile, bool>> predicate);
        Task Add(UserProfile entity);
        Task Update(UserProfile entity);
        Task DeleteWhere(Expression<Func<UserProfile, bool>> predicate);
    }
}
