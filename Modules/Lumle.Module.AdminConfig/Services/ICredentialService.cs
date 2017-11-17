using Lumle.Data.Models;
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
        Task<Credential> Update(CredentialModel model, User loggedUser);
        IQueryable<CredentialModel> GetAllCredential(Expression<Func<Credential, bool>> predicate);
    }
}
