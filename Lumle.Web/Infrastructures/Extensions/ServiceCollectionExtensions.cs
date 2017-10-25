using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Autofac.Extensions.DependencyInjection;
using Lumle.Core.Localizer;
using Lumle.Core.Services;
using Lumle.Core.Services.Abstracts;
using Lumle.Infrastructure.Utilities;
using Lumle.Infrastructure.Utilities.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Lumle.Data.Models;
using Lumle.Data.Data;
using Lumle.Data.Extensions;
using Lumle.Module.Schedular.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using NodaTime.TimeZones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Lumle.Web.Infrastructures.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection LoadInstalledModules(this IServiceCollection services,
            IList<ModuleInfo> modules, IHostingEnvironment hostingEnvironment)
        {
            var moduleRootFolder = new DirectoryInfo(Path.Combine(hostingEnvironment.ContentRootPath, "Modules"));
            var moduleFolders = moduleRootFolder.GetDirectories();

            foreach (var moduleFolder in moduleFolders)
            {
                var binFolder = new DirectoryInfo(Path.Combine(moduleFolder.FullName, "bin"));
                if (!binFolder.Exists)
                {
                    continue;
                }

                foreach (var file in binFolder.GetFileSystemInfos("*.dll", SearchOption.AllDirectories))
                {
                    Assembly assembly;
                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                    }
                    catch (FileLoadException)
                    {
                        // Get loaded assembly
                        assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));

                        if (assembly == null)
                        {
                            throw;
                        }
                    }

                    if (assembly.FullName.Contains(moduleFolder.Name))
                    {
                        modules.Add(new ModuleInfo
                        {
                            Name = moduleFolder.Name,
                            Assembly = assembly,
                            Path = moduleFolder.FullName
                        });
                    }
                }
            }

            Infrastructure.GlobalConfiguration.Modules = modules;
            return services;
        }
        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services, IList<ModuleInfo> modules)
        {
            var mvcBuilder = services.AddMvc()
                .AddRazorOptions(o =>
                {
                    foreach (var module in modules)
                    {
                        o.AdditionalCompilationReferences.Add(MetadataReference.CreateFromFile(module.Assembly.Location));
                    }
                })
                .AddViewLocalization()
                .AddMvcOptions(o => o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
                .AddDataAnnotationsLocalization()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            //Register for automapper
            AutoMapper.Mapper.Initialize(
                cfg => cfg.AddProfiles(modules.Select(x => x.Assembly))
            );

            foreach (var module in modules)
            {
                // Register controller from modules
                mvcBuilder.AddApplicationPart(module.Assembly);

                // Register dependency in modules
                var moduleInitializerType =
                    module.Assembly.GetTypes().FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));
                if ((moduleInitializerType == null) || (moduleInitializerType == typeof(IModuleInitializer))) continue;
                var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                moduleInitializer.Init(services);
            }

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(op =>
                {
                    // Password settings
                    op.Password.RequireDigit = true;
                    op.Password.RequiredLength = 6;
                    op.Password.RequireNonAlphanumeric = true;
                    op.Password.RequireUppercase = true;
                    op.Password.RequireLowercase = true;

                    //user unique email
                    op.User.RequireUniqueEmail = true;
                })
                .AddRoleStore<LumleRoleStore>()
                .AddUserStore<LumleUserStore>()
                .AddSignInManager<LumleSignInManager<User>>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Expiration = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            });

            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromSeconds(0));

            return services;
        }

        public static IServiceCollection AddMsSqlDataStore(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<BaseContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SQLConnection"),
                    b => b.MigrationsAssembly(migrationsAssembly)));
            return services;
        }

        public static IServiceCollection AddFrameworkServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(BaseContext));
            services.AddScoped<SignInManager<User>, LumleSignInManager<User>>();

            services.AddScoped<IActionContextAccessor,ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddScoped<IApplicationTokenService, ApplicationTokenService>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSingleton<IStringLocalizerFactory, EFStringLocalizerFactory>();

            services.TryAddSingleton<IDateTimeZoneProvider>(new DateTimeZoneCache(TzdbDateTimeZoneSource.Default));
            services.TryAddScoped<ITimeZoneHelper, TimeZoneHelper>();
            services.TryAddScoped<IFileHandler, FileHandler>();

            services.AddTransient<IUtilities, Utilities>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddTransient<ITwilioSmsService, SmsService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddScoped<IBaseRoleClaimService, BaseRoleClaimService>();
            
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceProvider Build(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SendSystemHealthReportViaEmailService>().AsSelf().As<ISendSystemHealthReportViaEmailService>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            foreach (var module in Infrastructure.GlobalConfiguration.Modules)
            {
                builder.RegisterAssemblyTypes(module.Assembly).AsImplementedInterfaces();
            }

            builder.RegisterInstance(configuration);
            builder.RegisterInstance(hostingEnvironment);
            builder.Populate(services);
            var container = builder.Build();
            return container.Resolve<IServiceProvider>();
        }
    }
}
