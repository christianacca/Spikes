using System;

namespace Eca.Commons.Dates
{
    public interface IPublicHolidayCalendar
    {
        bool IsHoliday(DateTime candidate);
        string Country { get; }
    }
}