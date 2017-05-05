using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Module.PublicUser.Entities;
using Lumle.Data.Data.Abstracts;
using NLog;
using Lumle.Infrastructure.Constants.LumleLog;

namespace Lumle.Module.PublicUser.Services
{
    public class PublicUserService : IPublicUserService
    {
        
        private readonly IRepository<CustomUser> _customUserRepository;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public PublicUserService(IRepository<CustomUser> customUserRepository)
        {
            _customUserRepository = customUserRepository;
        }
        public void Add(CustomUser entity)
        {
            try
            {
                _customUserRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<CustomUser, bool>> predicate)
        {
            try
            {
                _customUserRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DeleteError);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CustomUser> GetAll()
        {
            try
            {
                var publicUsers = _customUserRepository.GetAll();
                return publicUsers;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CustomUser> GetAll(Expression<Func<CustomUser, bool>> predicate)
        {
            try
            {
                var publicUsers = _customUserRepository.GetAll(predicate);
                return publicUsers;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public CustomUser GetSingle(Expression<Func<CustomUser, bool>> predicate)
        {
            try
            {
                var customUser = _customUserRepository.GetSingle(predicate);
                return customUser;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public void Update(CustomUser entity)
        {
            try
            {
                _customUserRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw new Exception(ex.Message);
            }
        }
    }
}
