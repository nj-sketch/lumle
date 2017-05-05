using Lumle.Data.Data.Abstracts;
using Lumle.Module.Blog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Module.Blog.Data
{
    public class BlogCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<Article>()
                .ToTable("Blog_Article");

            builder.Entity<Article>()
                .HasKey(p => p.Id);

        }
    }
}
