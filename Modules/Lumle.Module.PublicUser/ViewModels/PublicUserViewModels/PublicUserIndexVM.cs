using System;

namespace Lumle.Module.PublicUser.ViewModels.PublicUserViewModels
{
    public class PublicUserIndexVM
    {
        public int SN { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string ProfileUrl { get; set; }
        public string Gender { get; set; }
        public bool IsStaff { get; set; }
        public string Provider { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FormatedCreatedDate { get; set; }
    }
}
