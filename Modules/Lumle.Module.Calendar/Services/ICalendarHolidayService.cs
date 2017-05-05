using System;
using System.Collections.Generic;
using Lumle.Module.Calendar.Entities;
using System.Linq.Expressions;

namespace Lumle.Module.Calendar.Services
{
    public interface ICalendarHolidayService
    {
        int Count();
        int Count(Expression<Func<CalendarHoliday, bool>> predicate);
        IEnumerable<CalendarHoliday> GetAll(Expression<Func<CalendarHoliday, bool>> predicate);
        IEnumerable<CalendarHoliday> GetAll();
        CalendarHoliday GetSingle(Expression<Func<CalendarHoliday, bool>> predicate);
        void Add(CalendarHoliday entity);
        void Update(CalendarHoliday entity);
        void DeleteWhere(Expression<Func<CalendarHoliday, bool>> predicate);
    }
}
