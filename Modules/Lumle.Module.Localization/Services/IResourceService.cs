using Lumle.Module.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Core.Models;

namespace Lumle.Module.Localization.Services
{
    public interface IResourceService
    {
        IEnumerable<Resource> GetAll(Expression<Func<Resource, bool>> predicate);
        IEnumerable<Resource> GetAll();
        Resource GetSingle(Expression<Func<Resource, bool>> predicate);
        void Add(Resource entity);
        void Update(Resource entity);
        void DeleteWhere(Expression<Func<Resource, bool>> predicate);
        IEnumerable<ResourceModel> GetAllResource(string culture);
        IEnumerable<ResourceModel> GetAllResource(int resourceCategoryId, string culture);

        bool IsCultureContainKey(string culture, int resourceCategoryId, string key);
    }
}
