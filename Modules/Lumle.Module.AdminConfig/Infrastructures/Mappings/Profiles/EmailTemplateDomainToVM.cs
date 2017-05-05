using AutoMapper;
using Lumle.Core.Models;
using Lumle.Data.Models;
using Lumle.Module.AdminConfig.Entities;
using Lumle.Module.AdminConfig.ViewModels;

namespace Lumle.Module.AdminConfig.Infrastructures.Mappings.Profiles
{
    public class EmailTemplateDomainToVM : Profile
    {
        public EmailTemplateDomainToVM()
        {
            CreateMap<EmailTemplate, EmailTemplateVM>();
            CreateMap<AppSystem, SystemSettingVM>();
        }
    }
}
