// origianl source: http://www.yoda.arachsys.com/csharp/miscutil/

using System;
using System.Collections.Generic;

namespace Eca.Commons.Comparers
{
    /// <summary>
    /// Comparer to daisy-chain two existing comparers and 
    /// apply in sequence (i.e. sort by x then y)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class LinkedComparer<T> : IComparer<T>
    {
        #region Member Variables

        private readonly IComparer<T> primary, secondary;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new LinkedComparer
        /// </summary>
        /// <param name="primary">The first comparison to use</param>
        /// <param name="secondary">The next level of comparison if the primary returns 0 (equivalent)</param>
        public LinkedComparer(
            IComparer<T> primary,
            IComparer<T> secondary)
        {
            if (primary == null) throw new ArgumentNullException("primary");
            if (secondary == null) throw new ArgumentNullException("secondary");

            this.primary = primary;
            this.secondary = secondary;
        }

        #endregion


        #region IComparer<T> Members

        int IComparer<T>.Compare(T x, T y)
        {
            int result = primary.Compare(x, y);
            return result == 0 ? secondary.Compare(x, y) : result;
        }

        #endregion
    }
}