using Lumle.Api.Service.Services.Abstracts;
using System;
using System.Collections.Generic;
using Lumle.Api.Data.Entities;
using System.Linq.Expressions;
using Lumle.Api.Data.Abstracts;
using Lumle.Infrastructure.Utilities;

namespace Lumle.Api.Service.Services
{
    public class AccountService : IAccountService
    {


        private readonly IRepository<MobileUser> _mobileUserRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IRepository<MobileUser> mobileUserRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mobileUserRepository = mobileUserRepository;
        }

        #region Basic CRUD
        public void Add(MobileUser entity)
        {
            try
            {
                _mobileUserRepository.Add(entity);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<MobileUser, bool>> predicate)
        {
            try
            {
                _mobileUserRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<MobileUser> GetAll(Expression<Func<MobileUser, bool>> predicate)
        {
            try
            {
                var users = _mobileUserRepository.GetAll(predicate);
                return users;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<MobileUser> GetAll()
        {
            try
            {
                var users = _mobileUserRepository.GetAll();
                return users;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<MobileUser> GetAll(int count)
        {
            try
            {
                var users = _mobileUserRepository.GetAll(count);
                return users;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public MobileUser GetSingle(Expression<Func<MobileUser, bool>> predicate)
        {
            try
            {
                var user = _mobileUserRepository.GetSingle(predicate);
                return user;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        
        public void Update(MobileUser entity)
        {
            try
            {
                _mobileUserRepository.Update(entity);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        #endregion


        #region Business Layers

        public bool IsUserAvailable(Expression<Func<MobileUser, bool>> predicate)
        {
            try
            {
                var user = GetSingle(predicate);

                return user != null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CreateSignupUser(MobileUser entity, string password)
        {
            try
            {
                var passwordSalt = CryptoService.GenerateSalt();
                var passwordHash = CryptoService.ComputeHash(password, passwordSalt);

                entity.SubjectId = Guid.NewGuid().ToString();
                entity.PasswordSalt = Convert.ToBase64String(passwordSalt);
                entity.PasswordHash = Convert.ToBase64String(passwordHash);
                entity.IsStaff = false;
                entity.IsBlocked = false;
                entity.IsEmailVerified = false;
                entity.CreatedDate = DateTime.UtcNow;
                entity.LastUpdated = DateTime.UtcNow;
                entity.Provider = "application";

                Add(entity);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }


        #endregion


    }
}
