using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Lumle.Data.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        public override string UserId { get; set; }

        public User User { get; set; }

        public override string RoleId { get; set; }

        public Role Role { get; set; }
    }
}
