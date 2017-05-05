using Lumle.Data.Data;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lumle.Web.DataSeed
{
    public static class DataSeeder
    {
        public static void SeedData(this IApplicationBuilder app, BaseContext context)
        {
            if (context.Roles.Any())
            {
                return;
            }
            var typeToRegisters = new List<Type>();
            foreach (var module in GlobalConfiguration.Modules)
            {
                typeToRegisters.AddRange(module.Assembly.DefinedTypes.Select(t => t.AsType()));
            }
            var customModelBuilderTypes = typeToRegisters.Where(x => typeof(IDataInitializer).IsAssignableFrom(x));

            foreach (var builderType in customModelBuilderTypes)
            {
                if (builderType == null || builderType == typeof(IDataInitializer)) continue;

                var builder = (IDataInitializer)Activator.CreateInstance(builderType);
                builder.Initialize(context);
            }
        }
    }
}
