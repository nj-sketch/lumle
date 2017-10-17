using Lumle.Api.Data.Entities;
using Lumle.Api.Data.Infrastructures.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Api.Data.Contexts
{
    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions<BaseContext> options)
           : base(options)
        {

        }
        public DbSet<MobileUser> MobileUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            //builder.PlaceConfiguration();
            builder.MobileUserConfiguration();

        }
    }
}
