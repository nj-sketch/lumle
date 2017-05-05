using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Data.Abstracts;

namespace Lumle.Core.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly IRepository<AppSystem> _appSystemRepository;

        public SystemSettingService(IRepository<AppSystem> appSystemRepository)
        {
            _appSystemRepository = appSystemRepository;
        }
        public IEnumerable<AppSystem> GetAll()
        {
            try
            {
                return _appSystemRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<AppSystem> GetAll(Expression<Func<AppSystem, bool>> predicate)
        {
            try
            {
                return _appSystemRepository.GetAll(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public AppSystem GetSingle(Expression<Func<AppSystem, bool>> predicate)
        {
            try
            {
                return _appSystemRepository.GetSingle(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(AppSystem entity)
        {
            try
            {
                _appSystemRepository.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
