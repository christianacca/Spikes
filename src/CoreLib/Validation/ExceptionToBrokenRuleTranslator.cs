using System;
using System.Collections.Generic;
using System.Linq;
using NValidate.Framework;

namespace Eca.Commons.Validation
{
    /// <summary>
    /// Translates geeky the message from an ArgumentException thrown by NValidate library into a more end user-friendly
    /// message, that could potentially be displayed on screen
    /// </summary>
    /// <remarks>
    /// Note: this code has been copied from BrokenRule class in Commons.CustomValidators 
    /// - ideally this code would be moved into CoreLib once all this duplication is once again removed
    /// </remarks>
    public class ExceptionToBrokenRuleTranslator
    {
        #region Member Variables

        private const string ContainsExceptionStandardMessageFragment = "Does not contain expected item: ";
        private const string DoesNotContainExceptionStandardMessageFragement = "Already contains expected item";
        private const string EndsWithExceptionStandardMessageFragment = "Parameter does not end with expected value ";
        private const string GreaterThanExceptionStandardMessageFragement = "Must be greater than";
        private const string GreaterThanOrEqualToExceptionStandardMessageFragement = "Must be greater than or equal to";
        private const string HasLengthExceptionStandardMessageFragment = "String parameter has length";
        private const string InRangeExceptionStandardMessageFragement = "Expected value between";
        private const string InvalidDateExceptionStandardMessageFragement = "is not a valid date";
        private const string InvalidEmailExceptionStandardMessageFragement = "is not a valid email address";
        private const string IsInFutureExceptionStandardMessageFragement = "must not be a future date";
        private const string IsNotInPastExceptionStandardMessageFragement = "must be a date in past";
        private const string IsUniqueExceptionStandardMessageFragment = "Sequence expected to be unique";
        private const string LessThanExceptionStandardMessageFragement = "Expected a value less than";
        private const string LessThanOrEqualToExceptionStandardMessageFragement = "Expected value less than or equal to";
        private const string NonZeroExceptionStandardMessageFragement = "Nonzero value expected";
        private const string NullArgExceptionStandardMessageFragment = "Expected non-null value";
        private const string StartsWithExceptionStandardMessageFragment = "Parameter does not start with expected value";
        private const string StringNotEmptyExceptionStandardMessageFragment = "String parameter is empty";

        #endregion


        #region Overridden object methods

        private static bool MapsToStringBrokenRule(ArgumentException sourceException)
        {
            return sourceException.GetType() == typeof (ArgumentNullException) || IsHasLengthException(sourceException) ||
                   IsStartsWithException(sourceException) || IsEndsWithException(sourceException);
        }

        #endregion


        #region Class Members

        private static bool ConatinsDate(string val)
        {
            return val.IndexOf(" ") != -1;
        }


