using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Authorization.ViewModels.PermissionViewModels
{
    public class PermissionVM
    {
        public int Id { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        public string DisplayName { get; set; }
    }
}
