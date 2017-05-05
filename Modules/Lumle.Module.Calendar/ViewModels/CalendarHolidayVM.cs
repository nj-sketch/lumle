using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Lumle.Module.Calendar.ViewModels
{
    public class CalendarHolidayVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter title!")]
        [MaxLength(50)]
        public string Title { get; set; }
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Please enter date!")]
        public string HolidayDate { get; set; }
        [MaxLength(200)]
        public string Remarks { get; set; }

        public string CreatedBy { get; set; }
    }
}
