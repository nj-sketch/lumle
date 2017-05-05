using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Core.ViewModels.ManageViewModels
{
    public class VerifyPhoneNumberVM
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
