using Lumle.Core.Models;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Lumle.Module.Authorization.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Module.Authorization.Data
{
    public class AuthorizationCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        { 

            modelBuilder.Entity<Permission>()
              .ToTable("Authorization_Permission");

            modelBuilder.Entity<Permission>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            modelBuilder.Entity<UserProfile>()
            .ToTable("Authorization_UserProfile");

        }
    }
}
