using System;
using System.Linq.Expressions;
using Lumle.Api.Data.Entities;
using Lumle.Api.ViewModels.Account;

namespace Lumle.Api.BusinessRules.Abstracts
{
    public interface IAccountBusinessRule
    {

        void RegisterUser(SignupVM model);
        bool IsUserAvailable(Expression<Func<MobileUser, bool>> predicate);

    }
}
