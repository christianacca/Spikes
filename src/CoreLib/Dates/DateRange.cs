using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons.Extensions;
#if !SILVERLIGHT
using NValidate.Framework;
#endif

namespace Eca.Commons.Dates
{
#if !SILVERLIGHT
    [Serializable]
    [IsBuiltinType]
#endif
    public class DateRange : IEquatable<DateRange>
    {
        #region Member Variables

        internal const string NullString = "N/A";
        private bool? _containsPublicHoliday;
        private int? _days;
        protected readonly Date _end;
        private bool? _isPublicHolidayRange;
        protected readonly Date _start;
        private int? _weekDays;
        private int? _workDays;

        #endregion


        #region Constructors

        protected DateRange() {}


        public DateRange(Date start, Date end)
        {
#if !SILVERLIGHT
            Check.Require(start != null, "start be null");
            Check.Require(end != null, "end be null");
            Check.Require(start <= end, "Start date after end date");
#endif

            _start = start;
            _end = end;
        }

        #endregion


        #region Properties

        public int Days
        {
            get
            {
                if (_days == null) _days = GetDays(date => true).Count();
                return _days ?? 0;
            }
        }


        public int Years
        {
            get { return Days/365; }
        }


        public Date End
        {
            get { return _end; }
        }

        public virtual bool IsNull
        {
            get { return false; }
        }

        public bool IsPublicHolidayRange
        {
            get
            {
                if (_isPublicHolidayRange == null) _isPublicHolidayRange = Start.IsPublicHoliday || End.IsPublicHoliday;
                return _isPublicHolidayRange ?? false;
            }
        }

        public Date Start
        {
            get { return _start; }
        }

        public int WeekDays
        {
            get
            {
                if (_weekDays == null) _weekDays = GetWeekDays().Count();
                return _weekDays ?? 0;
            }
        }

        public int WorkDays
        {
            get
            {
                if (_workDays == null) _workDays = GetWorkDays().Count();
                return _workDays ?? 0;
            }
        }

        #endregion


        #region IEquatable<DateRange> Members

        public bool Equals(DateRange dateRange)
        {
            if (ReferenceEquals(null, dateRange)) return false;
            if (ReferenceEquals(this, dateRange)) return true;
            return Equals(_end, dateRange._end) && Equals(_start, dateRange._start);
        }

        #endregion


        public virtual bool After(Date date)
        {
            return _start > date;
        }


        public bool After(DateRange value)
        {
            return After(value.End);
        }


        public virtual bool Before(Date date)
        {
            return _end < date;
        }


        public bool Before(DateRange value)
        {
            return Before(value.Start);
        }


        public virtual bool Contains(Date dateToCheck)
        {
            return dateToCheck >= _start && dateToCheck <= _end;
        }


        public virtual bool Contains(DateRange dateRange)
        {
            return dateRange.Start >= _start && dateRange.End <= _end;
        }


        public virtual bool ContainsPublicHoliday()
        {
            if (_containsPublicHoliday == null) _containsPublicHoliday = ContainsPublicHolidayNonCached();
            return _containsPublicHoliday ?? false;
        }


        public virtual DateRange EnlargeToContain(Date dateToContain)
        {
            if (dateToContain == null || Contains(dateToContain)) return this;

            return new DateRange(Date.EarlierOf(dateToContain, Start), Date.LaterOf(dateToContain, End));
        }


        private bool ContainsPublicHolidayNonCached()
        {
            Date currentDate = _start;
            do
            {
                if (currentDate.IsPublicHoliday) return true;
                currentDate = currentDate.NextWeekDay();
            } while (currentDate <= _end);
            return false;
        }


        public virtual bool ContainsStartOf(DateRange rangeToCheck)
        {
            return Contains(rangeToCheck.Start);
        }


        public virtual IList<DateRange> DivideBefore(Date date)
        {
            IList<DateRange> dateRanges = new List<DateRange>();
            if (Contains(date))
            {
                dateRanges.Add(new DateRange(_start, date.PreviousWorkDay()));
                dateRanges.Add(new DateRange(date, _end));
            }
            else
            {
                dateRanges.Add(this);
            }
            return dateRanges;
        }


