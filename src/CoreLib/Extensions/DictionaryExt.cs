using System;
using System.Collections.Generic;
using System.Linq;
using Eca.Commons;
using Eca.Commons.Extensions;
#if !SILVERLIGHT
using System.Web;
using NValidate.Framework;
#endif

namespace MiscUtil.Collections.Extensions
{
    /// <summary>
    /// Extensions to IDictionary
    /// </summary>
    public static class DictionaryExt
    {
        #region Class Members

        /// <summary>
        /// Adds the <paramref name="value"/> using a key derived from <typeparamref name="T"/> using <see cref="DeriveKey{T}"/>
        /// </summary>
        /// <typeparam name="T">The type from which the key is derived</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="value">The value to add</param>
        public static void Add<T>(this IDictionary<string, object> dictionary, T value)
        {
            dictionary[DeriveKey<T>()] = value;
        }


        /// <seealso cref="Add{T}(System.Collections.Generic.IDictionary{string,object},T)"/>
        public static void Add<T>(this IDictionary<string, T> dictionary, T value)
        {
            dictionary[DeriveKey<T>()] = value;
        }


        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                  IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            // Check to see that dictionary is not null
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            // Check to see that pairs is not null
            if (pairs == null)
                throw new ArgumentNullException("pairs");

            foreach (var pair in pairs)
            {
                dictionary.Add(pair.Key, pair.Value);
            }
        }


        /// <summary>
        /// Searches the <paramref name="dictionary"/> for a key that is derived from <typeparamref name="T"/> using <see cref="DeriveKey{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type from which the key is derived</typeparam>
        public static bool ContainsKey<T>(this IDictionary<string, object> dictionary) where T : class
        {
            return dictionary.ContainsKey(DeriveKey<T>());
        }


        public static bool ContainsKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key)
        {
            return source.Any(x => Equals(x.Key, key));
        }


        public static string DeriveKey<T>()
        {
            return String.Format("{0}Key", typeof (T).Name);
        }


        /// <summary>
        /// Returns the value associated with the specified key if there
        /// already is one, or inserts a new value for the specified key and
        /// returns that.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value, which must either have
        /// a public parameterless constructor or be a value type</typeparam>
        /// <param name="dictionary">Dictionary to access</param>
        /// <param name="key">Key to lookup</param>
        /// <returns>Existing value in the dictionary, or new one inserted</returns>
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                       TKey key)
            where TValue : new()
        {
            TValue ret;
            if (!dictionary.TryGetValue(key, out ret))
            {
                ret = new TValue();
                dictionary[key] = ret;
            }
            return ret;
        }


        /// <summary>
        /// Returns the value associated with the specified key if there already
        /// is one, or calls the specified delegate to create a new value which is
        /// stored and returned.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">Dictionary to access</param>
        /// <param name="key">Key to lookup</param>
        /// <param name="valueProvider">Delegate to provide new value if required</param>
        /// <returns>Existing value in the dictionary, or new one inserted</returns>
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                       TKey key,
                                                       Func<TValue> valueProvider)
        {
            TValue ret;
            if (!dictionary.TryGetValue(key, out ret))
            {
                ret = valueProvider();
                dictionary[key] = ret;
            }
            return ret;
        }


        /// <summary>
        /// Returns the value associated with the specified key if there
        /// already is one, or inserts the specified value and returns it.
        /// </summary>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="dictionary">Dictionary to access</param>
        /// <param name="key">Key to lookup</param>
        /// <param name="missingValue">Value to use when key is missing</param>
        /// <returns>Existing value in the dictionary, or new one inserted</returns>
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                       TKey key,
                                                       TValue missingValue)
        {
            TValue ret;
            if (!dictionary.TryGetValue(key, out ret))
            {
                ret = missingValue;
                dictionary[key] = ret;
            }
            return ret;
        }


        /// <summary>
        /// Returns the key-value pair matching the supplied <paramref name="key"/>
        /// </summary>
        public static KeyValuePair<TKey, TValue> GetPair<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key)
        {
            return source.Single(x => Equals(x.Key, key));
        }


        /// <summary>
        /// Returns the value associated with a key that is derived from <typeparamref name="T"/> using <see cref="DeriveKey{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type from which the key is derived</typeparam>
        /// <seealso cref="TryGetValue{T}(System.Collections.Generic.IDictionary{string,object})"/>
        public static T GetValue<T>(this IDictionary<string, object> dictionary) where T : class
        {
            return (T) dictionary[DeriveKey<T>()];
        }


        /// <summary>
        /// Returns the value for the <paramref name="key"/> supplied
        /// </summary>
        public static TValue GetValue<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key)
        {
            return source.Single(x => Equals(x.Key, key)).Value;
        }


        /// <summary>
        /// Tests that <paramref name="keyValuePair"/> is equal to a default initialised instance
        /// </summary>
        public static bool IsDefaultValue<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValuePair)
        {
            return new KeyValuePair<TKey, TValue>().Equals(keyValuePair);
        }


        public static IEnumerable<TKey> Keys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.Select(x => x.Key);
        }


        /// <summary>
        /// Merges all the public properties of <paramref name="annoymousObject"/> into the <paramref name="source"/>
        /// dictionary
        /// </summary>
        /// <remarks>
        /// If <paramref name="annoymousObject"/> is actually a <see cref="IDictionary{TKey,TValue}"/> of the same
        /// type as <paramref name="source"/>, then its key value pairs will be merged into <paramref name="source"/>
        /// </remarks>
        public static IDictionary<string, object> Merge(this IDictionary<string, object> source, object annoymousObject)
        {
#if !SILVERLIGHT
            Check.Require(() => Demand.The.Param(() => source).IsNotNull());
#endif

            if (annoymousObject == null) return source;

            IDictionary<string, object> additionalElements = annoymousObject.ToDictionary();
            var mergeAttributes =
                source.Where(pair => !additionalElements.ContainsKey(pair.Key)).Union(additionalElements);
            return mergeAttributes.ToDictionary();
        }


        public static void RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
        {
            // Check to see that dictionary is not null
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            // Check to see that keys is not null
            if (keys == null)
                throw new ArgumentNullException("keys");

            foreach (var key in keys.ToArray())
            {
                dictionary.Remove(key);
            }
        }


        public static void RemoveValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            // Check to see that dictionary is not null
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            foreach (var key in (from pair in dictionary
                                 where EqualityComparer<TValue>.Default.Equals(value, pair.Value)
                                 select pair.Key).ToArray())
            {
                dictionary.Remove(key);
            }
        }


        public static void RemoveValueRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                          IEnumerable<TValue> values)
        {
            // Check to see that dictionary is not null
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            // Check to see that values is not null
            if (values == null)
                throw new ArgumentNullException("values");

            foreach (var value in values.ToArray())
            {
                RemoveValue(dictionary, value);
            }
        }


        public static IDictionary<TKey, TValue> Safe<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            return source ?? new Dictionary<TKey, TValue>();
        }


        /// <summary>
        /// Adds an element with the provide key and value where the key is not present, otherwise update the existing value
        /// at the key supplied
        /// </summary>
        public static void SafeAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);
        }


        public static bool SafeContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            return source != null && source.ContainsKey(key);
        }


        /// <summary>
        /// <para>Merges all the public properties of <paramref name="annoymousObject"/> into the <paramref name="source"/> dictionary using <see cref="Merge"/></para>
        /// <para>If <paramref name="source"/> is null then return just the public properties of <paramref name="annoymousObject"/> as a dictionary</para>
        /// <para>If <paramref name="annoymousObject"/> is null then return <paramref name="source"/> (which may be null)</para>
        /// </summary>
        public static IDictionary<string, object> SafeMerge(this IDictionary<string, object> source,
                                                            object annoymousObject)
        {
            if (ReferenceEquals(annoymousObject, null)) return source;
            if (ReferenceEquals(source, null)) return annoymousObject.ToDictionary();
            return source.Merge(annoymousObject);
        }


