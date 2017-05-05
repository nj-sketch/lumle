using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Authorization.ViewModels.UserViewModels
{
    public class EditUserVM
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage ="Please select role")]
        [Display(Name = "Role")]
        public string RoleId { get; set; }

        [Required(ErrorMessage = "Please select time zone")]
        public string TimeZone { get; set; }

        [Required]
        public int AccountStatus { get; set; }
    }
}
