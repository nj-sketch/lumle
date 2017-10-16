using Lumle.Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Authorization.ViewModels.UserViewModels
{
    public class UserVM
    {
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please select time zone")]
        public string TimeZone { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "Role")]
        [Required(ErrorMessage ="Please select role")]
        public string RoleId { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public CountryEnum EnumCountry { get; set; }

        public StateEnum EnumState { get; set; }
    }
}
