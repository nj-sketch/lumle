using System;
using System.Linq.Expressions;
using Lumle.Core.Models;
using Lumle.Core.Services.Abstracts;
using Lumle.Data.Data.Abstracts;

namespace Lumle.Core.Services
{
    public class ApplicationTokenService : IApplicationTokenService
    {
        private readonly IRepository<ApplicationToken> _appTokenRepository;

        public ApplicationTokenService(IRepository<ApplicationToken> appTokenRepository)
        {
            _appTokenRepository = appTokenRepository;
        }

        public ApplicationToken GetSingle(Expression<Func<ApplicationToken, bool>> predicate)
        {
            try
            {
                var article = _appTokenRepository.GetSingle(predicate);
                return article;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(ApplicationToken entity)
        {
            try
            {
                _appTokenRepository.Add(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(ApplicationToken entity)
        {
            try
            {
                _appTokenRepository.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<ApplicationToken, bool>> predicate)
        {
            try
            {
                _appTokenRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
    }
}
