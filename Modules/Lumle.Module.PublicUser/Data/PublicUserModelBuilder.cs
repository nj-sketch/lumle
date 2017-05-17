using Lumle.Data.Data.Abstracts;
using Lumle.Module.PublicUser.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Module.PublicUser.Data
{
    public class PublicUserModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<CustomUser>()
            .ToTable("PublicUser_MobileUser");
        }
    }
}
