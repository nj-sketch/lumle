using Lumle.Module.AdminConfig.Entities;
using Lumle.Module.AdminConfig.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public interface ICredentialCategoryService
    {
        IQueryable<CredentialCategory> GetAll(Expression<Func<CredentialCategory, bool>> predicate);
        IQueryable<CredentialCategory> GetAll();
        Task<CredentialCategory> GetSingle(Expression<Func<CredentialCategory, bool>> predicate);
        IQueryable<CredentialCategoryModel> GetAllCredentialCategory();
    }
}
