using Lumle.Infrastructure.Utilities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime.TimeZones;
using NodaTime;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lumle.Infrastructure.Utilities
{
    public class TimeZoneHelper : ITimeZoneHelper
    {
        private readonly IDateTimeZoneProvider _dateTimeZoneProvider;

        public TimeZoneHelper(IDateTimeZoneProvider dateTimeZoneProvider)
        {
            _dateTimeZoneProvider = dateTimeZoneProvider;
        }

        public DateTime ConvertToLocalTime(DateTime utcDateTime, string timeZoneId)
        {
            DateTime formatedUtcDateTime;
            switch (utcDateTime.Kind)
            {
                case DateTimeKind.Local:
                    formatedUtcDateTime = utcDateTime.ToUniversalTime();
                    break;
                case DateTimeKind.Utc:
                    formatedUtcDateTime = utcDateTime;
                    break;
                default://DateTimeKind.Unspecified
                    formatedUtcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
                    break;
            }

            var timeZone = _dateTimeZoneProvider.GetZoneOrNull(timeZoneId);
            if (timeZone == null) return utcDateTime;

            var instant = Instant.FromDateTimeUtc(formatedUtcDateTime);
            var zoned = new ZonedDateTime(instant, timeZone);
            return new DateTime(zoned.Year, zoned.Month, zoned.Day, zoned.Hour, zoned.Minute, zoned.Second, zoned.Millisecond, DateTimeKind.Unspecified);
        }

        public DateTime ConvertToUtc(DateTime localDateTime, string timeZoneId, ZoneLocalMappingResolver resolver = null)
        {
            if (localDateTime.Kind == DateTimeKind.Utc) return localDateTime;

            if (resolver == null) resolver = Resolvers.LenientResolver;
            var timeZone = _dateTimeZoneProvider.GetZoneOrNull(timeZoneId);

            if (timeZone == null) return localDateTime;

            var local = LocalDateTime.FromDateTime(localDateTime);
            var zoned = timeZone.ResolveLocal(local, resolver);
            return zoned.ToDateTimeUtc();
        }

        public IReadOnlyCollection<string> GetTimeZoneList()
        {
            return _dateTimeZoneProvider.Ids;
        }

        public List<SelectListItem> GetTimeZoneSelectListItem()
        {
             return _dateTimeZoneProvider.Ids.Select(x => new SelectListItem { Text = x, Value = x }).ToList();
        }
    }
}
