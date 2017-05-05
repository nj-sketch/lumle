using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Module.AdminConfig.Entities
{
    public class SystemHealth : EntityBase
    {
        [Required]
        public string Username { get; set; }
        public virtual ICollection<ServiceHealth> ServiceHealths { get; set; }
    }
}
