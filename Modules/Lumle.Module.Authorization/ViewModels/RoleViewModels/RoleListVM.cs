using Lumle.Core.Models;
using Lumle.Module.Authorization.Models;
using System.Collections.Generic;

namespace Lumle.Module.Authorization.ViewModels.RoleViewModels
{
    public class RoleListVM
    {
        public List<RoleModel> RoleModels { get; set; }
        public ActionOperation ActionOperation { get; set; }
    }
}
