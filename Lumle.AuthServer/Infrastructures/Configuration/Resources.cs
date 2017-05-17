using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace Lumle.AuthServer.Infrastructures.Configuration
{
    public class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
                // some standard scopes from the OIDC spec
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),

                // custom identity resource with some consolidated claims
                new IdentityResource("custom.profile", new[] { JwtClaimTypes.Name, JwtClaimTypes.Email, "location" })
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {   
                // expanded version if more control is needed
                new ApiResource
                {
                    Name = "LumleApi",

                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    UserClaims =
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email
                    },

                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "LumleApi.full_access",
                            DisplayName = "Full access to Lumle API.",
                        },
                        new Scope
                        {
                            Name = "LumleApi.read_only",
                            DisplayName = "Read only access to Lumle API."
                        }
                    }
                }
            };
        }
    }
}
