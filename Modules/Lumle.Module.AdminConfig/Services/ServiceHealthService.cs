using System;
using Lumle.Data.Data.Abstracts;
using Lumle.Infrastructure.Constants.LumleLog;
using Lumle.Module.AdminConfig.Entities;
using NLog;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public class ServiceHealthService : IServiceHealthService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<ServiceHealth> _serviceHealthRepository;

        public ServiceHealthService(IRepository<ServiceHealth> serviceHealthRepository)
        {
            _serviceHealthRepository = serviceHealthRepository;
        }

        public async Task Add(ServiceHealth serviceHealth)
        {
            try
            {
                if (serviceHealth == null) return;

                await _serviceHealthRepository.Add(serviceHealth);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.SaveError);
                throw;
            }
        }

    }
}
