using System;
using System.Linq.Expressions;
using Lumle.Core.Models;

namespace Lumle.Core.Services.Abstracts
{
    public interface IApplicationTokenService
    {
        ApplicationToken GetSingle(Expression<Func<ApplicationToken, bool>> predicate);
        void Add(ApplicationToken entity);
        void Update(ApplicationToken entity);
        void DeleteWhere(Expression<Func<ApplicationToken, bool>> predicate);
    }
}
