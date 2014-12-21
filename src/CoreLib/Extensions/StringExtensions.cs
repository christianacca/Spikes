using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#if !SILVERLIGHT
using NValidate.Framework;
#endif

namespace Eca.Commons.Extensions
{
    public static class StringExtensions
    {
        #region Class Members

        public static string Args(this string source, params object[] args)
        {
            return string.Format(source, args);
        }


        public static bool HasLeadingWhitespace(this string value)
        {
            if (String.IsNullOrEmpty(value)) return false;

            return value.TrimStart().Length != value.Length;
        }


        public static bool HasTrailingWhitespace(this string value)
        {
            if (String.IsNullOrEmpty(value)) return false;

            return value.TrimEnd().Length != value.Length;
        }


        /// <summary>
        /// Return <paramref name="defaultValue"/> if <paramref name="source"/> is null or an empty string
        /// </summary>
        public static string IfNullOrEmpty(this string source, string defaultValue)
        {
            return string.IsNullOrEmpty(source) ? defaultValue : source;
        }


        /// <summary>
        /// note: non-printable characters such as carriage return and line feeds, and space character are non-alpha
        /// </summary>
        /// <remarks>For further information on which string would be considered alpha, see the unit test class IsAlphaExamples (StringExtenstionExamples.cs)</remarks>
        public static bool IsAlpha(this string source)
        {
#if !SILVERLIGHT
            Demand.The.Param(source, "source").IsNotNull();
#endif

            const string pattern = @"^[a-z]*\z";
            return source == String.Empty ||
                   Regex.IsMatch(source, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// <para>Returns whether the <paramref name="name"/> is a valid first or last name for a person.</para>
        /// <para>note: non-printable characters such as carriage return and line feeds, and space character will return false</para>
        /// </summary>
        /// <remarks>
        /// For further information on which string would be considered a valid name, see the unit test class
        /// IsPersonNamePartExamples (StringExtenstionExamples.cs)
        /// </remarks>
        public static bool IsPersonNamePart(this string name)
        {
#if !SILVERLIGHT
            Demand.The.Param(name, "source").IsNotNull();
#endif

            if (name.StartsWith("-") || name.StartsWith("'") || name.EndsWith("-") || name.EndsWith("'") ||
                name.Contains("''") || name.Contains("--") || name.HasTrailingWhitespace() ||
                name.HasLeadingWhitespace())
            {
                return false;
            }

            //note: '\040' == spacebar
            return Regex.IsMatch(name,
                                 @"^(\(?\040*[a-z'-]+\040*\)?\040*)+\z",
                                 RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }


        public static bool IsUpper(this string source)
        {
            return !source.Any(Char.IsLower);
        }


        public static string SafeToLower(this string source)
        {
            return String.IsNullOrEmpty(source) ? null : source.ToLower();
        }


        public static bool SafeStartsWith(this string source, string toFind)
        {
            return source != null && toFind != null && source.StartsWith(toFind);
        }


        public static bool SafeStartsWith(this string source, string toFind, StringComparison comparison)
        {
            return source != null && toFind != null && source.StartsWith(toFind, comparison);
        }


        public static bool SafeEquals(this string left, string right, StringComparison comparison)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;

            return left.Equals(right, comparison);
        }


        public static bool SafeContains(this string source, string toFind)
        {
            return source != null && toFind != null && source.Contains(toFind);
        }


        public static bool SafeContains(this string source, string toFind, StringComparison comparison)
        {
            return source != null && toFind != null && source.IndexOf(toFind, comparison) >= 0;
        }


        public static bool Contains(this string source, string toFind, StringComparison comparison)
        {
            return source.IndexOf(toFind, comparison) >= 0;
        }


        public static bool SafeEndsWith(this string source, string toFind)
        {
            return source != null && toFind != null && source.EndsWith(toFind);
        }


        public static bool SafeEndsWith(this string source, string toFind, StringComparison comparison)
        {
            return source != null && toFind != null && source.EndsWith(toFind, comparison);
        }


        /// <summary>
        /// Removes the prefix supplied from the front of the string if it is present.
        /// </summary>
        public static string RemoveFromStart(this string source, string prefixToRemove)
        {
            if (String.IsNullOrEmpty(source))
                return String.Empty;

            if (source.Length <= prefixToRemove.Length ||
                source.Substring(0, prefixToRemove.Length) != prefixToRemove)
                return source;

            return (source.Substring(prefixToRemove.Length, source.Length - prefixToRemove.Length));
        }


        public static string RemoveWhitespace(this string value)
        {
            if (String.IsNullOrEmpty(value)) return value;

            var sb = new StringBuilder(value.Length);
            foreach (char c in value.Where(c => !Char.IsWhiteSpace(c)))
            {
                sb.Append(c);
            }
            return sb.ToString();
        }


        public static string ReverseString(this string source)
        {
            if (source == String.Empty) return source;

            char[] chars = source.ToArray();
            Array.Reverse(chars);
            return new string(chars);
        }


        public static Guid ToGuid(this string source)
        {
            return new Guid(source);
        }


        public static bool StartsWithAlphaNumeric(this string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            return Regex.IsMatch(value, @"^\w+");
        }


        /// <summary>
        /// This is an alias to <see cref="TextHelper.ToTitleCase"/>
        /// </summary>
        public static string ToProperCase(this string source)
        {
            return source.ToTitleCase();
        }


        public static string TruncateAt(this string source, int length)
        {
            return source.Length <= length ? source : source.Substring(0, length);
        }


        public static string TruncateAfterWholeWordAt(this string value, int length)
        {
            if (value == null || value.Length < length || value.IndexOf(" ", length, StringComparison.Ordinal) == -1)
                return value;

            return value.Substring(0, value.IndexOf(" ", length, StringComparison.Ordinal));
        }

        #endregion
    }
}