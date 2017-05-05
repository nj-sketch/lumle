using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Core.ViewModels.AccountViewModels
{
    public class ExternalLoginConfirmationVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Username { get; set; }

    }
}
