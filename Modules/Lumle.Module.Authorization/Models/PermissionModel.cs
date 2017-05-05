using System;
using Lumle.Data.Models;

namespace Lumle.Module.Authorization.Models
{
    public class PermissionModel : EntityBaseModel
    {
        public string Slug { get; set; }

        public string DisplayName { get; set; }

        public string Menu { get; set; }

        public string SubMenu { get; set; }
    }
}
