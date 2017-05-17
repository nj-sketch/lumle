using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumle.AuthServer.Data.Entities
{
    public class MobileUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Column(TypeName = "bigserial ")]
        public int Id { get; set; }

        [Required]
        public string SubjectId { get; set; }
        
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string UserName { get; set; }

        public string PhoneNo { get; set;}

        public string ProfileUrl { get; set; }

        public int Gender { get; set; } //1 = Male, 2 = Female, 3 = Other, 0= Unknown

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

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; }
    }
}
