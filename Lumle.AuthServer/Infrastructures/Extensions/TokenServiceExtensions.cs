namespace Lumle.AuthServer.Infrastructures.Extensions
{

    /// <summary>
    /// Custom extension of string. Ref. from "StringExtensions"
    /// </summary>
    internal static class TokenServiceExtensions
    {
        public static string EnsureAudienceLeadingSlash(this string url)
        {
            if (url.StartsWith("/"))
            {
                return "/" + url;
            }
            else
            {
                return url + "/";
            }

            
        }

        public static bool IsValuePresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

    }
}
