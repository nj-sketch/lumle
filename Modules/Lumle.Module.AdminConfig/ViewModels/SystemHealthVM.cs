using Lumle.Module.AdminConfig.Models;
using System.Collections.Generic;

namespace Lumle.Module.AdminConfig.ViewModels
{
    public class SystemHealthVM
    {
        public IEnumerable<CredentialCategoryModel> Categories { get; set; }
        public string LastCheckedDate { get; set; }
    }
}
