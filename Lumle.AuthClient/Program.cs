using IdentityModel.Client;
using System;
using System.Threading.Tasks;

namespace Lumle.AuthClient
{
    class Program
    {
        static void Main(string[] args)
        {

            Task.Run(async () => { await GetDataAsync(); });
        
            Console.ReadLine();
        }


        private async static Task GetDataAsync()
        {
            var discoveryClient = new DiscoveryClient("http://localhost:30193");
            var doc = await discoveryClient.GetAsync();

            var tokenEndpoint = doc.TokenEndpoint;
            var keys = doc.KeySet.Keys;

            var access_token = @"eyJhbGciOiJSUzI1NiIsImtpZCI6IjU4OTRkZDNmYTI3OWYzZjM1MTk3ZTdmN2EzOGE4MWJiIiwidHlwIjoiSldUIn0.eyJuYmYiOjE0OTQzMjExMDIsImV4cCI6MTQ5NDMyNDcwMiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDozMDE5MyIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjMwMTkzL3Jlc291cmNlcyIsImFwaTIiXSwiY2xpZW50X2lkIjoibHVtbGUtbWJsIiwic3ViIjoiMGM0ZDViNDMtZjI5Yy00ZjdiLWJjODgtMDA0MWEwNmZkYzc4IiwiYXV0aF90aW1lIjoxNDk0MzIwOTQxLCJpZHAiOiJsb2NhbCIsImlzU3RhZmYiOiJUcnVlIiwiaXNFbWFpbFZlcmlmaWVkIjoiVHJ1ZSIsImVtYWlsIjoiamFuYWtAZWtiYW5hLmNvbSIsImp0aSI6IjNlNDAzNGZkYjI1NDNiOTY3Y2VhNWM1ZDFlNzdkZGJlIiwic2NvcGUiOlsiYXBpMi5mdWxsX2FjY2VzcyIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwd2QiXX0.KkMIV9oaEAuFlzksYNhY4CaIr4omKp8RrBWTNFoTH1o-TMc5s2jFyA4SYXHWGFOno5B1NkKBFJb5uXLHNyemguQjJzTYo4i-NYIAwhj3bVUdLX2TEWfDVK6l5GdDicax_P-CSTFrWAK11Z1YQ_dpGRVaRlAp-liqOEjx3bbuX4ZtkJeRL3wYsMjWOmKZNalThj3a112nzmFKcY_bKNYRJWDAognTkJ25kp-sw_R2T3593aCR521ZuoBQBUw-OZTaDdePwH3sKuYghSx7GVjI9ncdXUF0-Z19nmHa2MBPanuR3xTcF8DWlTbSpgISNxJX0jmR934UuqatFS9IFcpJ1Q";


            var introspectionClient = new IntrospectionClient(
                doc.IntrospectionEndpoint,
                "api2",
                "secret");

            var response = await introspectionClient.SendAsync(
                new IntrospectionRequest { Token = access_token });

            var isActice = response.IsActive;
            var claims = response.Claims;

            Console.WriteLine(isActice);
            Console.WriteLine(response.Raw);
        }
    }
}