using Lumle.Data.Models;

namespace Lumle.Module.AdminConfig.Entities
{
    public class ServiceHealth : EntityBase
    {
        public string Message { get; set; }
        public string ServiceName { get; set; }
        public bool Status { get; set; }
        public int SystemHealthId { get; set; }
        public virtual SystemHealth SystemHealth { get; set; }
    }
}
