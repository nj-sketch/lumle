using AutoMapper;
using Lumle.Module.Blog.Entities;
using Lumle.Module.Blog.Models;

namespace Lumle.Module.Blog.Infrastructures.Mappings.Profiles
{
    public class BlogModelToDomain: Profile
    {
        public BlogModelToDomain()
        {
            CreateMap<ArticleModel, Article>();
        }
    }
}
