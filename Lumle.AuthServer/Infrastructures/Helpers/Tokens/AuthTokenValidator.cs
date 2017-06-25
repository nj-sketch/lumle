using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Lumle.AuthServer.Infrastructures.Helpers.Constants;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Lumle.AuthServer.Infrastructures.Helpers.Tokens
{
    public static class AuthTokenValidator
    {

        public static async Task<JwtSecurityToken> ValidateGoogleIdTokenAsync(string idToken)
        {

            try
            {
                var client = new HttpClient { BaseAddress = new Uri(GoogleAuthConstants.GoogleMetaDataUrl) };

                // 1. Get Google signing keys
                var response = await client.GetAsync(
                    GoogleAuthConstants.X509CertificateEndpoint);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Unable to access google secure token Api.");
                }
                var x509Data = await response.Content.ReadAsDictionaryAsync();
                var keys = x509Data.Values.Select(CreateSecurityKeyFromPublicKey).ToArray();


                //2.Configure validation parameters
                const string firebaseProjectId = GoogleAuthConstants.FireBaseProjectId;
                var parameters = new TokenValidationParameters
                {
                    ValidIssuer = GoogleAuthConstants.GoogleSecureTokenBaseUrl + firebaseProjectId,
                    ValidAudience = firebaseProjectId,
                    IssuerSigningKeys = keys,
                };


                // 3. Use JwtSecurityTokenHandler to validate signature, issuer, audience and lifetime
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(idToken, parameters, out SecurityToken token);
                var jwt = (JwtSecurityToken)token;

                return jwt;
            }
            catch (Exception e)
            {
                throw new TimeoutException(e.Message);
            }
        }

        private static SecurityKey CreateSecurityKeyFromPublicKey(string data)
        {
            return new X509SecurityKey(new X509Certificate2(Encoding.UTF8.GetBytes(data)));
        }

        private static async Task<Dictionary<string, string>> ReadAsDictionaryAsync(this HttpContent content)
        {

            var decodedContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(await content
                .ReadAsStringAsync());

            return decodedContent;
        }


    }
}
