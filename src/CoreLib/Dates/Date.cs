using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Eca.Commons.Extensions;

namespace Eca.Commons.Dates
{
    internal class DateConvertor : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (DateTime) || sourceType == typeof (string);
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value != null ? (Date) value.ToString() : value;
        }
    }



#if !SILVERLIGHT
    [Serializable]
    [IsBuiltinType]
    [KnownType(typeof (UKPublicHolidayCalendar))]
    [KnownType(typeof (PublicHolidayCalendar))]
#endif
    [TypeConverter(typeof (DateConvertor))]
    public sealed class Date : IEquatable<Date>, IComparable<Date>, IComparable
    {
        #region Member Variables

        private readonly DateTime _dateTime;
        private readonly IPublicHolidayCalendar _publicHolidayCalendar;

        #endregion


        #region Constructors

        public Date(int year, int month, int day)
            : this(year, month, day, Dates.PublicHolidayCalendar.Default) {}


        public Date(int year, int month, int day, IPublicHolidayCalendar calandar)
            : this(new DateTime(year, month, day), calandar) {}


        public Date(DateTime dateTime) : this(dateTime.Year, dateTime.Month, dateTime.Day) {}


        public Date(String dateTimeString) : this(DateTime.Parse(dateTimeString)) {}


        public Date(DateTime dateTime, IPublicHolidayCalendar calendar)
        {
            _publicHolidayCalendar = calendar ?? Dates.PublicHolidayCalendar.Default;
            _dateTime = dateTime;
        }

        #endregion


        #region Properties

        public int Day
        {
            get { return _dateTime.Day; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return _dateTime.DayOfWeek; }
        }


        public bool IsPublicHoliday
        {
            get { return _publicHolidayCalendar.IsHoliday(_dateTime); }
        }

        public bool IsWeekend
        {
            get { return IsDateWeekend(_dateTime); }
        }

        public bool IsWorkDay
        {
            get { return !IsWeekend && !IsPublicHoliday; }
        }

        public int Month
        {
            get { return _dateTime.Month; }
        }

        public IPublicHolidayCalendar PublicHolidayCalendar
        {
            get { return _publicHolidayCalendar; }
        }

        public Date ThisDayOrNextWeekDay
        {
            get { return IsWeekend ? NextWeekDay() : this; }
        }


        public Date ThisDayOrNextWorkDay
        {
            get { return !IsWorkDay ? NextWorkDay() : this; }
        }

        public Date ThisDayOrPreviousWeekDay
        {
            get { return IsWeekend ? PreviousWeekDay() : this; }
        }


        public Date ThisDayOrPreviousWorkDay
        {
            get { return !IsWorkDay ? PreviousWorkDay() : this; }
        }

        public int Year
        {
            get { return _dateTime.Year; }
        }

        #endregion


        #region IComparable Members

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Date);
        }

        #endregion


        #region IComparable<Date> Members

        public int CompareTo(Date other)
        {
            if (other == null) return 1;

            return _dateTime.CompareTo(other.ToDateTime());
        }

        #endregion


        #region IEquatable<Date> Members

        public bool Equals(Date date)
        {
            if (date == null) return false;
            return Equals(_dateTime, date._dateTime) && Equals(_publicHolidayCalendar, date._publicHolidayCalendar);
        }

        #endregion


        public Date AddDays(int days)
        {
            return DoAddDays(days, DoNotSkip);
        }


        private Date AddOrSubtractDays(int daysToAddOrSubtract, Func<DateTime, bool> skip, int increment)
        {
            int daysAddedOrSubtracted = 0;
            DateTime current = _dateTime;
            while (daysAddedOrSubtracted < daysToAddOrSubtract)
            {
                current = current.AddDays(increment);
                while (skip(current))
                {
                    current = current.AddDays(increment);
                }
                daysAddedOrSubtracted++;
            }
            return new Date(current, _publicHolidayCalendar);
        }


        private Date AddOrSubtractDays(int daysToAddOrSubtract, Func<Date, bool> skip, int increment)
        {
            int daysAddedOrSubtracted = 0;
            DateTime current = _dateTime;
            while (daysAddedOrSubtracted < daysToAddOrSubtract)
            {
                current = current.AddDays(increment);
                while (skip(current))
                {
                    current = current.AddDays(increment);
                }
                daysAddedOrSubtracted++;
            }
            return new Date(current, _publicHolidayCalendar);
        }


        /// <summary>
        /// Adds <paramref name="daysToAdd"/>, skipping weekends
        /// </summary>
        public Date AddWeekDays(int daysToAdd)
        {
            return DoAddDays(daysToAdd, SkipWeekends);
        }


        /// <summary>
        /// Adds <paramref name="daysToAdd"/>, skipping weekends and public holidays
        /// </summary>
        /// <remarks>By default the UK calander is used to determine public holidays</remarks>
        public Date AddWorkDays(int daysToAdd)
        {
            return DoAddDays(daysToAdd, SkipHolidaysAndWeekends);
        }


        private Date DoAddDays(int daysToAdd, Func<DateTime, bool> testForDaysToSkip)
        {
            return AddOrSubtractDays(daysToAdd, testForDaysToSkip, 1);
        }


        private bool DoNotSkip(DateTime date)
        {
            return false;
        }


        public Date IncrementDays(int days, Func<Date, bool> testForDaysToSkip)
        {
            if (days > 0)
                return AddOrSubtractDays(days, testForDaysToSkip, 1);
            else
                return AddOrSubtractDays(-days, testForDaysToSkip, -1);
        }


        /// <summary>
        /// Adds or subtracts <paramref name="days"/>, skipping weekends
        /// </summary>
        public Date IncrementWeekDays(int days)
        {
            return days < 0 ? LessWeekDays(-days) : AddWeekDays(days);
        }


        /// <summary>
        /// Adds or subtracts <paramref name="days"/>, skipping weekends
        /// </summary>
        public Date IncrementWorkDays(int days)
        {
            return days < 0 ? LessWorkDays(-days) : AddWorkDays(days);
        }


        private bool IsDateWeekend(DateTime date)
        {
            return (date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday);
        }


        public Date LessDays(int days)
        {
            return SubtractDays(days, DoNotSkip);
        }


        /// <summary>
        /// Subtracts <paramref name="daysToSubtract"/>, skipping weekends
        /// </summary>
        public Date LessWeekDays(int daysToSubtract)
        {
            return SubtractDays(daysToSubtract, SkipWeekends);
        }


        /// <summary>
        /// Subtracts <paramref name="daysToSubtract"/>, skipping weekends and public holidays
        /// </summary>
        /// <remarks>By default the UK calander is used to determine public holidays</remarks>
        public Date LessWorkDays(int daysToSubtract)
        {
            return SubtractDays(daysToSubtract, SkipHolidaysAndWeekends);
        }


        public Date NextDay()
        {
            return AddDays(1);
        }


        /// <summary>
        /// Gets next week day, skipping weekends
        /// </summary>
        public Date NextWeekDay()
        {
            return AddWeekDays(1);
        }


        /// <summary>
        /// Gets next work day, skipping weekends and public holidays
        /// </summary>
        public Date NextWorkDay()
        {
            return AddWorkDays(1);
        }


        public Date PreviousDay()
        {
            return LessDays(1);
        }


        /// <summary>
        /// Gets previous week day, skipping weekends 
        /// </summary>
        public Date PreviousWeekDay()
        {
            return LessWeekDays(1);
        }


        /// <summary>
        /// Gets previous work day, skipping weekends and public holidays
        /// </summary>
        public Date PreviousWorkDay()
        {
            return LessWorkDays(1);
        }


        private bool SkipHolidaysAndWeekends(DateTime date)
        {
            return _publicHolidayCalendar.IsHoliday(date) || IsDateWeekend(date);
        }


        private bool SkipWeekends(DateTime date)
        {
            return IsDateWeekend(date);
        }


        private Date SubtractDays(int daysToSubtract, Func<DateTime, bool> testForDaysToSkip)
        {
            return AddOrSubtractDays(daysToSubtract, testForDaysToSkip, -1);
        }


        public DateTime ToDateTime()
        {
            return _dateTime;
        }


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Date);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return _dateTime.GetHashCode() + 29*_publicHolidayCalendar.GetHashCode();
            }
        }


        public override string ToString()
        {
            return _dateTime.ToShortDateString();
        }


        public string ToString(string format)
        {
            return _dateTime.ToString(format);
        }

        #endregion


        #region Class Members

        static Date()
        {
            MinValue = new Date(DateTime.MinValue);
            MaxValue = new Date(DateTime.MaxValue);
        }


        public static Date MaxValue { get; private set; }


        public static Date MinValue { get; private set; }

        public static Date Today
        {
            get { return new Date(Clock.Now); }
        }


        public static Date EarlierOf(params Date[] dates)
        {
            //this is an optimised way of checking that dates is not null and at the same time executing the
            //query. It relies on the fact that dates.Min throws an ArgumentNullException when dates is null
            Date result = null;
#if !SILVERLIGHT
            Check.Require(delegate {
                result = dates.Min();
            });
#endif
            return result;
        }


        public static Date From(DateTime? value)
        {
            return value != null ? new Date((DateTime) value) : null;
        }


        /// <summary>
        /// Determines whether the Date part of <paramref name="left"/> and <paramref name="right"/> are equal
        /// </summary>
        public static bool DatesEqual(DateTime? left, DateTime? right)
        {
            return From(left) == From(right);
        }


        /// <summary>
        /// Determines whether the Date part of <paramref name="left"/> and <paramref name="right"/> are equal
        /// </summary>
        public static bool DatesEqual(DateTime left, DateTime right)
        {
            return From(left) == From(right);
        }


        public static Date LaterOf(params Date[] dates)
        {
            //this is an optimised way of checking that dates is not null and at the same time executing the
            //query. It relies on the fact that dates.Max throws an ArgumentNullException when dates is null
            Date result = null;
#if !SILVERLIGHT
            Check.Require(delegate {
                result = dates.Max();
            });
#endif
            return result;
        }

        #endregion


        public static bool operator ==(Date date1, Date date2)
        {
            return Equals(date1, date2);
        }


        public static bool operator >(Date date1, Date date2)
        {
            if (date1 == null || date2 == null)
                return false;
            else
                return (date1.CompareTo(date2) > 0);
        }


        public static bool operator >=(Date date1, Date date2)
        {
            if (date1 == null || date2 == null)
                return false;
            else
                return (date1.CompareTo(date2) >= 0);
        }


        public static implicit operator Date(string dateString)
        {
            if (StringComparer.InvariantCultureIgnoreCase.Equals(dateString, "today"))
            {
                return Today;
            }
            return new Date(DateTime.Parse(dateString));
        }


        public static implicit operator Date(DateTime dateTime)
        {
            return new Date(dateTime);
        }


        public static implicit operator Date(DateTime? dateTime)
        {
            return dateTime != null ? new Date((DateTime) dateTime) : null;
        }


        public static implicit operator DateTime(Date date)
        {
            return date != null ? date._dateTime : DateTime.MinValue;
        }


        public static implicit operator DateTime?(Date date)
        {
            return date != null ? date._dateTime : (DateTime?) null;
        }


        public static bool operator !=(Date date1, Date date2)
        {
            return !Equals(date1, date2);
        }


        public static bool operator <(Date date1, Date date2)
        {
            if (date1 == null || date2 == null)
                return false;
            else
                return (date1.CompareTo(date2) < 0);
        }


        public static bool operator <=(Date date1, Date date2)
        {
            if (date1 == null || date2 == null)
                return false;
            else
                return (date1.CompareTo(date2) <= 0);
        }


        public static int operator -(Date date1, Date date2)
        {
            return date1 < date2
                       ? new DateRange(date1, date2).WeekDays - 1
                       : -(new DateRange(date2, date1).WeekDays - 1);
        }
    }



    public static class DateExtensions
    {
        #region Class Members

        public static IEnumerable<DateRange> ToDateRanges(this IEnumerable<Date> source)
        {
            Date startOfCurrentRange = null;
            foreach (var current in new SmartEnumerable<Date>(source.SkipNulls().Distinct()))
            {
                if (current.IsFirst && current.IsLast)
                {
                    yield return new DateRange(current.Value, current.Value);
                    break;
                }

                if (current.IsFirst)
                {
                    startOfCurrentRange = current.Value;
                    continue;
                }

                if (current.Value.PreviousDay() != current.Previous)
                {
                    yield return new DateRange(startOfCurrentRange, current.Previous);
                    startOfCurrentRange = current.Value;
                }
                if (current.IsLast)
                {
                    yield return new DateRange(startOfCurrentRange, current.Value);
                }
            }
        }

        #endregion
    }
}