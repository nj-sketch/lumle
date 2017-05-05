namespace Lumle.Infrastructure
{
    public class TwilioSmsCredentials
    {
        public string AccountSid { get; set; }
        public string Token { get; set; }
        public string BaseUri { get; set; }
        public string RequestUri { get; set; }
        public string From { get; set; }
    }
}
