using System;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.AdminConfig.Entities;
using NLog;
using Lumle.Infrastructure.Constants.LumleLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public class SystemHealthService : ISystemHealthService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<SystemHealth> _systemHealthRepository;
        private readonly IRepository<ServiceHealth> _serviceHealthRepository;
        private readonly ICredentialCategoryService _credentialCategoryService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IMessagingService _messagingService;

        public SystemHealthService(
            IRepository<SystemHealth> systemHealthRepository,
            IRepository<ServiceHealth> serviceHealthRepository,
            ICredentialCategoryService credentialCategoryService,
            IEmailTemplateService emailTemplateService,
            IMessagingService messagingService
        )
        {
            _systemHealthRepository = systemHealthRepository;
            _credentialCategoryService = credentialCategoryService;
            _emailTemplateService = emailTemplateService;
            _messagingService = messagingService;
            _serviceHealthRepository = serviceHealthRepository;

        }

        public IQueryable<SystemHealth> GetAll()
        {
            try
            {
                return _systemHealthRepository.GetAll();
            }
            catch(Exception ex)
            {
                Logger.Error(ex, ErrorLog.DataFetchError);
                throw;
            }
        }

        public async Task<ICollection<ServiceHealth>> GetSystemHealthReport(string loggedInUserEmail)
        {
            try
            {
                // Add system health into the database
                var systemHealth = new SystemHealth()
                {
                    Username = loggedInUserEmail,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };

                await _systemHealthRepository.AddAsync(systemHealth);
                await _systemHealthRepository.SaveChangesAsync();

                var serviceHealths = GetServiceHealthReport();
                foreach (var item in serviceHealths)
                {
                    item.SystemHealth = systemHealth;
                    await _serviceHealthRepository.AddAsync(item);
                    await _serviceHealthRepository.SaveChangesAsync();
                }

                var data = systemHealth.ServiceHealths;
                return data;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.Undefinederror);
                throw;
            }
        }

        public ICollection<ServiceHealth> GetServiceHealthReport()
        {
            try
            {
                var credentialCategory = _credentialCategoryService.GetAllCredentialCategory();
                var serviceHealths = new List<ServiceHealth>();
                foreach (var item in credentialCategory)
                {
                    var serviceHealth = new ServiceHealth()
                    {
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                    };
                    switch (item.Name)
                    {
                        case "Email Service":
                            var emailTemplate = _emailTemplateService.GetAll().First();
                            serviceHealth.ServiceName = "EmailService";
                            try
                            {
                                _messagingService.VerifySMTPStatus();
                                serviceHealth.Status = true;
                                serviceHealth.Message = "Operational";
                            }
                            catch (Exception ex)
                            {
                                serviceHealth.Status = false;
                                serviceHealth.Message = ex.Message;
                            }
                            break;
                        case "Twillio SMS":
                            // await _messagingService.SendSmsAsync("9779849615041", "hello niraj");
                            serviceHealth.ServiceName = "TwillioSMS";
                            serviceHealth.Status = true;
                            serviceHealth.Message = "Operational";
                            break;
                        default:
                            serviceHealth.ServiceName = "General";
                            serviceHealth.Status = true;
                            serviceHealth.Message = "Operational";
                            break;
                    } // end of switch statement

                    serviceHealths.Add(serviceHealth);
                } // end of foreach loop

                return serviceHealths;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.Undefinederror);
                throw;
            }
        }

    }
}