        public virtual IEnumerable<Date> GetAllDays()
        {
            return GetDays(obj => true);
        }


        private Date GetCurrentRangeEndDate(Date currentDate, Date rangeEndDate, Predicate<Date> criterion)
        {
            return (currentDate == rangeEndDate && !criterion(currentDate))
                       ? currentDate
                       : currentDate.PreviousWorkDay();
        }


        /// <summary>
        /// Returns an enumerator that iterates the days that satisfy the Predicate <paramref name="test"/>
        /// </summary>
        /// <param name="test">test that determines the days to enumerate</param>
        /// <returns></returns>
        public virtual IEnumerable<Date> GetDays(Func<Date, bool> test)
        {
            Date currentDate = _start;
            while (currentDate <= _end)
            {
                if (test(currentDate)) yield return currentDate;
                currentDate = currentDate.NextDay();
            }
        }


        /// <summary>
        /// Returns an enumerator that iterates the days that do not satisfy the Predicate <paramref name="test"/>
        /// </summary>
        /// <param name="test">test that determines the days to enumerate</param>
        /// <returns></returns>
        public virtual IEnumerable<Date> GetDaysExcept(Func<Date, bool> test)
        {
            Date currentDate = _start;
            while (currentDate <= _end)
            {
                if (!test(currentDate)) yield return currentDate;
                currentDate = currentDate.NextDay();
            }
        }


        private Date GetLastContinuousDayThatSatisfiesCriterion(Date date, Predicate<Date> criterion)
        {
            if (!criterion(date)) return null;
            while (criterion(date))
            {
                date = date.NextWeekDay();
            }
            return date.PreviousWeekDay();
        }


        public virtual IEnumerable<Date> GetWeekDays()
        {
            return GetDays(date => !date.IsWeekend);
        }


        public virtual IEnumerable<Date> GetWorkDays()
        {
            return GetDays(date => date.IsWorkDay);
        }


        public IEnumerable<DateRange> InsertAndShiftForward(DateRange insert, Func<Date, bool> skip)
        {
            if (Before(insert))
            {
                yield return this;
            }
            else
            {
                int daysToMove = insert.GetDaysExcept(skip).Count();

                if (After(insert) || OverlapsEnd(insert) || Equals(insert) ||
                    (Start.Equals(insert.Start) && Contains(insert)) || IsSubsumedBy(insert))
                {
                    yield return MoveByDays(daysToMove, skip);
                }
                else if (OverlapsStart(insert) || (End.Equals(insert.End) && Contains(insert)))
                {
                    DateRange beforeInsert = Subtract(insert).Single();
                    yield return beforeInsert;

                    DateRange overlapping = Intersect(insert).MoveByDays(daysToMove, skip);
                    yield return overlapping;
                }
                else //Subsumes insert
                {
                    DateRange beforeInsert = Subtract(insert).First();
                    yield return beforeInsert;

                    var afterInsert = new DateRange(beforeInsert.End.IncrementDays(1, skip), End);
                    yield return afterInsert.MoveByDays(daysToMove, skip);
                }
            }
        }


        public DateRange Intersect(DateRange value)
        {
#if !SILVERLIGHT
            Check.Require(() => Demand.The.Param(() => value).IsNotNull());
#endif

            if (!Overlaps(value)) return null;
            return new DateRange(Date.LaterOf(Start, value.Start), Date.EarlierOf(End, value.End));
        }


        public bool IsContigousOrOverlaps(DateRange range)
        {
            if (range == null) return false;
            IList<DateRange> ranges = new List<DateRange> {this, range}.OrderBy(dateRange => dateRange.Start).ToList();
            return
                ranges.Satisfy(
                    (previous, current) =>
                    current.Start.PreviousWeekDay() == previous.End || current.Overlaps(previous));
        }


        private bool IsSubsumedBy(DateRange value)
        {
            return value.Start <= Start && End <= value.End;
        }


        public DateRange Join(DateRange range)
        {
#if !SILVERLIGHT
            Check.Require(() => Demand.The.Param(range.IsNull, "dateRange").IsFalse("Null-object received"));
            Check.Require(
                () =>
                Demand.The.Param(IsContigousOrOverlaps(range), "dateRange").IsTrue(
                    "Ranges are not contiguous and do not overlap"));
#endif
            return new DateRange(Date.EarlierOf(Start, range.Start), Date.LaterOf(End, range.End));
        }


