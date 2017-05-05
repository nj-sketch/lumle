using System;
using System.ComponentModel.DataAnnotations;
using Lumle.Data.Models;

namespace Lumle.Module.Calendar.Entities
{
    public class CalendarEvent : EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Start { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime End { get; set; }


        [MaxLength(250)]
        public string Remarks { get; set; }

        public string CreatedBy { get; set; }
    }
}
