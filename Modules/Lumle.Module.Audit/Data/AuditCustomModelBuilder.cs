using Lumle.Data.Data.Abstracts;
using Microsoft.EntityFrameworkCore;
using Lumle.Module.Audit.Entities;

namespace Lumle.Module.Audit.Data
{
    public class AuditCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<AuditLog>()
                .ToTable("Audit_AuditLog");

            builder.Entity<CustomLog>(b =>
            {
                b.ToTable("Audit_CustomLog");
                b.Property(x => x.CreatedDate).HasDefaultValueSql("now()");
                b.Property(x => x.LastUpdated).HasDefaultValueSql("now()");
            });
        }
    }
}
