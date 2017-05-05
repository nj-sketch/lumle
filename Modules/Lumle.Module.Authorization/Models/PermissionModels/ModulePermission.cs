namespace Lumle.Module.Authorization.Models.PermissionModels
{
    public class ModulePermission
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string DisplayName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
