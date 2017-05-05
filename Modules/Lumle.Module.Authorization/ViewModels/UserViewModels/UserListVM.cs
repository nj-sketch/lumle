using System;

namespace Lumle.Module.Authorization.ViewModels.UserViewModels
{
    public class UserListVM
    {
        public int SN { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int AccountStatus { get; set; }
        public bool EmailConfirmed { get; set; }
        public string RoleName { get; set; }
        public int RolePriority { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
