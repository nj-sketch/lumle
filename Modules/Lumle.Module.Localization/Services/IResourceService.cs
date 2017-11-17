using Lumle.Module.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Core.Models;
using System.Threading.Tasks;
using Lumle.Data.Models;
using static Lumle.Infrastructure.Helpers.DataTableHelper;
using Lumle.Module.Localization.Helpers;

namespace Lumle.Module.Localization.Services
{
    public interface IResourceService
    {
        Task<Resource> GetSingle(Expression<Func<Resource, bool>> predicate);
        IEnumerable<ResourceModel> GetAllResource(string culture);
        IEnumerable<ResourceModel> GetAllResource(int resourceCategoryId, string culture);
        Boolean IsCultureContainKey(string culture, int resourceCategoryId, string key);
        Task<Resource> AddAsync(Resource entity);
        Task UpdateAsync(Resource entity);
        Task CreateResource(ResourceModel model, User loggedUser);       
        DTResult<ResourceModel> GetDataTableResult(CultureDTParameters parameters);
    }
}
