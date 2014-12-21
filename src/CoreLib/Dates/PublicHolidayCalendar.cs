using System;
using System.Collections.Generic;
using System.Linq;
#if !SILVERLIGHT
using NValidate.Framework;
#endif

namespace Eca.Commons.Dates
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class PublicHolidayCalendar : IEquatable<PublicHolidayCalendar>
    {
        private readonly string _country;


        protected PublicHolidayCalendar(string country)
        {
            Check.Require(country != null, "country missing");
            _country = country;
        }


        public string Country
        {
            get { return _country; }
        }


        static PublicHolidayCalendar()
        {
            UK = UKPublicHolidayCalendar.Default;
            Default = UK;
        }


        public static IPublicHolidayCalendar Default { get; private set; }
        public static IPublicHolidayCalendar UK { get; private set; }


        public bool Equals(PublicHolidayCalendar obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._country, _country);
        }


        public abstract bool IsHoliday(DateTime candidate);


        public override bool Equals(object obj)
        {
            return Equals(obj as PublicHolidayCalendar);
        }


        public override int GetHashCode()
        {
            return Country.GetHashCode();
        }


        public override string ToString()
        {
            return string.Format("{{ {0}: Country = {1} }}", GetType().Name, Country);
        }


        public static bool IsWeekend(DateTime date)
        {
            return (date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday);
        }
    }



#if !SILVERLIGHT
    [Serializable]
