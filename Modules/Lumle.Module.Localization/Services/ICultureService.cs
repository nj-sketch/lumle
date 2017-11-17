using Lumle.Module.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Module.Localization.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Lumle.Data.Models;

namespace Lumle.Module.Localization.Services
{
    public interface ICultureService
    {
        IQueryable<Culture> GetAll(Expression<Func<Culture, bool>> predicate);
        Task<Culture> GetSingle(Expression<Func<Culture, bool>> predicate);
        IEnumerable<CultureModel> GetAllCulture();
        CultureSelectListVM GetAllActiveCultures();
        Task Update(Culture entity);
        Task CreateResource(User loggedUser, Culture selectedCulture);
    }
}
