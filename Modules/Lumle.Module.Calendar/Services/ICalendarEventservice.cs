using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Module.Calendar.Entities;

namespace Lumle.Module.Calendar.Services
{
    public interface ICalendarEventservice
    {
        IEnumerable<CalendarEvent> GetAll();
        IEnumerable<CalendarEvent> GetAll(Expression<Func<CalendarEvent, bool>> predicate);
        CalendarEvent GetSingle(Expression<Func<CalendarEvent, bool>> predicate);
        void Add(CalendarEvent entity);
        void Update(CalendarEvent entity);
        void DeleteWhere(Expression<Func<CalendarEvent, bool>> predicate);
    }
}
