using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lumle.Module.Audit.Models
{
    public class CustomLogFilter
    {
        public string LogLevelSearch { get; set; }
        public List<SelectListItem> LogLevels { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Select Log Level" },
            new SelectListItem { Value = "Info", Text = "Info" },
            new SelectListItem { Value = "Error", Text = "Error" },
            new SelectListItem { Value = "Fatal", Text = "Fatal" }
        };
        public string UsernameSearch { get; set; }
        public string BrowserSearch { get; set; }
        public List<SelectListItem> BrowserList { get; } = new List<SelectListItem>
        {
            new SelectListItem {Value = "", Text = "Select Browser"},
            new SelectListItem {Value = "Chrome", Text = "Google Chrome"},
            new SelectListItem {Value = "Firefox", Text = "Mozilla Firefox"},
            new SelectListItem {Value = "Safari", Text = "Safari"},
            new SelectListItem {Value = "Edge", Text = "Microsoft Edge"}
        };
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
