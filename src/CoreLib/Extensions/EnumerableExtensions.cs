using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#if !SILVERLIGHT
using NValidate.Framework;
#endif

namespace Eca.Commons.Extensions
{
    public static class EnumerableExtensions
    {
        #region Class Members

        public static ICollection<T> AsReadOnlyList<T>(this ICollection<T> source)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => source).IsNotNull();
#endif

            var list = source as List<T>;
            if (list != null)
                return list.AsReadOnly();
            else
                return new List<T>(source);
        }


        /// <summary>
        /// Converts <paramref name="source"/> sequence into a sequence of strings by calling the <see cref="object.ToString"/>
        /// method on each element
        /// </summary>
        public static IEnumerable<string> AsStrings<T>(this IEnumerable<T> source)
        {
            return source.Select(item => item.ToString());
        }


        /// <summary>
        /// Same as <see cref="Enumerable.ElementAt{TSource}"/>
        /// </summary>
        public static T At<T>(this IEnumerable<T> source, int index)
        {
            return source.ElementAt(index);
        }


        /// <summary>
        /// Concatenates a sequence of strings
        /// </summary>
        public static string Concat<T>(this IEnumerable<T> source)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => source).IsNotNull();
#endif

            var result = new StringBuilder();
            foreach (T t in source)
            {
                result.Append(t);
            }
            return result.ToString();
        }


        /// <summary>
        /// Returns an empty sequence if <paramref name="source"/> is null
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }


        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items,
                                                     IEnumerable<T> other,
                                                     Func<T, TKey> getKey)
        {
            return from item in items
                   join otherItem in other on getKey(item)
                       equals getKey(otherItem) into tempItems
                   from temp in tempItems.DefaultIfEmpty()
                   where ReferenceEquals(null, temp) ||
                         temp.Equals(default(T))
                   select item;
        }


        public static IEnumerable<T> Except<T, TOther, TKey>(
            this IEnumerable<T> items,
            IEnumerable<TOther> other,
            Func<T, TKey> getItemKey,
            Func<TOther, TKey> getOtherKey)
        {
            return from item in items
                   join otherItem in other on getItemKey(item)
                       equals getOtherKey(otherItem) into tempItems
                   from temp in tempItems.DefaultIfEmpty()
                   where ReferenceEquals(null, temp) || temp.Equals(default(TOther))
                   select item;
        }


        /// <summary>
        /// Executes <paramref name="action"/> for each element in <paramref name="action"/>
        /// </summary>
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => source).IsNotNull();
#endif

            foreach (T item in source)
                action(item);
        }


        /// <summary>
        /// Executes <paramref name="action"/> for each element in <paramref name="action"/>
        /// </summary>
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => source).IsNotNull();
#endif

            foreach (var item in new SmartEnumerable<T>(source))
                action(item.Value, item.Index);
        }


        /// <summary>
        /// Computes  the hashcode for every element in <paramref name="source"/>
        /// </summary>
        /// <remarks>
        /// Compliment to <see cref="Enumerable.SequenceEqual{T}(IEnumerable{T},IEnumerable{T})"/>
        /// </remarks>
        public static int GetSequenceHashCode<T>(this IEnumerable<T> source)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => source).IsNotNull();
#endif

            unchecked
            {
                int result = 0;
                foreach (int hash in source.Select(t => t.GetHashCode()))
                {
                    result += 29*result + hash;
                }
                return result;
            }
        }


#if !SILVERLIGHT
        /// <summary>
        /// Uses <see cref="CollectionComparer"/> to determine whether the two collections are equivalent
        /// </summary>
        /// <param name="left">collection to compare</param>
        /// <param name="right">collection to compare</param>
        /// <param name="comparer">The comparer that will determine whether the elements in the collection are equal</param>
        public static bool SequenceEquivalent<T>(this T left, T right, IEqualityComparer comparer) where T : IEnumerable
        {
            return new CollectionComparer(comparer).Compare(left, right) == 0;
        }
