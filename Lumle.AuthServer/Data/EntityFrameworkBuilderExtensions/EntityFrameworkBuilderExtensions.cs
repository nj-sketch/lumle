using System;
using Lumle.AuthServer.Data.Contexts;
using Lumle.AuthServer.Data.Interfaces;
using Lumle.AuthServer.Data.Store;
using Lumle.AuthServer.Infrastructures.GrantTypes;
using Lumle.AuthServer.Infrastructures.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lumle.AuthServer.Data.EntityFrameworkBuilderExtensions
{
    public static class EntityFrameworkBuilderExtensions
    {

        public static IIdentityServerBuilder AddCustomUserStore(
            this IIdentityServerBuilder builder,
            Action<DbContextOptionsBuilder> dbContextOptionsAction = null,
            Action<UserStoreOptions> storeOptionsAction = null,
            Action<TokenSnapShotOptions> tokenOptionsAction = null)
        {
            builder.Services.AddDbContext<UserDbContext>(dbContextOptionsAction);
            builder.Services.AddScoped<IUserDbContext, UserDbContext>();

            builder.Services.AddTransient<IUserStore, UserStore>();
            builder.Services.AddTransient<ITokenSnapShotStore, TokenSnapShotStore>();
            builder.AddProfileService<CustomProfileService>();
            builder.AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();

            builder.AddExtensionGrantValidator<GoogleAuthValidator>();
            builder.AddExtensionGrantValidator<FacebookAuthValidator>();

            var options = new UserStoreOptions();
            storeOptionsAction?.Invoke(options);
            var tokenOptions = new TokenSnapShotOptions();
            tokenOptionsAction?.Invoke(tokenOptions);

            builder.Services.AddSingleton(options);
            builder.Services.AddSingleton(tokenOptions);

            return builder;

        }


    }
}
