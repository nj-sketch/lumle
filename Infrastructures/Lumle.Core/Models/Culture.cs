using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Core.Models
{
    public class Culture: EntityBase
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string DisplayName { get; set; }
        [Required]
        public bool IsEnable { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public IList<Resource> Resources { get; set; }
    }
}
