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
        private readonly TokenSnapShotOptions _tokenOptions;

        public UserDbContext(DbContextOptions<UserDbContext> options, UserStoreOptions storeOptions, TokenSnapShotOptions tokenOptions)
            :base(options)
        {
            _storeOptions = storeOptions ?? throw new ArgumentNullException(nameof(storeOptions));
            _tokenOptions = tokenOptions ?? throw new ArgumentNullException(nameof(tokenOptions));
        }


        public DbSet<MobileUser> Customers { get; set; }

        public DbSet<TokenSnapShot> TokenSnapShots { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureUserContext(_storeOptions);
            modelBuilder.ConfigureTokenSnapShot(_tokenOptions);

            base.OnModelCreating(modelBuilder);
        }
    }
}
