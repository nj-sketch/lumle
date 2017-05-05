using System;

namespace Lumle.Data.Models
{
    public class EntityBaseModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
