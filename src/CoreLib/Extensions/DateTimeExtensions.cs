using System;
using System.Globalization;

namespace Eca.Commons.Extensions
{
    public static class DateTimeExtensions
    {
        #region Class Members

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            return StartOfWeek(dt, startOfWeek).AddDays(7).AddSeconds(-1);
        }


        /// <summary>
        /// Returns the start date of the week based upon the week number.
        /// </summary>
        /// <param name="year">Year in question</param>
        /// <param name="weekOfYear">Week number of the year</param>
        /// <returns></returns>
        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum*7);
            return result.AddDays(-3);
        }


        /// <summary>
        /// Tests that the supplied <paramref name="dateTime"/> is not null and is not
        /// <see cref="DateTime.MinValue"/>
        /// </summary>
        public static bool IsMissing(this DateTime? dateTime)
        {
            return dateTime == null || dateTime == DateTime.MinValue;
        }


        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1*diff).Date;
        }


        /// <summary>
        /// Returns the week number of a <paramref name="dateTime"/>
        /// </summary>
        /// <param name="dateTime">Date to get the week number for</param>
        /// <returns></returns>
        public static int WeekNumber(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime,
                                                                     CalendarWeekRule.FirstFourDayWeek,
                                                                     DayOfWeek.Monday);
        }

        #endregion
    }
}