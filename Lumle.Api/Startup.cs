using Lumle.Api.Data.Contexts;
using Lumle.Api.Data.Entities;
using Lumle.Api.Infrastructures.Extensions;
using Lumle.Api.Infrastructures.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Lumle.Api
{
    public class Startup
    {
        private IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            _hostingEnvironment = env;
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }


        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddMsSqlDbProvider(Configuration, migrationAssembly);

            services.AddServices(Configuration);

            services.AddApiVersioning();

            AutoMapperConfiguration.Configure();

            services.AddAuthentication()
              .AddJwtBearer(cfg =>
              {
                  cfg.RequireHttpsMetadata = false;
                  cfg.SaveToken = false;

                  cfg.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidIssuer = Configuration["Tokens:Issuer"],
                      ValidAudience = Configuration["Tokens:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"])),
                      ValidateIssuerSigningKey = true
                  };

              });



            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Sipradi Mobile API",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Email = "info@ekbana.com",
                        Name = "EKbana Solutions"
                    }
                });

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "LumleApiDocs.xml");
                c.IncludeXmlComments(filePath);
            });


            return services.Build(Configuration, _hostingEnvironment);
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, BaseContext context)
        {
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 docs");
            });

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
