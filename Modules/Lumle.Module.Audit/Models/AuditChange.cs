using System;
using System.Collections.Generic;

namespace Lumle.Module.Audit.Models
{
    public class AuditChange
    {
        public int AuditId { get; set; }
        public string ActionPerformedBy { get; set; }
        public string CreatedDate { get; set; }
        public string AuditActionTypeName { get; set; }
        public List<AuditInfo> AuditInfos { get; set; }

        public AuditInfo AuditInfo { get; set; }
        public AuditChange()
        {
            AuditInfos = new List<AuditInfo>();
        }
    }
}
