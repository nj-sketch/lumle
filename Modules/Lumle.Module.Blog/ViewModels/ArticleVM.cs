using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Blog.ViewModels
{
    public class ArticleVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public IFormFile FeaturedImage { get; set; }
        public string FeaturedImageUrl { get; set; }
    }
}
