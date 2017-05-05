using Lumle.Core.Models;
using Lumle.Data.Data.Abstracts;
using Microsoft.EntityFrameworkCore;
using Lumle.Module.AdminConfig.Entities;
using Lumle.Data.Models;

namespace Lumle.Module.AdminConfig.Data
{
    public class AdminConfigCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailTemplate>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            modelBuilder.Entity<EmailTemplate>()
                .ToTable("AdminConfig_EmailTemplate");


            modelBuilder.Entity<CredentialCategory>()
                .ToTable("AdminConfig_CredentialCategory");

            modelBuilder.Entity<Credential>()
                .HasOne(x => x.CredentialCategory)
                .WithMany(x => x.Credentials)
                .HasForeignKey(x => x.CredentialCategoryId);


            modelBuilder.Entity<Credential>()
                .HasIndex(x => new { x.CredentialCategoryId, x.Slug })
                .IsUnique();

            modelBuilder.Entity<Credential>()
                .ToTable("AdminConfig_Credential");

            modelBuilder.Entity<AppSystem>(b => {
                b.HasKey(s => s.Id);
                b.HasIndex(s => new { s.Slug }).HasName("SlugIndex").IsUnique();
                b.ToTable("AdminConfig_AppSystem");
            });

            modelBuilder.Entity<SystemHealth>()
                .ToTable("AdminConfig_SystemHealth");

            modelBuilder.Entity<SystemHealth>()
                .HasMany(b => b.ServiceHealths)
                .WithOne(p => p.SystemHealth);

            modelBuilder.Entity<ServiceHealth>()
                .ToTable("AdminConfig_ServiceHealth");

            modelBuilder.Entity<ServiceHealth>()
                .HasOne(p => p.SystemHealth)
                .WithMany(b => b.ServiceHealths);
        }
    }
}
