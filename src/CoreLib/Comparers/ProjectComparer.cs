// origianl source: http://www.yoda.arachsys.com/csharp/miscutil/

using System;
using System.Collections.Generic;

namespace Eca.Commons.Comparers
{
    /// <summary>
    /// Non-generic class to produce instances of the generic class,
    /// optionally using type inference.
    /// </summary>
    public static class ProjectionComparer
    {
        #region Class Members

        /// <summary>
        /// Creates an instance of ProjectionComparer using the specified projection.
        /// </summary>
        /// <typeparam name="TSource">Type parameter for the elements to be compared</typeparam>
        /// <typeparam name="TKey">Type parameter for the keys to be compared, after being projected from the elements</typeparam>
        /// <param name="projection">Projection to use when determining the key of an element</param>
        /// <returns>A comparer which will compare elements by projecting each element to its key, and comparing keys</returns>
        public static ProjectionComparer<TSource, TKey> Create<TSource, TKey>(Func<TSource, TKey> projection)
        {
            return new ProjectionComparer<TSource, TKey>(projection);
        }


        /// <summary>
        /// Creates an instance of ProjectionComparer using the specified projection.
        /// The ignored parameter is solely present to aid type inference.
        /// </summary>
        /// <typeparam name="TSource">Type parameter for the elements to be compared</typeparam>
        /// <typeparam name="TKey">Type parameter for the keys to be compared, after being projected from the elements</typeparam>
        /// <param name="ignored">Value is ignored - type may be used by type inference</param>
        /// <param name="projection">Projection to use when determining the key of an element</param>
        /// <returns>A comparer which will compare elements by projecting each element to its key, and comparing keys</returns>
        public static ProjectionComparer<TSource, TKey> Create<TSource, TKey>
            (TSource ignored,
             Func<TSource, TKey> projection)
        {
            return new ProjectionComparer<TSource, TKey>(projection);
        }

        #endregion
    }



    /// <summary>
    /// Class generic in the source only to produce instances of the 
    /// doubly generic class, optionally using type inference.
    /// </summary>
    [SkipFormatting]
    public static class ProjectionComparer<TSource>
    {
        #region Class Members

        /// <summary>
        /// Creates an instance of ProjectionComparer using the specified projection.
        /// </summary>
        /// <typeparam name="TKey">Type parameter for the keys to be compared, after being projected from the elements</typeparam>
        /// <param name="projection">Projection to use when determining the key of an element</param>
        /// <returns>A comparer which will compare elements by projecting each element to its key, and comparing keys</returns>        
        public static ProjectionComparer<TSource, TKey> Create<TKey>(Func<TSource, TKey> projection)
        {
            return new ProjectionComparer<TSource, TKey>(projection);
        }

        #endregion
    }



    /// <summary>
    /// Comparer which projects each element of the comparison to a key, and then compares
    /// those keys using the specified (or default) comparer for the key type.
    /// </summary>
    /// <typeparam name="TSource">Type of elements which this comparer will be asked to compare</typeparam>
    /// <typeparam name="TKey">Type of the key projected from the element</typeparam>
    public class ProjectionComparer<TSource, TKey> : IComparer<TSource>
    {
        #region Member Variables

        private readonly IComparer<TKey> comparer;
        private readonly Func<TSource, TKey> projection;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new instance using the specified projection, which must not be null.
        /// The default comparer for the projected type is used.
        /// </summary>
        /// <param name="projection">Projection to use during comparisons</param>
        public ProjectionComparer(Func<TSource, TKey> projection)
            : this(projection, null) {}


        /// <summary>
        /// Creates a new instance using the specified projection, which must not be null.
        /// </summary>
        /// <param name="projection">Projection to use during comparisons</param>
        /// <param name="comparer">The comparer to use on the keys. May be null, in
        /// which case the default comparer will be used.</param>
        public ProjectionComparer(Func<TSource, TKey> projection, IComparer<TKey> comparer)
        {
            if (projection == null) throw new ArgumentNullException("projection");
            this.comparer = comparer ?? Comparer<TKey>.Default;
            this.projection = projection;
        }

        #endregion


        #region IComparer<TSource> Members

        /// <summary>
        /// Compares x and y by projecting them to keys and then comparing the keys. 
        /// Null values are not projected; they obey the
        /// standard comparer contract such that two null values are equal; any null value is
        /// less than any non-null value.
        /// </summary>
        public int Compare(TSource x, TSource y)
        {
            // Don't want to project from nullity
            if (x == null && y == null)
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            return comparer.Compare(projection(x), projection(y));
        }

        #endregion
    }
}