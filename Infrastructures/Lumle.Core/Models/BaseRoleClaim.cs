using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Core.Models
{
    public class BaseRoleClaim : EntityBase
    {
        [Required]
        public string ClaimType { get; set; }
        [Required]
        public string ClaimValue { get; set; }
        [Required]
        public string RoleId { get; set; }
    }
}
