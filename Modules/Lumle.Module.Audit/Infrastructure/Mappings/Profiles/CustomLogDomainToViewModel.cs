using AutoMapper;
using Lumle.Module.Audit.Entities;
using Lumle.Module.Audit.ViewModels;

namespace Lumle.Module.Audit.Infrastructure.Mappings.Profiles
{
    public class CustomLogDomainToViewModel : Profile
    {
        public CustomLogDomainToViewModel()
        {
            CreateMap<CustomLog, CustomLogVM>();
        }
    }
}
