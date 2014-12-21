using System;
using NValidate.Framework;

namespace Eca.Commons.Extensions
{
    public static class TimeSpanExtensions
    {
        #region Class Members

        public static TimeSpan? SafeStripDayComponent(this TimeSpan? value)
        {
            return value == null ? null : value.StripDayComponent();
        }


        public static TimeSpan? StripDayComponent(this TimeSpan? value)
        {
            Check.Require(() => Demand.The.Param(() => value).IsNotNull());

// ReSharper disable PossibleInvalidOperationException
            return new TimeSpan(0, value.Value.Hours, value.Value.Minutes, value.Value.Seconds, value.Value.Milliseconds);
// ReSharper restore PossibleInvalidOperationException
        }

        #endregion
    }
}