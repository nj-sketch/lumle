using AutoMapper;
using Lumle.Api.Infrastructures.Handlers.ApiResponse.Models;

namespace Lumle.Api.Infrastructures.Mappers.Profiles
{
    public class ModelToModel: Profile
    {
        public ModelToModel()
        {
            CreateMap<ShortMessage, Message>();
        }
    }
}
