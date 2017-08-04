using Lumle.Api.Data.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Api.Data.Entities
{
    public class Place: EntityBase
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }
    }
}
