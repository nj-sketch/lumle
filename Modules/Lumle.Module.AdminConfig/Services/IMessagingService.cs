using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.Services
{
    public interface IMessagingService
    {
        Task SendMailAsync(string emailTemplateSlug,string to);
        Task SendForgotPasswordMailAsync(string to, string username, string url);
        Task SendEmailConfirmationMailAsync(string to, string username, string url);
        Task SendLoginCredentialMailAsync(string to, string username, string role, string email, string password);
        Task SendEmailWithBody(Dictionary<string, string> emailReceiverList, Dictionary<string, string> mailBodyDictionary);
        void VerifySMTPStatus();
    }
}
