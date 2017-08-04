using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lumle.Api.Data.Abstracts;
using Lumle.Api.Data.Contexts;
using Lumle.Api.Data.Repositiries;
using Lumle.Api.Service.Services;
using Lumle.Api.Service.Services.Abstracts;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using Lumle.Api.BusinessRules;
using Lumle.Api.BusinessRules.Abstracts;
using Lumle.Api.Infrastructures.Handlers.ApiResponse;

namespace Lumle.Api.Infrastructures.Extensions
{
    public static class ServiceExtension
    {
        /// <summary>
        /// Holds all services that need to register for IOC container
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton(config => configuration);

            #region Misc registration

            services.AddScoped(typeof(BaseContext));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IActionResponse, ActionResponse>();

            #endregion
            
            #region Service registration

            services.AddScoped<IAccountService, AccountService>();

            #endregion

            #region Businessrule registration
            services.AddScoped<IAccountBusinessRule, AccountBusinessRule>();
            #endregion
            
            return services;
        }


        public static IServiceProvider Build(this IServiceCollection services,
            IConfigurationRoot configuration, IHostingEnvironment hostingEnvironment)
        {
            var builder = new ContainerBuilder();
            //builder.RegisterType<SomeType>().AsSelf().As<IService>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));


            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();
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

            builder.RegisterInstance(configuration);
            builder.RegisterInstance(hostingEnvironment);
            builder.Populate(services);
            var container = builder.Build();
            return container.Resolve<IServiceProvider>();
        }

    }
}
