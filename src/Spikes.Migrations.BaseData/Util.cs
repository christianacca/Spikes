using System;
using System.Collections.Generic;

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
            IEnumerator<T> iterator = source.GetEnumerator();
            bool hasElement = iterator.MoveNext();
            if (!hasElement)
            {
                yield break;
            }
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