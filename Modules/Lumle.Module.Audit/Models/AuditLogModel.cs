using System.Collections.Generic;
using Lumle.Infrastructure.Utilities;
using Lumle.Module.Audit.Enums;

namespace Lumle.Module.Audit.Models
{
    public class AuditLogModel
    {
        public AuditActionType AuditActionType { get; set; }
        public object OldObject { get; set; }      
        public object NewObject { get; set; }
        public string KeyField { get; set; }
        public string LoggedUserEmail { get; set; }
        public string LoggedUserTimeZone { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public string OldString { get; set; }
        public string NewString { get; set; }
        public ModuleList ModuleList { get; set; }
        public List<string> OldStringList { get; set; }
        public List<string> NewStringList { get; set; }
        public string AdditionalInfo { get; set; }
    }

   
}
