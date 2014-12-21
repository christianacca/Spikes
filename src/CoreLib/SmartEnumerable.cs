// This code originates here: http://www.yoda.arachsys.com/csharp/miscutil/
// Copyright (c) 2004-2008 Jon Skeet and Marc Gravell.
// AssemblyFileVersion: 1.0.0.271
// Modifications:
// - Added SkipFormattingAttribute to keep reformatting code layout rules from being code applied to by Resharper
// - Code formatting rules (except layout) applied
// - Namespace renamed from MiscUtil.Collections to Eca.Commons
// 
// NOTE: Once there are sufficient usage of cloned classes from the MiscUtil project, the clones should be removed
//       and all references replaced, with a reference to the compiled MiscUtil library

using System;
using System.Collections;
using System.Collections.Generic;

namespace Eca.Commons
{
    /// <summary>
    /// Type chaining an IEnumerable&lt;T&gt; to allow the iterating code
    /// to detect the first and last entries simply.
    /// </summary>
    /// <typeparam name="T">Type to iterate over</typeparam>
    [SkipFormatting]
    public class SmartEnumerable<T> : IEnumerable<SmartEnumerable<T>.Entry>
    {
        /// <summary>
        /// Enumerable we proxy to
        /// </summary>
        private readonly IEnumerable<T> enumerable;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="enumerable">Collection to enumerate. Must not be null.</param>
        public SmartEnumerable(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }
            this.enumerable = enumerable;
        }


        #region IEnumerable<SmartEnumerable<T>.Entry> Members

        /// <summary>
        /// Returns an enumeration of Entry objects, each of which knows
        /// whether it is the first/last of the enumeration, as well as the
        /// current value.
        /// </summary>
        public IEnumerator<Entry> GetEnumerator()
        {
            using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }
                bool isFirst = true;
                bool isLast = false;
                int index = 0;
                T previous = default(T);
                while (!isLast)
                {
                    T current = enumerator.Current;
                    isLast = !enumerator.MoveNext();
                    yield return new Entry(isFirst, isLast, current, previous, index++);
                    previous = current;
                    isFirst = false;
                }
            }
        }


        /// <summary>
        /// Non-generic form of GetEnumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        #region Nested type: Entry

        /// <summary>
        /// Represents each entry returned within a collection,
        /// containing the value and whether it is the first and/or
        /// the last entry in the collection's. enumeration
        /// </summary>
        [SkipFormatting]
        public class Entry
        {
            private readonly int index;
            private readonly bool isFirst;
            private readonly bool isLast;
            private readonly T previous;
            private readonly T value;


            internal Entry(bool isFirst, bool isLast, T value, T previous, int index)
            {
                this.isFirst = isFirst;
                this.isLast = isLast;
                this.value = value;
                this.previous = previous;
                this.index = index;
            }


            /// <summary>
            /// The value of the entry.
            /// </summary>
            public T Value
            {
                get { return value; }
            }

            /// <summary>
            /// Whether or not this entry is first in the collection's enumeration.
            /// </summary>
            public bool IsFirst
            {
                get { return isFirst; }
            }

            /// <summary>
            /// Whether or not this entry is last in the collection's enumeration.
            /// </summary>
            public bool IsLast
            {
                get { return isLast; }
            }

            /// <summary>
            /// The 0-based index of this entry (i.e. how many entries have been returned before this one)
            /// </summary>
            public int Index
            {
                get { return index; }
            }

            /// <summary>
            /// The previous <see cref="Value"/>
            /// </summary>
            public T Previous
            {
                get { return previous; }
            }
        }

        #endregion
    }
}