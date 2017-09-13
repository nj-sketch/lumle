using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Lumle.Data.Models
{
    public class Role: IdentityRole<string>
    {

        public string Description { get; set; }

        [Required]
        public int Priority { get; set; }

        public bool IsBlocked { get; set; }

        /// <summary>
        /// Navigation property for the users in this role.
        /// </summary>
        public virtual ICollection<UserRole> Users { get; } = new List<UserRole>();

        /// <summary>
        /// Navigation property for claims in this role.
        /// </summary>
        public virtual ICollection<IdentityRoleClaim<string>> Claims { get; } = new List<IdentityRoleClaim<string>>();
    }
}
