using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Core.ViewModels.ManageViewModels
{
    public class AddPhoneNumberVM
    {
        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
