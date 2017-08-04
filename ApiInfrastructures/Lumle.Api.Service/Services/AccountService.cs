using Lumle.Api.Service.Services.Abstracts;
using System;
using System.Collections.Generic;
using Lumle.Api.Data.Entities;
using System.Linq.Expressions;
using Lumle.Api.Data.Abstracts;

namespace Lumle.Api.Service.Services
{
    public class AccountService : IAccountService
    {


        private readonly IRepository<MobileUser> _mobileUserRepository;

        public AccountService(IRepository<MobileUser> mobileUserRepository)
        {
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

    }
}
