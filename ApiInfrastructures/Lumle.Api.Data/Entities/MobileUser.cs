using Lumle.Api.Data.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Api.Data.Entities
{
    public class MobileUser: EntityBase
    {
        [Required]
        public string SubjectId { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string UserName { get; set; }
        
        public string PhoneNo { get; set; }
        
        public string ProfileUrl { get; set; }
        
        public int Gender { get; set; } //1 = Male, 2 = Female, 3 = Other, 0= unknown

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string PasswordSalt { get; set; }

        [Required]
        public bool IsStaff { get; set; }

        [Required]
        public string Provider { get; set; }

        [Required]
        public bool IsEmailVerified { get; set; }

        [Required]
        public bool IsBlocked { get; set; }
    }
}
