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

namespace Lumle.Module.AdminConfig.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;

        public EmailTemplateService(IRepository<EmailTemplate> emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public async Task Add(EmailTemplate entity)
        {
            try
            {
                await _emailTemplateRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

        public IQueryable<EmailTemplate> GetAll()
        {
            try
            {
              return  _emailTemplateRepository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
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

        public async Task<EmailTemplate> GetSingle(Expression<Func<EmailTemplate, bool>> predicate)
        {
            try
            {
              return  await _emailTemplateRepository.GetSingle(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task Update(EmailTemplate entity)
        {
            try
            {
                await _emailTemplateRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw;
            }
        }
    }
}
