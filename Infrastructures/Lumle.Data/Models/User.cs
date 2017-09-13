using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Lumle.Data.Models
{
    public class User: IdentityUser
    {
        [Required]
        public int AccountStatus { get; set; } //0->Disable, 1-> Enable

        public string CreatedBy { get; set; }

        [Required]
        [MaxLength(100)]
        public string TimeZone { get; set; }

        [Required]
        [MaxLength(20)]
        public string Culture { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<UserRole> Roles { get; } = new List<UserRole>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<int>> Claims { get; } = new List<IdentityUserClaim<int>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<int>> Logins { get; } = new List<IdentityUserLogin<int>>();

    }
}
