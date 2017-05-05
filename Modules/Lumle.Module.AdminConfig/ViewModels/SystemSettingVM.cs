using System.Collections.Generic;

namespace Lumle.Module.AdminConfig.ViewModels
{
    public class SystemSettingVM
    {
        public int Id { get; set; }

        public string Slug { get; set; }

        public string Status { get; set; }

        public List<RoleHelper> Roles { get; set; }
    }

    /// <summary>
    /// Role helper class
    /// </summary>
    public class RoleHelper
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public bool IsBlocked { get; set; }
    }
}
