﻿using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Data.Models
{
    public class User: IdentityUser<string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>>
    {
        [Required]
        public int AccountStatus { get; set; } //0->Disable, 1-> Enable

        public string CreatedBy { get; set; }

        [Required]
        [MaxLength(100)]
        public string TimeZone { get; set; }

        [Required]
        [MaxLength(20)]
        public string Culture { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

    }
}
