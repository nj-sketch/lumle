using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Localization.Models
{
    public class ImportResourceModel
    {
        [Required]
        public string Culture { get; set; }
        public int ResourceCategoryId { get; set; }

        public ICollection<IFormFile> files { get; set; }
    }
}
