using System;
using Lumle.Module.AdminConfig.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public interface ISystemHealthService
    {
        Task Add(SystemHealth systemHealth);
        IQueryable<SystemHealth> AllIncluding(params Expression<Func<SystemHealth, object>>[] includeProperties);
        IQueryable<SystemHealth> GetAll();
        Task<ICollection<ServiceHealth>> GetSystemHealthReport(string loggedInUserEmail);
        ICollection<ServiceHealth> GetServiceHealthReport();
    }
}
