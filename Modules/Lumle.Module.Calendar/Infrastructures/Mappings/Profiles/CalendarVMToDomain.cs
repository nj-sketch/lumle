using AutoMapper;
using Lumle.Module.Calendar.Entities;
using Lumle.Module.Calendar.ViewModels;

namespace Lumle.Module.Calendar.Infrastructures.Mappings.Profiles
{
    public class CalendarVMToDomain: Profile
    {
        public CalendarVMToDomain()
        {
            CreateMap<CalendarEventVM, CalendarEvent>();
            CreateMap<CalendarHolidayVM, CalendarHoliday>();
        }
    }
}
