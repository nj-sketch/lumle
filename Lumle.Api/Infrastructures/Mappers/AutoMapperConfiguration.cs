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
                x.AddProfile<ModelToModel>();
                x.AddProfile<ModelToDomain>();
                x.AddProfile<DomainToModel>();
                x.AddProfile<ModelToViewModel>();
                x.AddProfile<ViewModelToModel>();
                x.AddProfile<DomainToViewModel>();
                x.AddProfile<ViewModelToDomain>();
            });
        }

    }
}
