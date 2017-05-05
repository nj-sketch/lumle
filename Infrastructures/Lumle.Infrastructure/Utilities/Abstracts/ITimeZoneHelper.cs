using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.Infrastructure.Utilities.Abstracts
{
    public interface ITimeZoneHelper
    {
        DateTime ConvertToLocalTime(DateTime utcDateTime, string timeZoneId);
        DateTime ConvertToUtc(DateTime localDateTime, string timeZoneId, ZoneLocalMappingResolver resolver = null);
        IReadOnlyCollection<string> GetTimeZoneList();
        List<SelectListItem> GetTimeZoneSelectListItem();
    }
}
