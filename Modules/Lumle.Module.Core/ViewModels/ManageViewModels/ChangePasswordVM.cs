using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Core.ViewModels.ManageViewModels
{
    public class ChangePasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [RegularExpression("^(?=(.*\\d){1})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z\\d]).{6,50}$", ErrorMessage = "Password must be 6 to 50 in length and must contains atleast one lower, upper,numeric and special character.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
