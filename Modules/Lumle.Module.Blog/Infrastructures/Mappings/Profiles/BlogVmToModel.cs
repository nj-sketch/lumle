using AutoMapper;
using Lumle.Module.Blog.Models;
using Lumle.Module.Blog.ViewModels;

namespace Lumle.Module.Blog.Infrastructures.Mappings.Profiles
{
    public class BlogVmToModel : Profile
    {
        public BlogVmToModel()
        {
            CreateMap<ArticleVM, ArticleModel>();
        }
    }
}
