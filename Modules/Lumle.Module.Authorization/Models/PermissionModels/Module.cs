using System.Collections.Generic;

namespace Lumle.Module.Authorization.Models.PermissionModels
{
    public class Module
    {
        public string Name { get; set; }
        public IEnumerable<SubModule> SubModules { get; set; }
    }
}
