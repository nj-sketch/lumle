using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lumle.Module.AdminConfig.Entities;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.Log;
using Lumle.Module.AdminConfig.Models;
using NLog;

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
        public void Add(EmailTemplate entity)
        {
            try
            {
                _emailTemplateRepository.Add(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw new Exception(ex.Message);
            }
        }


        public IEnumerable<EmailTemplate> GetAll()
        {
            try
            {
              return  _emailTemplateRepository.GetAll();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<EmailTemplate> GetAll(Expression<Func<EmailTemplate, bool>> predicate)
        {
            try
            {
               return _emailTemplateRepository.GetAll(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
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

                return data.Select(x => { x.Sn = ++i; return x; }).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public EmailTemplate GetSingle(Expression<Func<EmailTemplate, bool>> predicate)
        {
            try
            {
              return  _emailTemplateRepository.GetSingle(predicate);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw new Exception(ex.Message);
            }
        }

        public void Update(EmailTemplate entity)
        {
            try
            {
                _emailTemplateRepository.Update(entity);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.UpdateError);
                throw new Exception(ex.Message);
            }
        }
    }
}
