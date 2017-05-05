using System.Threading.Tasks;
using Lumle.Infrastructure.Constants.Email;

namespace Lumle.Infrastructure.Utilities.Abstracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            SmtpOptions smtpOptions,
            string to,
            string from,
            string subject,
            string plainTextMessage,
            string htmlMessage,
            string replyTo = null,
            Importance importance = Importance.Normal
        );

        Task SendMultipleEmailAsync(
            SmtpOptions smtpOptions,
            string toCsv,
            string from,
            string subject,
            string plainTextMessage,
            string htmlMessage,
            string replyTo = null,
            Importance importance = Importance.Normal
        );
    }
}
