using System;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.Log;
using NLog;

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
        public UserProfile GetSingle(Expression<Func<UserProfile, bool>> predicate)
        {
            try
            {
                var userProfile = _profileRepository.GetSingle(predicate);
                return userProfile;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public void Add(UserProfile entity)
        {
            try
            {
                _profileRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw new Exception(ex.Message);
            }
        }

        public void Update(UserProfile entity)
        {
            try
            {
                _profileRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<UserProfile, bool>> predicate)
        {
            try
            {
                _profileRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DeleteError);
                throw new Exception(ex.Message);
            }
        }
    }
}
