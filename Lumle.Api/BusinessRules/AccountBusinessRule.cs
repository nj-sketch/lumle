using System;
using System.Linq.Expressions;
using Lumle.Api.BusinessRules.Abstracts;
using Lumle.Api.Data.Abstracts;
using Lumle.Api.Data.Entities;
using Lumle.Api.Service.Services.Abstracts;
using Lumle.Api.ViewModels.Account;
using Lumle.Infrastructure.Utilities;

namespace Lumle.Api.BusinessRules
{
    public class AccountBusinessRule : IAccountBusinessRule
    {
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountBusinessRule(IAccountService accountService,
            IUnitOfWork unitOfWork)
        {
            _accountService = accountService;
            _unitOfWork = unitOfWork;
        }

        public void RegisterUser(SignupVM model)
        {
            try
            {
                var passwordSalt = CryptoService.GenerateSalt();
                var passwordHash = CryptoService.ComputeHash(model.Password, passwordSalt);

                var entity = AutoMapper.Mapper.Map<MobileUser>(model);

                entity.SubjectId = Guid.NewGuid().ToString();
                entity.PasswordSalt = Convert.ToBase64String(passwordSalt);
                entity.PasswordHash = Convert.ToBase64String(passwordHash);
                entity.IsStaff = false;
                entity.IsBlocked = false;
                entity.IsEmailVerified = false;
                entity.CreatedDate = DateTime.UtcNow;
                entity.LastUpdated = DateTime.UtcNow;
                entity.Provider = "application";

                _accountService.Add(entity);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsUserAvailable(Expression<Func<MobileUser, bool>> predicate)
        {
            try
            {
                var user = _accountService.GetSingle(predicate);
                return user != null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
