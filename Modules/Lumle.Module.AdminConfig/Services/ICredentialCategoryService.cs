using Lumle.Module.AdminConfig.Entities;
using Lumle.Module.AdminConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lumle.Module.AdminConfig.Services
{
    public interface ICredentialCategoryService
    {
        IEnumerable<CredentialCategory> GetAll(Expression<Func<CredentialCategory, bool>> predicate);
        IEnumerable<CredentialCategory> GetAll();
        CredentialCategory GetSingle(Expression<Func<CredentialCategory, bool>> predicate);
        IEnumerable<CredentialCategoryModel> GetAllCredentialCategory();
    }
}
