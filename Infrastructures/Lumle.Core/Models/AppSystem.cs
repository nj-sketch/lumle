using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Core.Models
{
    public class AppSystem : EntityBase
    {
        [Required]
        public string Slug { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string LastUpdatedBy { get; set; }
    }
}
