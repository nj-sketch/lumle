using System;
using System.Linq.Expressions;
using Lumle.Core.Models;
using System.Threading.Tasks;

namespace Lumle.Core.Services.Abstracts
{
    public interface IApplicationTokenService
    {
        Task<ApplicationToken> GetSingle(Expression<Func<ApplicationToken, bool>> predicate);
        Task Add(ApplicationToken entity);
        Task Update(ApplicationToken entity);
        Task DeleteWhere(Expression<Func<ApplicationToken, bool>> predicate);
    }
}
