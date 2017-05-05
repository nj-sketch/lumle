using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.PublicUser.ViewModels.PublicUserViewModels
{
    public class PublicUserEditVM
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string IsStaff { get; set; }
        [Required]
        public string IsBlocked { get; set; }
      
    }
}
