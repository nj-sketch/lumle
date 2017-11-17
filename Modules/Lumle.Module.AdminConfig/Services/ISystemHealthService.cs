using Lumle.Module.AdminConfig.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public interface ISystemHealthService
    {
        IQueryable<SystemHealth> GetAll();
        Task<ICollection<ServiceHealth>> GetSystemHealthReport(string loggedInUserEmail);
        ICollection<ServiceHealth> GetServiceHealthReport();
    }
}
