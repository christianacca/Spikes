using System;
using System.Linq;
using Eca.Commons.Extensions;

namespace Eca.Commons.Dates
{
    public sealed class TimeRange : IEquatable<TimeRange>
    {
        #region Member Variables

        private readonly TimeSpan _end;
        private readonly TimeSpan _start;

        #endregion


        #region Constructors

        /// <remarks>
        /// <para>
        /// Any day component will be removed from the <paramref name="start"/> and <paramref name="end"/> supplied.
        /// </para>
        /// <para>
        /// If <paramref name="start"/> is null, then <see cref="Start"/> will be set to <see cref="MinStart"/>;
        /// if <paramref name="end"/> is null, then <see cref="End"/> will be set to <see cref="MaxEnd"/>.
        /// </para>
        /// </remarks>
        public TimeRange(TimeSpan? start, TimeSpan? end)
        {
            _start = start.SafeStripDayComponent() ?? MinStart;
            _end = end.SafeStripDayComponent() ?? MaxEnd;

            Check.Require(_start <= _end, "Start before End is not allowed");
        }

        #endregion


        #region Properties

        public TimeSpan End
        {
            get { return _end; }
        }


        public TimeSpan Start
        {
            get { return _start; }
        }

        #endregion


        #region IEquatable<TimeRange> Members

        public bool Equals(TimeRange other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._end.Equals(_end) && other._start.Equals(_start);
        }

        #endregion


        public bool Contains(TimeSpan? value)
        {
            if (value == null) return false;

            return Start <= value && value <= End;
        }


        public TimeRange EnlargeToContain(TimeSpan? timeToContain)
        {
            if (timeToContain == null || Contains(timeToContain)) return this;

            return new TimeRange(EarlierOf(timeToContain, Start), LaterOf(timeToContain, End));
        }


        private string FormateAsTime(TimeSpan value)
        {
// ReSharper disable FormatStringProblem
            return String.Format("{0:hh\\:mm\\:ss}", value);
// ReSharper restore FormatStringProblem
        }


        #region Overridden object methods

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TimeRange)) return false;
            return Equals((TimeRange) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return (_end.GetHashCode()*397) ^ _start.GetHashCode();
            }
        }


        public override string ToString()
        {
            return string.Format("{0} - {1}", FormateAsTime(_start), FormateAsTime(_end));
        }

        #endregion


        #region Class Members

        /// <summary>
        /// The maximum range possible
        /// </summary>
        public static readonly TimeRange Max;

        /// <summary>
        /// The latest end time for a range
        /// </summary>
        public static readonly TimeSpan MaxEnd;

        /// <summary>
        /// The earliest start time for a range
        /// </summary>
        public static readonly TimeSpan MinStart;


        static TimeRange()
        {
            MaxEnd = new TimeSpan(0, 23, 59, 59);
            MinStart = new TimeSpan(0, 0, 0, 1);
            Max = new TimeRange(MinStart, MaxEnd);
        }


        private static TimeSpan? EarlierOf(params TimeSpan?[] times)
        {
            //this is an optimised way of checking that times is not null and at the same time executing the
            //query. It relies on the fact that dates.Min throws an ArgumentNullException when dates is null
            TimeSpan? result = null;
            Check.Require(delegate {
                result = times.Select(t => t.SafeStripDayComponent()).Min();
            });
            return result;
        }


        private static TimeSpan? LaterOf(params TimeSpan?[] times)
        {
            //this is an optimised way of checking that times is not null and at the same time executing the
            //query. It relies on the fact that dates.Min throws an ArgumentNullException when dates is null
            TimeSpan? result = null;
            Check.Require(delegate {
                result = times.Select(t => t.SafeStripDayComponent()).Max();
            });
            return result;
        }

        #endregion


        public static bool operator ==(TimeRange left, TimeRange right)
        {
            return Equals(left, right);
        }


        public static bool operator !=(TimeRange left, TimeRange right)
        {
            return !Equals(left, right);
        }
    }
}