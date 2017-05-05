using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.Calendar.Entities;

namespace Lumle.Module.Calendar.Services
{
    public class CalendarHolidayService : ICalendarHolidayService
    {
        private readonly IRepository<CalendarHoliday> _calendarHolidayRepository;

        public CalendarHolidayService(IRepository<CalendarHoliday> calendarHolidayRepository)
        {
            _calendarHolidayRepository = calendarHolidayRepository;
        }

        public IEnumerable<CalendarHoliday> GetAll()
        {
            try
            {
                var events = _calendarHolidayRepository.GetAll();
                return events;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CalendarHoliday> GetAll(Expression<Func<CalendarHoliday, bool>> predicate)
        {
            try
            {
                var articles = _calendarHolidayRepository.GetAll(predicate);
                return articles;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public CalendarHoliday GetSingle(Expression<Func<CalendarHoliday, bool>> predicate)
        {
            try
            {
                var article = _calendarHolidayRepository.GetSingle(predicate);
                return article;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(CalendarHoliday entity)
        {
            try
            {
                _calendarHolidayRepository.Add(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(CalendarHoliday entity)
        {
            try
            {
                _calendarHolidayRepository.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteWhere(Expression<Func<CalendarHoliday, bool>> predicate)
        {
            try
            {
                _calendarHolidayRepository.DeleteWhere(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<CalendarHoliday, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
