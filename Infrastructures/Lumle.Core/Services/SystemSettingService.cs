using System;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Data.Abstracts;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Core.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly IRepository<AppSystem> _appSystemRepository;

        public SystemSettingService(IRepository<AppSystem> appSystemRepository)
        {
            _appSystemRepository = appSystemRepository;
        }

        public IQueryable<AppSystem> GetAll()
        {
            try
            {
                return _appSystemRepository.GetAll();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IQueryable<AppSystem> GetAll(Expression<Func<AppSystem, bool>> predicate)
        {
            try
            {
                return _appSystemRepository.GetAll(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AppSystem> GetSingleAsync(Expression<Func<AppSystem, bool>> predicate)
        {
            try
            {
                return await _appSystemRepository.GetSingleAsync(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Update(AppSystem entity)
        {
            try
            {
                await _appSystemRepository.UpdateAsync(entity, entity.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
