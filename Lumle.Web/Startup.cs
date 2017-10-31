using System;
using System.Collections.Generic;
using System.IO.Compression;
using Lumle.Infrastructure;
using Lumle.Infrastructure.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Lumle.Web.Infrastructures.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Http;
using Lumle.Data.Data;
using GlobalConfiguration = Lumle.Infrastructure.GlobalConfiguration;
using Lumle.Web.DataSeed;
using NLog.Web;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Lumle.Infrastructure.Models;
using WebMarkupMin.AspNetCore2;
using WebMarkupMin.Core;

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

            // Configure LumleSettings
            services.Configure<LumleSettings>(Configuration.GetSection("LumleSettings"));

            //call this in case you need aspnet-user-authtype/aspnet-user-identity
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<RazorViewEngineOptions>(
                options =>
                {
                    options.ViewLocationExpanders.Add(new ModuleViewLocationExpander());

                });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression();

            //services.AddWebOptimizer();

            services.AddCustomizedMvc(GlobalConfiguration.Modules);
            services.AddMemoryCache();
            // HTML minifier
            services
             .AddWebMarkupMin(options =>
             {
                 options.AllowMinificationInDevelopmentEnvironment = true;
                 options.DisablePoweredByHttpHeaders = true;
             })
             .AddHtmlMinification(options =>
             {
                 options.MinificationSettings.RemoveOptionalEndTags = false;
                 options.MinificationSettings.WhitespaceMinificationMode = WhitespaceMinificationMode.Safe;
             });

            //services.AddWebOptimizer(pipeline =>
            //{
            //    pipeline.MinifyJsFiles();
            //    pipeline.MinifyCssFiles()
            //            .InlineImages(1);
            //});

            return services.Build(Configuration, _hostingEnvironment);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, BaseContext context)
        {
            env.ConfigureNLog("nlog.config");
            loggerFactory.AddNLog();
            app.AddNLogWeb();

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

            app.UseAuthentication();
            app.SeedData(context);

            app.UseCustomizedRequestLocalization();
            //app.UseWebOptimizer();
            app.UseCustomizedStaticFiles(Modules);

            // Checking System maintenance mode 
            app.UseAppSystemMiddleware();

            // Use only while using scheduler
            // app.UseSchedularMiddleware();

            app.UseStatusCodePagesWithRedirects("~/{0}");
            // For HTMl minifier
            app.UseWebMarkupMin();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

