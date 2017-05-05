using System;
using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Core.Models
{
    public class UserProfile : EntityBase
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [Phone]
        public string Phone { get; set; }

        [MaxLength(500)]
        public string Website { get; set; }

        public string AboutMe { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime DeletedDate { get; set; }

    }
}
