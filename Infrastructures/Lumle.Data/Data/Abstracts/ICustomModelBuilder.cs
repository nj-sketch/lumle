using Microsoft.EntityFrameworkCore;

namespace Lumle.Data.Data.Abstracts
{
    public interface ICustomModelBuilder
    {
        void Build(ModelBuilder modelBuilder);
    }
}