        private static string DeriveRuleMessageFrom(ArgumentException sourceException)
        {
            if (HasNonStandardExceptionMessage(sourceException))
                return sourceException.Message.Replace("\r\nParameter name: someParam", "");

            string userFriendlyPropertyName = GetPropertyNameFromParamName(sourceException).SplitUpperCaseToString();

            if (MapsToNotMissingBrokenRule(sourceException))
                return String.Format("{0} is mandatory", userFriendlyPropertyName);
            else if (MapsToOutOfRangeBrokenRule(sourceException))
            {
                string customMessage = String.Empty;
                if (IsGreaterThanException(sourceException))
                    customMessage = ConatinsDate(GreaterThanBound(sourceException))
                                        ? GreaterThanExceptionMessageForDate(sourceException)
                                        : GreatetThanExceptionMessageForInt(sourceException);
                else if (IsLessThanException(sourceException))
                    customMessage = ConatinsDate(LessThanBound(sourceException))
                                        ? LessThanExceptionMessageForDate(sourceException)
                                        : LessThanExceptionMessageForInt(sourceException);
                else if (IsGreaterThanOrEqualToException(sourceException))
                    customMessage = "must be greater than or equal to " + GreaterThanOrEqualToBound(sourceException);
                else if (IsLessThanOrEqualToException(sourceException))
                    customMessage = "must be less than or equal to " + LessThanOrEqualToBound(sourceException);
                else if (IsInRangeException(sourceException))
                    customMessage = ConatinsDate(MinimumValue(sourceException))
                                        ? InRangeExceptionMessageForDate(sourceException)
                                        : InRangeExceptionMessageForInt(sourceException);

                return String.Format("{0} {1}", userFriendlyPropertyName, customMessage);
            }
            else if (MapsToEmailBrokenRule(sourceException))
                return String.Format("{0} {1}", userFriendlyPropertyName, "is not a valid email address");
            else if (MapsToDateBrokenRule(sourceException))
            {
                if (IsInvalidDateException(sourceException))
                    return String.Format("{0} {1}", userFriendlyPropertyName, "is not a valid date");
                else if (IsInFututeException(sourceException))
                    return String.Format("{0} {1}", userFriendlyPropertyName, "Cannot supply a date in future");
                else if (IsNotInPastException(sourceException))
                    return String.Format("{0} {1}", userFriendlyPropertyName, "must be a date in past");
            }
            else if (MapsToCollectionBrokenRule(sourceException))
            {
                if (IsDoesNotContainException(sourceException))
                    return String.Format("{0} {1} {2}",
                                         userFriendlyPropertyName,
                                         "must not contain item",
                                         GetExpectedItemValueFromException(sourceException));
                if (IsContainsException(sourceException))
                    return String.Format("{0} {1} {2}",
                                         userFriendlyPropertyName,
                                         "must contain item",
                                         GetExpectedItemValueFromException(sourceException));
                if (IsUniqueException(sourceException))
                    return String.Format("{0} {1}", userFriendlyPropertyName, "must be unique, cannot supply duplicates");
            }
            else if (MapsToStringBrokenRule(sourceException))
            {
                if (IsHasLengthException(sourceException))
                    return String.Format("{0} {1} {2}",
                                         userFriendlyPropertyName,
                                         "must be of length",
                                         GetExpectedLengthValueFromException(sourceException));
                else if (IsStartsWithException(sourceException))
                    return String.Format("{0} {1} {2}",
                                         userFriendlyPropertyName,
                                         "must start with",
                                         GetExpectedStartValueFromException(sourceException));
                else if (IsEndsWithException(sourceException))
                    return String.Format("{0} {1} {2}",
                                         userFriendlyPropertyName,
                                         "must end with",
                                         GetExpectedEndValueFromException(sourceException));
            }


            return String.Format("Possible bad value for: {0}", userFriendlyPropertyName);
        }


        /// <seealso cref="From(System.ArgumentException,System.Type)"/>
        public static BrokenRule From(ArgumentException sourceException)
        {
            return From(sourceException, null);
        }


        /// <summary>
        /// Converts an <see cref="ArgumentException"/> into a broken rule
        /// </summary>
        /// <param name="sourceException">the exception being converted</param>
        /// <param name="source">the type of the object that threw the exception</param>
        /// <returns></returns>
        public static BrokenRule From(ArgumentException sourceException, Type source)
        {
            Check.Require(() => Demand.The.Param(() => sourceException).IsNotNull());
            Check.Require(!string.IsNullOrEmpty(sourceException.ParamName),
                          string.Format(
                              "ArgumentException ('{0}') must have a ParamName to be converted to a BrokenRule",
                              sourceException.Message));

            string message = DeriveRuleMessageFrom(sourceException);

            return new BrokenRule(GetPropertyNameFromParamName(sourceException), message, RuleSeverity.Error)
                       {Source = source};
        }


        private static string GetExpectedEndValueFromException(ArgumentException sourceException)
        {
            var temp = sourceException.Message.Remove(sourceException.Message.IndexOf('.'));
            temp = temp.Replace(EndsWithExceptionStandardMessageFragment, "");
            return temp.Trim();
        }


        private static string GetExpectedItemValueFromException(ArgumentException sourceException)
        {
            var temp = sourceException.Message.Substring(sourceException.Message.IndexOf(':'));
            return temp.Remove(temp.IndexOf("\r")).Trim();
        }


        private static string GetExpectedLengthValueFromException(ArgumentException sourceException)
        {
            var temp = sourceException.Message.Substring(sourceException.Message.IndexOf(';') + 1);
            var temp1 = temp.Remove(temp.IndexOf('.'));
            temp = temp1.Replace("expected length", "");
            return temp.Trim();
        }


