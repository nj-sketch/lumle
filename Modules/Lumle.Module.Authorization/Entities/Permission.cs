using Lumle.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Authorization.Entities
{
    public class Permission : EntityBase
    { 
        [Required]
        public string Slug { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Menu { get; set; }

        public string SubMenu { get; set; }
    }
}
