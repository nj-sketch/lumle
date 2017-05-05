using System;
using System.Threading.Tasks;
using Lumle.AuthServer.Data.Entities;
using Lumle.AuthServer.Data.Interfaces;
using Lumle.AuthServer.Data.ModelBuilderExtensions;
using Lumle.AuthServer.Infrastructures.Options;
using Microsoft.EntityFrameworkCore;

namespace Lumle.AuthServer.Data.Contexts
{
    public class UserDbContext : DbContext, IUserDbContext
    {
        private readonly UserStoreOptions _storeOptions;

        public UserDbContext(DbContextOptions<UserDbContext> options, UserStoreOptions storeOptions)
            :base(options)
        {
            if (storeOptions == null) throw  new ArgumentNullException(nameof(storeOptions));
            _storeOptions = storeOptions;
        }


        public DbSet<CustomUser> Customers { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return  base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureUserContext(_storeOptions);

            base.OnModelCreating(modelBuilder);
        }
    }
}
