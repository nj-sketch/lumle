using JsonApiDotNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Lumle.Api.Infrastructures.Helpers
{
    public static class AppUtil
    {
        public static IActionResult UnprocessableEntity()
        {
            return new StatusCodeResult(422);
        }

        public static IActionResult Forbidden()
        {
            return new StatusCodeResult(403);
        }

        public static IActionResult Error(Error error)
        {
            var errorCollection = new ErrorCollection
            {
                Errors = new List<Error> { error }
            };
            var result = new ObjectResult(errorCollection);
            result.StatusCode = error.StatusCode;

            return result;
        }

        public static IActionResult Errors(ErrorCollection errors)
        {
            var result = new ObjectResult(errors);
            result.StatusCode = GetErrorStatusCode(errors);

            return result;
        }

        private static int GetErrorStatusCode(ErrorCollection errors)
        {
            var statusCodes = errors.Errors
                .Select(e => e.StatusCode)
                .Distinct()
                .ToList();

            if (statusCodes.Count == 1)
                return statusCodes[0];

            return int.Parse(statusCodes.Max().ToString()[0] + "00");
        }
    }
}