        public DateRange MoveByDays(int days, Func<Date, bool> skip)
        {
            int existingDuration = GetDaysExcept(skip).Count();
            Date newStart = _start.IncrementDays(days, skip);
            Date newEnd = newStart.IncrementDays(existingDuration - 1, skip);
            return new DateRange(newStart, newEnd);
        }


        public DateRange MoveByWeekDays(int days)
        {
            return MoveByDays(days, OnWeekend);
        }


        public DateRange MoveByWorkDays(int days)
        {
            return MoveByDays(days, WeekendOrPublicHoliday);
        }


        private bool NotInsideCriteriaRange(Date currentDate, Date currentRangeStartDate, Predicate<Date> criterion)
        {
            return currentDate >= currentRangeStartDate && !criterion(currentRangeStartDate);
        }


        public bool Overlaps(DateRange value)
        {
            return End >= value.Start && Start <= value.End;
        }


        public bool OverlapsEnd(DateRange value)
        {
            return Start <= value.End && (Start > value.Start && End > value.End);
        }


        public bool OverlapsStart(DateRange value)
        {
            return Start < value.Start && (End >= value.Start && End < value.End);
        }


        public virtual IList<DateRange> SplitOn(Predicate<Date> criterion, bool includeCriterionDates, bool shiftDates)
        {
            DateRange initialRange = Trim(OnWeekend);
            Date currentRangeStart = initialRange.Start;
            Date rangeEnd = initialRange.End;

            IList<DateRange> result = new List<DateRange>();
            Date currentDate = initialRange.Start;
            do
            {
                if (criterion(currentDate) || currentDate == rangeEnd)
                {
                    Date currentRangeEnd = GetCurrentRangeEndDate(currentDate, rangeEnd, criterion);

                    if (NotInsideCriteriaRange(currentDate, currentRangeStart, criterion))
                    {
                        result.Add(new DateRange(currentRangeStart, currentRangeEnd));
                        //currentDate = currentDate.NextWeekDay();
                        //continue;
                    }

                    if (criterion(currentDate) && (shiftDates || includeCriterionDates))
                    {
                        currentRangeEnd = GetLastContinuousDayThatSatisfiesCriterion(currentDate, criterion);
                        var rangeToExclude = new DateRange(currentDate, currentRangeEnd);
                        if (includeCriterionDates)
                            result.Add(rangeToExclude);
                        currentDate = currentRangeEnd;
                        if (shiftDates)
                            rangeEnd = rangeEnd.AddWeekDays(rangeToExclude.WeekDays);
                    }

                    currentRangeStart = currentDate.NextWorkDay();
                }
                currentDate = currentDate.NextWeekDay();
            } while (currentDate <= rangeEnd);
            return result;
        }


        public virtual IList<DateRange> SplitOnPublicHoliday(bool includePublicHolidayInRanges)
        {
            return SplitOn(date => date.IsPublicHoliday, includePublicHolidayInRanges, false);
        }


        public IEnumerable<DateRange> Subtract(DateRange value)
        {
            DateRange intersection = Intersect(value);
            if (intersection == null)
            {
                yield return this;
            }
            else
            {
                if (Start < intersection.Start) yield return new DateRange(Start, intersection.Start.PreviousDay());
                if (End > intersection.End) yield return new DateRange(intersection.End.NextDay(), End);
            }
        }


        public IEnumerable<DateRange> Subtract(IEnumerable<DateRange> rangesToSubtract)
        {
            IEnumerable<DateRange> result = new List<DateRange> {this};
            foreach (DateRange range in rangesToSubtract)
            {
                result = result.Subtract(range);
            }

            return result;
        }


        /// <summary>
        /// Returns this range trimmed at both start and end for the reason specified.
        /// </summary>
        /// <param name="reason">The test that determines that a date at the start and end should be trimmed.</param>
        public virtual DateRange Trim(Func<Date, bool> reason)
        {
            DateRange trimmedStart = TrimStart(reason);
            return trimmedStart == null ? null : trimmedStart.TrimEnd(reason);
        }


