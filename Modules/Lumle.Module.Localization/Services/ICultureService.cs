using Lumle.Module.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Module.Localization.ViewModels;

namespace Lumle.Module.Localization.Services
{
    public interface ICultureService
    {
        IEnumerable<Culture> GetAll(Expression<Func<Culture, bool>> predicate);
        IEnumerable<Culture> GetAll();
        Culture GetSingle(Expression<Func<Culture, bool>> predicate);
        void Add(Culture entity);
        void Update(Culture entity);
        void DeleteWhere(Expression<Func<Culture, bool>> predicate);
        IEnumerable<CultureModel> GetAllCulture();
        CultureSelectListVM GetAllActiveCultures();
    }
}
