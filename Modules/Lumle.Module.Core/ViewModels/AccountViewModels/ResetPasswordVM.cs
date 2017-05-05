using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Core.ViewModels.AccountViewModels
{
    public class ResetPasswordVM
    {
        public string UserId { get; set; }

        [Required]
        [RegularExpression("^(?=(.*\\d){1})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\\d]).{6,50}$", ErrorMessage = "Password must be 6 to 50 in length and must contains atleast one lower, upper,numeric and special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