#endif
    public class UKPublicHolidayCalendar : PublicHolidayCalendar, IPublicHolidayCalendar
    {
        #region Member Variables

        private static readonly DateTime RoyalJubileeSpringBankHol;
        public static readonly DateTime RoyalJubilee;

        #endregion


        public static IDictionary<int, IEnumerable<DateTime>> SpecialDates
        {
            get { return new Dictionary<int, IEnumerable<DateTime>> {{2012, new[] {RoyalJubilee}}}; }
        }


        #region Constructors

        public UKPublicHolidayCalendar() : base("UK")
        {
            _specialHolidayDates = new Dictionary<int, IEnumerable<DateTime>>(SpecialDates);
        }

        #endregion


        #region Properties

        #endregion


        #region IEquatable<PublicHolidayCalendar> Members

        #endregion


        #region IPublicHolidayCalendar Members

        public override bool IsHoliday(DateTime candidate)
        {
            return GetHolidaysCloseTo(candidate).Contains(candidate);
        }

        #endregion


        #region Overridden object methods

        #endregion


        #region Class Members

        static UKPublicHolidayCalendar()
        {
            BoxingDayHolidays = new Dictionary<int, DateTime>(20);
            XmasDayHolidays = new Dictionary<int, DateTime>(20);
            AugustBankHolidays = new Dictionary<int, DateTime>(20);
            MayBankHolidays = new Dictionary<int, DateTime>(20);
            SpringBankHolidays = new Dictionary<int, DateTime>(20);
            GoodFridayHolidays = new Dictionary<int, DateTime>(20);
            EasterMondayHolidays = new Dictionary<int, DateTime>(20);
            NewYearsHolidays = new Dictionary<int, DateTime>(20);

            RoyalJubileeSpringBankHol = new DateTime(2012, 06, 04);
            RoyalJubilee = new DateTime(2012, 06, 05);

            Default = new UKPublicHolidayCalendar();
        }


        private readonly IDictionary<int, IEnumerable<DateTime>> _specialHolidayDates;

        private static Dictionary<int, DateTime> AugustBankHolidays { get; set; }
        public static Dictionary<int, DateTime> BoxingDayHolidays { get; private set; }
        public new static IPublicHolidayCalendar Default { get; private set; }
        private static Dictionary<int, DateTime> EasterMondayHolidays { get; set; }
        private static Dictionary<int, DateTime> GoodFridayHolidays { get; set; }
        private static Dictionary<int, DateTime> MayBankHolidays { get; set; }
        private static Dictionary<int, DateTime> NewYearsHolidays { get; set; }
        private static Dictionary<int, DateTime> SpringBankHolidays { get; set; }


        private static Dictionary<int, DateTime> XmasDayHolidays { get; set; }


        public static void ClearCache()
        {
            BoxingDayHolidays.Clear();
            XmasDayHolidays.Clear();
        }


        public static DateTime GetAugustBankHoliday(int year)
        {
            DateTime cachedDate;
            bool cached = AugustBankHolidays.TryGetValue(year, out cachedDate);
            if (cached) return cachedDate;

            DateTime date = GetPreviousMonday(new DateTime(year, 9, 1));
            AugustBankHolidays.Add(year, date);
            return date;
        }


        /// <summary>
        /// This method returns all public holidays in a date range. The order of the holidays is according to the
        /// holiday event. That is (in case you ask over a calendar year):
        /// <list type="bullet">
        /// <item>New Year's Day</item>
        /// <item>Good Friday</item>
        /// <item>Easter Monday</item>
        /// <item>Early May Bank Holiday</item>
        /// <item>Spring Bank Holiday</item>
        /// <item>Summer Bank Holiday</item>
        /// <item>Christmas Day</item>
        /// <item>Boxing Day</item>
        /// </list> 
        /// </summary>
        /// <remarks>
        /// If the start date is mid-year, the first entry will be the closest event and then continues according to list.
        /// If you need them in real date order of the public holiday, you need to sort the collection that is returned.
        /// </remarks>
        public static IList<DateTime> GetBetweenIncluding(DateTime startDate, DateTime endDate)
        {
            return ((UKPublicHolidayCalendar) Default).GetBetweenIncludingImpl(startDate, endDate);
        }


        private IList<DateTime> GetBetweenIncludingImpl(DateTime startDate, DateTime endDate)
        {
#if !SILVERLIGHT
            Check.Require(() => Demand.The.Param(() => startDate).IsLessThanOrEqualTo(endDate));
#endif

            int startYear = startDate.Year;
            int endYear = endDate.Year;

            bool requiresSorting = false;
            var allHolidaysInYears = new List<DateTime>();
            for (int year = startYear; year <= endYear; year++)
            {
                allHolidaysInYears.Add(GetNewYearsDay(year));
                allHolidaysInYears.Add(GetGoodFriday(year));
                allHolidaysInYears.Add(GetEasterMonday(year));
                allHolidaysInYears.Add(GetMayBankHoliday(year));
                allHolidaysInYears.Add(GetSpringBankHoliday(year));
                allHolidaysInYears.Add(GetAugustBankHoliday(year));
                allHolidaysInYears.Add(GetChristmasHoliday(year));
                allHolidaysInYears.Add(GetBoxingDayHoliday(year));

                IEnumerable<DateTime> specialDates;
                bool match = _specialHolidayDates.TryGetValue(year, out specialDates);
                if (match)
                {
                    allHolidaysInYears.AddRange(specialDates);
                    requiresSorting = true;
                }
            }

            IEnumerable<DateTime> unsortedResults =
                allHolidaysInYears.Where(date => date >= startDate && date <= endDate);
            return requiresSorting ? unsortedResults.OrderBy(d => d).ToList() : unsortedResults.ToList();
        }


        public static DateTime GetBoxingDayHoliday(int year)
        {
            DateTime cachedDate;
            bool cached = BoxingDayHolidays.TryGetValue(year, out cachedDate);
            if (cached) return cachedDate;

            var date = new DateTime(year, 12, 26);
            if (IsWeekend(date))
            {
                date = new DateTime(year, 12, 28);
            }
            BoxingDayHolidays.Add(year, date);
            return date;
        }


        public static DateTime GetChristmasHoliday(int year)
        {
            DateTime cachedDate;
            bool cached = XmasDayHolidays.TryGetValue(year, out cachedDate);
            if (cached) return cachedDate;

            var date = new DateTime(year, 12, 25);
            if (IsWeekend(date))
            {
                date = new DateTime(year, 12, 27);
            }
            XmasDayHolidays.Add(year, date);
            return date;
        }


        /// <remarks>
        /// adapted from the algorithm from the U.S. Naval Observatory  Astronomical Applications Department
        /// website http://aa.usno.navy.mil/faq/docs/easter.php
        /// </remarks>
        public static DateTime GetEasterDay(int year)
        {
            int century = year/100;
            int n = year - 19*(year/19);
            int k = (century - 17)/25;
            int i = century - century/4 - (century - k)/3 + 19*n + 15;
            i = i - 30*(i/30);
            i = i - (i/28)*(1 - (i/28)*(29/(i + 1))*((21 - n)/11));
            int j = year + year/4 + i + 2 - century + century/4;
            j = j - 7*(j/7);
            int l = i - j;
            int month = 3 + (l + 40)/44;
            int day = l + 28 - 31*(month/4);

            return new DateTime(year, month, day);
        }


        public static DateTime GetEasterMonday(int year)
        {
            DateTime cachedDate;
            bool cached = EasterMondayHolidays.TryGetValue(year, out cachedDate);
            if (cached) return cachedDate;

            DateTime date = GetEasterDay(year).AddDays(1);
            EasterMondayHolidays.Add(year, date);
            return date;
        }


        public static DateTime GetGoodFriday(int year)
        {
            DateTime cachedDate;
            bool cached = GoodFridayHolidays.TryGetValue(year, out cachedDate);
            if (cached) return cachedDate;

            DateTime date = GetEasterDay(year).AddDays(-2);
            GoodFridayHolidays.Add(year, date);
            return date;
        }


        private IEnumerable<DateTime> GetHolidaysCloseTo(DateTime date)
        {
            switch (date.Month)
            {
                case 1:
                    yield return GetNewYearsDay(date.Year);
                    break;
                case 3:
                case 4:
                    yield return GetGoodFriday(date.Year);
                    yield return GetEasterMonday(date.Year);
                    break;
                case 5:
                    yield return GetMayBankHoliday(date.Year);
                    yield return GetSpringBankHoliday(date.Year);
                    break;
                case 6:
                    if (IsRoyalJubileeYear(date.Year)) yield return RoyalJubileeSpringBankHol;
                    break;
                case 8:
                    yield return GetAugustBankHoliday(date.Year);
                    break;
                case 12:
                    yield return GetChristmasHoliday(date.Year);
                    yield return GetBoxingDayHoliday(date.Year);
                    break;
            }

            IEnumerable<DateTime> specialDates;
            bool match = _specialHolidayDates.TryGetValue(date.Year, out specialDates);
            if (match)
            {
                foreach (DateTime dtm in specialDates)
                {
                    yield return dtm;
                }
            }
        }


        private static bool IsRoyalJubileeYear(int year)
        {
            return year == 2012;
        }


        public static DateTime GetMayBankHoliday(int year)
        {
            DateTime cachedDate;
            bool cached = MayBankHolidays.TryGetValue(year, out cachedDate);
            if (cached) return cachedDate;

            DateTime mayBankHoliday = GetNextMonday(new DateTime(year, 4, 30));
            if (year == 1995)
            {
                mayBankHoliday = new DateTime(year, 5, 8);
            }
            MayBankHolidays.Add(year, mayBankHoliday);
            return mayBankHoliday;
        }


        public static DateTime GetNewYearsDay(int year)
        {
            DateTime cachedDate;
            bool cached = NewYearsHolidays.TryGetValue(year, out cachedDate);
            if (cached) return cachedDate;

            var newYearsDay = new DateTime(year, 1, 1);
            if (IsWeekend(newYearsDay))
            {
                newYearsDay = GetNextMonday(newYearsDay);
            }
            NewYearsHolidays.Add(year, newYearsDay);
            return newYearsDay;
        }


        private static DateTime GetNextMonday(DateTime date)
        {
            int[] daysToAdd = {1, 7, 6, 5, 4, 3, 2};

            return date.AddDays(daysToAdd[(int) date.DayOfWeek]);
        }


        private static DateTime GetPreviousMonday(DateTime date)
        {
            int[] daysToSubstract = {6, 7, 1, 2, 3, 4, 5};

            return date.AddDays(-daysToSubstract[(int) date.DayOfWeek]);
        }


        public static DateTime GetSpringBankHoliday(int year)
        {
            if (IsRoyalJubileeYear(year)) return RoyalJubileeSpringBankHol;

            DateTime cachedDate;
            bool cached = SpringBankHolidays.TryGetValue(year, out cachedDate);
            if (cached) return cachedDate;

            DateTime date = GetPreviousMonday(new DateTime(year, 6, 1));

            SpringBankHolidays.Add(year, date);
            return date;
        }

        #endregion
    }
}