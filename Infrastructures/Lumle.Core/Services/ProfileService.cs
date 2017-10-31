using System;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using NLog;
using System.Threading.Tasks;

namespace Lumle.Core.Services
{
    public class ProfileService : IProfileService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<UserProfile> _profileRepository;

        public ProfileService(IRepository<UserProfile> profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<UserProfile> GetSingle(Expression<Func<UserProfile, bool>> predicate)
        {
            try
            {
                var userProfile = await _profileRepository.GetSingle(predicate);
                return userProfile;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task Add(UserProfile entity)
        {
            try
            {
                await _profileRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        public async Task Update(UserProfile entity)
        {
            try
            {
                await _profileRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }

        public async Task DeleteWhere(Expression<Func<UserProfile, bool>> predicate)
        {
            try
            {
                await _profileRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DeleteError);
                throw;
            }
        }
    }
}
