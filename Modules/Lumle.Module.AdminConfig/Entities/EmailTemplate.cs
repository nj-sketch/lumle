using Lumle.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.AdminConfig.Entities
{
    public class EmailTemplate : EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string  Slug { get; set; }

        [Required]
        [MaxLength(200)]
        public string LastSlugDisplayName { get; set; }

        [Required]
        [MaxLength(200)]
        public string DefaultSlugDisplayName { get; set; }

        [Required]
        [MaxLength(200)]
        public string SlugDisplayName { get; set; }

        [Required]
        [MaxLength(500)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(500)]
        public string LastSubject { get; set; }

        [Required]
        [MaxLength(500)]
        public string DefaultSubject { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public string LastBody { get; set; }

        [Required]
        public string DefaultBody { get; set; }

    }
}
