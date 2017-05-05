using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.ViewModels
{
    public class SystemSettingUpdateVM
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public string[] Roles { get; set; }
    }
}
