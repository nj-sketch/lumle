using AutoMapper;
using Lumle.Module.Blog.Entities;
using Lumle.Module.Blog.Models;

namespace Lumle.Module.Blog.Infrastructures.Mappings.Profiles
{
    public class BlogDomainToModel: Profile
    {
        public BlogDomainToModel()
        {
            CreateMap<Article, ArticleModel>();
        }
    }
}
