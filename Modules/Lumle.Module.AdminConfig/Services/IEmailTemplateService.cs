using Lumle.Module.AdminConfig.Entities;
using Lumle.Module.AdminConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public interface IEmailTemplateService
    {
        IQueryable<EmailTemplate> GetAll(Expression<Func<EmailTemplate, bool>> predicate);
        IQueryable<EmailTemplate> GetAll();
        Task<EmailTemplate> GetSingle(Expression<Func<EmailTemplate, bool>> predicate);
        Task Add(EmailTemplate entity);
        Task Update(EmailTemplate entity);
        IEnumerable<EmailTemplateModel> GetAllEmailTemplate();
    }
}
