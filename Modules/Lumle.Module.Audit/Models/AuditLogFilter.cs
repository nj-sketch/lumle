using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lumle.Module.Audit.Models
{
    public class AuditLogFilter
    {         
        public string ModuleSearch { get; set; }
        public string ActionSearch { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string UserSearch { get; set; }
        public string FieldSearch { get; set; }
        public List<SelectListItem> ModuleList { get; set; }
        public List<SelectListItem> ActionList { get; set; }
    }
}

