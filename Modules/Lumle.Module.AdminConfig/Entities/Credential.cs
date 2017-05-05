using Lumle.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.AdminConfig.Entities
{
    public class Credential :EntityBase
    {
        [Required]
        public int CredentialCategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; }

        [Required]
        [MaxLength(250)]
        public string DisplayName { get; set; }
        
        [MaxLength(1000)]
        public string Value { get; set; }

        public CredentialCategory CredentialCategory { get; set; }
    }
}
