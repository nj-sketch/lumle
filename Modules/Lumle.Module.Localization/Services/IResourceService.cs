using Lumle.Module.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.Localization.Services
{
    public interface IResourceService
    {
        IQueryable<Resource> GetAll(Expression<Func<Resource, bool>> predicate);
        IQueryable<Resource> GetAll();
        Task<Resource> GetSingle(Expression<Func<Resource, bool>> predicate);
        Task Add(Resource entity);
        Task Update(Resource entity);
        Task DeleteWhere(Expression<Func<Resource, bool>> predicate);
        IEnumerable<ResourceModel> GetAllResource(string culture);
        IEnumerable<ResourceModel> GetAllResource(int resourceCategoryId, string culture);

        bool IsCultureContainKey(string culture, int resourceCategoryId, string key);
    }
}
