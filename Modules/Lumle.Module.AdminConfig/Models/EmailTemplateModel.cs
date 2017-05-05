using Lumle.Data.Models;

namespace Lumle.Module.AdminConfig.Models
{
    public class EmailTemplateModel : EntityBaseModel
    {
        public int Sn { get; set; }
        public string Slug { get; set; }
        public string SlugDisplayName { get; set; }
        public string Subject { get; set; }
    }
}
