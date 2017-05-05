using Lumle.Data.Models;

namespace Lumle.Module.AdminConfig.Models
{
    public class CredentialModel : EntityBaseModel
    {
        public int Sn { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }

    }
}
