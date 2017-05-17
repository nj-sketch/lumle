using IdentityServer4.EntityFramework.Options;
using Lumle.AuthServer.Data.Entities;
using Lumle.AuthServer.Infrastructures.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lumle.AuthServer.Data.ModelBuilderExtensions
{
    public static class ModelBuilderExtensions
    {
        private static EntityTypeBuilder<TEntity> ToTable<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, TableConfiguration configuration)
            where TEntity : class
        {
            return string.IsNullOrWhiteSpace(configuration.Schema) ? entityTypeBuilder.ToTable(configuration.Name) : entityTypeBuilder.ToTable(configuration.Name, configuration.Schema);
        }


        public static void ConfigureUserContext(this ModelBuilder modelBuilder,
            UserStoreOptions storeOptions)
        {
            modelBuilder.Entity<MobileUser>(customUser =>
            {
                customUser.ToTable(storeOptions.CustomUser);
            });
        }

        public static void ConfigureTokenSnapShot(this ModelBuilder modelBuilder, TokenSnapShotOptions tokenOptions)
        {
            modelBuilder.Entity<TokenSnapShot>(token =>
            {
                token.ToTable(tokenOptions.TokenSnapShot);
            });
        }

    }
}
