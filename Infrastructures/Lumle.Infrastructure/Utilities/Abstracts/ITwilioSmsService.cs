using System.Threading.Tasks;

namespace Lumle.Infrastructure.Utilities.Abstracts
{
    public interface ITwilioSmsService
    {
        /// <summary>
        /// Send an sms message using Twilio REST API
        /// </summary>
        /// <param name="credentials">TwilioSmsCredentials</param>
        /// <param name="toPhoneNumber">E.164 formatted phone number, e.g. +16175551212</param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageAsync(
            TwilioSmsCredentials credentials,
            string toPhoneNumber,
            string message);
    }
}
