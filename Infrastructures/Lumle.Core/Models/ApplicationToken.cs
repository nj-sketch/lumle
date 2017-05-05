using System;
using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Core.Models
{
    public class ApplicationToken : EntityBase
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string TokenType { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public bool IsUsed { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UsedDate { get; set; }

        public User User { get; set; }
    }
}
