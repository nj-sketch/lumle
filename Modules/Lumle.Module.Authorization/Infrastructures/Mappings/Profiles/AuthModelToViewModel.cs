using AutoMapper;
using Lumle.Module.Authorization.Models;
using Lumle.Module.Authorization.ViewModels.RoleViewModels;

namespace Lumle.Module.Authorization.Infrastructures.Mappings.Profiles
{
    public class AuthModelToViewModel : Profile
    {

        public AuthModelToViewModel()
        {
            CreateMap<AuthorizationModel, AssignpermissionVM>();
        }
    }
}
