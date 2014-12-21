using System;
using System.ComponentModel;
using Eca.Commons.Dates;
using Eca.Commons.Reflection;

namespace Eca.Commons
{
    /// <summary>
    /// Enhanced replacement for <see cref="Convert"/>.<see cref="Convert.ChangeType(object,Type)"/>
    /// </summary>
    /// <remarks>
    /// Will convert enum string names to enum's, strings to Guid's and most importantly handle
    /// nullable values which <see cref="Convert"/>.<see cref="Convert.ChangeType(object,Type)"/>
    /// does not
    /// </remarks>
    public static class EnhancedConvertor
    {
        #region Class Members

        /// <summary>
        /// Returns an Object with the specified Type and whose value is equivalent to the specified object.
        /// </summary>
        /// <param name="value">An Object that implements the IConvertible interface.</param>
        /// <param name="conversionType">The Type to which value is to be converted.</param>
        /// <returns>An object whose Type is <paramref name="conversionType"/> (or conversionType's underlying type if conversionType
        /// is Nullable&lt;&gt;) and whose value is equivalent to <paramref name="value"/>. -or- a null reference, if value is a null
        /// reference and conversionType is not a value type.</returns>
        /// <remarks>
        /// This method exists as a workaround to System.Convert.ChangeType(Object, Type) which does not handle
        /// nullables as of version 2.0 (2.0.50727.42) of the .NET Framework. The idea is that this method will
        /// be deleted once Convert.ChangeType is updated in a future version of the .NET Framework to handle
        /// nullable types, so we want this to behave as closely to Convert.ChangeType as possible.
        /// This method was written by Peter Johnson at:
        /// http://aspalliance.com/author.aspx?uId=1026.
        /// 
        /// Origianal source http://aspalliance.com/852
        /// 
        /// </remarks>
        public static object ChangeType(object value, Type conversionType)
        {
            // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
            // checking properties on conversionType below.
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            } // end if

            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType

            if (conversionType.IsGenericType &&
                conversionType.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
            {
                // It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
                // InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
                // determine what the underlying type is
                // If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
                // have a type--so just return null
                // Note: We only do this check if we're converting to a nullable type, since doing it outside
                // would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
                // value is null and conversionType is a value type.
                if (value == null)
                {
                    return null;
                } // end if

#if SILVERLIGHT
                conversionType = Nullable.GetUnderlyingType(conversionType);
#else

                // It's a nullable type, and not null, so that means it can be converted to its underlying type,
                // so overwrite the passed-in conversion type with this underlying type
                var nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
#endif
            } // end if


            if (value != null && value.GetType() == conversionType) return value;

            if (value is string && conversionType == typeof (Guid)) return new Guid(value as string);

            if (value is Date && conversionType == typeof (DateTime))
                return new DateTime(((Date) value).Year, ((Date) value).Month, ((Date) value).Day);

            if (value is DateTime && conversionType == typeof (Date))
            {
                var dateTime = (DateTime) value;
                return DateTime.MinValue.Equals(dateTime)
                           ? (Date) null
                           : new Date(dateTime.Year, dateTime.Month, dateTime.Day);
            }


            if (conversionType.IsEnum)
            {
                return ConvertToEnum(conversionType, value);
            }


#if !SILVERLIGHT
            if (!ReferenceEquals(value, null) && ReflectionUtil.GetTypeOrUnderlyingType(value.GetType()).IsEnum)
            {
                return ConvertFromEnum(conversionType, value);
            }
#endif

            var convertible = value as IConvertible;
            if (convertible == null)
            {
                if (value != null && conversionType.IsAssignableFrom(value.GetType()))
                    return value;
            }

            if (value != null && conversionType == typeof (string))
                return value.ToString();

            // Handle non-text-based boolean variables (e.g. ones and zeroes from bit columns)
            // This is required as the conversion of an object to bool only works with text values, e.g. 'true' or 'false'
            // If it's not a "1" or "0", we'll drop down into the next block of code
            if (value != null && conversionType == typeof (bool))
            {
                switch (value.ToString())
                {
                    case "1":
                        return true;
                    case "0":
                        return false;
                    default:
                        break;
                }
            }

            // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
            // nullable type), pass the call on to Convert.ChangeType
            return Convert.ChangeType(value, conversionType, null);
        }


        /// <seealso cref="ChangeType"/>
        public static T ChangeType<T>(object value)
        {
            return (T) ChangeType(value, typeof (T));
        }


#if !SILVERLIGHT
        private static object ConvertFromEnum(Type conversionType, object enumValue)
        {
            if (conversionType == typeof (Int32))
            {
                return (Int32) enumValue;
            }

            if (conversionType == typeof (Int16))
            {
                return (Int16) enumValue;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(enumValue.GetType());
            return converter.ConvertTo(enumValue, conversionType);
        }
#endif


        private static object ConvertToEnum(Type enumType, object value)
        {
            //annoyingly, the built-in type convertor will not convert Int32 to enum. If it did then we would use it for all our conversions here

            TypeConverter customConvertor = GetCustomEnumConverter(enumType);

            if (customConvertor != null && !ReferenceEquals(value, null) &&
                customConvertor.CanConvertFrom(value.GetType()))
            {
                return customConvertor.ConvertFrom(value);
            }

            if (!(value is string))
            {
                //if we are here then I think its probably appropriate that an exception will be thrown if the value is null
                return Enum.ToObject(enumType, value);
            }

            return ConvertToEnumFromString(enumType, value as string);
        }


        private static object ConvertToEnumFromString(Type enumType, string value)
        {
            if (String.IsNullOrEmpty(value)) return null;
            try
            {
                return Enum.Parse(enumType, value, false);
            }
            catch (ArgumentException e)
            {
                throw new FormatException(string.Format("{0} is not a valid value for {1}", value, enumType.Name), e);
            }
        }


        private static TypeConverter GetCustomEnumConverter(Type enumType)
        {
#if SILVERLIGHT
            return null;
#else
            TypeConverter converter = TypeDescriptor.GetConverter(enumType);
            if (converter == null || typeof (EnumConverter) == converter.GetType())
            {
                return null;
            }
            return converter;
#endif
        }

        #endregion
    }
}