#endif


        /// <summary>
        /// Determins whether there is an element within <paramref name="source"/> at the 
        /// <paramref name="index"/> position supplied
        /// </summary>
        public static bool HasElementAt<T>(this IEnumerable<T> source, int index)
        {
            try
            {
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
                source.ElementAt(index);
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }


        /// <summary>
        /// Determins the index of the specfic <paramref name="item"/> in the <paramref name="items"/> using
        /// the default <see cref="EqualityComparer{T}"/>
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            return IndexOf(items, item, (t, t1) => EqualityComparer<T>.Default.Equals(t, t1));
        }


        /// <summary>
        /// Determins the index of the specfic <paramref name="item"/> in the <paramref name="items"/> using
        /// the <paramref name="comparer"/>
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> items, T item, IEqualityComparer<T> comparer)
        {
            return IndexOf(items, item, comparer.Equals);
        }


        /// <summary>
        /// Determins the index of the specfic <paramref name="item"/> in the <paramref name="items"/> using
        /// the <paramref name="predicate"/>
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> items, T item, Func<T, T, bool> predicate)
        {
            int index = 0;

            foreach (T instance in items)
            {
                if (predicate(item, instance))
                {
                    return index;
                }

                ++index;
            }

            return -1;
        }


        /// <summary>
        /// Searches for the object that is referenced by <paramref name="objectReference"/> and returns the zero-based 
        /// index of the first occurance in the entire <paramref name="list"/>.
        /// </summary>
        /// <param name="list">the list to search</param>
        /// <param name="objectReference">contains a reference to the object to find (can be null)</param>
        /// <remarks>
        /// Returns -1 if the object referenced by <paramref name="objectReference"/> is not found
        /// </remarks>
        /// <exception cref="ArgumentNullException">where <paramref name="list"/> is null</exception>
        public static int IndexOfSame<T>(this IEnumerable<T> list, T objectReference) where T : class
        {
            return IndexOf(list, objectReference, ReferenceEquals);
        }


        /// <summary>
        /// Produces a new sequence that interleaves the <paramref name="separator"/> between each item within the <paramref name="source"/> sequence
        /// </summary>
        /// <remarks>
        /// Use this method in conjunction with <see cref="Concat{T}"/> to create a <paramref name="separator"/> delimited string containing the elements in <paramref name="source"/>
        /// Eg: <code>string pipeDelimitedList = new[] { "A", "C", "B" }.Intersperse("|").Concat();</code>
        /// </remarks>
        public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> source, T separator)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => source).IsNotNull();
#endif

            foreach (SmartEnumerable<T>.Entry entry in new SmartEnumerable<T>(source))
            {
                if (!entry.IsFirst) yield return separator;
                yield return entry.Value;
            }
        }


        /// <summary>
        /// Tests that there are no duplicate elements within this sequence by comparing elements 
        /// using <paramref name="comparer"/>
        /// </summary>
        public static bool IsUnique<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            var uniqueSequence = source.Distinct(comparer);
            return source.SequenceEqual(uniqueSequence, comparer);
        }


        /// <summary>
        /// Tests that there are no duplicate elements within this sequence by comparing elements 
        /// using the elements default equality comparison
        /// </summary>
        public static bool IsUnique<T>(this IEnumerable<T> source)
        {
            var uniqueSequence = source.Distinct();
            return source.SequenceEqual(uniqueSequence);
        }


        /// <summary>
        /// Add each item within <paramref name="itemsToAdd"/> into <paramref name="source"/>.
        /// </summary>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> itemsToAdd)
        {
#if !SILVERLIGHT
            Check.Require(() => Demand.The.Param(() => source).IsNotNull());
#endif
            if (itemsToAdd == null) return;

            itemsToAdd.ForEach(source.Add);
        }


        /// <summary>
        /// Projects each element of a sequence into a new form. Tests that there are no duplicate elements within the projected sequence 
        /// by comparing the projected elements for equality
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/></typeparam>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/></typeparam>
        /// <param name="source">A sequence of elements to test</param>
        /// <param name="selector">A transform function to apply to each element</param>
        /// <returns></returns>
        public static bool IsUnique<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var projected = source.Select(selector);
            return projected.SequenceEqual(projected.Distinct());
        }


        /// <summary>
        /// Produces a <paramref name="seperator"/> delimited string that concatenates all the elements in <paramref name="source"/>
        /// after they have been converted to a string by calling their <see cref="Object.ToString"/> method.
        /// </summary>
        /// <remarks>
        /// If <paramref name="source"/> is empty will return an empty string
        /// </remarks>
        public static string Join<T>(this IEnumerable<T> source, string seperator)
        {
            return Join(source, seperator, String.Empty);
        }


        /// <summary>
        /// Produces a <paramref name="seperator"/> delimited string that concatenates all the elements in <paramref name="source"/>
        /// after they have been converted to a string by calling their <see cref="Object.ToString"/> method
        /// </summary>
        public static string Join<T>(this IEnumerable<T> source, string seperator, string defaultIfEmpty)
        {
            return Join(source, seperator, defaultIfEmpty, item => item.ToString());
        }


        /// <summary>
        /// Produces a <paramref name="seperator"/> delimited string that concatenates all the elements in <paramref name="source"/> 
        /// after they have been converted to a string by the <paramref name="converter"/> function
        /// </summary>
        /// <remarks>
        /// If <paramref name="source"/> is empty will return an empty string
        /// </remarks>
        public static string Join<T>(this IEnumerable<T> source, string seperator, Converter<T, string> converter)
        {
            return Join(source, seperator, String.Empty, converter);
        }


        /// <summary>
        /// Produces a <paramref name="seperator"/> delimited string that concatenates all the elements in <paramref name="source"/> 
        /// after they have been converted to a string by the <paramref name="converter"/> function
        /// </summary>
        public static string Join<T>(this IEnumerable<T> source,
                                     string seperator,
                                     string defaultIfEmpty,
                                     Converter<T, string> converter)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => seperator).IsNotNull();
            Demand.The.Param(() => converter).IsNotNull();