        /// <summary>
        /// Returns this range trimmed at the end for the reason specified
        /// </summary>
        /// <param name="reason">The test that determines that a date at the start and end should be trimmed.</param>
        public virtual DateRange TrimEnd(Func<Date, bool> reason)
        {
            var trimmedDateRange = new DateRange(_start, _end);

            Date currentDate = _end;
            while (currentDate > _start && reason(currentDate))
            {
                currentDate = currentDate.PreviousDay();
                trimmedDateRange = new DateRange(_start, currentDate);
            }
            if (reason(trimmedDateRange.End)) return null;
            return trimmedDateRange;
        }


        /// <summary>
        /// Returns this range trimmed at the start for the reason specified
        /// </summary>
        /// <param name="reason">The test that determines that a date at the start and end should be trimmed.</param>
        public virtual DateRange TrimStart(Func<Date, bool> reason)
        {
            var trimmedDateRange = new DateRange(_start, _end);

            Date currentDate = _start;
            while (currentDate < _end && reason(currentDate))
            {
                currentDate = currentDate.NextDay();
                trimmedDateRange = new DateRange(currentDate, _end);
            }
            if (reason(trimmedDateRange.Start)) return null;
            return trimmedDateRange;
        }


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as DateRange);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return ((_end != null ? _end.GetHashCode() : 0)*397) ^ (_start != null ? _start.GetHashCode() : 0);
            }
        }


        public override string ToString()
        {
            return String.Format("{0} - {1}", _start, _end);
        }


        public string ToString(string dateFormat)
        {
            return String.Format("{0} - {1}", _start.ToString(dateFormat), _end.ToString(dateFormat));
        }

        #endregion


        #region Class Members

        private static readonly DateRange _nullInstance = new NullDateRange();

        public static DateRange Null
        {
            get { return _nullInstance; }
        }


        /// <exception cref="ArgumentException">Either both must be null or neither</exception>
        public static DateRange From(DateTime? start, DateTime? end)
        {
            if (end == null && start != null)
            {
                throw new ArgumentException("end is null, start non-null\r\nEither both must be null or neither");
            }


            if (start == null && end != null)
            {
                throw new ArgumentException("start is null, start non-null\r\nEither both must be null or neither");
            }

            if (start == null) return Null;

            return new DateRange(start, end);
        }


        private static int IndexOf(string search, string find, int occurance)
        {
            //TODO: Change this terrible implementation: not robust, not very reabable
            string fragment = search;
            int result = 0;
            for (int i = 0; i < occurance; i++)
            {
                int currentPosition = fragment.IndexOf(find);
                result += currentPosition;
                fragment = fragment.Substring(currentPosition + 1);
            }
            return result + (occurance - 1);
        }


        public static bool OnWeekend(Date date)
        {
            return date.IsWeekend;
        }


#if !SILVERLIGHT
        public static DateRange TryCreate(Date start, Date end)
        {
            using (new Check.DisableDotNetAssertsScope())
            {
                try
                {
                    return new DateRange(start, end);
                }
                catch (Exception)
                {
                    return Null;
                }
            }
        }
