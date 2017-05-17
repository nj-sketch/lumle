using System.ComponentModel.DataAnnotations;

namespace Lumle.AuthServer.Models.Token
{
    public class RevokeToken
    {
        [Required]
        public string JwtId { get; set; }

        [Required]
        public string SubId { get; set; }
    }
}
