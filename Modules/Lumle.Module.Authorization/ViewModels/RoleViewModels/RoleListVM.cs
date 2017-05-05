namespace Lumle.Module.Authorization.ViewModels.RoleViewModels
{
    public class RoleListVM
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int NumberOfUsers { get; set; }
        public int Priority { get; set; }
    }
}
