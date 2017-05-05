using Lumle.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Audit.Entities
{
    public class AuditLog : EntityBase
    {
        [Required]
        public string AuditType { get; set; }
        [Required]
        public string TableName { get; set; }
        [Required]
        public string KeyField { get; set; }
        [Required]
        public string OldValue { get; set; }
        [Required]
        public string NewValue { get; set; }
        [Required]
        public string Changes { get; set; }
        [Required]
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string AuditSummary { get; set; }
        public string MachineName { get; set; }
        public string ModuleInfo { get; set; }
    }
}
