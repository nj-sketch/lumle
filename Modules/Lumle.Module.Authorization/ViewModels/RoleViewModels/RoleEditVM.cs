using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Authorization.ViewModels.RoleViewModels
{
    public class RoleEditVM
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9\\-\\s]+$", ErrorMessage = "Only alphabet or numeric value are supported.")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        public string Description { get; set; }

        [Required]
        [Display(Name = "Role Priority")]
        public int Priority { get; set; }
    }
}
