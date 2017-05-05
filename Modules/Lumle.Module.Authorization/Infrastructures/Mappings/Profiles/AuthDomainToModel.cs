using AutoMapper;
using Lumle.Module.Authorization.Entities;
using Lumle.Module.Authorization.Models;

namespace Lumle.Module.Authorization.Infrastructures.Mappings.Profiles
{
    public class AuthDomainToModel : Profile
    {
        public AuthDomainToModel()
        {
            CreateMap<Permission, PermissionModel>();
        }
    }
}
