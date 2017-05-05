using AutoMapper;
using Lumle.Module.Audit.Entities;
using Lumle.Module.Audit.ViewModels;

namespace Lumle.Module.Audit.Infrastructure.Mappings.Profiles
{
    public class AuditLogDomainToViewModel : Profile
    {     
        public AuditLogDomainToViewModel()
        {
            CreateMap<AuditLog, AuditLogVM>();
            //.ForMember(d => d.CreatedDate, expression => expression.ResolveUsing(s => s.CreatedDate.ToString("g")));
        }
    }
}
