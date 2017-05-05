using System.Collections.Generic;

namespace Lumle.Module.Authorization.Models.PermissionModels
{
    public class SubModule
    {
        public string Name { get; set; }
        public IEnumerable<ModulePermission> ModulePermissions { get; set; }
           
    }
}
