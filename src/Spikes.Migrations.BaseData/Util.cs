using System;
using System.Collections.Generic;
using System.Linq;

namespace Spikes.Migrations.BaseData
{
    public static class UtilExts
    {
        /// <summary>
        /// Slice <paramref name="source"/> every time <paramref name="slicingCondition"/> is satisfied
        /// </summary>
        /// <param name="source">sequence to slice</param>
        /// <param name="slicingCondition">function that will be called when iterating <paramref name="source"/>. Function will be passed the previous and current element that is being iterated</param>
        public static IEnumerable<IEnumerable<T>> SliceWhen<T>(this IEnumerable<T> source,
                                                               Func<T, T, bool> slicingCondition)
        {
            if (!source.Any()) yield break;

            IEnumerator<T> iterator = source.GetEnumerator();
            iterator.MoveNext();
            T prev = iterator.Current;
            var currentGroup = new List<T> { prev };
            while (iterator.MoveNext())
            {
                T current = iterator.Current;
                if (slicingCondition(prev, current))
                {
                    yield return currentGroup;
                    currentGroup = new List<T>();
                }
                currentGroup.Add(current);
                prev = current;
            }
            yield return currentGroup;
        }
    }
}