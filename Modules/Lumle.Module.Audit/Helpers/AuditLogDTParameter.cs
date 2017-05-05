using Lumle.Infrastructure.Helpers;

namespace Lumle.Module.Audit.Helpers
{
    public class AuditLogDTParameter : DataTableHelper.DTParameters
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ModuleName { get; set; }
        public string ActionName { get; set; }
        public string Username { get; set; }
        public string FieldName { get; set; }
    }
}
