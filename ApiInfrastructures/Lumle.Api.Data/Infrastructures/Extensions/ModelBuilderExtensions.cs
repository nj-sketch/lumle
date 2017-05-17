using Lumle.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Api.Data.Infrastructures.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder PlaceConfiguration(this ModelBuilder builder)
        {

            builder.Entity<Place>()
                .ToTable("Place");

            return builder;
        }

        public static ModelBuilder MobileUserConfiguration(this ModelBuilder builder)
        {

            builder.Entity<MobileUser>()
                .ToTable("MobileUser");

            return builder;
        }

    }
}
