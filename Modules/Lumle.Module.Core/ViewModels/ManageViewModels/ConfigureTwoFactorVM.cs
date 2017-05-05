using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lumle.Module.Core.ViewModels.ManageViewModels
{
    public class ConfigureTwoFactorVM
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }
    }
}
