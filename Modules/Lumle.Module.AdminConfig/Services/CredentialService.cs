using System;
using System.Linq.Expressions;
using Lumle.Module.AdminConfig.Entities;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.AdminConfig.Models;
using System.Linq;
using Lumle.Infrastructure.Constants.LumleLog;
using NLog;
using System.Threading.Tasks;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Enums;
using Lumle.Data.Models;
using Lumle.Module.Audit.Services;

namespace Lumle.Module.AdminConfig.Services
{
    public class CredentialService : ICredentialService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<Credential> _credentialRepository;
        private readonly IAuditLogService _auditLogService;

        public CredentialService(IRepository<Credential> credentialRepository, IAuditLogService auditLogService)
        {
            _credentialRepository = credentialRepository;
            _auditLogService = auditLogService;
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

        public async Task<Credential> Update(CredentialModel model, User loggedUser)
        {
            try
            {
                var credential = await _credentialRepository.GetSingleAsync(x => x.Id == model.Id);

                #region Credential Audit Log
                // Add previous data in old record object for comparison
                var oldRecord = new Credential
                {
                    Id = credential.Id,
                    CredentialCategoryId = credential.CredentialCategoryId,
                    Slug = credential.Slug,
                    DisplayName = credential.DisplayName,
                    Value = credential.Value
                };

                // update in database
                credential.Value = credential.Value.Trim();
                await _credentialRepository.UpdateAsync(credential, credential.Id);

                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Update,
                    KeyField = oldRecord.Id.ToString(),
                    OldObject = oldRecord,
                    NewObject = credential,
                    LoggedUserEmail = loggedUser.Email,
                    ComparisonType = ComparisonType.ObjectCompare
                };

                await _auditLogService.Create(auditLogModel);

                #endregion

                return credential;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }
    }
}
