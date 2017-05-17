using AutoMapper;
using Lumle.Api.Data.Entities;
using Lumle.Api.ViewModels.Account;

namespace Lumle.Api.Infrastructures.Mappers.Profiles
{
    public class ViewModelToModel : Profile
    {

        public ViewModelToModel()
        {
            CreateMap<SignupVM, MobileUser>();
        }
    }
}
