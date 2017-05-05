using System.ComponentModel.DataAnnotations.Schema;
using Lumle.Core.Models;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Module.Core.Data
{
    public class CoreCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b =>
            {
                b.ToTable("Core_User");
                b.HasIndex(x  =>  new { x.Email, x.NormalizedEmail }).IsUnique(true);
                b.Property(x => x.Culture).HasDefaultValue("en-US");
            });

            modelBuilder.Entity<Role>()
                .ToTable("Core_Role");

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.ToTable("Core_UserClaim");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("Core_RoleClaim");
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasKey(ur => new { ur.UserId, ur.RoleId });
                b.HasOne(ur => ur.Role).WithMany(r => r.Users).HasForeignKey(r => r.RoleId);
                b.HasOne(ur => ur.User).WithMany(u => u.Roles).HasForeignKey(u => u.UserId);
                b.ToTable("Core_UserRole");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.HasKey(p => new { p.LoginProvider, p.ProviderKey } );
                b.ToTable("Core_UserLogin");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.HasKey(p => p.UserId);
                b.ToTable("Core_UserToken");
            });

            modelBuilder.Entity<ApplicationToken>(b => {
                b.HasKey(at => at.Id);
                b.ToTable("Core_ApplicationToken");
            });

            modelBuilder.Entity<BaseRoleClaim>()
                .ToTable("Core_BaseRoleClaim");

        }
    }
}
