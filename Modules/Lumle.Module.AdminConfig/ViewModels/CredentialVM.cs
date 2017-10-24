using Lumle.Module.AdminConfig.Models;
using System.Collections.Generic;

namespace Lumle.Module.AdminConfig.ViewModels
{
    public class CredentialVM
    {
        public List<CredentialModel> CredentialModels { get; set; }
        public bool UpdateAction { get; set; } // Denotes the update previlege according to the user role
    }
}
