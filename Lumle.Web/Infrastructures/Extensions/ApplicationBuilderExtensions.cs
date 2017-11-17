using Microsoft.AspNetCore.Builder;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Lumle.Web.Infrastructures.Middlewares;

namespace Lumle.Web.Infrastructures.Extensions
{
    public static class ApplicationBuilderExtensions
    {

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