        private static string GetExpectedStartValueFromException(ArgumentException sourceException)
        {
            var temp = sourceException.Message.Remove(sourceException.Message.IndexOf('.'));
            temp = temp.Replace(StartsWithExceptionStandardMessageFragment, "");
            return temp.Trim();
        }


        private static string GetPropertyNameFromParamName(ArgumentException exception)
        {
            string paramName = exception.ParamName;
            string firstLetterUpperCased = paramName.Substring(0, 1).ToUpper() +
                                           paramName.Substring(1, paramName.Length - 1);
            return firstLetterUpperCased;
        }


        private static string GreaterThanBound(ArgumentException sourceException)
        {
            return sourceException.Message.Remove(sourceException.Message.IndexOf(';')).Remove(0,
                                                                                               GreaterThanExceptionStandardMessageFragement
                                                                                                   .Length + 1);
        }


        private static string GreaterThanExceptionMessageForDate(ArgumentException sourceException)
        {
            return String.Format("must fall later than {0}",
                                 GreaterThanBound(sourceException).Remove(
                                     GreaterThanBound(sourceException).IndexOf(" ")));
        }


        private static string GreaterThanOrEqualToBound(ArgumentException sourceException)
        {
            return sourceException.Message.Remove(sourceException.Message.IndexOf(';')).Remove(0,
                                                                                               GreaterThanOrEqualToExceptionStandardMessageFragement
                                                                                                   .Length + 1);
        }


        private static string GreatetThanExceptionMessageForInt(ArgumentException sourceException)
        {
            return "must be greater than " + GreaterThanBound(sourceException);
        }


        private static bool HasNonStandardExceptionMessage(ArgumentException exception)
        {
            var standardExceptionMessageFragements = new List<string>
                                                         {
                                                             NullArgExceptionStandardMessageFragment,
                                                             StringNotEmptyExceptionStandardMessageFragment,
                                                             GreaterThanExceptionStandardMessageFragement,
                                                             LessThanExceptionStandardMessageFragement,
                                                             GreaterThanOrEqualToExceptionStandardMessageFragement,
                                                             LessThanOrEqualToExceptionStandardMessageFragement,
                                                             InRangeExceptionStandardMessageFragement,
                                                             NonZeroExceptionStandardMessageFragement,
                                                             InvalidEmailExceptionStandardMessageFragement,
                                                             InvalidDateExceptionStandardMessageFragement,
                                                             IsInFutureExceptionStandardMessageFragement,
                                                             IsNotInPastExceptionStandardMessageFragement,
                                                             DoesNotContainExceptionStandardMessageFragement,
                                                             IsUniqueExceptionStandardMessageFragment,
                                                             HasLengthExceptionStandardMessageFragment,
                                                             StartsWithExceptionStandardMessageFragment,
                                                             EndsWithExceptionStandardMessageFragment,
                                                             ContainsExceptionStandardMessageFragment
                                                         };
            return !standardExceptionMessageFragements.Any(fragment => exception.Message.Contains(fragment));
        }


        private static string InRangeExceptionMessageForDate(ArgumentException sourceException)
        {
            return String.Format("must fall within {0} and {1}",
                                 MinimumValue(sourceException).Remove(
                                     MinimumValue(sourceException).IndexOf(" ")),
                                 MaximumValue(sourceException).Remove(
                                     MaximumValue(sourceException).IndexOf(" ")));
        }


        private static string InRangeExceptionMessageForInt(ArgumentException sourceException)
        {
            return String.Format("must be between {0} and {1}",
                                 MinimumValue(sourceException),
                                 MaximumValue(sourceException));
        }


        private static bool IsContainsException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(ContainsExceptionStandardMessageFragment);
        }


