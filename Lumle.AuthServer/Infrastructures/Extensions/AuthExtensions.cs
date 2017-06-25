using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Lumle.AuthServer.Infrastructures.GrantTypes.Response;

namespace Lumle.AuthServer.Infrastructures.Extensions
{
    public static class AuthExtensions
    {

        public static GoogleFirebaseAuthResponse MapToGoogleAuthResponse(this JwtSecurityToken securityToken)
        {
            if (securityToken == null)
            {
                throw new NullReferenceException("security token is invalid");
            }

            var securityClaims = securityToken.Claims.ToList();

            var googleAuthResponse = new GoogleFirebaseAuthResponse
            {
                User_id = securityClaims.FirstOrDefault(x => x.Type == "user_id")?.Value,
                Sub = securityClaims.FirstOrDefault(x => x.Type == "sub")?.Value,
                Aud = securityClaims.FirstOrDefault(x => x.Type == "aud")?.Value,
                Auth_time = int.Parse(securityClaims.FirstOrDefault(x => x.Type == "auth_time")?.Value),
                Iat = int.Parse(securityClaims.FirstOrDefault(x => x.Type == "iat")?.Value),
                Exp = int.Parse(securityClaims.FirstOrDefault(x => x.Type == "exp")?.Value),
                Iss = securityClaims.FirstOrDefault(x => x.Type == "iss")?.Value,
                Email = securityClaims.FirstOrDefault(x => x.Type == "email")?.Value,
                Name = securityClaims.FirstOrDefault(x => x.Type == "name")?.Value,
                Picture = securityClaims.FirstOrDefault(x => x.Type == "picture")?.Value,

            };

            return googleAuthResponse;
        }


    }
}
