using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Lumle.Data.Models
{
    public class Role: IdentityRole<string, UserRole, IdentityRoleClaim<string>>
    {

        public string Description { get; set; }

        [Required]
        public int Priority { get; set; }

        public bool IsBlocked { get; set; }
    }
}
