using System;
using System.Collections.Generic;
using System.IO.Compression;
using Lumle.Infrastructure;
using Lumle.Infrastructure.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lumle.Web.Infrastructures.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.AspNetCore.Http;
using Lumle.Data.Data;
using GlobalConfiguration = Lumle.Infrastructure.GlobalConfiguration;
using Lumle.Web.DataSeed;

namespace Lumle.Web
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private static readonly IList<ModuleInfo> Modules = new List<ModuleInfo>();

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            GlobalConfiguration.WebRootPath = _hostingEnvironment.WebRootPath;
            GlobalConfiguration.ContentRootPath = _hostingEnvironment.ContentRootPath;
            services.LoadInstalledModules(Modules, _hostingEnvironment);

            services.AddMsSqlDataStore(Configuration);
            services.AddIdentity();
            services.AddFrameworkServices(Configuration);     
                  
            //call this in case you need aspnet-user-authtype/aspnet-user-identity
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<RazorViewEngineOptions>(
                options =>
                {
                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander());

                });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression();

            services.AddCustomizedMvc(GlobalConfiguration.Modules);
            services.AddMemoryCache();
          
            return services.Build(Configuration, _hostingEnvironment);
        }
       
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BaseContext context)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.SeedData(context);

            app.UseCustomizedRequestLocalization();
            app.UseCustomizedStaticFiles(Modules);

            // Checking System maintenance mode 
            app.UseAppSystemMiddleware();

            // Use only while using scheduler
           // app.UseSchedularMiddleware();

            app.UseStatusCodePagesWithRedirects("~/{0}");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Account}/{action=Index}/{id?}");
            });
        }        
    }
}

