using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

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
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });
        }

        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var identityServerValidationOptions = new IdentityServerAuthenticationOptions
            {
                Authority = "http://localhost:30193/",
                AllowedScopes = new List<string> { "LumleApi.full_access" },
                ApiSecret = "secret",
                ApiName = "LumleApi",
                AutomaticAuthenticate = true,
                SupportedTokens = SupportedTokens.Jwt,
                AutomaticChallenge = true,
            };

            app.UseIdentityServerAuthentication(identityServerValidationOptions);

            app.UseMvc();
        }
    }
}
