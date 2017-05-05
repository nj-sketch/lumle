using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Module.Blog.Entities
{
    public class Article : EntityBase
    {
        [Required]
        public string Author { get; set; }

        [Required]
        public string Title { get; set; }
        
        public string Content { get; set; }

        [Required]
        public string Slug { get; set; }

        public string FeaturedImageUrl { get; set; }
    }
}
