using AutoMapper;
using Lumle.Core.Models;
using Lumle.Data.Models;
using Lumle.Module.Authorization.Entities;
using Lumle.Module.Authorization.ViewModels.PermissionViewModels;
using Lumle.Module.Authorization.ViewModels.UserViewModels;

namespace Lumle.Module.Authorization.Infrastructures.Mappings.Profiles
{
    public class AuthDomainToViewModel : Profile
    {
        public AuthDomainToViewModel()
        {
            CreateMap<Permission, PermissionVM>();
            CreateMap<UserProfile, ProfileVM>();
        }
    }
}
