using System.Text.RegularExpressions;

namespace Eca.Commons.Email
{
    public static class EmailAddressValidator
    {
        #region Class Members

        public static string EmailPattern()
        {
            const string numerial = @"[\x30-\x39]";
            const string quotes = @"\x22";
            const string whiteSpace = @"[\x09\x20]";
            const string noWhiteSpaceCtrl = @"[\x01-\x08\x0B\x0C\x0E-\x1F\x7F]";
            const string alpha = @"([\x41-\x5A]|[\x61-\x7A])";
            const string text = @"[\x01-\x09\x0B\x0C\x0E-\x7F]";

            const string a = "(" + alpha + "|" + numerial + @"|[\!\#\$\%\&\'\*\+\-\/\=\?\^_\`\{\|\}\~])";
            const string b = "(" + noWhiteSpaceCtrl + @"|[\x21\x23-\x5B\x5D-\x7E])";
            const string c = "(" + noWhiteSpaceCtrl + @"|[\x21-\x5A\x5E-\x7E])";
            const string dotA = a + @"+(\." + a + "+)*";
            const string quoteA = @"\\" + text;
            const string quoteB = "(" + b + "|" + quoteA + ")";
            const string quoteC = "(" + c + "|" + quoteA + ")";
            const string quoteD = quotes + "(" + whiteSpace + "?" + quoteB + ")*" + whiteSpace + "?" + quotes;
            const string local = "(" + dotA + "|" + quoteD + ")";
            const string domainA = @"\[(" + whiteSpace + "?" + quoteC + ")*" + whiteSpace + "?" + @"\]";
            const string domain = "(" + dotA + "|" + domainA + ")";
            const string addrressExpression = "^" + local + "\\@" + domain + "$";

            return addrressExpression;
        }


        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and contains a valid E-Mail address;
        /// otherwise false.</returns>
        public static bool IsEmail(string email)
        {
            if (email != null)
            {
                //EmailAddress emailAddress = new EmailAddress(email);
                return Regex.IsMatch(email, EmailPattern());
                //return emailAddress.IsValid;
            }
            return false;
        }

        #endregion
    }
}