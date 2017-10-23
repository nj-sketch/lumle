using System.ComponentModel.DataAnnotations;
using Lumle.Infrastructure.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Lumle.Module.Authorization.ViewModels.UserViewModels
{
    public class ProfileVM
    {
        [Required]
        public  int Id { get; set; }
        
        public string UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }        

        public string TimeZone { get; set; }

        public string RoleName { get; set; }

        public int AccountStatus { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public CountryEnum EnumCountry { get; set; }

        public StateEnum EnumState { get; set; }

        public List<SelectListItem> TimeZoneList { get; set; }

        public IFormFile ProfileImage { get; set; }

        public string ProfileImageUrl { get; set; }
    }
}