        private static bool IsDoesNotContainException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(DoesNotContainExceptionStandardMessageFragement);
        }


        private static bool IsEndsWithException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(EndsWithExceptionStandardMessageFragment);
        }


        private static bool IsGreaterThanException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(GreaterThanExceptionStandardMessageFragement) &&
                   !sourceException.Message.Contains(GreaterThanOrEqualToExceptionStandardMessageFragement);
        }


        private static bool IsGreaterThanOrEqualToException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(GreaterThanOrEqualToExceptionStandardMessageFragement);
        }


        private static bool IsHasLengthException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(HasLengthExceptionStandardMessageFragment);
        }


        private static bool IsInFututeException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(IsInFutureExceptionStandardMessageFragement);
        }


        private static bool IsInRangeException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(InRangeExceptionStandardMessageFragement);
        }


        private static bool IsInvalidDateException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(InvalidDateExceptionStandardMessageFragement);
        }


        private static bool IsLessThanException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(LessThanExceptionStandardMessageFragement) &&
                   !sourceException.Message.Contains(LessThanOrEqualToExceptionStandardMessageFragement);
        }


        private static bool IsLessThanOrEqualToException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(LessThanOrEqualToExceptionStandardMessageFragement);
        }


        private static bool IsNotInPastException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(IsNotInPastExceptionStandardMessageFragement);
        }


        private static bool IsStartsWithException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(StartsWithExceptionStandardMessageFragment);
        }


        private static bool IsUniqueException(ArgumentException sourceException)
        {
            return sourceException.Message.Contains(IsUniqueExceptionStandardMessageFragment);
        }


        private static string LessThanBound(ArgumentException sourceException)
        {
            return sourceException.Message.Remove(sourceException.Message.IndexOf(';')).Remove(0,
                                                                                               LessThanExceptionStandardMessageFragement
                                                                                                   .Length + 1);
        }


        private static string LessThanExceptionMessageForDate(ArgumentException sourceException)
        {
            return String.Format("must fall earlier than {0}",
                                 LessThanBound(sourceException).Remove(
                                     LessThanBound(sourceException).IndexOf(" ")));
        }


        private static string LessThanExceptionMessageForInt(ArgumentException sourceException)
        {
            return "must be less than " + LessThanBound(sourceException);
        }


        private static string LessThanOrEqualToBound(ArgumentException sourceException)
        {
            return sourceException.Message.Remove(sourceException.Message.IndexOf(';')).Remove(0,
                                                                                               LessThanOrEqualToExceptionStandardMessageFragement
                                                                                                   .Length + 1);
        }


        private static bool MapsToCollectionBrokenRule(ArgumentException sourceException)
        {
            return sourceException.GetType() == typeof (ArgumentNullException) ||
                   IsDoesNotContainException(sourceException) || IsUniqueException(sourceException) ||
                   IsContainsException(sourceException);
        }


        private static bool MapsToDateBrokenRule(ArgumentException sourceException)
        {
            return sourceException.GetType() == typeof (ArgumentNullException) ||
                   IsInvalidDateException(sourceException) ||
                   IsInFututeException(sourceException) ||
                   IsNotInPastException(sourceException);
        }


        private static bool MapsToEmailBrokenRule(ArgumentException sourceException)
        {
            return sourceException.GetType() == typeof (ArgumentNullException) ||
                   sourceException.Message.Contains(InvalidEmailExceptionStandardMessageFragement);
        }


        private static bool MapsToNotMissingBrokenRule(ArgumentException sourceException)
        {
            return sourceException.GetType() == typeof (ArgumentNullException) ||
                   sourceException.Message.Contains(StringNotEmptyExceptionStandardMessageFragment) ||
                   sourceException.Message.Contains(NonZeroExceptionStandardMessageFragement);
        }


        private static bool MapsToOutOfRangeBrokenRule(ArgumentException sourceException)
        {
            return sourceException.GetType() == typeof (ArgumentOutOfRangeException) ||
                   IsGreaterThanException(sourceException) ||
                   IsLessThanException(sourceException) ||
                   IsGreaterThanOrEqualToException(sourceException) ||
                   IsLessThanOrEqualToException(sourceException) ||
                   IsInRangeException(sourceException);
        }


        private static string MaximumValue(ArgumentException sourceException)
        {
            var minAndMax = sourceException.Message.Remove(sourceException.Message.IndexOf(';')).Remove(0,
                                                                                                        InRangeExceptionStandardMessageFragement
                                                                                                            .Length + 1);
            int indexOfAnd = minAndMax.IndexOf("and") + 4;
            var max = minAndMax.Substring(indexOfAnd);
            return max;
        }


        private static string MinimumValue(ArgumentException sourceException)
        {
            var minAndMax = sourceException.Message.Remove(sourceException.Message.IndexOf(';')).Remove(0,
                                                                                                        InRangeExceptionStandardMessageFragement
                                                                                                            .Length + 1);
            var min = minAndMax.Remove(minAndMax.IndexOf("and") - 1);
            return min;
        }

        #endregion
    }
}