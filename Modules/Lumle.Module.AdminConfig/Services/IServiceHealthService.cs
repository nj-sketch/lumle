using Lumle.Module.AdminConfig.Entities;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public interface IServiceHealthService
    {
        Task Add(ServiceHealth serviceHealth);
    }
}
