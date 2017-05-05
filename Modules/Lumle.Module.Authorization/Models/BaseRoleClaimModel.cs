using Lumle.Data.Models;

namespace Lumle.Module.Authorization.Models
{
    public class BaseRoleClaimModel : EntityBaseModel
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string RoleId { get; set; }
    }
}