#if !SILVERLIGHT
        /// <summary>
        /// Serializes the collection of key/value pairs as a html query string
        /// </summary>
        public static string ToQueryString(this IEnumerable<KeyValuePair<string, object>> source)
        {
            var list = source.SafeToList();

            if (list.Count == 0) return "";

            var queryParams
                = list.Where(x => x.Value != null)
                    .Select(x => string.Format("{0}={1}",
                                               HttpUtility.UrlEncode(x.Key),
                                               HttpUtility.UrlEncode(x.Value.ToString())));
            return string.Format("?{0}", queryParams.Join("&"));
        }

#endif


#if !SILVERLIGHT
        /// <summary>
        /// Serializes the collection of key/value pairs as a html query string
        /// </summary>
        public static string ToQueryString(this IEnumerable<KeyValuePair<string, string>> source)
        {
            Dictionary<string, string> dictionary = source.ToDictionary(x => x.Key, v => v.ToString());
            return ToQueryString(dictionary as IDictionary<string, object>);
        }
#endif


        /// <summary>
        /// Returns the key-value pair matching the supplied <paramref name="key"/>, returning a default
        /// <see cref="KeyValuePair{TKey,TValue}"/> where key is not found within <paramref name="source"/>
        /// </summary>
        public static KeyValuePair<TKey, TValue> TryGetPair<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key)
        {
            return source.FirstOrDefault(x => Equals(x.Key, key));
        }


        /// <summary>
        /// Returns the key-value pair matching the supplied <paramref name="key"/> using the <paramref name="keyComparer"/> to perform the comparison.
        /// Returns a default <see cref="KeyValuePair{TKey,TValue}"/> where key is not found within <paramref name="source"/>
        /// </summary>
        public static KeyValuePair<TKey, TValue> TryGetPair<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key, IEqualityComparer<TKey> keyComparer)
        {
            return source.FirstOrDefault(x => keyComparer.Equals(x.Key, key));
        }


        /// <summary>
        /// Returns the value associated with a key that is derived from <typeparamref name="T"/> using <see cref="DeriveKey{T}"/>.
        /// If the key is not found returns null.
        /// </summary>
        /// <typeparam name="T">The type from which the key is derived</typeparam>
        public static T TryGetValue<T>(this IDictionary<string, object> dictionary) where T : class
        {
            return dictionary.TryGetValue<T>(DeriveKey<T>());
        }


        /// <summary>
        /// Simple convenince wrapper around the standard TryGetValue dictionary method
        /// </summary>
        public static T TryGetValue<T>(this IDictionary<string, object> dictionary, string key) where T : class
        {
            object result;
            dictionary.TryGetValue(key, out result);
            return (T) result;
        }


        public static IEnumerable<TValue> Values<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.Select(x => x.Value);
        }

        #endregion
    }
}