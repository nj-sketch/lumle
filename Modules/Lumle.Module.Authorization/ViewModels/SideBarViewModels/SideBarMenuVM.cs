using System.Collections.Generic;

namespace Lumle.Module.Authorization.ViewModels.SideBarViewModels
{
    public class SideBarMenuVM
    {
        public string Menu { get; set; }
        public string MenuDisplayName { get; set; }
        public string Icon { get; set; }
        public int Sequence { get; set; }
        public List<SideBarSubMenuVM> SubMenu { get; set; }
    }
}
