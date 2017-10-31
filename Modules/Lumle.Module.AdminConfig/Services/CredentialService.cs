using System;
using System.Linq.Expressions;
using Lumle.Module.AdminConfig.Entities;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.AdminConfig.Models;
using System.Linq;
using Lumle.Infrastructure.Constants.LumleLog;
using NLog;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public class CredentialService : ICredentialService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<Credential> _credentialRepository;

        public CredentialService(IRepository<Credential> credentialRepository)
        {
            _credentialRepository = credentialRepository;
        }

        public IQueryable<Credential> GetAll()
        {
            try
            {
                return _credentialRepository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IQueryable<Credential> GetAll(Expression<Func<Credential, bool>> predicate)
        {
            try
            {
                return _credentialRepository.GetAll(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IQueryable<CredentialModel> GetAllCredential(Expression<Func<Credential, bool>> predicate)
        {
            try
            {
                var i = 0;
                var credential = (from e in _credentialRepository.GetAll(predicate)
                                  select new CredentialModel
                                  {
                                      Id = e.Id,
                                      DisplayName = e.DisplayName,
                                      Value = e.Value
                                  }).ToList();

                return credential.Select(x =>
                {
                    x.Sn = ++i; return x;
                }).AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task<Credential> GetSingle(Expression<Func<Credential, bool>> predicate)
        {
            try
            {
                return await _credentialRepository.GetSingle(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task Update(Credential entity)
        {
            try
            {
                await _credentialRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }
    }
}
