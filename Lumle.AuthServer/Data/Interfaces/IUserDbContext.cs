using System;
using System.Threading.Tasks;
using Lumle.AuthServer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumle.AuthServer.Data.Interfaces
{
    public interface IUserDbContext: IDisposable
    {
        DbSet<CustomUser> Customers { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
