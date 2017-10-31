using System;
using System.Linq.Expressions;
using Lumle.Module.Audit.Entities;
using System.Linq;

namespace Lumle.Module.Audit.Services
{
    public interface ICustomLogService
    {
        IQueryable<CustomLog> GetAll(Expression<Func<CustomLog, bool>> predicate);
        IQueryable<CustomLog> GetAll();
    }
}
