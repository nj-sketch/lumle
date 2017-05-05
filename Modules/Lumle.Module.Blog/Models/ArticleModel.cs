using Lumle.Data.Models;

namespace Lumle.Module.Blog.Models
{
    public class ArticleModel : EntityBaseModel
    {
        public int SN { get; set; }
        public string Author { get; set; }
        
        public string Title { get; set; }

        public string Content { get; set; }

        public string Slug { get; set; }
        public string FormatedCreatedDate { get; set; }
        public string FeaturedImageUrl { get; set; }
    }
}
