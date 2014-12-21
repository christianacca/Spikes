using System;
using System.Collections.Generic;
#if !SILVERLIGHT
using System.Globalization;
#endif
using System.Linq;
using System.Text;
using Eca.Commons.Extensions;
#if !SILVERLIGHT
using MiscUtil.Collections.Extensions;
#endif

namespace Eca.Commons
{
    /// <summary>
    /// TODO: Merge this class into <see cref="StringExtensions"/>
    /// </summary>
    public static class TextHelper
    {
        #region Overridden object methods

        /// <summary>
        /// See <see cref="SafeToString(Func{Object},string)"/>
        /// </summary>
        public static string SafeToString(Func<object> propertyAccessor)
        {
            return SafeToString(propertyAccessor, "Bad-Value");
        }


        /// <summary>
        /// A safe way of calling <see cref="Object.ToString"/> on an object. 
        /// </summary>
        /// Use this when it is critical that a call to the <see cref="Object.ToString"/>
        /// should not throw, for example when logging
        /// <remarks>
        /// </remarks>
        /// <param name="propertyAccessor">a delegate instance that should return the object that you want to call <see cref="Object.ToString"/></param>
        /// <param name="safeStringValue">the value to return if <paramref name="propertyAccessor"/> throws</param>
        /// <returns></returns>
        public static string SafeToString(Func<object> propertyAccessor, string safeStringValue)
        {
            if (propertyAccessor == null) return safeStringValue;

            try
            {
                object objToSerialise = propertyAccessor();
                return objToSerialise != null ? objToSerialise.ToString() : null;
            }
            catch //yes we are swallowing all exceptions here
            {
                return safeStringValue;
            }
        }


        /// <summary>
        /// Parses a camel cased or pascal cased string and returns a new 
        /// string with spaces between the words in the string.
        /// </summary>
        /// <example>
        /// The string "PascalCasing" will return an array with two 
        /// elements, "Pascal" and "Casing".
        /// </example>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SplitUpperCaseToString(this string source)
        {
            return String.Join(" ", source.Split(' ').SelectMany(x => SplitUpperCase(x)).ToArray());
        }

        #endregion


        #region Class Members

        public static bool IsNumber(IEnumerable<char> data)
        {
            if (data.Contains('.'))
                throw new NotImplementedException("This needs to be supported but at the moment isn't implemented.");
            foreach (char c in data)
            {
                if (!Char.IsNumber(c))
                    return false;
            }
            return true;
        }


        public static bool IsValidDate(this string dateString)
        {
            DateTime ret;
            return DateTime.TryParse(dateString, out ret);
        }


        public static string NormaliseAndTrimString(string value)
        {
            if (String.IsNullOrEmpty(value)) return value;

            string trimmed = value.Trim();

            var sb = new StringBuilder(trimmed.Length);
            int whiteSpaceCount = 0;
            foreach (char c in trimmed)
            {
                if (Char.IsWhiteSpace(c))
                    whiteSpaceCount++;
                else
                    whiteSpaceCount = 0;

                if (whiteSpaceCount <= 1)
                    sb.Append(c);
            }
            return sb.ToString();
        }


        public static string NullIfZeroLength(string value)
        {
            if (value == String.Empty)
                return null;
            else
                return value;
        }


        public static string SafeTrim(this string source)
        {
            if (String.IsNullOrEmpty(source)) return source;
            return source.Trim();
        }


        /// <summary>
        /// Parses a camel cased or pascal cased string and returns an array 
        /// of the words within the string.
        /// </summary>
        /// <example>
        /// The string "PascalCasing" will return an array with two 
        /// elements, "Pascal" and "Casing".
        /// </example>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string[] SplitUpperCase(string source)
        {
            //Source: http://haacked.com/archive/2005/09/24/10334.aspx

            if (source == null)

                return new string[] {}; //Return empty array.


            if (source.Length == 0)

                return new[] {""};

            if (source.IsUpper()) return new[] {source};


            var words = new List<string>();

            int wordStartIndex = 0;


            char[] letters = source.ToCharArray();

            // Skip the first letter. we don't care what case it is.

            for (int i = 1; i < letters.Length; i++)
            {
                if (Char.IsUpper(letters[i]))
                {
                    //Grab everything before the current index.

                    words.Add(new String(letters, wordStartIndex, i - wordStartIndex));

                    wordStartIndex = i;
                }
            }

            //We need to have the last word.

            words.Add(new String(letters, wordStartIndex, letters.Length - wordStartIndex));


            //Copy to a string array.

            var wordArray = new string[words.Count];

            words.CopyTo(wordArray, 0);

            return wordArray;
        }


