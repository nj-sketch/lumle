using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Lumle.Data.Models;
using Lumle.Module.AdminConfig.Services;
using Lumle.Module.Authorization.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Lumle.Module.Schedular.Services
{
    public class SendSystemHealthReportViaEmailService : ISendSystemHealthReportViaEmailService
    {
        private readonly IMessagingService _messagingService;
        private readonly ISystemHealthService _systemHealthService;
        private readonly UserManager<User> _userManager;

        public SendSystemHealthReportViaEmailService(
            IMessagingService messagingService,
            ISystemHealthService systemHealthService,
            UserManager<User> userManager
        )
        {
            _messagingService = messagingService;
            _systemHealthService = systemHealthService;
            _userManager = userManager;       
        }

        public async Task SendHealthReportViaMailAsync()
        {
            var data = await _systemHealthService.GetServiceHealthReportAsync();

            if (data == null) return;

            // Get superadmin users
            var users = _userManager.Users
                .Where(x => x.Roles.Select(y => y.Role.Name).Contains("superadmin"))
                .Select(u => new UserVM()
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName
                });

            var emailReceiversList = new Dictionary<string, string>();
            foreach (var userItem in users)
            {
                emailReceiversList.Add(userItem.UserName, userItem.Email);
            }

            var dictionary = data.ToDictionary(item => item.ServiceName, item => item.Message);

            //change the Cron to every 60 minutes    
            RecurringJob.AddOrUpdate(() => SendMail(emailReceiversList, dictionary), Cron.Hourly);

        }

        public void SendMail(Dictionary<string, string> emailList, Dictionary<string, string> dictionary)
        {          
            _messagingService.SendEmailWithBody(emailList, dictionary);
        }
    }
}
