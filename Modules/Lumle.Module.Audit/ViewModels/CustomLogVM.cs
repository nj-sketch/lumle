namespace Lumle.Module.Audit.ViewModels
{
    public class CustomLogVM
    {
        public int Id;
        public int Sn { get; set; }
        public string Level { get; set; }
        public string RequestMethod { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public string ServerName { get; set; }
        public string Port { get; set; }
        public string Url { get; set; }
        public string ServerAddress { get; set; }
        public string RemoteAddress { get; set; }
        public string UserAgent { get; set; }
        public string CallSite { get; set; }
        public string Exception { get; set; }
        public string LoggedDate { get; set; }
    }
}