        /// <summary>
        /// WARNING: Not yet tested, and not very robust
        /// Returns the characters that fall after the first occurance of <paramref name="fragment"/> 
        /// </summary>
        public static string SubstringAfter(this string toSearch,
                                            string fragment,
                                            StringComparison comparisonType)
        {
            string beforeCharactersToReturn =
                toSearch.Substring(
                    toSearch.IndexOf(fragment, comparisonType) +
                    fragment.Length);
            return beforeCharactersToReturn;
        }


        /// <summary>
        /// WARNING: Not yet tested, and not very robust
        /// Returns the characters that fall after the first occurance of <paramref name="fragment"/>,
        /// using a culture invariant, case insensitive comparison
        /// </summary>
        public static string SubstringAfter(this string toSearch,
                                            string fragment)
        {
            return SubstringAfter(toSearch, fragment, StringComparison.InvariantCultureIgnoreCase);
        }


        /// <summary>
        /// WARNING: Not yet tested, and not very robust
        /// Returns the characters that fall between the first occurance of <paramref name="leftFragment"/> 
        /// and <paramref name="rightFragment"/>, using a culture invariant, case insensitive comparison
        /// </summary>
        public static string SubstringBetween(this string toSearch, string leftFragment, string rightFragment)
        {
            return SubstringBetween(toSearch, leftFragment, rightFragment, StringComparison.InvariantCultureIgnoreCase);
        }


        /// <summary>
        /// WARNING: Not yet tested, and not very robust
        /// Returns the characters that fall between the first occurance of <paramref name="leftFragment"/> 
        /// and <paramref name="rightFragment"/>
        /// </summary>
        public static string SubstringBetween(this string toSearch,
                                              string leftFragment,
                                              string rightFragment,
                                              StringComparison comparisonType)
        {
            string beforeCharactersToReturn =
                toSearch.Substring(
                    toSearch.IndexOf(leftFragment, comparisonType) +
                    leftFragment.Length);
            return beforeCharactersToReturn.Substring(0, beforeCharactersToReturn.IndexOf(rightFragment));
        }


        public static void AddTitleCaseException(string exception)
        {
            _toTitleCaseExceptions[exception.ToLower()] = exception;
        }


        public static void RemoveTitleCaseException(string exception)
        {
            _toTitleCaseExceptions.Remove(exception.ToLower());
        }


        public static void AddTitleCasePrefixException(string prefixException)
        {
            _toTitleCasePrefixExceptions[prefixException.ToLower()] = prefixException;
        }


        public static void RemoveTitleCasePrefixException(string prefixException)
        {
            _toTitleCasePrefixExceptions.Remove(prefixException.ToLower());
        }


        private static readonly IDictionary<string, string> _toTitleCasePrefixExceptions =
            new Dictionary<string, string> {{"mc", "Mc"}, {"mac", "Mac"}};

        private static readonly IDictionary<string, string> _toTitleCaseExceptions = new Dictionary<string, string>();

        public static IEnumerable<string> ToTitleCaseExceptions
        {
            get { return _toTitleCaseExceptions.Values; }
        }

        public static IEnumerable<string> ToTitleCasePrefixExceptions
        {
            get { return _toTitleCasePrefixExceptions.Values; }
        }



        /// <summary>
        /// Returns <paramref name="value"/> in title case (aka proper case), handling the common exceptions to the
        /// normal rule of captilizing just the first letter of each word
        /// </summary>
        public static string ToTitleCase(this string value)
        {
            if (value == null) return null;

#if !SILVERLIGHT
            string exception;
            _toTitleCaseExceptions.TryGetValue(value.ToLower(), out exception);
            if (exception != null)
            {
                return exception;
            }

            var prefixException = _toTitleCasePrefixExceptions.SingleOrDefault(x => value.ToLower().StartsWith(x.Key));
            if (!prefixException.IsDefaultValue())
            {
                string prefix = prefixException.Value;
                string remainder = value.Substring(prefix.Length, value.Length - prefix.Length);
                return String.Format("{0}{1}",
                                     prefix,
                                     CultureInfo.CurrentCulture.TextInfo.ToTitleCase(remainder.ToLower()));
            }

            if (value.Contains("'"))
            {
                return ToTitleCaseHandlingApostrophe(value);
            }

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
#else
            string[] words = value.Split(' ');

	        for (int i = 0; i <= words.Length - 1; i++)
            {
		        if ((!object.ReferenceEquals(words[i], string.Empty)))
                {
			        string firstLetter = words[i].Substring(0, 1);
			        string rest = words[i].Substring(1);
			        string result = firstLetter.ToUpper() + rest.ToLower();
			        words[i] = result;
		        }
	        }
	        return String.Join(" ", words);
#endif
        }

#if !SILVERLIGHT
        private static string ToTitleCaseHandlingApostrophe(string value)
        {
            var parts = value.Split(new[] {"'"}, StringSplitOptions.None);
            return parts
                .Select(p => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(p.ToLower()))
                .Join("'");
        }
#endif

        #endregion
    }
}