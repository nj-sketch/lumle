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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

            services.AddScoped(typeof(BaseContext));

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();



            return services;
        }
        


        public static IServiceProvider Build(this IServiceCollection services,
            IConfigurationRoot configuration, IHostingEnvironment hostingEnvironment)
        {
            var builder = new ContainerBuilder();
            //builder.RegisterType<SomeType>().AsSelf().As<IService>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            //TODO: Find alternative of AppDomain. Until then resolve dependency in manual way
            //RefLink: http://stackoverflow.com/questions/18263852/how-to-register-services-and-types-that-are-in-separate-assemblies-in-autofac
            //builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
            //       .Where(t => t.Name.EndsWith("Service"))
            //       .AsImplementedInterfaces();


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
