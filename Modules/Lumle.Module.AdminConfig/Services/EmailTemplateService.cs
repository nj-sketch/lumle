using System;
using System.Linq;
using System.Linq.Expressions;
using Lumle.Module.AdminConfig.Entities;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.AdminConfig.Models;
using NLog;
using System.Threading.Tasks;
using System.Collections.Generic;
using Lumle.Data.Models;
using Lumle.Module.AdminConfig.ViewModels;
using Lumle.Module.Audit.Models;
using Lumle.Module.Audit.Enums;
using Lumle.Module.Audit.Services;

namespace Lumle.Module.AdminConfig.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IAuditLogService _auditLogService;

        public EmailTemplateService(IRepository<EmailTemplate> emailTemplateRepository, IAuditLogService auditLogService)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _auditLogService = auditLogService;
        }

        public IQueryable<EmailTemplate> GetAll(Expression<Func<EmailTemplate, bool>> predicate)
        {
            try
            {
               return _emailTemplateRepository.GetAll(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IQueryable<EmailTemplate> GetAll()
        {
            try
            {
                return _emailTemplateRepository.GetAll();
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public IEnumerable<EmailTemplateModel> GetAllEmailTemplate()
        {
            try
            {
                var i = 0;
                var data = (from e in _emailTemplateRepository.GetAll()
                            select new EmailTemplateModel
                            {
                               Id=e.Id,
                               Slug=e.Slug,
                               SlugDisplayName=e.SlugDisplayName,
                               Subject=e.Subject
                            }).ToList();

                return data.Select(x => { x.Sn = ++i; return x; });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task<EmailTemplate> GetSingleAsync(Expression<Func<EmailTemplate, bool>> predicate)
        {
            try
            {
                return  await _emailTemplateRepository.GetSingleAsync(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task<EmailTemplate> Update(EmailTemplateVM model, User loggedUser)
        {
            try
            {
                var emailTemplate = await _emailTemplateRepository.GetSingleAsync(x => x.Id == model.Id);
                // Add previous data in old record object for comparison
                var oldRecord = new EmailTemplate
                {
                    Id = emailTemplate.Id,
                    Slug = emailTemplate.Slug,
                    SlugDisplayName = emailTemplate.SlugDisplayName,
                    Subject = emailTemplate.Subject,
                    Body = emailTemplate.Body,
                    DefaultBody = emailTemplate.DefaultBody,
                    DefaultSubject = emailTemplate.DefaultSubject,
                    LastSlugDisplayName = emailTemplate.LastSlugDisplayName,
                    LastSubject = emailTemplate.LastSubject,
                    LastBody = emailTemplate.LastBody
                };

                // update in database
                emailTemplate.LastSlugDisplayName = emailTemplate.LastSlugDisplayName;
                emailTemplate.LastSubject = emailTemplate.Subject;
                emailTemplate.LastBody = emailTemplate.Body;

                emailTemplate.SlugDisplayName = model.SlugDisplayName;
                emailTemplate.Subject = model.Subject;
                emailTemplate.Body = model.Body;

                await _emailTemplateRepository.UpdateAsync(emailTemplate, emailTemplate.Id);

                #region EmailTemplate Audit Log

                var auditLogModel = new AuditLogModel
                {
                    AuditActionType = AuditActionType.Update,
                    KeyField = oldRecord.Id.ToString(),
                    OldObject = oldRecord,
                    NewObject = emailTemplate,
                    LoggedUserEmail = loggedUser.Email,
                    ComparisonType = ComparisonType.ObjectCompare
                };

                await _auditLogService.Create(auditLogModel);
                #endregion

                return emailTemplate;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }
    }
}
