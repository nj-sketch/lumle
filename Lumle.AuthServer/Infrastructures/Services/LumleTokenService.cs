using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Lumle.AuthServer.Infrastructures.Extensions;
using Lumle.AuthServer.Infrastructures.Helpers.Constants;
using Lumle.AuthServer.Data.Interfaces;
using Lumle.AuthServer.Data.Contexts;
using Lumle.AuthServer.Data.Entities;
using System.Linq;

namespace Lumle.AuthServer.Infrastructures.Services
{
    public class LumleTokenService : DefaultTokenService
    {

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The HTTP context accessor
        /// </summary>
        protected readonly new IHttpContextAccessor Context;

        /// <summary>
        /// The claims provider
        /// </summary>
        protected readonly new IClaimsService ClaimsProvider;

        /// <summary>
        /// The reference token store
        /// </summary>
        protected readonly new IReferenceTokenStore ReferenceTokenStore;

        /// <summary>
        /// The signing service
        /// </summary>
        protected readonly new ITokenCreationService CreationService;

        /// <summary>
        /// The events service
        /// </summary>
        protected readonly new IEventService Events;

        /// <summary>
        /// User DbContext
        /// </summary>
        public readonly UserDbContext UserContext;

        public LumleTokenService(IHttpContextAccessor context, IClaimsService claimsProvider, IReferenceTokenStore referenceTokenStore, ITokenCreationService creationService, IEventService events, ILogger<DefaultTokenService> logger, UserDbContext userDbContext) : base(context, claimsProvider, referenceTokenStore, creationService, events, logger)
        {
            Logger = logger;
            Context = context;
            ClaimsProvider = claimsProvider;
            ReferenceTokenStore = referenceTokenStore;
            CreationService = creationService;
            Events = events;
            UserContext = userDbContext;
        }


        //Override other Function as per needed
        public override async Task<Token> CreateAccessTokenAsync(TokenCreationRequest request)
        {
            Logger.LogTrace("Creating access token");
            ValidateRequest(request);

            var claims = new List<Claim>();
            claims.AddRange(await ClaimsProvider.GetAccessTokenClaimsAsync(
                request.Subject,
                request.ValidatedRequest.Client,
                request.Resources,
                request.ValidatedRequest));

            if (request.ValidatedRequest.Client.IncludeJwtId)
            {
                claims.Add(new Claim(JwtClaimTypes.JwtId, CryptoRandom.CreateUniqueId(16)));
            }

            var issuer = Context.HttpContext.GetIdentityServerIssuerUri();
            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                Audiences = { $"{issuer.EnsureAudienceLeadingSlash()}{IdSrvConstants.AccessTokenAudience}"},
                Issuer = issuer,
                Lifetime = request.ValidatedRequest.AccessTokenLifetime,
                Claims = claims,
                ClientId = request.ValidatedRequest.Client.ClientId,
                AccessTokenType = request.ValidatedRequest.AccessTokenType
            };

            foreach (var api in request.Resources.ApiResources)
            {
                if (api.Name.IsValuePresent())
                {
                    token.Audiences.Add(api.Name);
                }
            }

            //Save Token in token snapshot table
            var tokenSnapShot = new TokenSnapShot
            {
                JwtId = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.JwtId).Value,
                IsActive = true,
                SubId = token.SubjectId,
                CreatedDate = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.AddHours(TotalHours(token.Lifetime)),
                LastUpdated = DateTime.UtcNow
            };

            UserContext.TokenSnapShots.Add(tokenSnapShot);
            await UserContext.SaveChangesAsync();



            return token;
        }

        private void ValidateRequest(TokenCreationRequest request)
        {
            if (request.Resources == null) LogAndStop("resources");
            if (request.ValidatedRequest == null) LogAndStop("validatedRequest");

        }

        private void LogAndStop(string name)
        {
            throw new ArgumentNullException(name);
        }

        private double TotalHours(int totalSeconds)
        {
            return totalSeconds / 3600; //totalseconds/totalsecondsinhour
        }

    }
}
