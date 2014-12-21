using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using MiscUtil.Collections.Extensions;

namespace Eca.Commons.Extensions
{
    public static class NameValueCollectionExtensions
    {
        #region Class Members

        public static IDictionary<string, string> ToDictionary(this NameValueCollection values)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            foreach (string name in values.AllKeys)
            {
                result.Add(name, values[name]);
            }
            return result;
        }


        public static NameValueCollection ToNameValueCollection(this IDictionary<string, string> values)
        {
            var result = new NameValueCollection();
            values.ForEach(pair => result[pair.Key] = pair.Value);
            return result;
        }


#if !SILVERLIGHT
        /// <summary>
        /// Serializes the collection of key/value pairs as a html query string
        /// </summary>
        public static string ToQueryString(this NameValueCollection values)
        {
            return values.ToDictionary().ToQueryString();
        }
#endif

        #endregion
    }
}