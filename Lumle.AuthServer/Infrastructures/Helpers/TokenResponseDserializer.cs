using System;
using Lumle.AuthServer.Infrastructures.Helpers.ContractResolver;
using Newtonsoft.Json;

namespace Lumle.AuthServer.Infrastructures.Helpers
{
    public class TokenResponseDserializer
    {
        public static T DserializeIdToken<T>(string responseData)
        {
            var authResponse = JsonConvert.DeserializeObject<T>(responseData,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new LowerCaseContractResolver()
                });

            return authResponse;
        }

        public static T DserializeAccessToken<T>(string responseData)
        {
            throw new NotImplementedException();
        }

    }
}
