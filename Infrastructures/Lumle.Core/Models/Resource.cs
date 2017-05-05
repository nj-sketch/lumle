using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Core.Models
{
    public class Resource : EntityBase
    {
        [Required]
        public int CultureId { get; set; }
        [Required]
        public int ResourceCategoryId { get; set; }
        [Required]
        public string Key { get; set; }
        public string Value { get; set; }
        public Culture Culture { get; set; }
        public ResourceCategory ResourceCategory { get; set; }
    }
}
