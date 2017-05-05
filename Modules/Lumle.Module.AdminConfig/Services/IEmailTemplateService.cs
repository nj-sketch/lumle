using Lumle.Module.AdminConfig.Entities;
using Lumle.Module.AdminConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lumle.Module.AdminConfig.Services
{
    public interface IEmailTemplateService
    {
        IEnumerable<EmailTemplate> GetAll(Expression<Func<EmailTemplate, bool>> predicate);
        IEnumerable<EmailTemplate> GetAll();
        EmailTemplate GetSingle(Expression<Func<EmailTemplate, bool>> predicate);
        void Add(EmailTemplate entity);
        void Update(EmailTemplate entity);
        IEnumerable<EmailTemplateModel> GetAllEmailTemplate();
    }
}
