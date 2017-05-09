using System;
using Lumle.Module.AdminConfig.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public interface ISystemHealthService
    {
        void Add(SystemHealth systemHealth);
        IEnumerable<SystemHealth> AllIncluding(params Expression<Func<SystemHealth, object>>[] includeProperties);
        IEnumerable<SystemHealth> GetAll();
        Task<ICollection<ServiceHealth>> GetSystemHealthReportAsync(string loggedInUserEmail);
        Task<ICollection<ServiceHealth>> GetServiceHealthReportAsync();
    }
}
