using Lumle.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.AdminConfig.Entities
{
    public class CredentialCategory:EntityBase
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public List<Credential> Credentials { get; set; }


    }
}
