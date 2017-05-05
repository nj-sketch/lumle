using AutoMapper;
using Lumle.Module.Authorization.Entities;
using Lumle.Module.Authorization.Models;

namespace Lumle.Module.Authorization.Infrastructures.Mappings.Profiles
{
    public class AuthModelToDomain : Profile
    {
        public AuthModelToDomain()
        {
            CreateMap<PermissionModel, Permission>();
        }
    }
}
