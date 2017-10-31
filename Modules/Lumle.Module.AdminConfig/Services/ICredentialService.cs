using Lumle.Module.AdminConfig.Entities;
using Lumle.Module.AdminConfig.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public interface ICredentialService
    {
        IQueryable<Credential> GetAll(Expression<Func<Credential, bool>> predicate);
        IQueryable<Credential> GetAll();
        Task<Credential> GetSingle(Expression<Func<Credential, bool>> predicate);
        Task Update(Credential entity);
        IQueryable<CredentialModel> GetAllCredential(Expression<Func<Credential, bool>> predicate);
    }
}
