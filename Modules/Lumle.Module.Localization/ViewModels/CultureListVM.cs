using Lumle.Module.Localization.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Lumle.Module.Localization.ViewModels
{
    public class CultureListVM
    {
        public List<CultureModel> EnabledCultures { get; set; }
        public List<CultureModel> InActiveCultures { get; set; }
        public int InActiveCultureCount { get; set; }
        public bool CreateAction { get; set; } // Check for the create action previlege
    }

    public class CultureSelectListVM
    {
        public List<SelectListItem> CulturesList { get; set; }
    }
}
