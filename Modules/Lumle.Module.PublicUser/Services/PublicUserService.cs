using System;
using System.Linq.Expressions;
using Lumle.Module.PublicUser.Entities;
using Lumle.Data.Data.Abstracts;
using NLog;
using Lumle.Infrastructure.Constants.LumleLog;
using System.Threading.Tasks;
using System.Linq;

namespace Lumle.Module.PublicUser.Services
{
    public class PublicUserService : IPublicUserService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<CustomUser> _customUserRepository;

        public PublicUserService(IRepository<CustomUser> customUserRepository)
        {
            _customUserRepository = customUserRepository;
        }

        public async Task Add(CustomUser entity)
        {
            try
            {
                await _customUserRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        public async Task DeleteWhere(Expression<Func<CustomUser, bool>> predicate)
        {
            try
            {
                await _customUserRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DeleteError);
                throw;
            }
        }

        public IQueryable<CustomUser> GetAll()
        {
            try
            {
                var publicUsers = _customUserRepository.GetAll();
                return publicUsers;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IQueryable<CustomUser> GetAll(Expression<Func<CustomUser, bool>> predicate)
        {
            try
            {
                var publicUsers = _customUserRepository.GetAll(predicate);
                return publicUsers;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task<CustomUser> GetSingle(Expression<Func<CustomUser, bool>> predicate)
        {
            try
            {
                var customUser = await _customUserRepository.GetSingle(predicate);
                return customUser;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task Update(CustomUser entity)
        {
            try
            {
                await _customUserRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }
    }
}
