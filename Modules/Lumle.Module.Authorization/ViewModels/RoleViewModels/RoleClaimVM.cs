using System;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Authorization.ViewModels.RoleViewModels
{
    //[Serializable]
    public class RoleClaimVM
    {
        [Required]
        public string[] ClaimValues { get; set; }

        [Required]
        public string  RoleId{ get; set; }
    }
}
