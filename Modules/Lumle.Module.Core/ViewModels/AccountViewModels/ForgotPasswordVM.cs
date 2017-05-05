using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Core.ViewModels.AccountViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
