using System;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Data.Abstracts;
using System.Threading.Tasks;

namespace Lumle.Core.Services
{
    public class ApplicationTokenService : IApplicationTokenService
    {
        private readonly IRepository<ApplicationToken> _appTokenRepository;

        public ApplicationTokenService(IRepository<ApplicationToken> appTokenRepository)
        {
            _appTokenRepository = appTokenRepository;
        }

        public async Task<ApplicationToken> GetSingle(Expression<Func<ApplicationToken, bool>> predicate)
        {
            try
            {
                var article = await _appTokenRepository.GetSingle(predicate);
                return article;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Add(ApplicationToken entity)
        {
            try
            {
               await _appTokenRepository.Add(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Update(ApplicationToken entity)
        {
            try
            {
                await _appTokenRepository.Update(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteWhere(Expression<Func<ApplicationToken, bool>> predicate)
        {
            try
            {
                await _appTokenRepository.DeleteWhere(predicate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
    }
}
