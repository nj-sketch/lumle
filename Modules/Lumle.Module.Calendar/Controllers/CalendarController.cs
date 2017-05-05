using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.Calendar.Entities;
using Lumle.Module.Calendar.Services;
using Lumle.Module.Calendar.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Lumle.Infrastructure.Constants.AuthorizeRules;
using Microsoft.AspNetCore.Identity;
using Lumle.Data.Models;
using Lumle.Infrastructure.Utilities;
using System.Globalization;
using Lumle.Core.Attributes;
using Lumle.Core.Localizer;
using Lumle.Infrastructure.Constants.Localization;
using Microsoft.Extensions.Localization;

namespace Lumle.Module.Calendar.Controllers
{
    [Route("calendar")]
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ICalendarEventservice _calendarEventService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICalendarHolidayService _calendarHolidayService;
        private readonly UserManager<User> _userManager;
        private readonly IStringLocalizer<ResourceString> _localizer;

        public CalendarController(ICalendarEventservice calendarEventService,
            ICalendarHolidayService calendarHolidayService,
            IUnitOfWork unitOfWork,         
            UserManager<User> userManager,
            IStringLocalizer<ResourceString> localizer)
        {
            _calendarEventService = calendarEventService;
            _calendarHolidayService = calendarHolidayService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _localizer = localizer;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("event")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarView)]
        public async Task<IActionResult> Events()
        {
            try
            {
                var allEvents = _calendarEventService.GetAll();
                var calendarEvents = allEvents as IList<CalendarEvent> ?? allEvents.ToList();

                var events = new List<CalendarEvent>();
                if (calendarEvents.Any())
                {
                    var user = await GetCurrentUserAsync();
                    events = calendarEvents.Select(x =>
                    {
                        x.Start = DateTimeZone.ConvertUtcToUserDateTime(x.Start, user.TimeZone);
                        x.End = DateTimeZone.ConvertUtcToUserDateTime(x.End, user.TimeZone);

                        return x;
                    }).ToList();
                }

                var eventVm = Mapper.Map<IEnumerable<CalendarEventVM>>(events);

                return View("Event", eventVm);
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.EventNotFoundErrorMessage].Value;
                return RedirectToAction("Index");
            }

        }

        [HttpGet("event/add")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarCreate)]
        public IActionResult CreateEvent()
        {
            return View("CreateEvent");
        }

        [HttpPost("event/add")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarCreate)]
        public IActionResult CreateEvent(CalendarEventVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                return View("CreateEvent", model);
            }

            var eventTimes = model.EventDate.Split('-');
            model.Start = DateTime.Parse(eventTimes[0],CultureInfo.InvariantCulture).ToUniversalTime();
            model.End = DateTime.Parse(eventTimes[1], CultureInfo.InvariantCulture).ToUniversalTime();

            model.CreatedBy = HttpContext.User.Identity.Name;
            var entity = Mapper.Map<CalendarEvent>(model);

            _calendarEventService.Add(entity);
            _unitOfWork.Save();

            TempData["SuccessMsg"] = _localizer[ActionMessageConstants.AddedSuccessfully].Value;
            return RedirectToAction("Events");
        }

        [HttpGet("event/edit/{id:int}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarUpdate)]
        public async Task<IActionResult> EditEvent(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.EventNotFoundErrorMessage].Value;
                return RedirectToAction("Events");
            }

            var eventEntity = _calendarEventService.GetSingle(x => x.Id == id);
            if (eventEntity == null)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.EventNotFoundErrorMessage].Value;
                return RedirectToAction("Events");
            }

            var eventVm = Mapper.Map<CalendarEventVM>(eventEntity);

            var user = await GetCurrentUserAsync();

            var startDate = DateTimeZone.ConvertUtcToUserDateTime(eventVm.Start, user.TimeZone);
            var endDate = DateTimeZone.ConvertUtcToUserDateTime(eventVm.End, user.TimeZone);

            eventVm.EventDate = $"{startDate}-{endDate}";

            return View("EditEvent", eventVm);
        }

        [HttpPost("event/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarUpdate)]
        public IActionResult EditEvent(CalendarEventVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                    return View("EditEvent", model);
                }

                var eventTimes = model.EventDate.Split('-');
                model.Start = DateTime.Parse(eventTimes[0],CultureInfo.InvariantCulture).ToUniversalTime();
                model.End = DateTime.Parse(eventTimes[1],CultureInfo.InvariantCulture).ToUniversalTime();

                model.CreatedBy = HttpContext.User.Identity.Name;
                var entity = Mapper.Map<CalendarEvent>(model);

                _calendarEventService.Update(entity);
                _unitOfWork.Save();

                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value;
                return RedirectToAction("Events");
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToDeleteErrorMessage].Value;
                return View("EditEvent", model);
            }

        }

        [HttpPost("event/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarDelete)]
        public IActionResult DeleteEvent(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.EventNotFoundErrorMessage].Value;
                    return RedirectToAction("Events");
                }

                var eventEntity = _calendarEventService.GetSingle(x => x.Id == id);
                if (eventEntity == null)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.EventNotFoundErrorMessage].Value;
                    return RedirectToAction("Events");
                }

                _calendarEventService.DeleteWhere(x => x.Id == id);
                _unitOfWork.Save();

                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.DeletedSuccessfully].Value;
                return RedirectToAction("Events");
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToDeleteErrorMessage].Value;
                return RedirectToAction("Events");
            }
        }


        [HttpGet("holiday/add")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarCreate)]
        public IActionResult CreateHoliday()
        {
            return View("CreateHoliday");
        }

        [HttpPost("holiday/add")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarCreate)]
        public IActionResult CreateHoliday(CalendarHolidayVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                    return View("CreateHoliday", model);
                }

                model.Date = DateTime.Parse(model.HolidayDate,CultureInfo.InvariantCulture);
                model.CreatedBy = HttpContext.User.Identity.Name;
                var entity = Mapper.Map<CalendarHoliday>(model);

                _calendarHolidayService.Add(entity);
                _unitOfWork.Save();

                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.AddedSuccessfully].Value;
                return RedirectToAction("Holidays");
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToAddErrorMessage].Value;
                return View("CreateHoliday", model);
            }
        }


        [HttpGet("holiday")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarView)]
        public IActionResult Holidays()
        {
            try
            {
                var allHolidays = _calendarHolidayService.GetAll();
                var calendarHolidays = allHolidays as IList<CalendarHoliday> ?? allHolidays.ToList();

                var holidayVm = Mapper.Map<IEnumerable<CalendarHolidayVM>>(calendarHolidays);
                holidayVm = holidayVm.Select(x => { x.HolidayDate = x.Date.ToString("d"); return x; }).ToList();
                return View("Holiday", holidayVm);
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.HolidayNotFoundErrorMessage].Value;
                return RedirectToAction("Index");
            }
        }


        [HttpGet("holiday/edit/{id:int}")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarUpdate)]
        public async Task<IActionResult> EditHoliday(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.HolidayNotFoundErrorMessage].Value;
                return RedirectToAction("Holidays");
            }

            var eventEntity = _calendarHolidayService.GetSingle(x => x.Id == id);
            if (eventEntity == null)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.HolidayNotFoundErrorMessage].Value;
                return RedirectToAction("Holidays");
            }

            var user = await GetCurrentUserAsync();
            eventEntity.Date = DateTimeZone.ConvertUtcToUserDateTime(eventEntity.Date, user.TimeZone);

            var eventVm = Mapper.Map<CalendarHolidayVM>(eventEntity);
            eventVm.HolidayDate = eventVm.Date.ToString("d");

            return View("EditHoliday", eventVm);

        }

        [HttpPost("holiday/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarUpdate)]
        public IActionResult EditHoliday(CalendarHolidayVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.PleaseFillAllTheRequiredFieldErrorMessage].Value;
                    return View("EditHoliday", model);
                }

                model.Date = DateTime.Parse(model.HolidayDate,CultureInfo.InvariantCulture);
                model.CreatedBy = HttpContext.User.Identity.Name;
                var entity = Mapper.Map<CalendarHoliday>(model);

                _calendarHolidayService.Update(entity);
                _unitOfWork.Save();

                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.UpdatedSuccessfully].Value;
                return RedirectToAction("Holidays");
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToDeleteErrorMessage].Value;
                return View("EditHoliday", model);
            }

        }

        [HttpPost("holiday/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarDelete)]
        public IActionResult DeleteHoliday(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.HolidayNotFoundErrorMessage].Value;
                    return RedirectToAction("Holidays");
                }

                var eventEntity = _calendarHolidayService.GetSingle(x => x.Id == id);
                if (eventEntity == null)
                {
                    TempData["ErrorMsg"] = _localizer[ActionMessageConstants.HolidayNotFoundErrorMessage].Value;
                    return RedirectToAction("Holidays");
                }

                _calendarHolidayService.DeleteWhere(x => x.Id == id);
                _unitOfWork.Save();

                TempData["SuccessMsg"] = _localizer[ActionMessageConstants.DeletedSuccessfully].Value;
                return RedirectToAction("Holidays");
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = _localizer[ActionMessageConstants.UnableToDeleteErrorMessage].Value;
                return RedirectToAction("Holidays");
            }
        }

        [HttpGet("events")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarView)]
        public async Task<JsonResult> GetEvents()
        {
            var start = DateTime.UtcNow;

            var eventEntity = _calendarEventService.GetAll(x => x.Start.Year >= start.Year && x.End.Year <= start.Year);

            if (eventEntity == null) return null;

            var user = await GetCurrentUserAsync();
          
            var convertedEntity = eventEntity.Select(x =>
                { 
                    x.Start = DateTimeZone.ConvertUtcToUserDateTime(x.Start, user.TimeZone);
                    x.End = DateTimeZone.ConvertUtcToUserDateTime(x.End, user.TimeZone);

                    return x;
                });
            var eventVm = Mapper.Map<IEnumerable<CalendarEventVM>>(convertedEntity).ToArray();

            return Json(eventVm);

        }


        [HttpGet("holidays")]
        [ClaimRequirement(CustomClaimtypes.Permission, Permissions.CalendarView)]
        public JsonResult GetHolidays()
        {
            var start = DateTime.UtcNow;

            var holyEntity = _calendarHolidayService.GetAll(x => x.Date.Year >= start.Year);

            if (holyEntity == null) return null;
            {
                var holidayVm = Mapper.Map<IEnumerable<CalendarHolidayVM>>(holyEntity).ToArray();
                holidayVm = holidayVm.Select(x => { x.HolidayDate = x.Date.ToString("d"); return x; }).ToArray();
                return Json(holidayVm);
            }
        }

        #region Helpers
        private async Task<User> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }

}
