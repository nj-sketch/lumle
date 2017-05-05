using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Core.Models;

namespace Lumle.Core.Services.Abstracts
{
    public interface ISystemSettingService
    {
        IEnumerable<AppSystem> GetAll(Expression<Func<AppSystem, bool>> predicate);
        IEnumerable<AppSystem> GetAll();
        AppSystem GetSingle(Expression<Func<AppSystem, bool>> predicate);
        void Update(AppSystem entity);
    }
}
