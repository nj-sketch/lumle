using Lumle.Core.Models;
using Lumle.Data.Data.Abstracts;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Module.Localization.Data
{
    public class LocalizationCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Culture>()
                .ToTable("Localization_Culture");

            modelBuilder.Entity<Culture>()
                .HasIndex(x => x.Name)
                .IsUnique(true);

            modelBuilder.Entity<Resource>()
                .ToTable("Localization_Resource");

            modelBuilder.Entity<Resource>()
                .HasIndex(x => new { x.CultureId, x.Key })
                .IsUnique(true);

            modelBuilder.Entity<Resource>()
                .HasOne(x => x.Culture)
                .WithMany(x => x.Resources)
                .HasForeignKey(x => x.CultureId);

            modelBuilder.Entity<Resource>()
                .HasOne(x => x.ResourceCategory)
                .WithMany(x => x.Resources)
                .HasForeignKey(x => x.ResourceCategoryId);

            modelBuilder.Entity<ResourceCategory>()
                .ToTable("Localization_ResourceCategory")
                .HasIndex(x => x.Name).IsUnique(true);
            
        }
    }
}
