using AutoMapper;
using Lumle.Module.PublicUser.Entities;
using Lumle.Module.PublicUser.ViewModels.PublicUserViewModels;

namespace Lumle.Module.PublicUser.Infastructures.Mappings.Profiles
{
    public class PublicUserDomainToVM : Profile
    {
        public PublicUserDomainToVM()
        {
            CreateMap<CustomUser, PublicUserIndexVM>();
            CreateMap<CustomUser, PublicUserEditVM>();
        }
    }
}
