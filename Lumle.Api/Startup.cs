using JsonApiDotNetCore.Extensions;
using JsonApiDotNetCore.Services;
using Lumle.Api.Data.Contexts;
using Lumle.Api.Data.Entities;
using Lumle.Api.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Reflection;

namespace Lumle.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<BaseContext>(builder =>
                                builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                                options => options.MigrationsAssembly(migrationAssembly)));


            services.AddScoped<IResourceService<MobileUser>, MobileUserService>();

            services.AddJsonApi<BaseContext>( op => 
            {
                op.DefaultPageSize = 10;
                op.IncludeTotalRecordCount = true;
            });


            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }

        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, BaseContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //var identityServerValidationOptions = new IdentityServerAuthenticationOptions
            //{
            //    Authority = "http://localhost:30193/",
            //    AllowedScopes = new List<string> { "LumleApi.full_access" },
            //    ApiSecret = "secret",
            //    ApiName = "LumleApi",
            //    AutomaticAuthenticate = true,
            //    SupportedTokens = SupportedTokens.Jwt,
            //    AutomaticChallenge = true,
            //};

            //app.UseIdentityServerAuthentication(identityServerValidationOptions);

            context.Database.EnsureCreated();
            if (!context.Places.Any())
            {
                context.Places.Add(new Place
                {
                    Name = "Pokhara",
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    Location = "WSD" 
                });
                context.SaveChanges();
            }

            app.UseJsonApi();
        }
    }
}
