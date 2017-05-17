using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumle.AuthServer.Data.Entities
{
    public class TokenSnapShot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Column(TypeName = "bigserial ")]
        public int Id { get; set; }

        [Required]
        public string JwtId { get; set; }
        [Required]
        public string SubId { get; set; }
        [Required]
        public bool IsActive { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ExpireDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; }
    }
}
