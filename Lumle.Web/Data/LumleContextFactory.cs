using Lumle.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Lumle.Web.Data
{
    public class LumleContextFactory : IDesignTimeDbContextFactory<BaseContext>
    {      
        public BaseContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<BaseContext>();

            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .Build();

            builder.UseSqlServer(configuration["ConnectionStrings:LocalConnection"], b => b.MigrationsAssembly("Lumle.Web"));

            return new BaseContext(builder.Options);
        }
    }
}
