using Lumle.Module.AdminConfig.Entities;
using Lumle.Module.AdminConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lumle.Module.AdminConfig.Services
{
    public interface ICredentialService
    {
        IEnumerable<Credential> GetAll(Expression<Func<Credential, bool>> predicate);
        IEnumerable<Credential> GetAll();
        Credential GetSingle(Expression<Func<Credential, bool>> predicate);
        void Update(Credential entity);
        IEnumerable<CredentialModel> GetAllCredential(Expression<Func<Credential, bool>> predicate);
    }
}
