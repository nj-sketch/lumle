using System;

namespace Lumle.Module.Audit.ViewModels
{
    public class AuditLogVM
    {
        public int Id { get; set; }
        public int Sn { get; set; }
        public string AuditType { get; set; }
        public string TableName { get; set; }
        public string KeyField { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string AuditSummary { get; set; }
        public string MachineName { get; set; }
        public string ModuleInfo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ConvertedCreatedDate { get; set; }
    }
}
