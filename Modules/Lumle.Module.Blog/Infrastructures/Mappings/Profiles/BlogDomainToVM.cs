using AutoMapper;
using Lumle.Module.Blog.Entities;
using Lumle.Module.Blog.ViewModels;

namespace Lumle.Module.Blog.Infrastructures.Mappings.Profiles
{
    public class BlogDomainToVM : Profile
    {
        public BlogDomainToVM()
        {
            CreateMap<Article, ArticleVM>();
        }
    }
}
