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
        Task<IEnumerable<ResourceModel>> GetAllResource(string culture);
        Task<IEnumerable<ResourceModel>> GetAllResource(int resourceCategoryId, string culture);
        Boolean IsCultureContainKey(string culture, int resourceCategoryId, string key);
    }
}
