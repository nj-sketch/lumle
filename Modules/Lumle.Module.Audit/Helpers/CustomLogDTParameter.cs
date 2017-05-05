using Lumle.Infrastructure.Helpers;

namespace Lumle.Module.Audit.Helpers
{
    public class CustomLogDTParameter : DataTableHelper.DTParameters
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LogLevel { get; set; }
        public string Username { get; set; }
        public string UserAgent { get; set; }
    }
}