#endif

            IEnumerable<string> strings = source.SkipNulls().Select(item => converter(item));
            return strings.Intersperse(seperator).Concat().IfNullOrEmpty(defaultIfEmpty);
        }


        /// <summary>
        /// Invokes a transformation function on each element in <paramref name="source"/> and returns the element
        /// that has the resulting maximum value
        /// </summary>
        /// <param name="source">sequence of elements to select from</param>
        /// <param name="selector">transformation function</param>
        public static TSource MaxElement<TSource, TData>(this IEnumerable<TSource> source, Func<TSource, TData> selector)
            where TData : IComparable<TData>
        {
            TData maxValue = source.Max(selector);
            EqualityComparer<TData> comparer = EqualityComparer<TData>.Default;
            return source.First(tSource => comparer.Equals(selector(tSource), maxValue));
        }


        /// <summary>
        /// Invokes a transformation function on each element in <paramref name="source"/> and returns the element
        /// that has the resulting maximum value or a default value if no match found
        /// </summary>
        /// <param name="source">sequence of elements to select from</param>
        /// <param name="selector">transformation function</param>
        public static TSource MaxElementOrDefault<TSource, TData>(this IEnumerable<TSource> source,
                                                                  Func<TSource, TData> selector)
            where TData : IComparable<TData>
        {
            TData maxValue = source.Max(selector);
            EqualityComparer<TData> comparer = EqualityComparer<TData>.Default;
            return source.FirstOrDefault(element => comparer.Equals(selector(element), maxValue));
        }


        /// <summary>
        /// Invokes a transformation function on each element in <paramref name="source"/> and returns the element
        /// that has the resulting minimum value
        /// </summary>
        /// <param name="source">sequence of elements to select from</param>
        /// <param name="selector">transformation function</param>
        public static TSource MinElement<TSource, TData>(this IEnumerable<TSource> source, Func<TSource, TData> selector)
            where TData : IComparable<TData>
        {
            TData minValue = source.Min(selector);
            EqualityComparer<TData> comparer = EqualityComparer<TData>.Default;
            return source.First(tSource => comparer.Equals(selector(tSource), minValue));
        }


        /// <summary>
        /// Invokes a transformation function on each element in <paramref name="source"/> and returns the element
        /// that has the resulting minimum value or a default value if no match found
        /// </summary>
        /// <param name="source">sequence of elements to select from</param>
        /// <param name="selector">transformation function</param>
        public static TSource MinElementOrDefault<TSource, TData>(this IEnumerable<TSource> source,
                                                                  Func<TSource, TData> selector)
            where TData : IComparable<TData>
        {
            TData minValue = source.Min(selector);
            EqualityComparer<TData> comparer = EqualityComparer<TData>.Default;
            return source.FirstOrDefault(tSource => comparer.Equals(selector(tSource), minValue));
        }


        /// <summary>
        /// Returns the penultimate element in the sequence or <c>default(T)</c>
        /// </summary>
        public static T PenultimateOrDefault<T>(this IEnumerable<T> source)
        {
            var enumerable = new SmartEnumerable<T>(source);
            var lastItem = enumerable.LastOrDefault();
            return lastItem != null ? lastItem.Previous : default(T);
        }


        /// <summary>
        /// Return the element in the sequence positioned immediately before the first occurance of <paramref name="current"/>
        /// as determined by <see cref="EqualityComparer{T}"/>
        /// </summary>
        public static T PreviousToOrDefault<T>(this IEnumerable<T> source, T current)
        {
            T previous = default(T);
            foreach (T item in source)
            {
                if (EqualityComparer<T>.Default.Equals(current, item)) break;

                previous = item;
            }
            return previous;
        }


        /// <summary>
        /// Make <paramref name="source"/> safe to method chain with other linq extension
        /// methods when <paramref name="source"/> is null.
        /// </summary>
        /// <returns>
        /// Retuns <see cref="Enumerable.Empty{TResult}"/> when <paramref name="source"/> is null otherwise
        /// the original <paramref name="source"/>
        /// </returns>
        public static IEnumerable<T> Safe<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }


        public static int SafeCount<T>(this IEnumerable<T> source)
        {
            return source != null ? source.Count() : 0;
        }


        /// <summary>
        /// Creates a <see cref="List{T}"/> from an <see cref="IEnumerable{T}"/>, returning an empty
        /// <see cref="List{T}"/> where <paramref name="source"/> is null
        /// </summary>
        public static List<T> SafeToList<T>(this IEnumerable<T> source)
        {
            return source != null ? source.ToList() : new List<T>();
        }


        /// <summary>
        /// Tests that each elements within <paramref name="ranges"/> are connected as determined by
        /// <paramref name="predicate"/>
        /// </summary>
        /// <param name="ranges"></param>
        /// <param name="predicate">function that should test whether the previous and current element are connected</param>
        /// <returns></returns>
        public static bool Satisfy<T>(this IEnumerable<T> ranges, Func<T, T, bool> predicate)
        {
#if !SILVERLIGHT
            Check.Require(() => {
                Demand.The.Param(() => predicate).IsNotNull();
                Demand.The.Param(() => ranges).IsNotNull();
            });
#endif

            bool result = false;
            foreach (var entry in new SmartEnumerable<T>(ranges))
            {
                result = true;
                if (EqualityComparer<T>.Default.Equals(entry.Previous, default(T))) continue;
                if (!predicate(entry.Previous, entry.Value)) return false;
            }
            return result;
        }


        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.SelectMany(enumeration => enumeration);
        }


        public static IEnumerable<T> SelectMany<T>(this IEnumerable<T[]> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.SelectMany(enumeration => enumeration);
        }


        /// <summary>
        /// returns <paramref name="source"/> with any null elements removed
        /// </summary>
        public static IEnumerable<T> SkipNulls<T>(this IEnumerable<T> source)
        {
            return source.Where(item => !ReferenceEquals(null, item));
        }


        /// <summary>
        /// returns <paramref name="source"/> with any null or empty string elements removed
        /// </summary>
        public static IEnumerable<string> SkipNullOrEmpty(this IEnumerable<string> source)
        {
            return source.Where(item => !String.IsNullOrEmpty(item));
        }


        /// <summary>
        /// Slice <paramref name="source"/> every time <paramref name="slicingCondition"/> is satisfied
        /// </summary>
        /// <param name="source">sequence to slice</param>
        /// <param name="slicingCondition">function that will be called when iterating <paramref name="source"/>. Function will be passed the previous and current element that is being iterated</param>
        public static IEnumerable<IEnumerable<T>> SliceWhen<T>(this IEnumerable<T> source,
                                                               Func<T, T, bool> slicingCondition)
        {
#if !SILVERLIGHT
            Demand.The.Param(() => source).IsNotNull();
#endif

            if (!source.Any()) yield break;

            IEnumerator<T> iterator = source.GetEnumerator();
            iterator.MoveNext();
            T prev = iterator.Current;
            var currentGroup = new List<T> {prev};
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


        /// <summary>
        /// Recreates a dictionary from an enumeration of key-value pairs.
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> enumeration)
        {
            // Check to see that enumeration is not null
            if (enumeration == null)
                throw new ArgumentNullException("enumeration");

            return enumeration.ToDictionary(item => item.Key, item => item.Value);
        }


        /// <summary>
        /// Recreates a dictionary from an enumeration of key-value pairs.
        /// </summary>
        /// <param name="enumeration">sequence to return as a dictionary</param>
        /// <param name="keyComparer">The comparer that the constructed dictionary will use when it needs to compare keys</param>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> enumeration, IEqualityComparer<TKey> keyComparer)
        {
            // Check to see that enumeration is not null
            if (enumeration == null)
                throw new ArgumentNullException("enumeration");

            return enumeration.ToDictionary(item => item.Key, item => item.Value, keyComparer);
        }


        public static IDictionary<TKey, IEnumerable<TValue>> ToDictionary<TKey, TValue>(
            this IEnumerable<IGrouping<TKey, TValue>> source)
        {
            return source.ToDictionary(g => g.Key, g => (IEnumerable<TValue>) g);
        }


#if SILVERLIGHT
        public static void RemoveAll<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            while (source.Count() > 0)
            {
                list.RemoveAt(0);
            }
        }


        public static IEnumerable<T> RemoveAll<T>(this IEnumerable<T> items, Predicate<T> match)
        {
            int index = items.Count() - 1;
            var list = items.ToList();

            while (index >= 0)
            {
                if (match(list[index]))
                {
                    list.RemoveAt(index);
                }
                index--;
            }
            return list;
        }
#endif


        /// <summary>
        /// Attempts to fetch the element at the supplied <paramref name="index"/> return the default value for
        /// <typeparamref name="T"/> if no element exist at that index position
        /// </summary>
        public static T TryAt<T>(this IEnumerable<T> source, int index)
        {
            try
            {
                return source.ElementAt(index);
            }
            catch (ArgumentOutOfRangeException)
            {
                return default(T);
            }
        }

        #endregion
    }
}