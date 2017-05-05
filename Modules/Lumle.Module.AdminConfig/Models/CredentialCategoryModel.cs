using Lumle.Data.Models;

namespace Lumle.Module.AdminConfig.Models
{
    public class CredentialCategoryModel : EntityBaseModel
    {
        public int Sn { get; set; }
        public string Name { get; set; }
        public string NameIdentifier { get; set; }
    }
}
