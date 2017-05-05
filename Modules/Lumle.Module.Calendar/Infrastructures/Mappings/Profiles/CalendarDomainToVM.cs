using AutoMapper;
using Lumle.Module.Calendar.Entities;
using Lumle.Module.Calendar.ViewModels;

namespace Lumle.Module.Calendar.Infrastructures.Mappings.Profiles
{
    public class CalendarDomainToVM :Profile
    {
        public CalendarDomainToVM()
        {
            CreateMap<CalendarEvent, CalendarEventVM>();
            CreateMap<CalendarHoliday, CalendarHolidayVM>();
        }
    }
}
