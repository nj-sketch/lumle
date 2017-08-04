using Lumle.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lumle.Api.Service.Services.Abstracts
{
    public interface IAccountService
    {

        #region Basic CRUDs
        IEnumerable<MobileUser> GetAll(Expression<Func<MobileUser, bool>> predicate);
        IEnumerable<MobileUser> GetAll();
        IEnumerable<MobileUser> GetAll(int count);
        MobileUser GetSingle(Expression<Func<MobileUser, bool>> predicate);
        void Add(MobileUser entity);
        void Update(MobileUser entity);
        void DeleteWhere(Expression<Func<MobileUser, bool>> predicate);
        #endregion
        
    }
}
