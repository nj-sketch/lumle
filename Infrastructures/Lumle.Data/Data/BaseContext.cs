using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lumle.Data.Data.Abstracts;
using Lumle.Data.Models;
using Lumle.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Data.Data
{
    public class BaseContext : IdentityDbContext<User, Role,
        string, IdentityUserClaim<string>, UserRole,
        IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public BaseContext(DbContextOptions options)
            : base(options)
        {
          //Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var typeToRegisters = new List<Type>();
            foreach (var module in GlobalConfiguration.Modules)
            {
                typeToRegisters.AddRange(module.Assembly.DefinedTypes.Select(t => t.AsType()));
            }

            RegisterEntities(modelBuilder, typeToRegisters);

            //RegiserConvention(modelBuilder);

            base.OnModelCreating(modelBuilder);

            RegisterCustomMappings(modelBuilder, typeToRegisters);
        }

        private static void RegiserConvention(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.ClrType.Namespace == null) continue;

                var nameParts = entity.ClrType.Namespace.Split('.');
                var tableName = string.Concat(nameParts[2], "_", entity.ClrType.Name);
                modelBuilder.Entity(entity.Name).ToTable(tableName);
            }
        }

        private static void RegisterEntities(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            var entityTypes = typeToRegisters.Where(x => x.GetTypeInfo().IsSubclassOf(typeof(EntityBase)) && !x.GetTypeInfo().IsAbstract);

            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }

        private static void RegisterCustomMappings(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            var customModelBuilderTypes = typeToRegisters.Where(x => typeof(ICustomModelBuilder).IsAssignableFrom(x));

            foreach (var builderType in customModelBuilderTypes)
            {
                if (builderType == null || builderType == typeof(ICustomModelBuilder)) continue;

                var builder = (ICustomModelBuilder)Activator.CreateInstance(builderType);
                builder.Build(modelBuilder);
            }
        }
    }   
}

