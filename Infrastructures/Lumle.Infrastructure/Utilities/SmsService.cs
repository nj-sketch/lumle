using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Lumle.Infrastructure.Constants.Messenging;
using Lumle.Infrastructure.Utilities.Abstracts;
using Microsoft.Extensions.Logging;

namespace Lumle.Infrastructure.Utilities
{
    public class SmsService : ITwilioSmsService
    {

        public async Task SendMessageAsync(
            TwilioSmsCredentials credentials,
            string toPhoneNumber,
            string message)
        {
            if (string.IsNullOrWhiteSpace(toPhoneNumber))
            {
                throw new ArgumentException("no to phone number provided");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("no message  provided");
            }

            if (string.IsNullOrWhiteSpace(credentials.AccountSid)|| string.IsNullOrWhiteSpace(credentials.BaseUri)|| 
                string.IsNullOrWhiteSpace(credentials.From)|| string.IsNullOrWhiteSpace(credentials.RequestUri)||
                string.IsNullOrWhiteSpace(credentials.Token))
            {
                throw new ArgumentException("no all credentials provided" );
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = CreateBasicAuthenticationHeader(
                credentials.AccountSid,
                credentials.Token);

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("To", toPhoneNumber),
                new KeyValuePair<string, string>("From", credentials.From),
                new KeyValuePair<string, string>("Body", message)
            };

            var content = new FormUrlEncodedContent(keyValues);

            var endPoint = new Uri(new Uri(credentials.BaseUri), credentials.RequestUri.Replace("{twilioaccountsid}",credentials.AccountSid)).ToString();
            var postUrl = string.Format(
                    CultureInfo.InvariantCulture,
                     endPoint,
                    credentials.AccountSid);

            var response = await client.PostAsync(
                postUrl,
                content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                //
            }
            else
            {
               // 
            }

        }


        #region Helpers
        private AuthenticationHeaderValue CreateBasicAuthenticationHeader(string username, string password)
        {
            return new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes(
                     string.Format("{0}:{1}", username, password)
                     )
                 )
            );
        }
        #endregion


    }
}
