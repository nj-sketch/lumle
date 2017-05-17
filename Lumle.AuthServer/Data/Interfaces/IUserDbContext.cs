using System;
using System.Threading.Tasks;
using Lumle.AuthServer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumle.AuthServer.Data.Interfaces
{
    public interface IUserDbContext: IDisposable
    {
        DbSet<MobileUser> Customers { get; set; }

        DbSet<TokenSnapShot> TokenSnapShots { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
