using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Module.Calendar.Entities;
using Lumle.Data.Data.Abstracts;

namespace Lumle.Module.Calendar.Services
{
    public class CalendarEventService : ICalendarEventservice
    {
        private readonly IRepository<CalendarEvent> _calendarEventRepository;

        public CalendarEventService(IRepository<CalendarEvent> calendarEventRepository)
        {
            _calendarEventRepository = calendarEventRepository;
        }

        public IEnumerable<CalendarEvent> GetAll()
        {
            try
            {
                var events = _calendarEventRepository.GetAll();
                return events;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CalendarEvent> GetAll(Expression<Func<CalendarEvent, bool>> predicate)
        {
            try
            {
                var articles = _calendarEventRepository.GetAll(predicate);
                return articles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public CalendarEvent GetSingle(Expression<Func<CalendarEvent, bool>> predicate)
        {
            try
            {
                var article = _calendarEventRepository.GetSingle(predicate);
                return article;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(CalendarEvent entity)
        {
            try
            {
                _calendarEventRepository.Add(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(CalendarEvent entity)
        {
            try
            {
                _calendarEventRepository.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<CalendarEvent, bool>> predicate)
        {
            try
            {
                _calendarEventRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
