using AutoMapper;
using Lumle.Core.Models;
using Lumle.Module.Authorization.ViewModels.UserViewModels;

namespace Lumle.Module.Authorization.Infrastructures.Mappings.Profiles
{
    public class AuthVMToDomain : Profile
    {
        public AuthVMToDomain()
        {
            CreateMap<ProfileVM, UserProfile>();
        }
    }
}
