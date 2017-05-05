using Lumle.AuthServer.Data.EntityFrameworkBuilderExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lumle.AuthServer.Infrastructures.DbproviderExtensions
{
    public static class DbproviderExtensions
    {

        public static IServiceCollection AddMsSqlDataStore(this IServiceCollection services, IConfigurationRoot configuration, string migrationsAssembly)
        {
            var connectionString = configuration.GetConnectionString("SQLConnection");

            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddConfigurationStore(builder =>
                    builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(builder =>
                    builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                .AddCustomUserStore(builder =>
                    builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)));

            return services;
        }

        public static IServiceCollection AddMySqlDataStore(this IServiceCollection services, IConfigurationRoot configuration, string migrationsAssembly)
        {

            var connectionString = configuration.GetConnectionString("MySQLConnection");

            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddConfigurationStore(builder =>
                    builder.UseMySql(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(builder =>
                    builder.UseMySql(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                .AddCustomUserStore(builder =>
                    builder.UseMySql(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)));

            return services;
        }

        public static IServiceCollection AddPostgreSqlProvider(this IServiceCollection services, IConfigurationRoot configuration, string migrationsAssembly)
        {
            var connectionString = configuration.GetConnectionString("PostGreSQLConnection");

            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddConfigurationStore(builder =>
                    builder.UseNpgsql(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(builder =>
                    builder.UseNpgsql(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                .AddCustomUserStore(builder =>
                    builder.UseNpgsql(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)));

            return services;
        }

    }
}
