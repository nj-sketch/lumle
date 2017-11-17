using System;
using System.Linq.Expressions;
using Lumle.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Core.Services.Abstracts
{
    public interface ISystemSettingService
    {
        IQueryable<AppSystem> GetAll(Expression<Func<AppSystem, bool>> predicate);
        IQueryable<AppSystem> GetAll();
        Task<AppSystem> GetSingleAsync(Expression<Func<AppSystem, bool>> predicate);
        Task Update(AppSystem entity);
    }
}
