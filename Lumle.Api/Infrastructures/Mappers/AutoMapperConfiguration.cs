using AutoMapper;
using Lumle.Api.Infrastructures.Mappers.Profiles;

namespace Lumle.Api.Infrastructures.Mappers
{
    public static class AutoMapperConfiguration
    {

        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<DomainToModel>();
                x.AddProfile<DomainToViewModel>();
                x.AddProfile<ModelToDomain>();
                x.AddProfile<ModelToViewModel>();
                x.AddProfile<ViewModelToModel>();
                x.AddProfile<ViewModelToDomain>();
            });
        }

    }
}
