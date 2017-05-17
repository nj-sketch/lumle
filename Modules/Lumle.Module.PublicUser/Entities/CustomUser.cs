using Lumle.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.PublicUser.Entities
{
    public class CustomUser : EntityBase
    {
        [Required]
        public string SubjectId { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string UserName { get; set; }

        public string PhoneNo { get; set; }

        public string ProfileUrl { get; set; }

        public int Gender { get; set; }

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
