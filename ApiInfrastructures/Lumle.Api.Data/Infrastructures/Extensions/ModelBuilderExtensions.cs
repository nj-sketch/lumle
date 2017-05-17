using Lumle.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Api.Data.Infrastructures.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder PlaceConfiguration(this ModelBuilder builder)
        {

            builder.Entity<Place>()
                .ToTable("Api_Place");

            return builder;
        }

        public static ModelBuilder MobileUserConfiguration(this ModelBuilder builder)
        {

            builder.Entity<MobileUser>()
                .ToTable("PublicUser_MobileUser");

            return builder;
        }

    }
}
