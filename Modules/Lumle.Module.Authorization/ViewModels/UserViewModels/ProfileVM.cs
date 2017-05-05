using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lumle.Infrastructure.Utilities;

namespace Lumle.Module.Authorization.ViewModels.UserViewModels
{
    public class ProfileVM
    {
        [Required]
        public  int Id { get; set; }
        
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Phone(ErrorMessage = "Please enter valid phone number")]
        public string Phone { get; set; }
        
        public string Website { get; set; }

        public string AboutMe { get; set; }

        [Required(ErrorMessage = "Please select time zone")]
        public string TimeZone { get; set; }

    }
}
