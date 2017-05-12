using System.Collections.Generic;

namespace Lumle.Module.Authorization.Models
{
    public class SidebarMenuModel
    {
        public string Menu { get; set; }
        public string MenuDisplayName { get; set; }
        public string Icon { get; set; }
        public int Sequence { get; set; }
        public List<SubMenuModel> SubMenu { get; set; }
    }
}
