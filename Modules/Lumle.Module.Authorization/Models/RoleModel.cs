namespace Lumle.Module.Authorization.Models
{
    public class RoleModel
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int NumberOfUsers { get; set; }
        public int Priority { get; set; }
    }
}
