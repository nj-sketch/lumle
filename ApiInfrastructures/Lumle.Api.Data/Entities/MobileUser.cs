using JsonApiDotNetCore.Models;
using Lumle.Api.Data.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Api.Data.Entities
{
    public class MobileUser: EntityBase
    {
        [Required]
        [Attr("subject-id")]
        public string SubjectId { get; set; }

        [DataType(DataType.EmailAddress)]
        [Attr("email")]
        public string Email { get; set; }

        [Attr("first-name")]
        public string FirstName { get; set; }

        [Attr("last-name")]
        public string LastName { get; set; }

        [Attr("user-name")]
        public string UserName { get; set; }

        [Attr("phone-no")]
        public string PhoneNo { get; set; }

        [Attr("profile-url")]
        public string ProfileUrl { get; set; }

        [Attr("gender")]
        public int Gender { get; set; } //1 = Male, 2 = Female, 3 = Other, 0= unknown

        [Required]
        [Attr("password-hash")]
        public string PasswordHash { get; set; }

        [Required]
        [Attr("password-salt")]
        public string PasswordSalt { get; set; }

        [Required]
        [Attr("isstaff")]
        public bool IsStaff { get; set; }

        [Required]
        [Attr("provider")]
        public string Provider { get; set; }

        [Required]
        [Attr("isemailverified")]
        public bool IsEmailVerified { get; set; }

        [Required]
        [Attr("isblocked")]
        public bool IsBlocked { get; set; }
    }
}
