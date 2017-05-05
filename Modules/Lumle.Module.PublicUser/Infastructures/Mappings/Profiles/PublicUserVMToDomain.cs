using AutoMapper;
using Lumle.Module.PublicUser.Entities;
using Lumle.Module.PublicUser.ViewModels.PublicUserViewModels;

namespace Lumle.Module.PublicUser.Infastructures.Mappings.Profiles
{
    public class PublicUserVMToDomain : Profile
    {
        public PublicUserVMToDomain()
        {
            CreateMap<PublicUserIndexVM, CustomUser>();
            CreateMap<PublicUserEditVM, CustomUser>();
        }
    }
}
