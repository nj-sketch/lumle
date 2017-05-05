using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Core.Models
{ 
    public class ResourceCategory :EntityBase
    {
        [Required]
        public string Name { get; set; }
        public IList<Resource> Resources { get; set; }
    }
}
