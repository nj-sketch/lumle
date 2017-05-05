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

        //[Required]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        //[Display(Name = "Confirm Password")]
        //[DataType(DataType.Password)]
        //[Required]
        //[Compare("Password",ErrorMessage ="Password doesnot match")]
        //public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please select time zone")]
        public string TimeZone { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "Role")]
        [Required(ErrorMessage ="Please select role")]
        public string RoleId { get; set; }

    }
}
