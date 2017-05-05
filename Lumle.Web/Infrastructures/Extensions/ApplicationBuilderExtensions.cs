using System.Collections.Generic;
using System.IO;
using Lumle.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Lumle.Web.Infrastructures.Middlewares;

namespace Lumle.Web.Infrastructures.Extensions
{
    public static class ApplicationBuilderExtensions
    {

        public static IApplicationBuilder UseCustomizedStaticFiles(this IApplicationBuilder app, IList<ModuleInfo> modules)
        {
            app.UseStaticFiles();

            // Serving static file for modules
            foreach (var module in modules)
            {
                var wwwrootDir = new DirectoryInfo(Path.Combine(module.Path, "wwwroot"));
                if (!wwwrootDir.Exists)
                {
                    continue;
                }

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(wwwrootDir.FullName),
                    RequestPath = new PathString("/" + module.ShortName)
                });
            }
            return app;
        }

        public static IApplicationBuilder UseCustomizedRequestLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("fr-FR"),
                new CultureInfo("ne"),
                new CultureInfo("de"),
                new CultureInfo("es-ES"),
                new CultureInfo("ja"),
                new CultureInfo("ko"),
                new CultureInfo("zh-Hans"),
                new CultureInfo("it-it")

            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US", "en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            return app;
        }

        public static IApplicationBuilder UseAppSystemMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AppSystemMiddleware>();
        }

        public static IApplicationBuilder UseSchedularMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SchedularMiddleware>();
        }
    }
}
