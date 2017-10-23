namespace Lumle.Module.Authorization.Models
{
    public class AuthorizationModel
    {
        public string RoleId { get; set; }

        public string ModuleName { get; set; }

        public string ClaimValue { get; set; }

        public bool IsAssigned { get; set; }

    }
}
