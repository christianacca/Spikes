using System;
using System.Collections.Generic;
using System.Linq;

namespace Eca.Commons
{
    public static class ListExtensions
    {
        #region Class Members

        /// <summary> 
        /// Returns all distinct elements of the given source, where "distinctness" 
        /// is determined via a projection and the default eqaulity comparer for the projected type. 
        /// </summary> 
        /// <remarks> 
        /// This operator uses deferred execution and streams the results, although 
        /// a set of already-seen keys is retained. If a key is seen multiple times, 
        /// only the first element with that key is returned. 
        /// </remarks> 
        /// <typeparam name="TSource">Type of the source sequence</typeparam> 
        /// <typeparam name="TKey">Type of the projected element</typeparam> 
        /// <param name="source">Source sequence</param> 
        /// <param name="keySelector">Projection for determining "distinctness"</param> 
        /// <returns>A sequence consisting of distinct elements from the source sequence, 
        /// comparing them by the specified key projection.</returns>
        /// Ripped from http://code.google.com/p/morelinq/source/browse/trunk/MoreLinq/DistinctBy.cs
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                                     Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }


        /// <summary> 
        /// Returns all distinct elements of the given source, where "distinctness" 
        /// is determined via a projection and the specified comparer for the projected type. 
        /// </summary> 
        /// <remarks> 
        /// This operator uses deferred execution and streams the results, although 
        /// a set of already-seen keys is retained. If a key is seen multiple times, 
        /// only the first element with that key is returned. 
        /// </remarks> 
        /// <typeparam name="TSource">Type of the source sequence</typeparam> 
        /// <typeparam name="TKey">Type of the projected element</typeparam> 
        /// <param name="source">Source sequence</param> 
        /// <param name="keySelector">Projection for determining "distinctness"</param> 
        /// <param name="comparer">The equality comparer to use to determine whether or not keys are equal. 
        /// If null, the default equality comparer for <c>TSource</c> is used.</param> 
        /// <returns>A sequence consisting of distinct elements from the source sequence, 
        /// comparing them by the specified key projection.</returns> 
        ///Ripped from http://code.google.com/p/morelinq/source/browse/trunk/MoreLinq/DistinctBy.cs
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                                     Func<TSource, TKey> keySelector,
                                                                     IEqualityComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            return DistinctByImpl(source, keySelector, comparer);
        }


        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source,
                                                                          Func<TSource, TKey> keySelector,
                                                                          IEqualityComparer<TKey> comparer)
        {
#if !NO_HASHSET
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
#else 
    // 
    // On platforms where LINQ is available but no HashSet<T> 
    // (like on Silverlight), implement this operator using  
    // existing LINQ operators. Using GroupBy is slightly less 
    // efficient since it has do all the grouping work before 
    // it can start to yield any one element from the source. 
    // 
 
            return source.GroupBy(keySelector, comparer).Select(g => g.First());
#endif
        }


        /// <summary>
        /// Moves all items in <paramref name="source"/> that matches the supplied predicate into <paramref name="destination"/>
        /// </summary>
        public static void MoveMatchesTo<T>(this ICollection<T> source,
                                            ICollection<T> destination,
                                            Func<T, bool> selector)
        {
            var itemsToMove = source.Where(selector).ToList();
            foreach (var item in itemsToMove)
            {
                destination.Add(item);
                source.Remove(item);
            }
        }


        /// <summary>
        /// Moves all items in <paramref name="itemsToMove"/> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        public static void MoveRange<T>(this ICollection<T> source,
                                        IEnumerable<T> itemsToMove,
                                        ICollection<T> destination)
        {
            var list = itemsToMove.ToList();
            source.MoveMatchesTo(destination, list.Contains);
        }

        
        /// <summary>
        /// Moves <paramref name="itemToMove"/> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        public static void Move<T>(this ICollection<T> source,
                                        T itemToMove,
                                        ICollection<T> destination)
        {
            source.MoveRange(new[] { itemToMove }, destination);
        }


        /// <summary>
        /// WARNING - this is untested
        /// </summary>
        public static void RemoveDuplicates<T>(this IList<T> source)
        {
            var noDuplicates = new List<T>();
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (noDuplicates.Contains(source[i]))
                    source.RemoveAt(i);
                else
                    noDuplicates.Add(source[i]);
            }
        }


        /// <summary>
        /// Remove all items in <paramref name="source"/> that match the supplied predicate
        /// </summary>
        public static void RemoveMatches<T>(this ICollection<T> source, Func<T, bool> selector)
        {
            var itemsToRemove = source.Where(selector).ToList();
            foreach (var item in itemsToRemove)
            {
                source.Remove(item);
            }
        }


        /// <summary>
        /// Remove all items in <paramref name="itemsToRemove"/> from <paramref name="source"/>
        /// </summary>
        public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> itemsToRemove)
        {
            var list = itemsToRemove.ToList();
            RemoveMatches(source, list.Contains);
        }


        /// <summary>
        /// Performs an in-place replacement of <paramref name="replace"/> within <paramref name="source"/> with <paramref name="replacement"/>
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="replace"/> does not exist in list</exception>
        public static void Replace<T>(this IList<T> source, T replace, T replacement)
        {
            int existingPosition = source.IndexOf(replace);
            if (existingPosition == -1)
            {
                throw new ArgumentException("elment does not exist in collection", "replace");
            }
            source.RemoveAt(existingPosition);
            source.Insert(existingPosition, replacement);
        }

        #endregion
    }
}