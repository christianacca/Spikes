using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Eca.Commons.Extensions;
using Eca.Commons.Reflection;
using MiscUtil.Collections.Extensions;

namespace Eca.Commons
{
    /// <summary>
    /// Typed and enhanced version of the static methods defined by <see cref="Enum"/> class
    /// </summary>
    public static class EnumHelper
    {
        #region Class Members

        static EnumHelper()
        {
            EnumByDescriptionCache = new Dictionary<Type, IDictionary<string, Enum>>();
        }


        private static IDictionary<Type, IDictionary<string, Enum>> EnumByDescriptionCache { get; set; }


        private static string DoToDescriptionString(Enum val)
        {
            var attributes =
                (DescriptionAttribute[])
                val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof (DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : val.ToString();
        }


        private static StringComparer GetInvariantComparer(bool ignoreCase)
        {
            return ignoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture;
        }


        /// <summary>
        /// Better implementation of <see cref="Enum.GetValues"/> that accommodates <typeparamref name="T"/> being a <see cref="Nullable{T}"/>
        /// </summary>
        public static IEnumerable<T> GetValuesOfType<T>()
        {
            Type enumType = ReflectionUtil.GetTypeOrUnderlyingType<T>();
            return Enum.GetValues(enumType).Cast<T>();
        }


        /// <summary>
        /// Better implementation of <see cref="Enum.GetValues"/> that accommodates see cref="enumType" being a <see cref="Nullable{T}"/>
        /// </summary>
        public static IEnumerable<Enum> GetValuesOfType(Type enumType)
        {
            enumType = ReflectionUtil.GetTypeOrUnderlyingType(enumType);
            return Enum.GetValues(enumType).Cast<Enum>();
        }


        public static bool IsDefined<T>(string value, bool ignoreCase)
        {
            var comparer = GetInvariantComparer(ignoreCase);
            return GetValuesOfType<T>().AsStrings().Contains(value, comparer);
        }


        public static bool IsDescriptionStringDefined<T>(string value, bool ignoreCase)
        {
            var comparer = GetInvariantComparer(ignoreCase);
            return ToEnumsByDescriptionString<T>().Keys.Any(key => comparer.Compare(key, value) == 0);
        }


        /// <summary>
        /// See <see cref="Parse{T}(string,bool)"/>
        /// </summary>
        /// <remarks>
        /// Parse is case-sensitive
        /// </remarks>
        public static T Parse<T>(string value)
        {
            return Parse<T>(value, false);
        }


        /// <summary>
        /// Typed version of <see cref="Enum.Parse(Type,string,bool)"/> that accommodates <typeparamref name="T"/> being a <see cref="Nullable{T}"/>
        /// </summary>
        public static T Parse<T>(string value, bool ignoreCase)
        {
            Type enumType = ReflectionUtil.GetTypeOrUnderlyingType<T>();
            return (T) Enum.Parse(enumType, value, ignoreCase);
        }


        /// <summary>
        /// See <see cref="Parse{T}(string,T,bool)"/>
        /// </summary>
        /// <remarks>
        /// Parse is case-sensitive
        /// </remarks>
        public static T Parse<T>(string value, T defaultValue)
        {
            return Parse(value, defaultValue, false);
        }


        /// <summary>
        /// Typed version of <see cref="Enum.Parse(Type,string,bool)"/> that accommodates <typeparamref name="T"/> being a <see cref="Nullable{T}"/> 
        /// and allows the caller to define a default enum constant to be returned if <paramref name="value"/> is not 
        /// defined by <typeparamref name="T"/>
        /// </summary>
        public static T Parse<T>(string value, T defaultValue, bool ignoreCase)
        {
            if (!IsDefined<T>(value, ignoreCase)) return defaultValue;
            return Parse<T>(value, ignoreCase);
        }


        /// <summary>
        /// Returns the enum which has a <see cref="DescriptionAttribute"/> whose value equals <paramref name="description"/>
        /// </summary>
        public static T ParseDescription<T>(string description)
        {
            return (T) ParseDescription(description, typeof (T));
        }


        /// <summary>
        /// Returns the enum which has a <see cref="DescriptionAttribute"/> whose value equals <paramref name="description"/>
        /// </summary>
        public static object ParseDescription(string description, Type enumType)
        {
            IDictionary<string, Enum> enumDescriptions = ToEnumsByDescriptionString(enumType);

            if (!enumDescriptions.ContainsKey(description))
                throw new ArgumentException(string.Format("Enum not found for DescriptionString '{0}'", description));

            var match = enumDescriptions.Single(x => x.Key == description);
            return match.Value;
        }


        /// <summary>
        /// Returns the value of any <see cref="DescriptionAttribute"/> that is adorning the enum value supplied. If
        /// attribute has not been applied returns <see cref="Enum"/>.<see cref="Enum.ToString()"/>
        /// </summary>
        public static string ToDescriptionString(this Enum val)
        {
            return ToEnumsByDescriptionString(val.GetType()).Single(e => e.Value.Equals(val)).Key;
        }


        public static ICollection<string> ToDescriptionStrings<T>()
        {
            return ToDescriptionStrings(typeof (T));
        }


        public static ICollection<string> ToDescriptionStrings(Type enumType)
        {
            return ToEnumsByDescriptionString(enumType).Keys;
        }


        private static IDictionary<string, Enum> ToEnumsByDescriptionString<T>()
        {
            var enumType = ReflectionUtil.GetTypeOrUnderlyingType<T>();
            return ToEnumsByDescriptionString(enumType);
        }


        private static IDictionary<string, Enum> ToEnumsByDescriptionString(Type enumType)
        {
            Func<IDictionary<string, Enum>> create = () => {
                IEnumerable<Enum> enums = GetValuesOfType(enumType);
                return enums.ToDictionary(e => DoToDescriptionString(e), e => e);
            };
            return EnumByDescriptionCache.GetOrCreate(enumType, create);
        }

        #endregion
    }
}