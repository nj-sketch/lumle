using System;
using System.ComponentModel.DataAnnotations;

namespace Lumle.Module.Calendar.ViewModels
{
    public class CalendarEventVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter title!")]
        [MaxLength(50)]
        public string Title { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime Start { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime End { get; set; }

        [MaxLength(200)]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "Please select date!")]
        public string EventDate { get; set; }

        public string CreatedBy { get; set; }

    }
}
