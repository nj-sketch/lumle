using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Lumle.Api.Infrastructures.Handlers.ApiResponse.Models;

namespace Lumle.Api.Infrastructures.Extensions
{
    public static class AppExtensions
    {

        /// <summary>
        /// Add application error in API
        /// </summary>
        /// <param name="response"></param>
        /// <param name="message"></param>
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("access-control-expose-headers", "Application-Error");
        }

        /// <summary>
        /// Get validation errors from model state
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static IEnumerable<ShortMessage> GetErrors(this ModelStateDictionary modelState)
        {
            var errors = from modelstate in modelState.Where(f => f.Value.Errors.Count > 0)
                select new ShortMessage { Source = modelstate.Key.ToLower(), Title = modelstate.Value.Errors.FirstOrDefault().ErrorMessage };

            return errors;
        }

        /// <summary>
        /// Get subjectId of current user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetCurrentUserSubjectId(this ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
                return user.Claims.FirstOrDefault(x => x.Type == "sub").Value;

            throw new System.Exception("User not authenticated.");
        }


    }
}
