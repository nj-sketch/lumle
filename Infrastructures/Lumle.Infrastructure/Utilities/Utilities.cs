using System;
using System.Text.RegularExpressions;
using Lumle.Infrastructure.Models;
using Lumle.Infrastructure.Utilities.Abstracts;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Lumle.Infrastructure.Utilities
{
    public class Utilities : IUtilities
    {
        private readonly IActionContextAccessor _accessor;

        public Utilities(IActionContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public ClientInfo GetClientInformation()
        {
            var browserInfo = GetBrowserName(_accessor.ActionContext.HttpContext.Request.Headers["User-Agent"].ToString());
            var ip = _accessor.ActionContext.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            return new ClientInfo { Browser = browserInfo, Ip = ip};
        }

        #region Helpers
        private static string GetBrowserName(string userAgent)
        {
            string browserName;
            const string pattern = @"(opera|chrome|safari|firefox|msie)\/?\s*(\.?\d+(\.\d+)*)";
            const string pattern2 = @"^([^\/]*)";
            const RegexOptions options = RegexOptions.IgnoreCase;
            if (userAgent.IndexOf("Edge", StringComparison.Ordinal) > -1)
            {
                browserName = "Edge";
            }
            else
            {
                var m = Regex.Match(userAgent, pattern, options);
                browserName = m.Value;
                browserName = Regex.Match(browserName, pattern2, options).ToString();
            }

            return browserName;
        }
        #endregion
    }
}
