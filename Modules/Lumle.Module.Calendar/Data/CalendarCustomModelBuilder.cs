using Lumle.Data.Data.Abstracts;
using Lumle.Module.Calendar.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumle.Module.Calendar.Data
{
    public class CalendarCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<CalendarEvent>()
            .ToTable("Calendar_Event");

            builder.Entity<CalendarHoliday>()
            .ToTable("Calendar_Holiday");

        }
    }
}
