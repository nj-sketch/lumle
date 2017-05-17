using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

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
        public static IEnumerable<string> GetErrors(this ModelStateDictionary modelState)
        {
            var stateValues = modelState.Values;


            return (from errorItem in stateValues from error in errorItem.Errors select error.ErrorMessage).ToList();
        }


    }
}
