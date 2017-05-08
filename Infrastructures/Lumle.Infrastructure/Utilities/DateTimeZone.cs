using System;
using System.Collections.Generic;
using System.Linq;
using Lumle.Infrastructure.Constants.LumleLog;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;

namespace Lumle.Infrastructure.Utilities
{
    public static class DateTimeZone
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// get Time Zone List of system
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetTimeZoneList()
        {
            var timeZoneList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "--Select Time Zone--",
                    Value = ""
                }
            };
            timeZoneList.AddRange(TimeZoneInfo.GetSystemTimeZones().Select(z => new SelectListItem
            {
                Text = z.Id,
                Value = z.Id
            }));
            return timeZoneList;
        }


        /// <summary>
        /// Convert utc dateTime to User dateTime
        /// </summary>
        /// <returns></returns>
        public static DateTime ConvertUtcToUserDateTime(DateTime utcDateTime, string timeZone)
        {
            try
            {
                var formatedUtcDate = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                return TimeZoneInfo.ConvertTime(formatedUtcDate, timeZoneInfo);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.TimeZoneConversion);
                return utcDateTime;
            }

        }
    }
}
