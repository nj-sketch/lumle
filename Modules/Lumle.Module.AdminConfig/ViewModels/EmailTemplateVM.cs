using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Module.AdminConfig.ViewModels
{
    public class EmailTemplateVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The title field is required.",AllowEmptyStrings =false)]
        [MaxLength(200,ErrorMessage ="The title field maximum length is 200.")]
        [Display(Name ="Title")]
        public string SlugDisplayName { get; set; }

        [Required(ErrorMessage = "The subject field is required.",AllowEmptyStrings =false)]
        [MaxLength(500,ErrorMessage ="The subject field maximum length is 500.")]
        [Display(Name ="Email Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "The body field is required.",AllowEmptyStrings =false)]
        [Display(Name ="Email Body")]
        public string Body { get; set; }

    }
}