#endif


        static DateRange()
        {
            Max = new DateRange(DateTime.MinValue, Date.MaxValue);
        }


        public static bool WeekendOrPublicHoliday(Date date)
        {
            return date.IsWeekend || date.IsPublicHoliday;
        }

        #endregion


        private class NullDateRange : DateRange
        {
            #region Properties

            public override bool IsNull
            {
                get { return true; }
            }

            #endregion


            public override bool ContainsPublicHoliday()
            {
                return false;
            }


            public override IList<DateRange> SplitOnPublicHoliday(bool includePublicHolidayInRanges)
            {
                return new List<DateRange> {this};
            }


            public override DateRange Trim(Func<Date, bool> reason)
            {
                return this;
            }


            public override DateRange TrimEnd(Func<Date, bool> reason)
            {
                return this;
            }


            public override DateRange TrimStart(Func<Date, bool> reason)
            {
                return this;
            }


            #region Overridden object methods

            public override string ToString()
            {
                return NullString;
            }

            #endregion
        }



        public static implicit operator DateRange(string dateRangeString)
        {
            if (dateRangeString == NullString) return Null;

            int dateDelimiter = IndexOf(dateRangeString, "-", 3);
            string start = dateRangeString.Substring(0, dateDelimiter - 1).Trim();
            string end = dateRangeString.Substring(dateDelimiter + 1).Trim();
            return new DateRange(start, end);
        }


        public static readonly DateRange Max;
    }



    public static class DateRangeExtensions
    {
        #region Class Members

        /// <summary>
        /// Determins whether the elements within <paramref name="ranges"/> represent an uninterrupted 
        /// period of time (are connected without a break)
        /// </summary>
        public static bool AreContiguous(this IEnumerable<DateRange> ranges)
        {
            return ranges.Satisfy((previous, current) => current.Start.PreviousDay() == previous.End);
        }


        public static bool AreContiguousOrOverlap(this IEnumerable<DateRange> ranges)
        {
            return
                ranges.Satisfy(
                    (previous, current) => current.Start.PreviousWeekDay() == previous.End || current.Overlaps(previous));
        }


        public static Date End(this IEnumerable<DateRange> source)
        {
            return source.Max(period => period.End);
        }


        private static ICollection<DateRange> FindAllContiguousOrOverlapingItemsFor(DateRange range,
                                                                                    IEnumerable<DateRange> ranges)
        {
            IList<DateRange> result = new List<DateRange> {range};
            int i = ranges.IndexOf(range) + 1;
            int count = ranges.Count();
            while (i < count && ranges.At(i - 1).IsContigousOrOverlaps(ranges.At(i)))
            {
                result.Add(ranges.At(i));
                i++;
            }
            return result;
        }


        public static DateRange Flatten(this IEnumerable<DateRange> ranges)
        {
            return Flatten(ranges, false);
        }


        public static DateRange Flatten(this IEnumerable<DateRange> ranges, bool ignoreContiguity)
        {
#if !SILVERLIGHT
            if (!ignoreContiguity)
                Check.Require(() => Demand.The.Param(ranges.AreContiguousOrOverlap(), "ranges").IsTrue());
#endif
            IEnumerable<DateRange> orderedRanges = ranges.OrderBy(range => range.Start);
            return new DateRange(orderedRanges.First().Start, orderedRanges.Last().End);
        }


        public static IEnumerable<DateRange> Intersect(this IEnumerable<DateRange> source, DateRange value)
        {
            return source.Select(range => range.Intersect(value)).SkipNulls();
        }


        public static ICollection<DateRange> JoinContiguousAndOverlaps(this IEnumerable<DateRange> ranges)
        {
            IList<DateRange> result = new List<DateRange>();

            int i = 0;
            while (i < ranges.Count())
            {
                if (i < ranges.Count() - 1 && ranges.At(i).IsContigousOrOverlaps(ranges.At(i + 1)))
                {
                    IList<DateRange> contiguousItems =
                        FindAllContiguousOrOverlapingItemsFor(ranges.At(i), ranges).ToList();
                    result.Add(contiguousItems.Flatten());
                    i += contiguousItems.Count;
                    continue;
                }
                else
                {
                    result.Add(ranges.At(i));
                }
                i++;
            }

            return result.OrderBy(x => x.Start).ToList();
        }


        public static Date Start(this IEnumerable<DateRange> source)
        {
            return source.Min(period => period.Start);
        }


        public static IEnumerable<DateRange> Subtract(this IEnumerable<DateRange> ranges, DateRange toSubtract)
        {
            return ranges.SelectMany(range => range.Subtract(toSubtract));
        }


        public static IEnumerable<DateRange> Trim(this IEnumerable<DateRange> ranges,
                                                  Func<Date, bool> reason)
        {
            return ranges.Select(range => range.Trim(reason)).SkipNulls();
        }


        public static IEnumerable<DateRange> TrimEnd(this IEnumerable<DateRange> ranges,
                                                     Func<Date, bool> reason)
        {
            return ranges.Select(range => range.TrimEnd(reason)).SkipNulls();
        }


        public static IEnumerable<DateRange> TrimStart(this IEnumerable<DateRange> ranges,
                                                       Func<Date, bool> reason)
        {
            return ranges.Select(range => range.TrimStart(reason)).SkipNulls();
        }

        #endregion
    }
}