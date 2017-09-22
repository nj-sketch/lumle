using System;
using Lumle.Module.AdminConfig.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lumle.Module.AdminConfig.Services
{
    public interface ISystemHealthService
    {
        void Add(SystemHealth systemHealth);
        IEnumerable<SystemHealth> AllIncluding(params Expression<Func<SystemHealth, object>>[] includeProperties);
        IEnumerable<SystemHealth> GetAll();
        ICollection<ServiceHealth> GetSystemHealthReport(string loggedInUserEmail);
        ICollection<ServiceHealth> GetServiceHealthReport();
    }
}
