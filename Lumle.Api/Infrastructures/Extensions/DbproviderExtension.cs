﻿using Lumle.Api.Data.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Api.Infrastructures.Extensions
{
    public static class DbproviderExtension
    {

        public static IServiceCollection AddMsSqlDbProvider( this IServiceCollection services, IConfigurationRoot configuration, string migrationAssembly)
        {
            services.AddDbContext<BaseContext>(builder =>
                                builder.UseSqlServer(configuration.GetConnectionString("SQLConnection"),
                                options => options.MigrationsAssembly(migrationAssembly)));

            return services;
        }


        public static IServiceCollection AddPostGreSqlProvider(this IServiceCollection services, IConfigurationRoot configuration, string migrationAssembly)
        {
            services.AddDbContext<BaseContext>(builder =>
                                builder.UseNpgsql(configuration.GetConnectionString("PostGreSQLConnection"),
                                options => options.MigrationsAssembly(migrationAssembly)));

            return services;
        }

    }
}
