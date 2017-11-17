using Lumle.Data.Models;
using Lumle.Module.AdminConfig.Entities;
using Lumle.Module.AdminConfig.Models;
using Lumle.Module.AdminConfig.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public interface IEmailTemplateService
    {
        IQueryable<EmailTemplate> GetAll();
        IQueryable<EmailTemplate> GetAll(Expression<Func<EmailTemplate, bool>> predicate);
        Task<EmailTemplate> GetSingleAsync(Expression<Func<EmailTemplate, bool>> predicate);
        Task<EmailTemplate> Update(EmailTemplateVM entity, User loggedUser);
        IEnumerable<EmailTemplateModel> GetAllEmailTemplate();
    }
}
